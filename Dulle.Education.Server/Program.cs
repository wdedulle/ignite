using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Eviction;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Apache.Ignite.Core.Events;
using Dulle.Education.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Dulle.Education.Server
{
    class Program
    {
        static void Main(string[] args)
        {

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
                IncludedEventTypes = EventType.CacheAll,
                CacheConfiguration = new[]
                {
                    new CacheConfiguration
                    {
                        StartSize = 100 * 1024 * 1024,
                        CacheMode = CacheMode.Partitioned,
                        Backups = 0,
                        OffHeapMaxMemory = 10 * 1024L * 1024L * 1024L,
                        EvictionPolicy = new LruEvictionPolicy { MaxSize = 1000000 }
                    }
                },
                BinaryConfiguration = new BinaryConfiguration(typeof(Person),
                        typeof(PersonFilter))
            };
            IIgnite ignite = Ignition.Start(cfg);

            ICache<int, Person> cache = ignite.GetOrCreateCache<int, Person>(
                new CacheConfiguration("persons", typeof(Person)));

            Console.WriteLine(">>> Loading persons");
            cache.PutAll( CreateData(400000) );
            Console.WriteLine(">>> Loading persons DONE");
            Console.ReadKey();
        }

        private static Dictionary<int, Person> CreateData( int maxRecords )
        {
            string[] firstNames = File.ReadAllLines(@"c:\users\wim\documents\visual studio 2015\Projects\Dulle.Education.Server\Dulle.Education.Server\First_Names.csv", Encoding.UTF8);
            string[] lastNames = File.ReadAllLines(@"c:\users\wim\documents\visual studio 2015\Projects\Dulle.Education.Server\Dulle.Education.Server\Last_Names.csv", Encoding.UTF8);
            int key = 0;
            Dictionary<int, Person> persons = new Dictionary<int, Person>();
            foreach (string lastName in lastNames)
            {
                foreach (string firstName in firstNames)
                {
                    Random randomAge = new Random();

                    persons[key] = new Person { Name = lastName, Age = randomAge.Next(120), FirstName = firstName };
                    key++;

                    if( key > maxRecords )
                    {
                        return persons;
                    }
                }
            }

            return persons;
        }

    }    
}
