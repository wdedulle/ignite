using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;
using Apache.NMS.Util;
using Dulle.Education.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dulle.Education.Amq.Producer
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
            using (ISession session = connection.CreateSession())
            {

                //string destination = "Tui.Retail.Relations.Load";           
                IDestination destination = SessionUtil.GetDestination(session, "queue://Tui.Persons");
                Console.WriteLine("Using destination: " + destination);
                //ITopic topic = new ActiveMQTopic(destination);

                // Create a consumer and producer                
                using (IMessageProducer producer = session.CreateProducer(destination))
                {
                    // Start the connection so that messages will be processed.
                    connection.Start();
                    producer.DeliveryMode = MsgDeliveryMode.Persistent;

                    sendMessages(session, producer);
                }
            }
        }

        private static void sendMessages(ISession session, IMessageProducer producer)
        {
            Console.WriteLine(">>> Loading persons");
            string[] firstNames = File.ReadAllLines(@"..\..\First_Names.csv", Encoding.UTF8);
            string[] lastNames = File.ReadAllLines(@"..\..\Last_Names.csv", Encoding.UTF8);
            int key = 0;
            Dictionary<int, Person> persons = new Dictionary<int, Person>();

            IObjectMessage request;
            
            foreach (string lastName in lastNames)
            {
                foreach (string firstName in firstNames)
                {
                    Random randomAge = new Random();

                    persons[key] = new Person { Name = lastName, Age = randomAge.Next(120), FirstName = firstName };
                    key++;
                    if ((key % 100000) == 0)
                    {
                        request = session.CreateObjectMessage(persons);
                        request.Properties["NMSXGroupID"] = "persons";

                        producer.Send(request);
                        persons.Clear();
                        Console.WriteLine("  " + key);   
                        if(key == 4000000 )
                        {
                            return;
                        }
                    }
                }
            }
            Console.WriteLine(">>> Loading persons DONE");
        }
    }
}
