using AaruBenchmark.Checksums;
using AaruBenchmark.Compression;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace AaruBenchmark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    public class AppleRleBenchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.Aaru.AppleRle();

        [Benchmark]
        public void Aaru6() => Aaru6Compressions.AppleRle();

        [Benchmark]
        public void AaruNative() => Compression.AaruNative.AppleRle();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class TeleDiskLzhBenchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.Aaru.TeleDiskLzh();

        [Benchmark]
        public void Aaru6() => Aaru6Compressions.TeleDiskLzh();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class ADCBenchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Compression.SharpCompress.ADC();

        [Benchmark]
        public void Aaru6() => Aaru6Compressions.ADC();

        [Benchmark]
        public void AaruNative() => Compression.AaruNative.ADC();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class GzipBenchs
    {
        [Benchmark]
        public void SharpCompress() => Compression.SharpCompress.Gzip();

        [Benchmark(Baseline = true)]
        public void DotNetRuntime() => NetRuntime.Gzip();

        [Benchmark]
        public void DotNetZip() => Compression.DotNetZip.Gzip();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Bzip2Benchs
    {
        [Benchmark(Baseline = true)]
        public void SharpCompress() => Compression.SharpCompress.Bzip2();

        [Benchmark]
        public void DotNetZip() => Compression.DotNetZip.Bzip2();

        [Benchmark]
        public void AaruNative() => Compression.AaruNative.Bzip2();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Adler32Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Adler32();

        [Benchmark]
        public void Aaru6() => Checksums.Aaru6.Adler32();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Adler32();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Fletcher16Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Fletcher16();

        [Benchmark]
        public void Aaru6() => Checksums.Aaru6.Fletcher16();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Fletcher16();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Fletcher32Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Fletcher32();

        [Benchmark]
        public void Aaru6() => Checksums.Aaru6.Fletcher32();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Fletcher32();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Crc16CcittBenchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Crc16Ccitt();

        [Benchmark]
        public void Aaru6() => Checksums.Aaru6.Crc16Ccitt();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Crc16Ccitt();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Crc16Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Crc16();

        [Benchmark]
        public void Aaru6() => Checksums.Aaru6.Crc16();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Crc16();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Crc32Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Crc32();

        [Benchmark]
        public void Aaru6() => Checksums.Aaru6.Crc32();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Crc32();

        [Benchmark]
        public void rhash() => RHash.Crc32();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Crc64Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Crc64();

        [Benchmark]
        public void Aaru6() => Checksums.Aaru6.Crc64();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.Crc64();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Md5Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Md5();

        [Benchmark]
        public void OpenSSL() => OpenSsl.Md5();

        [Benchmark]
        public void rhash() => RHash.Md5();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Sha1Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Sha1();

        [Benchmark]
        public void OpenSSL() => OpenSsl.Sha1();

        [Benchmark]
        public void rhash() => RHash.Sha1();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Sha256Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Sha256();

        [Benchmark]
        public void OpenSSL() => OpenSsl.Sha256();

        [Benchmark]
        public void rhash() => RHash.Sha256();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Sha384Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Sha384();

        [Benchmark]
        public void OpenSSL() => OpenSsl.Sha384();

        [Benchmark]
        public void rhash() => RHash.Sha384();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Sha512Benchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.Sha512();

        [Benchmark]
        public void OpenSSL() => OpenSsl.Sha512();

        [Benchmark]
        public void rhash() => RHash.Sha512();
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class SpamSumBenchs
    {
        [Benchmark(Baseline = true)]
        public void Aaru() => Checksums.Aaru.SpamSum();

        [Benchmark]
        public void Aaru6() => Checksums.Aaru6.SpamSum();

        [Benchmark]
        public void AaruNative() => Checksums.AaruNative.SpamSum();

        [Benchmark]
        public void ssdeep() => Checksums.Aaru.CliSpamSum();
    }
}