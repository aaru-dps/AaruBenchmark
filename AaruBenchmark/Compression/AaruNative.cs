using System.IO;
using System.Runtime.InteropServices;
using Aaru6.Checksums;

namespace AaruBenchmark.Compression
{
    public class AaruNative
    {
        [DllImport("libAaru.Compression.Native", SetLastError = true)]
        static extern int apple_rle_decode_buffer(byte[] dst_buffer, int dst_size, byte[] src_buffer, int src_size);

        [DllImport("libAaru.Compression.Native", SetLastError = true)]
        static extern int adc_decode_buffer(byte[] dst_buffer, int dst_size, byte[] src_buffer, int src_size);

        [DllImport("libAaru.Compression.Native", SetLastError = true)]
        static extern int BZ2_bzBuffToBuffDecompress(byte[] dest, ref uint destLen, byte[] source, uint sourceLen,
                                                     int small, int verbosity);

        public static void AppleRle()
        {
            const int bufferSize = 32768;
            byte[]    input      = new byte[1102];

            var fs = new FileStream(Path.Combine(Program.Folder, "apple_rle.bin"), FileMode.Open, FileAccess.Read);

            fs.Read(input, 0, input.Length);
            fs.Close();
            fs.Dispose();

            byte[] output = new byte[bufferSize];

            int realSize = apple_rle_decode_buffer(output, output.Length, input, input.Length);

            if(realSize != 20960)
                throw new InvalidDataException("Incorrect decompressed size");

            string crc = Crc32Context.Data(output, (uint)realSize, out _);

            if(crc != "3525ef06")
                throw new InvalidDataException("Incorrect decompressed checksum");
        }

        public static void ADC()
        {
            const int bufferSize = 327680;
            byte[]    input      = new byte[34367];

            var fs = new FileStream(Path.Combine(Program.Folder, "adc.bin"), FileMode.Open, FileAccess.Read);

            fs.Read(input, 0, input.Length);
            fs.Close();
            fs.Dispose();

            byte[] output = new byte[bufferSize];

            int realSize = adc_decode_buffer(output, output.Length, input, input.Length);

            if(realSize != 262144)
                throw new InvalidDataException("Incorrect decompressed size");

            string crc = Crc32Context.Data(output, (uint)realSize, out _);

            if(crc != "5a5a7388")
                throw new InvalidDataException("Incorrect decompressed checksum");
        }

        public static void Bzip2()
        {
            const int bufferSize = 1048576;
            byte[]    input      = new byte[1053934];

            var fs = new FileStream(Path.Combine(Program.Folder, "bzip2.bz2"), FileMode.Open, FileAccess.Read);

            fs.Read(input, 0, input.Length);
            fs.Close();
            fs.Dispose();

            byte[] output = new byte[bufferSize];

            uint realSize = (uint)output.Length;
            int  bzError  = BZ2_bzBuffToBuffDecompress(output, ref realSize, input, (uint)input.Length, 0, 0);

            if(realSize != 1048576)
                throw new InvalidDataException("Incorrect decompressed size");

            string crc = Crc32Context.Data(output, realSize, out _);

            if(crc != "c64059c0")
                throw new InvalidDataException("Incorrect decompressed checksum");
        }
    }
}