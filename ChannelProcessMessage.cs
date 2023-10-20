
Channel<UserInfo> MessageChannel = Channel.CreateUnbounded<UserInfo>(
   new UnboundedChannelOptions
  {
      SingleWriter = true,
      SingleReader = false,
      AllowSynchronousContinuations = true,
  }
);

/*
.CreateBounded<User>(new BoundedChannelOptions(capacity: 1_000)
{
    FullMode = BoundedChannelFullMode.Wait,
    SingleWriter = true,
    SingleReader = false,
    AllowSynchronousContinuations = false,
});*/


_ = Task.Run(async () =>
{
    MessageChannel.Reader.ToObservable().Subscribe(item => Console.WriteLine("One-" + item));
});

_ = Task.Run(async () =>
{
    MessageChannel.Reader.ToObservable().Subscribe(item => Console.WriteLine("two-" + item));
});
