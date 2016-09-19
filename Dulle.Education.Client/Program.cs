using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Apache.Ignite.Core.Events;
using Dulle.Education.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dulle.Education.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Ignition.ClientMode = true;
            var cfg = new IgniteConfiguration
            {
                DiscoverySpi = new TcpDiscoverySpi
                {
                    IpFinder = new TcpDiscoveryStaticIpFinder
                    {
                        Endpoints = new[] { "127.0.0.1:47500..47509" }
                    },
                    SocketTimeout = TimeSpan.FromSeconds(0.3)
                },                
                BinaryConfiguration = new BinaryConfiguration(typeof(Person),
                        typeof(PersonFilter))                
            };
            IIgnite ignite = Ignition.Start(cfg);

            ICache<int, Person> cache = ignite.GetCache<int, Person>("persons");
            Console.WriteLine(cache.GetSize());
            var scanQuery = new ScanQuery<int, Person>(new PersonFilter("batiel"));
            IQueryCursor<ICacheEntry<int, Person>> queryCursor = cache.Query(scanQuery);

            ShowResult(queryCursor);

            //var sqlQuery = new SqlQuery(typeof(Person), "where firstname like ?", "%del%");
            //IQueryCursor<ICacheEntry<int, Person>> queryCursor = cache.Query(sqlQuery);

            //ShowResult(queryCursor);
            Console.ReadKey();
        }
        
        private static void ShowResult(IQueryCursor<ICacheEntry<int, Person>> queryCursor)
        {
            foreach (ICacheEntry<int, Person> cacheEntry in queryCursor)
                Console.WriteLine(cacheEntry);
        }
    }
}
