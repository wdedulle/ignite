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
                CacheConfiguration = new[]
                {
                    new CacheConfiguration
                    {
                        StartSize = 1073741824,
                        EnableSwap = false,
                        CacheMode = CacheMode.Partitioned,
                        Backups = 0,
                        OffHeapMaxMemory = 10737418240,
                        EvictionPolicy = new LruEvictionPolicy { MaxSize = 10000000 },
                        RebalanceBatchSize = 1024 * 1024,
                        CopyOnRead = false                                                
                    }
                },
                BinaryConfiguration = new BinaryConfiguration(typeof(Person),
                        typeof(PersonFilter))
            };
            cfg.JvmInitialMemoryMb = 10240;
            cfg.JvmMaxMemoryMb = 16384;
            
            IIgnite ignite = Ignition.Start(cfg);           
            
            Console.ReadKey();
        }
    }    
}
