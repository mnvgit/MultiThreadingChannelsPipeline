MultiThreadingChannelsPipeline</br>

This project demonstrates a multi-threaded processing pipeline in .NET using System.Threading.Channels.
It simulates a real-world scenario where raw sensor readings are collected, processed, published, and persisted — all concurrently — using separate tasks for each pipeline stage.
</br></br>

📌 Features

Producer–Consumer pattern implemented via Channel<T>.</br>
Bounded channels (capacity-limited) to apply backpressure and avoid unbounded memory growth.</br>
Pipeline architecture with four concurrent stages:</br>
SensorReader → Simulates sensor hardware by generating random temperature & voltage readings.</br>
SensorDataProcessor → Consumes sensor data in batches, computes aggregate values, and outputs processed results.</br>
ProcessedDataEventPublisher → Simulates publishing processed data (e.g., to UI, network, or message bus).</br>
ProcessedDataDatabaseWriter → Simulates storing processed data asynchronously in a database.

</br></br>

🛠️ How It Works

Each pipeline stage runs in its own task.</br>
A CancellationToken is passed to stop the pipeline gracefully.</br>
Logging to Console shows which stage is working and what data flows through the channels.
</br>

Example log:</br>
[SensorReader]: Write sensor data to sensor channel: Temperature=72.13, Voltage=4.37, Timestamp=638610129745555444</br>
[SensorDataProcessor]: Get sensor data batch and store processed data to processed channel: AvgTemp=71.56, PeakVolt=5.42</br>
[ProcessedDataEventPublisher]: Publish processed data event: AvgTemp=71.56, PeakVolt=5.42</br>
[ProcessedDataDatabaseWriter]: Write processed data to DB: AvgTemp=71.56, PeakVolt=5.42</br>

</br></br>

⚠️ Important: Channels Are Not Durable

System.Threading.Channels are in-memory only.</br>
This means:</br>
If the application crashes, the process restarts, or an exception is unhandled, messages in the channel are lost.</br>
Channels provide no persistence or replay guarantees.
</br>
👉 To handle this:</br>
Use a durable message broker (e.g., RabbitMQ, Kafka, Azure Service Bus) if reliability and persistence are required.</br>
Add retry and error-handling logic around consumers/writers.</br>
Consider writing critical sensor data directly to a database or persistent queue before further processing.

</br></br>
📚 Why Use Channels?

Safe communication between producer/consumer tasks.</br>
Backpressure support with bounded channels.</br>
High performance vs. alternatives like BlockingCollection<T> or raw locks/queues.</br>
Built-in async support (ReadAsync, WriteAsync).</br>
Great for ephemeral, high-throughput pipelines where occasional data loss is acceptable.
</br></br></br>

🔧 Possible Real-World Applications

Sensor data acquisition in semiconductors, robotics, or IoT.</br>
Data pipelines with batch processing.</br>
Event-driven processing with multiple independent consumers.</br>
High-throughput telemetry pipelines (e.g., processing millions of signals/second).
