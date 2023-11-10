
 
CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
var CancellationToken = cancellationTokenSource.Token;

ConnectionMultiplexer connectionMultiplexer = await GetConnectionMultiplexer();

var streamName = "events_stream";
var streamGroup = "events_consumer_group";
var ConsumerName = "consumer_2";

var database = connectionMultiplexer.GetDatabase(5);


if (!await database.KeyExistsAsync(streamName))
{
    var iscreate = database.StreamCreateConsumerGroup(streamName, streamGroup, StreamPosition.Beginning);
    Console.WriteLine($"{iscreate}已创建");
}


static Dictionary<string, string> ParseMessage(StreamEntry entry) => entry.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
 
/*
var StreamReader = Task.Run(async () =>
{
    while (!CancellationToken.IsCancellationRequested)
    {
        var messages = await database.StreamRangeAsync(streamName,
        minId: "0-0",
        maxId: "+",
        count: 10,
        messageOrder: Order.Descending);

        if (messages.Any())
        {
            var message = messages.First();

            var dict = ParseMessage(message);

            foreach (var item in dict)
            {
                Console.WriteLine(message.Id + "--" + item.Key + "---" + item.Value);
            }

        }

        await Task.Delay(1000).ConfigureAwait(false);
    }

});*/

var ConsumerGroup = Task.Run(async () =>
{
    while (!CancellationToken.IsCancellationRequested)
    {
       var messages= database.StreamReadGroup(streamName, streamGroup, ConsumerName, ">", count: 5,noAck:false);

       if(messages.Any())
       {
            var message = messages.First();

            var dict = ParseMessage(message);

            foreach (var item in dict)
            {
                Console.WriteLine("消费者:"+ConsumerName+"--"+message.Id + "--" + item.Key + "---" + item.Value);
            }

          await  database.StreamAcknowledgeAsync(streamName,streamGroup,message.Id).ConfigureAwait(false);
       }

        await Task.Delay(100).ConfigureAwait(false);
     }

});

var ConsumerGroup2 = Task.Run(async () =>
{
    string currconsumername="consumer_3";
    while (!CancellationToken.IsCancellationRequested)
    {
       var messages= database.StreamReadGroup(streamName, streamGroup, currconsumername, ">", count: 5,noAck:false);

       if(messages.Any())
       {
            var message = messages.First();

            var dict = ParseMessage(message);

            foreach (var item in dict)
            {
                Console.WriteLine("消费者:"+currconsumername+"--"+message.Id + "--" + item.Key + "---" + item.Value);
            }

            await  database.StreamAcknowledgeAsync(streamName,streamGroup,message.Id).ConfigureAwait(false);
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

         await database.StreamAddAsync(streamName, values).ConfigureAwait(false);

        await Task.Delay(500).ConfigureAwait(false);
    }

});




await Task.WhenAll(publisher, ConsumerGroup,ConsumerGroup2).ConfigureAwait(false);

Console.ReadKey();

