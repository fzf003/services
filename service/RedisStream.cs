using StackExchange.Redis;
using System;

class Program
{
    static void Main(string[] args)
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        IDatabase db = redis.GetDatabase();

        string streamKey = "mystream";
        string consumerGroup = "mygroup";

        // 创建消费者组
        db.StreamCreateConsumerGroup(streamKey, consumerGroup, StreamPosition.NewMessages);

        while (true)
        {
            // 读取新的消息
            StreamEntry[] messages = db.StreamReadGroup(streamKey, consumerGroup, "consumer1", count: 1);
          //StreamEntry[] messages = db.StreamReadGroup(streamKey, consumerGroup, "consumer1", block: TimeSpan.FromMinutes(1), count: 1); 阻塞状态

            foreach (var message in messages)
            {
                Console.WriteLine($"Received: {message.Id}");

                // 确认消息
                db.StreamAcknowledge(streamKey, consumerGroup, message.Id);
            }
        }
    }
}
