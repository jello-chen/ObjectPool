using System;

namespace ObjectPool.Core
{
    public interface IObjectPool<T> : IDisposable where T : class
    {
        T Rent();
        void Return(T obj);
    }
}
