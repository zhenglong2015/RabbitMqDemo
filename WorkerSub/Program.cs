using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace WorkerSub
{
    class Program
    {
        /// <summary>
        /// 一对多
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
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
                    String queueName = "workerQueue";
                    //声明一个队列
                    channel.QueueDeclare(
                      queue: queueName,//消息队列名称
                      durable: false,//是否缓存
                      exclusive: false,
                      autoDelete: false,
                      arguments: null
                       );
                    //告诉Rabbit每次只能向消费者发送一条信息,再消费者未确认之前,不再向他发送信息
                    channel.BasicQos(0, 1, false);
                    //创建消费者对象
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        Thread.Sleep((new Random().Next(1, 6)) * 1000);//随机等待,实现能者多劳,
                        byte[] message = ea.Body;//接收到的消息
                        Console.WriteLine("接收到信息为:" + Encoding.UTF8.GetString(message));
                        //返回消息确认
                        channel.BasicAck(ea.DeliveryTag, true);
                    };
                    //消费者开启监听
                    //将autoAck设置false 关闭自动确认
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                    Console.ReadKey();
                }
            }
        }
    }
}
