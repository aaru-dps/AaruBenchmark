using System.IO;
#if NET7_0
using Aaru6.Checksums;
#else
using Aaru.Checksums;
#endif
using SharpCompress.Compressors;
using SharpCompress.Compressors.ADC;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors.LZMA;

namespace AaruBenchmark.Compression;

public static class SharpCompress
{
    public static void Gzip()
    {
        var    _dataStream = new FileStream(Path.Combine(Program.Folder, "gzip.gz"), FileMode.Open, FileAccess.Read);
        Stream str         = new GZipStream(_dataStream, CompressionMode.Decompress);
        byte[] compressed  = new byte[1048576];
        int    pos         = 0;
        int    left        = 1048576;
        bool   oneZero     = false;

        while(left > 0)
        {
            int done = str.Read(compressed, pos, left);

            if(done == 0)
            {
                if(oneZero)
                    throw new IOException("Could not read the file!");

                oneZero = true;
            }

            left -= done;
            pos  += done;
        }

        str.Close();
        str.Dispose();
    }

    public static void CompressGzip()
    {
        var    dataStream   = new FileStream(Path.Combine(Program.Folder, "data.bin"), FileMode.Open, FileAccess.Read);
        byte[] decompressed = new byte[8388608];
        dataStream.Read(decompressed, 0, decompressed.Length);
        dataStream.Close();
        byte[] backendBuffer = new byte[8388608];

        Stream cmpStream = new GZipStream(new MemoryStream(backendBuffer), CompressionMode.Compress,
                                          CompressionLevel.Level9);

        cmpStream.Write(decompressed, 0, decompressed.Length);
        cmpStream.Close();

        /* This is just to test integrity, disabled for benchmarking
        Stream str        = new GZipStream(new MemoryStream(backendBuffer), CompressionMode.Decompress);
        byte[] compressed = new byte[decompressed.Length];
        int    pos        = 0;
        int    left       = compressed.Length;
        bool   oneZero    = false;

        while(left > 0)
        {
            int done = str.Read(compressed, pos, left);

            if(done == 0)
            {
                if(oneZero)
                    throw new IOException("Could not read the file!");

                oneZero = true;
            }

            left -= done;
            pos  += done;
        }

        string newCrc = Crc32Context.Data(compressed, (uint)compressed.Length, out _);

        if(newCrc != "954bf76e")
            throw new InvalidDataException("Incorrect decompressed checksum");
        */
    }

    public static void Bzip2()
    {
        var    _dataStream = new FileStream(Path.Combine(Program.Folder, "bzip2.bz2"), FileMode.Open, FileAccess.Read);
        Stream str         = new BZip2Stream(_dataStream, CompressionMode.Decompress, true);
        byte[] compressed  = new byte[1048576];
        int    pos         = 0;
        int    left        = 1048576;
        bool   oneZero     = false;

        while(left > 0)
        {
            int done = str.Read(compressed, pos, left);

            if(done == 0)
            {
                if(oneZero)
                    throw new IOException("Could not read the file!");

                oneZero = true;
            }

            left -= done;
            pos  += done;
        }

        str.Close();
        str.Dispose();
    }

    public static void CompressBzip2()
    {
        var    dataStream   = new FileStream(Path.Combine(Program.Folder, "data.bin"), FileMode.Open, FileAccess.Read);
        byte[] decompressed = new byte[8388608];
        dataStream.Read(decompressed, 0, decompressed.Length);
        dataStream.Close();
        byte[] backendBuffer = new byte[8388608];

        Stream cmpStream = new BZip2Stream(new MemoryStream(backendBuffer), CompressionMode.Compress, true);
        cmpStream.Write(decompressed, 0, decompressed.Length);
        cmpStream.Close();

        /* This is just to test integrity, disabled for benchmarking
        Stream str        = new BZip2Stream(new MemoryStream(backendBuffer), CompressionMode.Decompress, false);
        byte[] compressed = new byte[decompressed.Length];
        int    pos        = 0;
        int    left       = compressed.Length;
        bool   oneZero    = false;

        while(left > 0)
        {
            int done = str.Read(compressed, pos, left);

            if(done == 0)
            {
                if(oneZero)
                    throw new IOException("Could not read the file!");

                oneZero = true;
            }

            left -= done;
            pos  += done;
        }

        string newCrc = Crc32Context.Data(compressed, (uint)compressed.Length, out _);

        if(newCrc != "954bf76e")
            throw new InvalidDataException("Incorrect decompressed checksum");
        */
    }

    public static void ADC()
    {
        var    _dataStream = new FileStream(Path.Combine(Program.Folder, "adc.bin"), FileMode.Open, FileAccess.Read);
        Stream str         = new ADCStream(_dataStream);
        byte[] compressed  = new byte[262144];
        int    pos         = 0;
        int    left        = 262144;
        bool   oneZero     = false;

        while(left > 0)
        {
            int done = str.Read(compressed, pos, left);

            if(done == 0)
            {
                if(oneZero)
                    throw new IOException("Could not read the file!");

                oneZero = true;
            }

            left -= done;
            pos  += done;
        }

        str.Close();
        str.Dispose();

        string crc = Crc32Context.Data(compressed, 262144, out _);

        if(crc != "5a5a7388")
            throw new InvalidDataException("Incorrect decompressed checksum");
    }

    public static void Lzip()
    {
        var    _dataStream = new FileStream(Path.Combine(Program.Folder, "lzip.lz"), FileMode.Open, FileAccess.Read);
        Stream str         = new LZipStream(_dataStream, CompressionMode.Decompress);
        byte[] compressed  = new byte[1048576];
        int    pos         = 0;
        int    left        = 1048576;
        bool   oneZero     = false;

        while(left > 0)
        {
            int done = str.Read(compressed, pos, left);

            if(done == 0)
            {
                if(oneZero)
                    throw new IOException("Could not read the file!");

                oneZero = true;
            }

            left -= done;
            pos  += done;
        }

        str.Close();
        str.Dispose();

        string crc = Crc32Context.Data(compressed, 1048576, out _);

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

        Stream cmpStream = new LZipStream(new MemoryStream(backendBuffer), CompressionMode.Compress);
        cmpStream.Write(decompressed, 0, decompressed.Length);
        cmpStream.Close();

        /* This is just to test integrity, disabled for benchmarking
        Stream str        = new LZipStream(new MemoryStream(backendBuffer), CompressionMode.Decompress);
        byte[] compressed = new byte[decompressed.Length];
        int    pos        = 0;
        int    left       = compressed.Length;
        bool   oneZero    = false;

        while(left > 0)
        {
            int done = str.Read(compressed, pos, left);

            if(done == 0)
            {
                if(oneZero)
                    throw new IOException("Could not read the file!");

                oneZero = true;
            }

            left -= done;
            pos  += done;
        }

        string newCrc = Crc32Context.Data(compressed, (uint)compressed.Length, out _);

        if(newCrc != "954bf76e")
            throw new InvalidDataException("Incorrect decompressed checksum");
        */
    }

    public static void Lzma()
    {
        var _dataStream = new FileStream(Path.Combine(Program.Folder, "lzma.bin"), FileMode.Open, FileAccess.Read);

        Stream str = new LzmaStream(new byte[]
        {
            0x5D, 0x00, 0x00, 0x00, 0x02
        }, _dataStream);

        byte[] compressed = new byte[8388608];
        int    pos        = 0;
        int    left       = 8388608;
        bool   oneZero    = false;

        while(left > 0)
        {
            int done = str.Read(compressed, pos, left);

            if(done == 0)
            {
                if(oneZero)
                    throw new IOException("Could not read the file!");

                oneZero = true;
            }

            left -= done;
            pos  += done;
        }

        str.Close();
        str.Dispose();

        string crc = Crc32Context.Data(compressed, 8388608, out _);

        if(crc != "954bf76e")
            throw new InvalidDataException("Incorrect decompressed checksum");
    }

    public static void CompressLzma()
    {
        var    dataStream   = new FileStream(Path.Combine(Program.Folder, "data.bin"), FileMode.Open, FileAccess.Read);
        byte[] decompressed = new byte[8388608];
        dataStream.Read(decompressed, 0, decompressed.Length);
        dataStream.Close();
        byte[] backendBuffer = new byte[8388608];

        var cmpStream = new LzmaStream(new LzmaEncoderProperties(true, 1048576, 273), false,
                                       new MemoryStream(backendBuffer));

        byte[] propertiesArray = new byte[cmpStream.Properties.Length];
        cmpStream.Properties.CopyTo(propertiesArray, 0);

        cmpStream.Write(decompressed, 0, decompressed.Length);
        cmpStream.Close();

        /* This is just to test integrity, disabled for benchmarking
        Stream str        = new LzmaStream(propertiesArray, new MemoryStream(backendBuffer));
        byte[] compressed = new byte[decompressed.Length];
        int    pos        = 0;
        int    left       = compressed.Length;
        bool   oneZero    = false;

        while(left > 0)
        {
            int done = str.Read(compressed, pos, left);

            if(done == 0)
            {
                if(oneZero)
                    throw new IOException("Could not read the file!");

                oneZero = true;
            }

            left -= done;
            pos  += done;
        }

        string newCrc = Crc32Context.Data(compressed, (uint)compressed.Length, out _);

        if(newCrc != "954bf76e")
            throw new InvalidDataException("Incorrect decompressed checksum");
        */
    }
}