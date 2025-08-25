
namespace MultiThreadingChannelsPipeline
{
    public readonly struct SensorData(double temperature, double voltage, long timestamp)
    {
        public readonly double Temperature = temperature;
        public readonly double Voltage = voltage;
        public readonly long Timestamp = timestamp;
    }
}
