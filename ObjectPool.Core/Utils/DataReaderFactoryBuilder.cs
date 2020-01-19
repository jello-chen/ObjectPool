using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPool.Core
{
    static class DataReaderFactoryBuilder<T>
    {
        private static readonly MethodInfo getValueMethod = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(string) });
        private static readonly MethodInfo isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });

        public static Func<IDataRecord, T> NewFactory = BuildNewFactory();
        public static Func<IDataRecord, T, T> SetFactory = BuildSetFactory();

        private static Func<IDataRecord, T> BuildNewFactory()
        {
            var dataRecordParameterExpr = Expression.Parameter(typeof(IDataRecord), "dr");
            var memberBindings = new List<MemberBinding>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite);

            foreach (var p in properties)
            {
                var memberBindingExpr = Expression.Bind(
                    p.GetSetMethod(),
                    Expression.Convert(
                        Expression.Call(
                            dataRecordParameterExpr,
                            getValueMethod,
                            Expression.Constant(p.Name, typeof(string))),
                        p.PropertyType));
                memberBindings.Add(memberBindingExpr);
            }

            var bodyExpr = Expression.MemberInit(Expression.New(typeof(T)), memberBindings);
            var lambdaExpr = Expression.Lambda<Func<IDataRecord, T>>(bodyExpr, dataRecordParameterExpr);
            return lambdaExpr.Compile();
        }

        private static Func<IDataRecord, T, T> BuildSetFactory()
        {
            var dataRecordParameterExpr = Expression.Parameter(typeof(IDataRecord), "dr");
            var currentObjectExpr = Expression.Parameter(typeof(T), "t");
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite).ToArray();
            var setPropertyExprs = new List<Expression>(properties.Length);

            foreach (var p in properties)
            {
                var setPropertyExpr = Expression.Call(
                    currentObjectExpr,
                    p.SetMethod,
                    Expression.Convert(
                        Expression.Call(
                            dataRecordParameterExpr,
                            getValueMethod,
                            Expression.Constant(p.Name, typeof(string))),
                        p.PropertyType));
                setPropertyExprs.Add(setPropertyExpr);
            }

            var bodyExpr = Expression.Lambda<Func<IDataRecord, T, T>>(
                Expression.Block(setPropertyExprs),
                dataRecordParameterExpr,
                currentObjectExpr);
            return bodyExpr.Compile();
        }
    }
}