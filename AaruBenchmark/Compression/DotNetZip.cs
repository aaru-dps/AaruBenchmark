using System.IO;
using Ionic.BZip2;

// ReSharper disable ArrangeNamespaceBody

namespace AaruBenchmark.Compression
{
    public static class DotNetZip
    {
        public static void Bzip2()
        {
            var _dataStream = new FileStream(Path.Combine(Program.Folder, "bzip2.bz2"), FileMode.Open, FileAccess.Read);
            Stream str = new BZip2InputStream(_dataStream, true);
            var compressed = new byte[1048576];
            var pos = 0;
            var left = 1048576;
            var oneZero = false;

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
            var dataStream   = new FileStream(Path.Combine(Program.Folder, "data.bin"), FileMode.Open, FileAccess.Read);
            var decompressed = new byte[8388608];
            dataStream.Read(decompressed, 0, decompressed.Length);
            dataStream.Close();

            var cmpMs = new MemoryStream();

            Stream cmpStream = new BZip2OutputStream(cmpMs, 9, true);
            cmpStream.Write(decompressed, 0, decompressed.Length);
            cmpStream.Close();
            cmpMs.Position = 0;

            /* This is just to test integrity, disabled for benchmarking
            Stream str        = new BZip2InputStream(cmpMs, true);
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

            str.Close();
            str.Dispose();

            string newCrc = Crc32Context.Data(compressed, (uint)compressed.Length, out _);

            if(newCrc != "954bf76e")
                throw new InvalidDataException("Incorrect decompressed checksum");
            */
        }
    }
}