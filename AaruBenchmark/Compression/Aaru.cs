using System.IO;
using Aaru.Compression;
using Aaru6.Checksums;

namespace AaruBenchmark.Compression
{
    public class Aaru
    {
        const int BUFFER_SIZE = 20960;

        public static void AppleRle()
        {
            byte[] input = new byte[1102];

            var fs = new FileStream(Path.Combine(Program.Folder, "apple_rle.bin"), FileMode.Open, FileAccess.Read);

            fs.Read(input, 0, input.Length);
            fs.Close();
            fs.Dispose();

            byte[] output = new byte[BUFFER_SIZE];

            var rle = new AppleRle(new MemoryStream(input));

            for(int i = 0; i < BUFFER_SIZE; i++)
                output[i] = (byte)rle.ProduceByte();

            string crc = Crc32Context.Data(output, out _);

            if(crc != "3525ef06")
                throw new InvalidDataException("Incorrect decompressed checksum");
        }
    }
}