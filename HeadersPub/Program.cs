using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadersPub
{
    class Program
    {
        /// <summary>
        /// headers 模式
        /// </summary>
        /// <param name="args"></param>
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

                    //设置headers
                    var properties = channel.CreateBasicProperties();
                    properties.Headers = new Dictionary<string, object>();
                    properties.Headers.Add("username", "jack");

                    while (true)
                    {
                        Console.WriteLine("消息内容:");
                        String message = Console.ReadLine();
                        //消息内容
                        byte[] body = Encoding.UTF8.GetBytes(message);
                        //发送消息
                        channel.BasicPublish(exchange: exchangeName, routingKey: string.Empty, basicProperties: properties, body: body);
                        Console.WriteLine("成功发送消息:" + message);
                    }
                }
            }
        }
    }
}
