using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQConsumer1
{
    // 测试一对多工作模式
    class Program
    {
        static void Main(string[] args)
        {
            ExchagneRouteKeyMode();
        }

        // 普通消费者模式
        static void NormalConsumerMode()
        {
            Console.WriteLine("普通消费者_1");
            IConnectionFactory factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();
            string name = "demo";
            channel.QueueDeclare(
                queue: name,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                byte[] message = ea.Body; // 接收到的信息
                Console.WriteLine("消费者_1接受到的信息为：" + Encoding.UTF8.GetString(message));
            };

            // 消费者开启监听
            channel.BasicConsume(name, true, consumer);
            Console.ReadKey();
            channel.Dispose();
            conn.Close();
        }

        // 延迟消费者模式
        static void DelayConfirmMode()
        {
            Console.WriteLine("延迟确认消费者_1");
            IConnectionFactory factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();
            string name = "demo";
            channel.QueueDeclare(
                queue: name,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                byte[] message = ea.Body; // 接收到的信息
                Console.WriteLine("消费者_1接受到的信息为：" + Encoding.UTF8.GetString(message));
                Thread.Sleep(3000);
                channel.BasicAck(ea.DeliveryTag, true); // 返回消息确认
            };

            // 将AutoAck设置false，关闭自动确认
            // 消费者开启监听
            channel.BasicConsume(name, true, consumer);
            Console.ReadKey();
            channel.Dispose();
            conn.Close();
        }

        // 能者多劳模式
        static void AblePeopleShouldDoMoreWorkMode()
        {
            Console.WriteLine("延迟确认&能者多劳模式消费者_1");
            IConnectionFactory factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();
            string name = "demo";
            channel.QueueDeclare(
                queue: name,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // 能者多劳模式
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                byte[] message = ea.Body; // 接收到的信息
                Console.WriteLine("消费者_1接受到的信息为：" + Encoding.UTF8.GetString(message));
                Thread.Sleep(3000);
                channel.BasicAck(ea.DeliveryTag, true); // 返回消息确认
            };

            // 将AutoAck设置false，关闭自动确认
            // 消费者开启监听
            channel.BasicConsume(name, true, consumer);
            Console.ReadKey();
            channel.Dispose();
            conn.Close();
        }

        // Exchange发布订阅模式
        static void ExchangeFanoutMode()
        {
            Console.WriteLine("发布订阅消费者_1");
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
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
            // 消息队列名称
            var queueName = DateTime.Now.Second.ToString();
            // 声明队列
            channel.QueueDeclare(queueName, false, false, false, null);
            // 将队列与交换机进行绑定
            channel.QueueBind(queueName, exchangeName, "", null);

            // 定义消费者
            var consumer = new EventingBasicConsumer(channel);
            Console.WriteLine($"队列名称：{queueName}");

            // 接收事件
            consumer.Received += (model, ea) =>
            {
                // 接收到的消息
                byte[] message = ea.Body;
                Console.WriteLine($"接收到的信息为：{Encoding.UTF8.GetString(message)}");
                channel.BasicAck(ea.DeliveryTag,true);
            };

            // 开启监听
            channel.BasicConsume(queueName,false,consumer);
            Console.ReadLine();
        }

        // 路由模式
        static void ExchagneRouteKeyMode()
        {
            Console.WriteLine($"输入接受key名称：");
            var routeKey = Console.ReadLine();

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
            channel.ExchangeDeclare(exchangeName,ExchangeType.Direct);
            var queueName = DateTime.Now.Second.ToString();

            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routeKey, null);

            var consumer = new EventingBasicConsumer(channel);
            Console.WriteLine($"队列名称:{queueName}");

            consumer.Received += (model, ea) =>
            {
                byte[] message = ea.Body;
                Console.WriteLine($"接受到的信息为：{Encoding.UTF8.GetString(message)}");
                // 返回消息确认
                channel.BasicAck(ea.DeliveryTag, true);
            };

            // 开启监听
            channel.BasicConsume(queueName, false, consumer);
            Console.ReadKey();
        }

        // 通配符模式
        static void ExchagneTopicMode()
        {
            var routeKey = "key.*";

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
            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);
            var queueName = DateTime.Now.Second.ToString();
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routeKey, null);

            var consumer = new EventingBasicConsumer(channel);
            Console.WriteLine($"队列名称：{queueName}");

            consumer.Received += (model, ea) =>
            {
                byte[] message = ea.Body;
                Console.WriteLine($"接收到信息为：{Encoding.UTF8.GetString(message)}");
                channel.BasicAck(ea.DeliveryTag, true);
            };

            // 开启监听
            channel.BasicConsume(queueName, false, consumer);
            Console.ReadKey();
        }
    }
}
