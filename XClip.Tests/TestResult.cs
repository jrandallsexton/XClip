
namespace XClip.Tests
{
    public struct TestResult
    {
        public string Url { get; set; }
        public double TotalSeconds { get; set; }
        public long TotalBytes { get; set; }
        public double BytesPerSecond { get; set; }
        public string Method { get; set; }
    }
}