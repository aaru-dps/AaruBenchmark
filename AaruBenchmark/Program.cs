using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using AaruBenchmark.Checksums;

namespace AaruBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {

                    var summary = BenchmarkRunner.Run<GzipBenchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
                     //   var summary = BenchmarkRunner.Run<Bzip2Benchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
            //var foo = new BenchmarkRunner();
            //Checksums.Aaru.Crc64();

        //Summary summary = BenchmarkRunner.Run<Adler32Benchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
        //Summary summary = BenchmarkRunner.Run<Crc16CcittBenchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
        //Summary summary = BenchmarkRunner.Run<Crc16Benchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
       // Summary summary = BenchmarkRunner.Run<Crc32Benchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
        //Summary summary = BenchmarkRunner.Run<Crc64Benchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
        //Summary summary = BenchmarkRunner.Run<Fletcher16Benchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
        //Summary summary = BenchmarkRunner.Run<Fletcher32Benchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
        //var summary = BenchmarkRunner.Run<Md5Benchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
        //var summary = BenchmarkRunner.Run<Sha1Benchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
        //var summary = BenchmarkRunner.Run<Sha256Benchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
        //var summary = BenchmarkRunner.Run<Sha384Benchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
        //var summary = BenchmarkRunner.Run<Sha512Benchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
        //var summary = BenchmarkRunner.Run<SpamSumBenchs>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
        }
    }
}