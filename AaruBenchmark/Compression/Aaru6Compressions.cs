#if NET7_0
using System.IO;
using Aaru6.Checksums;
using Aaru6.Compression;

namespace AaruBenchmark.Compression;

public class Aaru6Compressions
{
    public static void AppleRle()
    {
        const int bufferSize = 32768;
        byte[]    input      = new byte[1102];

        var fs = new FileStream(Path.Combine(Program.Folder, "apple_rle.bin"), FileMode.Open, FileAccess.Read);

        fs.Read(input, 0, input.Length);
        fs.Close();
        fs.Dispose();

        byte[] output = new byte[bufferSize];

        int realSize = Aaru6.Compression.AppleRle.DecodeBuffer(input, output);

        if(realSize != 20960)
            throw new InvalidDataException("Incorrect decompressed size");

        string crc = Crc32Context.Data(output, (uint)realSize, out _);

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

    public static void ADC()
    {
        const int bufferSize = 262144;
        byte[]    input      = new byte[34367];

        var fs = new FileStream(Path.Combine(Program.Folder, "adc.bin"), FileMode.Open, FileAccess.Read);

        fs.Read(input, 0, input.Length);
        fs.Close();
        fs.Dispose();

        byte[] output = new byte[bufferSize];

        int realSize = Aaru6.Compression.ADC.DecodeBuffer(input, output);

        if(realSize != 262144)
            throw new InvalidDataException("Incorrect decompressed size");

        string crc = Crc32Context.Data(output, (uint)realSize, out _);

        if(crc != "5a5a7388")
            throw new InvalidDataException("Incorrect decompressed checksum");
    }
}
#endif