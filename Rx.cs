
static void AutoConnection()
{
    IObservable<long> observable = Observable
            .Interval(TimeSpan.FromSeconds(1))
            .Do(x => Console.WriteLine($"Publishing value {x}"))
            .Take(5)
            .Publish()
            .AutoConnect(2); // 当有两个订阅者时自动连接
    observable.Subscribe(
        x => Console.WriteLine($"Subscriber 1 received value {x}"),
        ex => Console.WriteLine($"Subscriber 1 received error {ex.Message}"),
        () => Console.WriteLine($"Subscriber 1 completed")
    );
    Console.WriteLine("待链接....");
    Thread.Sleep(3000); // 等待 3 秒钟
    Console.WriteLine("已链接....");
    observable.Subscribe(
        x => Console.WriteLine($"Subscriber 2 received value {x}"),
        ex => Console.WriteLine($"Subscriber 2 received error {ex.Message}"),
        () => Console.WriteLine($"Subscriber 2 completed")
    );
}


////重试延迟两秒并重试3次
static void CreateRetryWhen()
{
    Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(100))
              //Observable.Return(2)
              .Select(p => Observable.FromAsync(() => DoSomething(p)))
              .Switch()
              .Do(p => Console.WriteLine($"print:{p}"), err =>
              {
                  Console.WriteLine(err.Message);
              })
              .RetryWhen(CatchError)
              .Subscribe(result => Console.WriteLine($"Operation completed: {result}"),
               ex => Console.WriteLine($"Operation failed: {ex.Message}"),
               () =>
               {
                   Console.WriteLine("完成......");
               });
}

static IObservable<Exception> CatchError(IObservable<Exception> errors)
{
    return errors.Do(ex => Console.WriteLine($"An error occurred: {ex.Message}"))
                 .OfType<CustException>()
                 .Delay(TimeSpan.FromSeconds(2)).Take(10);
}

static async Task<string> DoSomething(long value)
{
    using var httpclient = new HttpClient();

    var response = await httpclient.GetAsync("http://www.baidu.com");

    if (!response.IsSuccessStatusCode)
    {
        throw new Exception();
    }

    Console.WriteLine(value);
    //Random rand = new Random();
    //  int value = rand.Next(0, 10);
    if (value % 2 == 0)
    {
        value++;
        // return -999; 
        //throw new Exception($"{value}出错了");
    }

    return await response.Content.ReadAsStringAsync();
}


///
static async Task CreteReactiveSkipUntil()
{
    var source = Observable.Interval(TimeSpan.FromSeconds(1));
    var trigger = Observable.Timer(TimeSpan.FromSeconds(5));///触发时开始发送数据。
    source.SkipUntil(trigger)
          .Subscribe(Console.WriteLine);
}

////开始发送数据前加Start
static async Task CreteReactivePrepend()
{
    var observable = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
    var result = observable.Select(p => p.ToString()).Prepend("Start");
    result.Subscribe(Console.WriteLine);
}


static async Task CreateETL()
{
    var processRunner = StreamProcessRunner.Create<ProcessArguments>(DefineProcess);

    processRunner.DebugNodeStream += (s, e) =>
    {

        Console.WriteLine(e.NodeName);

    };

    /* var observable = Observable.Timer(TimeSpan.Zero,TimeSpan.FromSeconds(1));

     observable.Select(p=>Observable.FromAsync<ExecutionStatus>(async ()=>await processRunner.ExecuteAsync(new ProcessArguments
     {
         AStringValue = Guid.NewGuid().ToString("N"),
         AnIntValue =p
     }))).Concat()
     .Do(Console.WriteLine)
     .Subscribe();*/



    var res = await processRunner.ExecuteAsync(new ProcessArguments
    {
        AStringValue = Guid.NewGuid().ToString("N"),
        AnIntValue = 564
    });

    Console.WriteLine(res.Failed);

    Console.ReadKey();


}

static void DefineProcess(ISingleStream<ProcessArguments> contextStream)
{
    contextStream.Do("show process params on console", i => Console.WriteLine($"{i.AStringValue}: {i.AnIntValue}"));
}

static async Task CreateTakeUntil()
{
    var observable1 = AsyncObservable.Interval(TimeSpan.FromSeconds(1));
    var observable2 = AsyncObservable.Timer(TimeSpan.FromSeconds(5));///发送完即停止
    var result = observable1.TakeUntil(observable2);
    await result.SubscribeAsync(Console.WriteLine);
}

static async Task Create()
{
    var observable = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5).Publish();
    var refCount = observable.RefCount();
    refCount.Subscribe(x => Console.WriteLine("Observer 1: " + x));
    Thread.Sleep(TimeSpan.FromSeconds(2));
    refCount.Subscribe(x => Console.WriteLine("Observer 2: " + x));

    Thread.Sleep(TimeSpan.FromSeconds(2 * 2));

    refCount.Subscribe(x => Console.WriteLine("Observer 3: " + x));
}

static IObservable<string> CreateStream()
{
    return Observable.Create<string>(async observer =>
    {

        observer.OnNext($"Publish:{Guid.NewGuid().ToString("N")}");
        observer.OnCompleted();



        return Disposable.Empty;

    });
}

static async Task AggregateAsync()
{
    await AsyncObservable.Range(0, 10).Aggregate(0, (sum, x) => sum + x).SubscribeAsync(Console.WriteLine);
}


static async Task ReplaySubjectAsync()
{
    var sub = new SequentialReplayAsyncSubject<int>(5);

    var d1 = await sub.SubscribeAsync(x => Console.WriteLine("1> " + x));

    await sub.OnNextAsync(40);
    await sub.OnNextAsync(41);

    var d2 = await sub.SubscribeAsync(x => Console.WriteLine("2> " + x));

    await sub.OnNextAsync(42);

    await d1.DisposeAsync();

    await sub.OnNextAsync(43);

    var d3 = await sub.SubscribeAsync(x => Console.WriteLine("3> " + x));

    await sub.OnNextAsync(44);
    await sub.OnNextAsync(45);

    await d3.DisposeAsync();

    await sub.OnNextAsync(46);

    await d2.DisposeAsync();

    await sub.OnNextAsync(47);
}


static async Task DelayAsync()
{
    await AsyncObservable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(1000))
                         .Timestamp()
                         .Retry(3)
                         .Delay(TimeSpan.FromSeconds(3))
                         .SubscribeAsync(p =>
                         {
                             Console.WriteLine($"{p.Value}--{p.Timestamp}");
                         });
}

public static class SampleExtensions
{
    public static void Dump<T>(this IObservable<T> source, string name)
    {
        source.Subscribe(
            value =>Console.WriteLine($"{name}-->{value}"), 
            ex => Console.WriteLine($"{name} error-->{ex.Message}"),
            () => Console.WriteLine($"{name} completed"));
    }
}
