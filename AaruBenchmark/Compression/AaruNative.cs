#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using Aaru6.Checksums;

namespace AaruBenchmark.Compression;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class AaruNative
{
    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern int AARU_adc_decode_buffer(byte[] dst_buffer, int dst_size, byte[] src_buffer, int src_size);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern int AARU_apple_rle_decode_buffer(byte[] dst_buffer, int dst_size, byte[] src_buffer, int src_size);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern nuint AARU_flac_decode_redbook_buffer(byte[] dst_buffer, nuint dst_size, byte[] src_buffer,
                                                        nuint src_size);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern nuint AARU_flac_encode_redbook_buffer(byte[] dst_buffer, nuint dst_size, byte[] src_buffer,
                                                        nuint src_size, uint blocksize, int do_mid_side_stereo,
                                                        int loose_mid_side_stereo, string apodization,
                                                        uint max_lpc_order, uint qlp_coeff_precision,
                                                        int do_qlp_coeff_prec_search, int do_exhaustive_model_search,
                                                        uint min_residual_partition_order,
                                                        uint max_residual_partition_order, string application_id,
                                                        uint application_id_len);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern int AARU_lzip_decode_buffer(byte[] dst_buffer, int dst_size, byte[] src_buffer, int src_size);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern int AARU_lzip_encode_buffer(byte[] dst_buffer, int dst_size, byte[] src_buffer, int src_size,
                                              int dictionary_size, int match_len_limit);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern int AARU_bzip2_decode_buffer(byte[] dst_buffer, ref uint dst_size, byte[] src_buffer, uint src_size);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern int AARU_bzip2_encode_buffer(byte[] dst_buffer, ref uint dst_size, byte[] src_buffer, uint src_size,
                                               int blockSize100k);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern nuint AARU_lzfse_decode_buffer(byte[] dst_buffer, nuint dst_size, byte[] src_buffer, nuint src_size,
                                                 byte[] scratch_buffer);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern nuint AARU_lzfse_encode_buffer(byte[] dst_buffer, nuint dst_size, byte[] src_buffer, nuint src_size,
                                                 byte[] scratch_buffer);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern int AARU_lzma_decode_buffer(byte[] dst_buffer, ref nuint dst_size, byte[] src_buffer,
                                              ref nuint src_size, byte[] props, nuint propsSize);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern int AARU_lzma_encode_buffer(byte[] dst_buffer, ref nuint dst_size, byte[] src_buffer, nuint src_size,
                                              byte[] outProps, ref nuint outPropsSize, int level, uint dictSize, int lc,
                                              int lp, int pb, int fb, int numThreads);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern nuint AARU_zstd_decode_buffer(byte[] dst_buffer, nuint dst_size, byte[] src_buffer, nuint src_size);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern nuint AARU_zstd_encode_buffer(byte[] dst_buffer, nuint dst_size, byte[] src_buffer, nuint src_size,
                                                int compressionLevel);

    public static void AppleRle()
    {
        const int bufferSize = 32768;
        byte[]    input      = new byte[1102];

        var fs = new FileStream(Path.Combine(Program.Folder, "apple_rle.bin"), FileMode.Open, FileAccess.Read);

        fs.Read(input, 0, input.Length);
        fs.Close();
        fs.Dispose();

        byte[] output = new byte[bufferSize];

        int realSize = AARU_apple_rle_decode_buffer(output, output.Length, input, input.Length);

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

        int realSize = AARU_adc_decode_buffer(output, output.Length, input, input.Length);

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
        int  bzError  = AARU_bzip2_decode_buffer(output, ref realSize, input, (uint)input.Length);

        if(realSize != 1048576)
            throw new InvalidDataException("Incorrect decompressed size");

        string crc = Crc32Context.Data(output, realSize, out _);

        if(crc != "c64059c0")
            throw new InvalidDataException("Incorrect decompressed checksum");
    }

    public static void CompressBzip2()
    {
        var    dataStream   = new FileStream(Path.Combine(Program.Folder, "data.bin"), FileMode.Open, FileAccess.Read);
        byte[] decompressed = new byte[8388608];
        dataStream.Read(decompressed, 0, decompressed.Length);
        dataStream.Close();
        byte[] backendBuffer = new byte[8388608];
        uint   cmpSize       = (uint)backendBuffer.Length;

        AARU_bzip2_encode_buffer(backendBuffer, ref cmpSize, decompressed, (uint)decompressed.Length, 9);

        /* This is just to test integrity, disabled for benchmarking
        byte[] compressed = new byte[decompressed.Length];
        uint   dcmpSize   = (uint)compressed.Length;
        AARU_bzip2_decode_buffer(compressed, ref dcmpSize, backendBuffer, cmpSize);

        string newCrc = Crc32Context.Data(compressed, (uint)compressed.Length, out _);

        if(newCrc != "954bf76e")
            throw new InvalidDataException("Incorrect decompressed checksum");
        */
    }

    public static void Lzip()
    {
        const int bufferSize = 1048576;
        byte[]    input      = new byte[1062874];

        var fs = new FileStream(Path.Combine(Program.Folder, "lzip.lz"), FileMode.Open, FileAccess.Read);

        fs.Read(input, 0, input.Length);
        fs.Close();
        fs.Dispose();

        byte[] output = new byte[bufferSize];

        int realSize = AARU_lzip_decode_buffer(output, output.Length, input, input.Length);

        if(realSize != 1048576)
            throw new InvalidDataException("Incorrect decompressed size");

        string crc = Crc32Context.Data(output, (uint)realSize, out _);

        if(crc != "c64059c0")
            throw new InvalidDataException("Incorrect decompressed checksum");
    }

    public static void CompressLzip()
    {
        var    dataStream   = new FileStream(Path.Combine(Program.Folder, "data.bin"), FileMode.Open, FileAccess.Read);
        byte[] decompressed = new byte[8388608];
        dataStream.Read(decompressed, 0, decompressed.Length);
        dataStream.Close();
        byte[] backendBuffer = new byte[8388608];
        int    cmpSize       = backendBuffer.Length;

        cmpSize = AARU_lzip_encode_buffer(backendBuffer, cmpSize, decompressed, decompressed.Length, 106496, 32);

        /* This is just to test integrity, disabled for benchmarking
        byte[] compressed = new byte[decompressed.Length];
        int  dcmpSize   = compressed.Length;
        AARU_lzip_decode_buffer(compressed, dcmpSize, backendBuffer, cmpSize);

        string newCrc = Crc32Context.Data(compressed, (uint)compressed.Length, out _);

        if(newCrc != "954bf76e")
            throw new InvalidDataException("Incorrect decompressed checksum");
        */
    }

    public static void Lzma()
    {
        const int bufferSize = 8388608;
        byte[]    input      = new byte[1200275];

        var fs = new FileStream(Path.Combine(Program.Folder, "lzma.bin"), FileMode.Open, FileAccess.Read);

        fs.Read(input, 0, input.Length);
        fs.Close();
        fs.Dispose();

        byte[] output = new byte[bufferSize];

        nuint destLen = bufferSize;
        nuint srcLen  = 1200275;

        int err = AARU_lzma_decode_buffer(output, ref destLen, input, ref srcLen, new byte[]
        {
            0x5D, 0x00, 0x00, 0x00, 0x02
        }, 5);

        if(err != 0)
            throw new InvalidDataException("Error decompressing");

        if(destLen != 8388608)
            throw new InvalidDataException("Incorrect decompressed size");

        string crc = Crc32Context.Data(output, (uint)destLen, out _);

        if(crc != "954bf76e")
            throw new InvalidDataException("Incorrect decompressed checksum");
    }

    public static void CompressLzma()
    {
        byte[] props        = new byte[5];
        var    dataStream   = new FileStream(Path.Combine(Program.Folder, "data.bin"), FileMode.Open, FileAccess.Read);
        byte[] decompressed = new byte[8388608];
        dataStream.Read(decompressed, 0, decompressed.Length);
        dataStream.Close();
        byte[] backendBuffer = new byte[8388608];
        nuint  cmpSize       = (uint)backendBuffer.Length;
        nuint  propsSize     = (uint)props.Length;

        AARU_lzma_encode_buffer(backendBuffer, ref cmpSize, decompressed, (nuint)decompressed.Length, props,
                                ref propsSize, 9, 1048576, 4, 0, 2, 273, 2);

        /* This is just to test integrity, disabled for benchmarking
        byte[] compressed = new byte[decompressed.Length];
        nuint   dcmpSize   = (uint)compressed.Length;
        AARU_lzma_decode_buffer(compressed, ref dcmpSize, backendBuffer, ref cmpSize, props, propsSize);

        string newCrc = Crc32Context.Data(compressed, (uint)compressed.Length, out _);

        if(newCrc != "954bf76e")
            throw new InvalidDataException("Incorrect decompressed checksum");
        */
    }

    public static void Flac()
    {
        const int bufferSize = 9633792;
        byte[]    input      = new byte[6534197];

        var fs = new FileStream(Path.Combine(Program.Folder, "flac.flac"), FileMode.Open, FileAccess.Read);

        fs.Read(input, 0, input.Length);
        fs.Close();
        fs.Dispose();

        byte[] output = new byte[bufferSize];

        ulong realSize = AARU_flac_decode_redbook_buffer(output, (nuint)output.Length, input, (nuint)input.Length);

        if(realSize != 9633792)
            throw new InvalidDataException("Incorrect decompressed size");

        string crc = Crc32Context.Data(output, (uint)realSize, out _);

        if(crc != "dfbc99bb")
            throw new InvalidDataException("Incorrect decompressed checksum");
    }

    public static void CompressFlac()
    {
        var    dataStream   = new FileStream(Path.Combine(Program.Folder, "audio.bin"), FileMode.Open, FileAccess.Read);
        byte[] decompressed = new byte[9633792];
        dataStream.Read(decompressed, 0, decompressed.Length);
        dataStream.Close();
        byte[] backendBuffer = new byte[9633792];
        nuint  cmpSize       = (uint)backendBuffer.Length;

        AARU_flac_encode_redbook_buffer(backendBuffer, cmpSize, decompressed, (nuint)decompressed.Length, 4608, 1, 0,
                                        "hamming", 12, 15, 1, 0, 0, 8, "Aaru.Compression.Native.Tests",
                                        (uint)"Aaru.Compression.Native.Tests".Length);
    }
}
#endif