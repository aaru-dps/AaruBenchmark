using System.IO;
using Ionic.BZip2;
using Ionic.Zlib;

namespace AaruBenchmark
{
    public static class DotNetZip
    {
        public static void Gzip()
        {
            var    _dataStream = new FileStream("/mnt/DiscImageChef/Filters/gzip.gz", FileMode.Open, FileAccess.Read);
            Stream str         = new GZipStream(_dataStream, CompressionMode.Decompress, true);
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

        public static void Bzip2()
        {
            var    _dataStream = new FileStream("/mnt/DiscImageChef/Filters/bzip2.bz2", FileMode.Open, FileAccess.Read);
            Stream str         = new BZip2InputStream(_dataStream, true);
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
    }
}