using System.Threading.Channels;

namespace MultiThreadingChannelsPipeline
{
    internal class ChannelsPipeline
    {
        private readonly Channel<SensorData> _sensorDataChannel = Channel.CreateBounded<SensorData>(1000);
        private readonly Channel<ProcessedData> _processedDataChannel = Channel.CreateBounded<ProcessedData>(1000);

        // Start the multithreading pipeline.
        // Each stage runs in its own task.
        // [SensorReader]: simulating reading from a sensor and writing to sensor channel
        // [SensorDataProcessor]: read from sensor channel, process, write to processed channel
        // [ProcessedDataEventPublisher]: read from processed channel, send to UI or network
        // [ProcessedDataDatabaseWriter]: read from processed channel, write to database
        public void Start(CancellationToken ct)
        {
            Task.Run(() => SensorReader(ct), ct);

            Task.Run(() => SensorDataProcessor(ct), ct);

            Task.Run(() => ProcessedDataEventPublisher(ct), ct);

            Task.Run(() => ProcessedDataDatabaseWriter(ct), ct);
        }

        // Sensor Reader, simulating reading from a sensor and writing to channel
        private async Task SensorReader(CancellationToken ct)
        {
            var rand = new Random();
            while (!ct.IsCancellationRequested)
            {
                // Simulate reading from a sensor
                var data = new SensorData(rand.NextDouble() * 100, rand.NextDouble() * 10, DateTime.UtcNow.Ticks);
                // Write sensor data to channel
                await _sensorDataChannel.Writer.WriteAsync(data, ct);
                Console.WriteLine($"[SensorReader]: Store sensor data to sensor channel: Temperature={data.Temperature:F2}, Voltage={data.Voltage:F2}, Timestamp={data.Timestamp}");
                await Task.Delay(5000, ct);
            }
        }

        // Sensor Data Processor (read from sensor channel, process, write to processed channel)
        private async Task SensorDataProcessor(CancellationToken ct)
        {
            // Batch processing to reduce overhead
            var batch = new List<SensorData>();
            while (!ct.IsCancellationRequested)
            {
                batch.Clear();
                while (batch.Count < 10 && _sensorDataChannel.Reader.TryRead(out var data))
                {
                    batch.Add(data);
                }

                if (batch.Count == 0)
                {
                    var data = await _sensorDataChannel.Reader.ReadAsync(ct);
                    batch.Add(data);
                }

                // Takes sensor batch data and computes average temperature and peak voltage 
                double avgTemp = 0;
                double peakVolt = double.MinValue;
                foreach (var d in batch)
                {
                    avgTemp += d.Temperature;
                    if (d.Voltage > peakVolt) 
                        peakVolt = d.Voltage;
                }
                avgTemp /= batch.Count;

                var processed = new ProcessedData(avgTemp, peakVolt, DateTime.UtcNow.Ticks);
                await _processedDataChannel.Writer.WriteAsync(processed, ct);
                Console.WriteLine($"[SensorDataProcessor]: Get sensor data batch and store to processed channel: AvgTemp={processed.AvgTemp:F2}, PeakVolt={processed.PeakVoltage:F2}");
                await Task.Delay(5000, ct);
            }
        }

        // Processed Data Event Publisher (read from processed channel, send to UI or network)
        private async Task ProcessedDataEventPublisher(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var processed = await _processedDataChannel.Reader.ReadAsync(ct);
                // Send to UI or network
                Console.WriteLine($"[ProcessedDataEventPublisher]: Publish processed data event: AvgTemp={processed.AvgTemp:F2}, PeakVolt={processed.PeakVoltage:F2}");
                await Task.Delay(5000, ct);
            }
        }

        // Processed Data Database Writer (read from processed channel, write to database)
        private async Task ProcessedDataDatabaseWriter(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var processed = await _processedDataChannel.Reader.ReadAsync(ct);
                // Simulate async DB insert
                Console.WriteLine($"[ProcessedDataDatabaseWriter]: Save processed data to DB: AvgTemp={processed.AvgTemp:F2}, PeakVolt={processed.PeakVoltage:F2}");
                await Task.Delay(5000, ct);
            }
        }
    }
}
