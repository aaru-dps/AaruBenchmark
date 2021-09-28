using System.IO;
using System.IO.Compression;

namespace AaruBenchmark.Compression
{
    public static class NetRuntime
    {
        public static void Gzip()
        {
            var _dataStream = new FileStream(Path.Combine(Program.Folder, "gzip.gz"), FileMode.Open, FileAccess.Read);
            Stream str = new GZipStream(_dataStream, CompressionMode.Decompress, true);
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