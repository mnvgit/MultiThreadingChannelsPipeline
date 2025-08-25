
namespace MultiThreadingChannelsPipeline
{
    public readonly struct ProcessedData(double avgTemp, double peakVoltage, long timestamp)
    {
        public readonly double AvgTemp = avgTemp;
        public readonly double PeakVoltage = peakVoltage;
        public readonly long Timestamp = timestamp;
    }
}
