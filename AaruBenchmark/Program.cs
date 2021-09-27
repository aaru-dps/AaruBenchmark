using BenchmarkDotNet.Running;

namespace AaruBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<GzipBenchs>();
            BenchmarkRunner.Run<Bzip2Benchs>();
            BenchmarkRunner.Run<Adler32Benchs>();
            BenchmarkRunner.Run<Crc16CcittBenchs>();
            BenchmarkRunner.Run<Crc16Benchs>();
            BenchmarkRunner.Run<Crc32Benchs>();
            BenchmarkRunner.Run<Crc64Benchs>();
            BenchmarkRunner.Run<Fletcher16Benchs>();
            BenchmarkRunner.Run<Fletcher32Benchs>();
            BenchmarkRunner.Run<Md5Benchs>();
            BenchmarkRunner.Run<Sha1Benchs>();
            BenchmarkRunner.Run<Sha256Benchs>();
            BenchmarkRunner.Run<Sha384Benchs>();
            BenchmarkRunner.Run<Sha512Benchs>();
            BenchmarkRunner.Run<SpamSumBenchs>();
        }
    }
}