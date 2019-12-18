using System;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            ExchangeTopicMode();
        }

        // 普通生产者
        static void NormalProducerMode()
        {
            Console.WriteLine("生产者");
            IConnectionFactory factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();

            var name = "demo";

            // 声明一个队列
            channel.QueueDeclare(
                queue: name, // 消息队列名称
                durable: false, // 是否持久化，持久化会保存到磁盘，服务器重启时可以保证不丢失相关信息
                exclusive: false, // 是否排他，如果一个队列声明为排他队列，该队列仅对首次声明他的连接可见，并在连接断开时自动销毁
                autoDelete: false, // 是否自动删除，自动删除的前提是：致少有一个消费者连接到这个队列，之后所有与这个队列连接的消费者都断开时,才会自动删除
                arguments: null // 设置队列的一些其他参数
            );
            var str = string.Empty;

            do
            {
                Console.WriteLine("发送内容。。。");
                str = Console.ReadLine();

                // 消息内容
                var body = Encoding.UTF8.GetBytes(str);

                // 发送消息
                channel.BasicPublish("", name, null, body);
                Console.WriteLine("成功发送消息：" + str);
            } while (str.Trim().ToLower() != "exit");

            conn.Close();
            channel.Close();
        }

        // Exchange模式-发布订阅模式
        static void ExchangeFanoutMode()
        {
            Console.WriteLine("发布订阅生产者");
            // 创建连接工厂对象
            IConnectionFactory factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            // 创建连接对象
            IConnection conn = factory.CreateConnection();
            // 创建连接会话对象
            IModel channel = conn.CreateModel();
            var exchangeName = "exchange1";
            channel.ExchangeDeclare(exchangeName,type:"fanout");
            var str = "";

            do
            {
                str = Console.ReadLine();
                // 消息内容
                byte[] body = Encoding.UTF8.GetBytes(str);

                // 发布消息
                channel.BasicPublish(exchangeName,"",null,body);
            } while (str.Trim().ToLower() != "exit");

            conn.Close();
            channel.Close();
        }

        // 路由模式
        static void ExchangeRouteKeyMode()
        {
            Console.WriteLine("路由模式生产者");
            // 创建连接工厂对象
            IConnectionFactory factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();

            var exchangeName = "exchange1";
            var routeKey = "key1";
            channel.ExchangeDeclare(exchangeName,ExchangeType.Direct);

            var str = string.Empty;
            do
            {
                str = Console.ReadLine();
                // 消息内容
                byte[] body = Encoding.UTF8.GetBytes(str);
                // 发送消息
                channel.BasicPublish(exchangeName, routeKey, null, body);

            } while (str.Trim().ToLower() != "exit");

            conn.Close();
            channel.Close();
        }

        // 通配符模式
        static void ExchangeTopicMode()
        {
            Console.WriteLine("通配符模式生产者");
            // 创建连接工厂对象
            IConnectionFactory factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();

            var exchangeName = "exchange1";
            var routeKey = "key.a";

            // 将交换机设置为topic模式
            channel.ExchangeDeclare(exchangeName,ExchangeType.Topic);
            var str = string.Empty;

            do
            {
                str = Console.ReadLine();
                // 消息内容
                var body = Encoding.UTF8.GetBytes(str);
                channel.BasicPublish(exchangeName, routeKey, null, body);
            } while (str.Trim().ToLower() != "exit");

            conn.Close();
            channel.Close();
        }
    }
}
