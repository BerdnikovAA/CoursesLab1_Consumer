using System.Configuration;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using static System.Console;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Lab1_courses
{

    class request
    {
        public string Surname;
        public string Name;
        public string Patronymic;
        public DateTime BirthDay;
        public int RequestType;
        public string Sex;

        public request()
        {
            Random rand = new Random();
            int length = rand.Next(5, 13);
            for(int i=0; i<=length;i++)
            { 
                Surname += (char)rand.Next(0x0410, 0x44F);
            }

            length = rand.Next(5, 13);
            for (int i = 0; i <= length; i++)
            {
                Name += (char)rand.Next(0x0410, 0x44F);
            }

            length = rand.Next(5, 13);
            for (int i = 0; i <= length; i++)
            {
                Patronymic += (char)rand.Next(0x0410, 0x44F);
            }

            BirthDay = DateTime.Now.AddDays(new Random().Next(100000));

            RequestType = rand.Next(1, 501);

            if (rand.Next(1, 3) == 1)
                Sex = "male";
            else Sex = "female";
        }


    }



    class MessageGenerator
    {
        
        private IConnection GetRabbitConnection()
        {
            ConnectionFactory factory = new ConnectionFactory {
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                HostName = "localhost"
            };
            var conn = factory.CreateConnection();
            return conn;
        }

        public string queueName = "test";
        public string exchangeName = "test";
        public string routingKey = "test";

        private IModel GetRabbitChannel(string exchangeName,string queueName,string routingKey)
        {
            IModel model= GetRabbitConnection().CreateModel();
            model.ExchangeDeclare("test",ExchangeType.Direct);
            model.QueueDeclare(queueName, false, false, false, null);
            model.QueueBind(queueName, exchangeName, routingKey, null);
            return model;
        }

        public List<object> CreatRequests(int n)
        {
            
            List<object> requests = new List<object>();
            for (int i = 0; i < n; i++)
                requests.Add(new request());
            return requests;
        }

        public void SendMessage(int n)
        {
            var model = GetRabbitChannel(exchangeName, queueName, routingKey);
            var newlist = CreatRequests(n);
            byte[] messageBody;
            for (int i = 0; i < newlist.Count; i++)
            {
                messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(newlist[i]));
                model.BasicPublish(exchangeName, routingKey, null ,messageBody);
            }
               
                
        }









    }
    class Program
    {
        static void Main(string[] args)
        {
            int n = int.Parse(ConfigurationManager.AppSettings["n"]);            
            MessageGenerator mg = new MessageGenerator();
            mg.SendMessage(n);
            ReadKey();

        }
    }
}
