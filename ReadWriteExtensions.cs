

using System.Buffers;
using System.IO.Pipelines;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Channels;

/*
var Observable = AsyncObservable.FromAsync(async () => await Task.FromResult(Guid.NewGuid().ToString()));
await Polling.Poll(Observable).SubscribeAsync(Console.WriteLine, err => Console.WriteLine(err));
*/

 public static partial class Polling
    {
        public static IAsyncObservable<Unit> DefaultPoller = AsyncObservable
            .Timer(TimeSpan.FromMilliseconds(100))
            .Select(_ => Unit.Default);

        internal static IAsyncObservable<T> Poll<T>(this IAsyncObservable<T> query, IAsyncObservable<Unit> poller) => poller
            .SelectMany(_ => query)
            .Repeat();

        internal static IAsyncObservable<T> Poll<T>(this IAsyncObservable<T> query) => query.Poll(DefaultPoller);
    }


public static class ReadWriteExtensions
{
  
    public static Task WithCancellation(this Task task, CancellationToken cancellationToken)
    {
        if (task.IsCompleted || !cancellationToken.CanBeCanceled)
            return task;

        var tcs = new TaskCompletionSource<object>();
        var r = cancellationToken.Register(() => { tcs.SetCanceled(); }, false);

        return Task.WhenAny(task, tcs.Task)

            .ContinueWith(_ => { r.Dispose(); }, TaskContinuationOptions.ExecuteSynchronously)

            .ContinueWith(_ => { }, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    public static IObservable<T> ToObservable<T>(this IAsyncEnumerable<T> asyncEnumerable, CancellationToken cancellation = default)
    {

        return Observable.Create<T>(async observer =>
       {
           while (!cancellation.IsCancellationRequested)
           {
               await foreach (var item in asyncEnumerable)
               {
                   observer.OnNext(item);
               }
               observer.OnCompleted();
           }

       });
    }

    public static IObservable<T> ToObservable<T>(this ChannelReader<T> channelReader, CancellationToken cancellation = default)
    {
        return Observable.Create<T>(async observer =>
        {
            while (!cancellation.IsCancellationRequested)
            {
                await foreach (var item in channelReader.ReadAllAsync(cancellation))
                {
                    observer.OnNext(item);
                }
                observer.OnCompleted();
            }

        });
    }


    public static IObservable<TMessage> ToObservable<TMessage>(this PipeReader pipeReader, CancellationToken cancellation = default) where TMessage : class
    {

        return Observable.Create<TMessage>(async observer =>
         {
             while (true)
             {
                 var result = await pipeReader.ReadAsync(cancellation);
                 var buffer = result.Buffer;
                 while (TryReadMessage(ref buffer, out var message))
                 {
                     observer.OnNext(message);
                 }
                 pipeReader.AdvanceTo(buffer.Start, buffer.End);

                 if (result.IsCanceled)
                 {
                     Console.WriteLine($"读取器已取消:");
                     break;
                 }

                 if (result.IsCompleted)
                 {
                     Console.WriteLine($"读取器已读取完成:");
                     break;
                 }
             }
             await pipeReader.CompleteAsync();
             observer.OnCompleted();
             Console.WriteLine("observer 完成");

         });

        static bool TryReadMessage(ref ReadOnlySequence<byte> buffer, out TMessage message)
        {
            SequencePosition? position = buffer.PositionOf((byte)'\r');

            if (position == null)
            {
                message = default;
                return false;
            }


            var Buffers = buffer.Slice(0, position.Value);

            #region  消息解析

            if (typeof(string) == typeof(TMessage))
            {
                message = Encoding.UTF8.GetString(Buffers) as TMessage;
            }
            else
            {
                message = System.Text.Json.JsonSerializer.Deserialize<TMessage>(Encoding.UTF8.GetString(Buffers));
            }
            #endregion
            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
            return true;
        }
    }

    public static ValueTask<FlushResult> WriteMessageAsync<T>(this PipeWriter writer, T message) where T : class
    {
        var sendmessage = System.Text.Json.JsonSerializer.Serialize(message);
        return WriteMessageAsync(writer, sendmessage);
    }

    public static ValueTask<FlushResult> WriteMessageAsync(this PipeWriter writer, string message)
    {
        var bytes = Encoding.UTF8.GetBytes($"{message}\r");
        writer.WriteAsync(bytes);
        return writer.FlushAsync();
    }

    public static async Task ReadAndProcessAsync<TMessage>(this PipeReader reader, Func<TMessage, Task> handler)
        where TMessage : class
    {
        while (true)
        {
            var result = await reader.ReadAsync();
            var buffer = result.Buffer;
            while (TryReadMessage(ref buffer, out var message))
            {
                await handler(message!);
            }
            reader.AdvanceTo(buffer.Start, buffer.End);
            if (result.IsCompleted)
            {
                break;
            }
        }


        static bool TryReadMessage(ref ReadOnlySequence<byte> buffer, out TMessage? message)
        {
            SequencePosition? position = buffer.PositionOf((byte)'\r');

            if (position == null)
            {
                message = default;
                return false;
            }

            if (typeof(string) == typeof(TMessage))
            {
                message = Encoding.UTF8.GetString(buffer.Slice(0, position.Value)) as TMessage;
            }
            else
            {
                message = System.Text.Json.JsonSerializer.Deserialize<TMessage>(Encoding.UTF8.GetString(buffer.Slice(0, position.Value)));
            }

            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
            return true;
        }
    }
}
