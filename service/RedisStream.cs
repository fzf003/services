

using System.Text.Json;
using StackExchange.Redis;

public record PayLoad(string messageId, string Temp, string Humidity, DateTime TimeStamp)
{
    public static PayLoad Create(string messageId, string Temp, string Humidity, DateTime TimeStamp) => new PayLoad(messageId, Temp, Humidity, TimeStamp);
}

public static class RedisStreamConsole
{

    public static async Task Run()
    {

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        var CancellationToken = cancellationTokenSource.Token;

        ConnectionMultiplexer connectionMultiplexer = await GetConnectionMultiplexer();

        var streamName = "events_stream";
        var streamGroup = "events_consumer_group";
        var ConsumerName = "consumer_2";

        var database = connectionMultiplexer.GetDatabase(5);

        //await database.KeyDeleteAsync(streamName).ConfigureAwait(false);

        if (!await database.KeyExistsAsync(streamName))
        {
            var iscreate = database.StreamCreateConsumerGroup(streamName, streamGroup, StreamPosition.Beginning);
            Console.WriteLine($"{iscreate}已创建");
        }


        static Dictionary<string, string> ParseMessage(StreamEntry entry) => entry.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
         

    var ConsumerInfo = Task.Run(async () =>
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                var pendingInfo = database.StreamPending(streamName, streamGroup);
                Console.WriteLine(pendingInfo.PendingMessageCount);
                Console.WriteLine(pendingInfo.LowestPendingMessageId);
                Console.WriteLine(pendingInfo.HighestPendingMessageId);
                Console.WriteLine($"Consumer count: {pendingInfo.Consumers.Length}.");
                Console.WriteLine(pendingInfo.Consumers.First().Name);
                Console.WriteLine(pendingInfo.Consumers.First().PendingMessageCount);
                await Task.Delay(1000).ConfigureAwait(false);
            }
        });

       // Console.ReadKey();


      /*  var StreamReader = Task.Run(async () =>
        {

            while (!CancellationToken.IsCancellationRequested)
            {
                var messages = await database.StreamRangeAsync(streamName, minId: "-", maxId: "+");

                if (messages.Any())
                {
                    // Console.WriteLine( messages.Select(p=>p.Id).Distinct().Count());

                    foreach (var item in messages)
                    {
                        var dict = ParseMessage(item);

                        foreach (var mitem in dict)
                        {
                            Console.WriteLine(item.Id + "--" + mitem.Key + "---" + mitem.Value);
                        }
                    }

                }

                await Task.Delay(1000).ConfigureAwait(false);
            }

        });*/

    
        var ConsumerGroup = Task.Run(async () =>
        {
            var ParseMessageBody=(StreamEntry entry) => 
            {
                PayLoad payLoad =PayLoad.Create(entry.Id,entry.Values[0].Value.ToString(),entry.Values[1].Value.ToString(),DateTime.Now);
                return payLoad;
            };

            while (!CancellationToken.IsCancellationRequested)
            {
                var messages = database.StreamReadGroup(streamName, streamGroup, ConsumerName, ">", count: 1, noAck: false);

                if (messages.Any())
                {
                    StreamEntry message = messages.First();

                    var payLoad = ParseMessageBody(message);

                    Console.WriteLine("消费者1:"+payLoad);

                   /* var dict = ParseMessage(message);

                    foreach (var item in dict)
                    {
                        Console.WriteLine("消费者1:" + ConsumerName + "--" + message.Id + "--" + item.Key + "---" + item.Value);
                    }*/

                   var  ackId=await database.StreamAcknowledgeAsync(streamName, streamGroup, message.Id).ConfigureAwait(false);

                     Console.WriteLine("AckId:",ackId);
                }

                await Task.Delay(100).ConfigureAwait(false);
            }

        });

        var ConsumerGroup2 = Task.Run(async () =>
        {
             var ParseMessageBody=(StreamEntry entry) => 
            {
                //entry.Values["fzf003"].Value.ToString();
               PayLoad payLoad =PayLoad.Create(entry.Id,entry.Values[0].Value.ToString(),entry.Values[1].Value.ToString(),DateTime.Now);
               return payLoad;
            };
 
            string currconsumername = "consumer_3";
            while (!CancellationToken.IsCancellationRequested)
            {
                var messages = database.StreamReadGroup(streamName, streamGroup, currconsumername, ">", count: 1, noAck: false);

                if (messages.Any())
                {
                    var message = messages.First();

                    Console.WriteLine(message);
 
                    var payLoad = ParseMessageBody(message);//ParseMessage(message);

                    Console.WriteLine("消费者2:"+payLoad);
 
                    /*
                    foreach (var item in dict)
                    {
                        Console.WriteLine("消费者2:" + currconsumername + "--" + message.Id + "--" + item.Key + "---" + item.Value);
                    }*/

                   var ackId= await database.StreamAcknowledgeAsync(streamName, streamGroup, message.Id).ConfigureAwait(false);

                   Console.WriteLine("AckId:",ackId);
                }


                await Task.Delay(100).ConfigureAwait(false);
            }

        });


        var publisher = Task.Run(async () =>
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                Random random = new Random();
                var values = new NameValueEntry[]
                {
            new("sensor_id",random.Next(1,999)),

            new("temp", DateTime.Now.ToString())
                };


               //var message=new NameValueEntry("fzf003",new RedisValue(JsonSerializer.Serialize(new { Temp = random.Next(1, 999), Humidity = random.Next(1, 999), TimeStamp = DateTime.Now })));

               //  await database.StreamAddAsync(streamName,new[]{message}).ConfigureAwait(false);
                await database.StreamAddAsync(streamName,values).ConfigureAwait(false);

                await Task.Delay(1000).ConfigureAwait(false);
            }

        });




        await Task.WhenAll( ConsumerGroup, ConsumerGroup2).ConfigureAwait(false);

        Console.ReadKey();
    }















    static async Task<ConnectionMultiplexer> GetConnectionMultiplexer()
    {
        ConfigurationOptions config = new ConfigurationOptions
        {
            EndPoints =
    {
        { "127.0.0.1", 6381 }
     },

            Password = "xx2021",
             DefaultDatabase=5,
        };
        return await ConnectionMultiplexer.ConnectAsync(config);
    }
}


