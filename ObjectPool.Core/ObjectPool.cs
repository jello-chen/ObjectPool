using System;
using System.Threading;

namespace ObjectPool.Core
{
    public class ObjectPool<T> : IObjectPool<T> where T : class
    {
        private struct Element
        {
            internal T Value;
        }

        private readonly Element[] _items;
        private readonly Func<T> _factory;

        public int ReusingNumber { get; private set; }

        public ObjectPool(Func<T> factory) : this(factory, Environment.ProcessorCount * 2)
        {
            
        }

        public ObjectPool(Func<T> factory,  int size)
        {
            this._factory = factory;
            this._items = new Element[size];
        }

        public T Rent()
        {
            Element[] items = _items;
            int num = 0;
            T val = default;
            while (true)
            {
                if (num < items.Length)
                {
                    val = items[num].Value;
                    if (val != null && val == Interlocked.CompareExchange(ref items[num].Value, null, val)) 
                    {
                        ReusingNumber++;
                        break;
                    }
                    num++;
                    continue;
                }
                val = CreateInstance();
                break;
            }
            return val;
        }

        public void Return(T obj)
        {
            Element[] items = _items;
            int num = 0;
            while (true)
            {
                if (num < items.Length)
                {
                    if (items[num].Value == null) break;
                    num++;
                    continue;
                }
                return;
            }
            items[num].Value = obj;
        }

        public void Dispose()
        {
            bool canDispose = typeof(IDisposable).IsAssignableFrom(typeof(T));
            for (int i = 0; i < _items.Length; i++)
            {
                if (canDispose)
                {
                    ((IDisposable)_items[i].Value).Dispose();
                }
                _items[i].Value = null;
            }
        }

        private T CreateInstance() => _factory();
    }
}