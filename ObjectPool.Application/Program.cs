using System;
using System.Collections.Generic;
using ObjectPool.Core;

namespace ObjectPool.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("开始调用前, 请按任意键开始...");
            //Console.ReadKey();
            //Console.WriteLine("开始调用");
            ////CommomCase();
            //ObjectPoolCase();
            //Console.WriteLine("结束调用");

            var p = new Product();
            p.ID = 1;

            Console.Read();
        }

        static void CommomCase()
        {
            var sum = 0L;
            var list = new List<Product>();
            for (int i = 0; i < 100000; i++)
            {
                var obj = new Product{ID = i, Name = "p" + i};
                sum += obj.ID;
                list.Add(obj);
            }

            System.Console.WriteLine($"Check ID Sum:{sum}");
        }

        static void ObjectPoolCase()
        {
            var sum = 0L;
            var pool = new ObjectPool<Product>(() => new Product(), 10000);
            for (int i = 0; i < 100000; i++)
            {
                var obj = pool.Rent();
                obj.ID = i;
                obj.Name = "p" + i;
                sum += obj.ID;
                pool.Return(obj);
            }
            //System.Console.WriteLine("Reusing Number:" + pool.ReusingNumber);
            System.Console.WriteLine($"Check ID Sum:{sum}");
        }
    }

    class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
