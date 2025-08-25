using MultiThreadingChannelsPipeline;

var pipeline = new ChannelsPipeline();
using var cts = new CancellationTokenSource();

pipeline.Start(cts.Token);

Console.WriteLine("Press Enter to stop...");
Console.ReadLine();
cts.Cancel();
