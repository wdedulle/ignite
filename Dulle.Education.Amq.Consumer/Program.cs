using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Eviction;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;
using Apache.NMS.Util;
using Dulle.Education.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dulle.Education.Amq.Consumer
{
    class Program
    {        
        static void Main(string[] args)
        {
            Uri connecturi = new Uri("activemq:tcp://localhost:61616");

            Console.WriteLine("About to connect to " + connecturi);

            // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);

            using (IConnection connection = factory.CreateConnection())
            using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
            {
                IDestination destination = SessionUtil.GetDestination(session, "queue://Tui.Persons");
                //string destination = "Tui.Retail.Relations.Load";                
                Console.WriteLine("Using destination: " + destination);
                //ITopic topic = new ActiveMQTopic(destination);

                // Create a consumer
                using (IMessageConsumer consumer = session.CreateConsumer(destination))
                {
                    IIgnite ignite = GetIgnition();
                    // Start the connection so that messages will be processed.
                    connection.Start();
                    while (true)
                    {
                        MyWorker worker = new MyWorker(consumer.Receive(), ignite);
                        Thread workerThread = new Thread(new ThreadStart(worker.process));
                        workerThread.Start();
                    }
                }
            }
        }

        private static IIgnite GetIgnition()
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
                        OffHeapMaxMemory = 0,
                        WriteSynchronizationMode = CacheWriteSynchronizationMode.PrimarySync,
                        RebalanceBatchSize = 1024 * 1024,
                        CopyOnRead = false
                    }
                },
                BinaryConfiguration = new BinaryConfiguration(typeof(Person),
               typeof(PersonFilter))
            };
            cfg.JvmInitialMemoryMb = 4096;
            cfg.JvmMaxMemoryMb = 8192;
            IIgnite ignite = Ignition.Start(cfg);
            return ignite;
        }
    }

    public class MyWorker
    {
        private IObjectMessage message;
        private IIgnite igniteDal;

        public MyWorker(IMessage receivedMsg, IIgnite ignite)
        {
            message = receivedMsg as IObjectMessage;
            igniteDal = ignite;
        }

        // This method that will be called when the thread is started
        public void process()
        {
            Console.WriteLine("Received message with ID:   " + message.NMSMessageId);

            ICache<int, Person> cache = igniteDal.GetOrCreateCache<int, Person>(
                            new CacheConfiguration("persons", typeof(Person)));
            var persons = message.Body as IDictionary<int, Person>;            
            cache.PutAll(persons);
            
            Console.WriteLine("Message DONE with ID:   " + message.NMSMessageId + " >>> " + cache.Count());            
        }
    }
}
