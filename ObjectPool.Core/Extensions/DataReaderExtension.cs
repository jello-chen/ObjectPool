using System;
using System.Collections.Generic;
using System.Data;

namespace ObjectPool.Core
{
    public static class DataReaderExtension
    {
        public static IEnumerable<T> AsEnumerable<T>(this IDataReader dataReader, Func<IDataReader, T> factory)
        {
            if (dataReader == null) throw new ArgumentNullException(nameof(dataReader));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            while (dataReader.Read())
            {
                yield return factory(dataReader);
            }
        }

        public static IEnumerable<T> AsEnumerable<T>(this IDataReader dataReader)
            => AsEnumerable(dataReader, DataReaderFactoryBuilder<T>.NewFactory);
    }
}