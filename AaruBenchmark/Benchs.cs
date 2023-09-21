using System;
using AaruBenchmark.Checksums;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;
#if NET8_0_OR_GREATER
using AaruBenchmark.Compression;

// ReSharper disable ArrangeNamespaceBody
#endif

namespace AaruBenchmark
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
    public class Core31RosettaJobAttribute : Attribute, IConfigSource
    {
        public Core31RosettaJobAttribute()
        {
            Job job = new Job(".NET Core 3.1 (x64)", RunMode.Default).WithRuntime(CoreRuntime.Core31);

            NetCoreAppSettings dotnetCli32Bit =
                NetCoreAppSettings.NetCoreApp31.WithCustomDotNetCliPath(@"/usr/local/share/dotnet/x64/dotnet", "x64");

            Config = ManualConfig.CreateEmpty().AddJob(job.WithToolchain(CsProjCoreToolchain.From(dotnetCli32Bit)));
        }

        public IConfig Config { get; }
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class ADCBenchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => Aaru6Compressions.ADC();

        [Benchmark]
        public void AaruNative() => Compression.AaruNative.ADC();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.SharpCompress.ADC();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();

        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class AppleRleBenchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => Aaru6Compressions.AppleRle();

        [Benchmark]
        public void AaruNative() => Compression.AaruNative.AppleRle();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.Aaru.AppleRle();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();

        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class TeleDiskLzhBenchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => Aaru6Compressions.TeleDiskLzh();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.Aaru.TeleDiskLzh();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class GzipBenchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => NetRuntime.Gzip();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.SharpCompress.Gzip();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();
    #endif

        [Benchmark]
        public void DotNetZip() => Compression.DotNetZip.Gzip();
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class CompressGzipBenchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => NetRuntime.CompressGzip();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.SharpCompress.CompressGzip();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();
    #endif

        [Benchmark]
        public void DotNetZip() => Compression.DotNetZip.CompressGzip();
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class Bzip2Benchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => DotNetZip.Bzip2();

        [Benchmark]
        public void AaruNative() => Compression.AaruNative.Bzip2();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.SharpCompress.Bzip2();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();

        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class CompressBzip2Benchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => DotNetZip.CompressBzip2();

        [Benchmark]
        public void AaruNative() => Compression.AaruNative.CompressBzip2();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.SharpCompress.CompressBzip2();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();

        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class LzipBenchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.SharpCompress.Lzip();

    #if NET8_0_OR_GREATER
        [Benchmark]
        public void AaruNative() => Compression.AaruNative.Lzip();
    #else
        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class CompressLzipBenchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.SharpCompress.CompressLzip();

    #if NET8_0_OR_GREATER
        [Benchmark]
        public void AaruNative() => Compression.AaruNative.CompressLzip();
    #else
        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class LzmaBenchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.SharpCompress.Lzma();
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void AaruNative() => Compression.AaruNative.Lzma();
    #else
        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class CompressLzmaBenchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.SharpCompress.CompressLzma();

    #if NET8_0_OR_GREATER
        [Benchmark]
        public void AaruNative() => Compression.AaruNative.CompressLzma();
    #else
        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class FlacBenchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.Aaru.Flac();

    #if NET8_0_OR_GREATER
        [Benchmark]
        public void AaruNative() => Compression.AaruNative.Flac();
    #else
        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class CompressFlacBenchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.Aaru.CompressFlac();

    #if NET8_0_OR_GREATER
        [Benchmark]
        public void AaruNative() => Compression.AaruNative.CompressFlac();
    #else
        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class Adler32Benchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => Checksums.Aaru6.Adler32();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Adler32();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Adler32();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();

        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class Fletcher16Benchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => Checksums.Aaru6.Fletcher16();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Fletcher16();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Fletcher16();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();

        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class Fletcher32Benchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => Checksums.Aaru6.Fletcher32();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Fletcher32();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Fletcher32();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();

        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class Crc16CcittBenchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => Checksums.Aaru6.Crc16Ccitt();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Crc16Ccitt();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Crc16Ccitt();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();

        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class Crc16Benchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => Checksums.Aaru6.Crc16();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Crc16();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Crc16();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();

        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class Crc32Benchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => Checksums.Aaru6.Crc32();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Crc32();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Crc32();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();

        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif

        [Benchmark]
        public void rhash() => RHash.Crc32();
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class Crc64Benchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => Checksums.Aaru6.Crc64();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Crc64();
    #else
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Crc64();

        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();

        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class Md5Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Md5();

        [Benchmark]
        public void OpenSSL() => OpenSsl.Md5();

        [Benchmark]
        public void rhash() => RHash.Md5();
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class Sha1Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Sha1();

        [Benchmark]
        public void OpenSSL() => OpenSsl.Sha1();

        [Benchmark]
        public void rhash() => RHash.Sha1();
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class Sha256Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Sha256();

        [Benchmark]
        public void OpenSSL() => OpenSsl.Sha256();

        [Benchmark]
        public void rhash() => RHash.Sha256();
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class Sha384Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Sha384();

        [Benchmark]
        public void OpenSSL() => OpenSsl.Sha384();

        [Benchmark]
        public void rhash() => RHash.Sha384();
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class Sha512Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Sha512();

        [Benchmark]
        public void OpenSSL() => OpenSsl.Sha512();

        [Benchmark]
        public void rhash() => RHash.Sha512();
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31), Core31RosettaJob, SimpleJob(RuntimeMoniker.Net80),
     SimpleJob(RuntimeMoniker.NativeAot80)]
    public class SpamSumBenchs
    {
    #if NET8_0_OR_GREATER
        [Benchmark]
        public void Aaru() => throw new NotImplementedException();

        [Benchmark(Baseline = true)]
        public void Aaru6() => Checksums.Aaru6.SpamSum();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.SpamSum();
    #else
        [Benchmark]
        public void Aaru6() => throw new NotImplementedException();

        [Benchmark]
        public void AaruNative() => throw new NotImplementedException();
    #endif

        [Benchmark]
        public void ssdeep() => Checksums.Aaru.CliSpamSum();
    }
}