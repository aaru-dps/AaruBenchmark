using System.IO;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.Deflate;

namespace AaruBenchmark
{
    public static class SharpCompress
    {
        public static void Gzip()
        {
            var _dataStream = new FileStream(Path.Combine(Program.Folder, "gzip.gz"), FileMode.Open, FileAccess.Read);
            Stream str = new GZipStream(_dataStream, CompressionMode.Decompress);
            byte[] compressed = new byte[1048576];
            int pos = 0;
            int left = 1048576;
            bool oneZero = false;

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

        public static void Bzip2()
        {
            var _dataStream = new FileStream(Path.Combine(Program.Folder, "bzip2.bz2"), FileMode.Open, FileAccess.Read);
            Stream str = new BZip2Stream(_dataStream, CompressionMode.Decompress, true);
            byte[] compressed = new byte[1048576];
            int pos = 0;
            int left = 1048576;
            bool oneZero = false;

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
    }
}