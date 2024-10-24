

using System.IO.Pipelines;
using System.Threading.Tasks;
/*
await ProcessLargeFileAsync("DataItems.xlsx","1.csv");

Console.ReadKey();


 async Task ProcessLargeFileAsync(string inputFilePath, string outputFilePath)
{
    var pipeLine= new PipelineExample();
    using var inputStream = File.OpenRead(inputFilePath);
    using var outputStream = File.OpenWrite(outputFilePath);

    await pipeLine.ProcessDataAsync(inputStream, outputStream);
}
*/
public class PipelineExample
{
    public async Task ProcessDataAsync(Stream inputStream, Stream outputStream)
    {
        var pipe = new Pipe();
        // Start reading from the stream
        var readingTask = FillPipeAsync(inputStream, pipe.Writer);
        // Start writing to the output stream
        var writingTask = ReadPipeAsync(outputStream, pipe.Reader);
        await Task.WhenAll(readingTask, writingTask);
    }
    private async Task FillPipeAsync(Stream inputStream, PipeWriter writer)
    {
        const int minimumBufferSize = 512;
        while (true)
        {
            var memory = writer.GetMemory(minimumBufferSize);
            var bytesRead = await inputStream.ReadAsync(memory);
            if (bytesRead == 0)
            {
                break;
            }
            writer.Advance(bytesRead);
            var result = await writer.FlushAsync();
            if (result.IsCompleted)
            {
                break;
            }
        }
        writer.Complete();
    }
    private async Task ReadPipeAsync(Stream outputStream, PipeReader reader)
    {
        while (true)
        {
            var result = await reader.ReadAsync();
            var buffer = result.Buffer;
            if (buffer.Length > 0)
            {
                foreach (var segment in buffer)
                {
                    await outputStream.WriteAsync(segment);
                }
            }
            reader.AdvanceTo(buffer.End);
            if (result.IsCompleted)
            {
                break;
            }
        }
        reader.Complete();
    }
}
