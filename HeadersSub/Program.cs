using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadersSub
{
    class Program
    {
        static void Main(string[] args)
        {
            IConnectionFactory connFactory = new ConnectionFactory//创建连接工厂对象
            {
                HostName = "192.168.1.107",//IP地址
                Port = 5672,//端口号
                UserName = "ztb",//用户账号
                Password = "123"//用户密码
            };
            using (IConnection conn = connFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    //交换机名称
                    String exchangeName = "pubSubHeardesExchange";
                    //声明交换机
                    channel.ExchangeDeclare(exchange: exchangeName, type: "headers");
                    //消息队列名称
                    String queueName = "myHeadersQueue";
                    //声明队列
                    channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    //匹配路由
                    channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: string.Empty, arguments: new Dictionary<string, object>()
                    {
                        {"x-match", "any"},
                        {"username", "jack"},
                        {"password", "12345" }
                    });


                    //定义消费者
                    var consumer = new EventingBasicConsumer(channel);
                    //接收事件
                    consumer.Received += (model, ea) =>
                    {
                        byte[] message = ea.Body;//接收到的消息
                        Console.WriteLine("接收到信息为:" + Encoding.UTF8.GetString(message));

                    };
                    //开启监听
                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                    Console.ReadKey();
                }
            }
        }
    }
}
