using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisLab
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379,password=michaelredis");

            IDatabase db = redis.GetDatabase();
            
            // set get
            db.StringSet("name", "Michael Jackson");
            var getResult = await db.StringGetAsync("name");
            Console.WriteLine($"{nameof(getResult)}:{getResult}");
            
            // getset
            var getSetResult = await db.StringGetSetAsync("name", "dhb");
            Console.WriteLine($"{nameof(getSetResult)}:is {getSetResult}");
            
            // incr 自增1
            await db.StringSetAsync("age", "11");
            await db.StringIncrementAsync("age");
            var age = await db.StringGetAsync("age");
            Console.WriteLine(age);
            
            // incrby 自增指定数
            age = await db.StringIncrementAsync("age", 5);
            Console.WriteLine(age);
            
            // decr 自减
            age = await db.StringDecrementAsync("age");
            Console.WriteLine(age);
            
            // decrby
            age = await db.StringDecrementAsync("age", 5);
            Console.WriteLine(age);
            
            // mset
            await db.StringSetAsync(new KeyValuePair<RedisKey, RedisValue>[]
            {
                new KeyValuePair<RedisKey, RedisValue>("country","china"),
                new KeyValuePair<RedisKey, RedisValue>("province","GuangDong"),
                new KeyValuePair<RedisKey, RedisValue>("city","GuangZhou")
            });
            
            // mget
            var values = await db.StringGetAsync(new RedisKey[] {"country","province","city" });
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }

        }
    }
}
