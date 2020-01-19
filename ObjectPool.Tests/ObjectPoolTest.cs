using System;
using Xunit;
using OPC = ObjectPool.Core;

namespace ObjectPool.Tests
{
    public class ObjectPoolTest
    {
        [Fact]
        public void TestNormal()
        {
            using (var pool = new OPC.ObjectPool<Product>(() => new Product()))
            {
                var product = pool.Rent();
                product.ID = 1;

                pool.Return(product);

                Assert.Equal(product, pool.Rent());
            }
        }
    }

    class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
