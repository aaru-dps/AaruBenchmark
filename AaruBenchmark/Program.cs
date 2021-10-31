using System;
using System.IO;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace AaruBenchmark;

internal static class Program
{
    internal static string Folder => Path.Combine(Environment.CurrentDirectory, "data");

    static void Main(string[] args)
    {
        var config = ManualConfig.Create(DefaultConfig.Instance);

        BenchmarkRunner.Run<ADCBenchs>(config);
        BenchmarkRunner.Run<AppleRleBenchs>(config);
        BenchmarkRunner.Run<TeleDiskLzhBenchs>(config);
        BenchmarkRunner.Run<GzipBenchs>(config);
        BenchmarkRunner.Run<Bzip2Benchs>(config);
        BenchmarkRunner.Run<LzipBenchs>(config);
        BenchmarkRunner.Run<LzmaBenchs>(config);
        BenchmarkRunner.Run<FlacBenchs>(config);
        BenchmarkRunner.Run<CompressGzipBenchs>(config);
        BenchmarkRunner.Run<CompressBzip2Benchs>(config);
        BenchmarkRunner.Run<CompressLzipBenchs>(config);
        BenchmarkRunner.Run<CompressLzmaBenchs>(config);
        BenchmarkRunner.Run<CompressFlacBenchs>(config);
        BenchmarkRunner.Run<Adler32Benchs>(config);
        BenchmarkRunner.Run<Crc16CcittBenchs>(config);
        BenchmarkRunner.Run<Crc16Benchs>(config);
        BenchmarkRunner.Run<Crc32Benchs>(config);
        BenchmarkRunner.Run<Crc64Benchs>(config);
        BenchmarkRunner.Run<Fletcher16Benchs>(config);
        BenchmarkRunner.Run<Fletcher32Benchs>(config);
        BenchmarkRunner.Run<Md5Benchs>(config);
        BenchmarkRunner.Run<Sha1Benchs>(config);
        BenchmarkRunner.Run<Sha256Benchs>(config);
        BenchmarkRunner.Run<Sha384Benchs>(config);
        BenchmarkRunner.Run<Sha512Benchs>(config);
        BenchmarkRunner.Run<SpamSumBenchs>(config);
    }
}