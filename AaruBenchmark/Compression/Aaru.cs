using System.IO;
using Aaru.Compression;
using Aaru6.Checksums;
using CUETools.Codecs;
using CUETools.Codecs.Flake;

namespace AaruBenchmark.Compression
{
    public class Aaru
    {
        public static void AppleRle()
        {
            const int bufferSize = 20960;

            byte[] input = new byte[1102];

            var fs = new FileStream(Path.Combine(Program.Folder, "apple_rle.bin"), FileMode.Open, FileAccess.Read);

            fs.Read(input, 0, input.Length);
            fs.Close();
            fs.Dispose();

            byte[] output = new byte[bufferSize];

            var rle = new AppleRle(new MemoryStream(input));

            for(int i = 0; i < bufferSize; i++)
                output[i] = (byte)rle.ProduceByte();

            string crc = Crc32Context.Data(output, out _);

            if(crc != "3525ef06")
                throw new InvalidDataException("Incorrect decompressed checksum");
        }

        public static void TeleDiskLzh()
        {
            const int bufsz = 512;

            byte[] input = new byte[9040];

            var fs = new FileStream(Path.Combine(Program.Folder, "teledisk_lzh.bin"), FileMode.Open, FileAccess.Read);

            fs.Read(input, 0, input.Length);
            fs.Close();
            fs.Dispose();

            int rd;
            int total_rd = 0;
            var lzh      = new TeleDiskLzh(new MemoryStream(input));
            var outMs    = new MemoryStream();

            do
            {
                if((rd = lzh.Decode(out byte[] obuf, bufsz)) > 0)
                    outMs.Write(obuf, 0, rd);

                total_rd += rd;
            } while(rd == bufsz);

            byte[] output = outMs.ToArray();

            if(total_rd != 39820)
                throw new InvalidDataException("Incorrect decompressed data");

            if(output.Length != 39820)
                throw new InvalidDataException("Incorrect decompressed data");

            string crc = Crc32Context.Data(output, out _);

            if(crc != "22bd5d44")
                throw new InvalidDataException("Incorrect decompressed checksum");
        }

        public static void Flac()
        {
            var    flacMs = new FileStream(Path.Combine(Program.Folder, "flac.flac"), FileMode.Open, FileAccess.Read);
            var    flakeReader = new AudioDecoder(new DecoderSettings(), "", flacMs);
            byte[] block = new byte[9633792];
            int    samples = block.Length / 2352 * 588;
            var    audioBuffer = new AudioBuffer(AudioPCMConfig.RedBook, block, samples);
            flakeReader.Read(audioBuffer, samples);
            flakeReader.Close();
            flacMs.Close();

            string crc = Crc32Context.Data(block, out _);

            if(crc != "dfbc99bb")
                throw new InvalidDataException("Incorrect decompressed checksum");
        }
    }
}