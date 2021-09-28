using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace AaruBenchmark.Checksums
{
    public class AaruNative
    {
        static readonly byte[] _expectedRandomAdler32 =
        {
            0x37, 0x28, 0xd1, 0x86
        };

        static readonly byte[] _expectedRandomFletcher16 =
        {
            0x33, 0x57
        };

        static readonly byte[] _expectedRandomFletcher32 =
        {
            0x21, 0x12, 0x61, 0xF5
        };

        static readonly byte[] _expectedRandomCrc16Ccitt =
        {
            0x36, 0x40
        };

        static readonly byte[] _expectedRandomCrc16 =
        {
            0x2d, 0x6d
        };

        static readonly byte[] _expectedRandomCrc32 =
        {
            0x2b, 0x6e, 0x68, 0x54
        };

        static readonly byte[] _expectedRandomCrc64 =
        {
            0xbf, 0x09, 0x99, 0x2c, 0xc5, 0xed, 0xe3, 0x8e
        };

        static readonly byte[] _expectedRandomMd5 =
        {
            0xd7, 0x8f, 0x0e, 0xec, 0x41, 0x7b, 0xe3, 0x86, 0x21, 0x9b, 0x21, 0xb7, 0x00, 0x04, 0x4b, 0x95
        };

        static readonly byte[] _expectedRandomSha1 =
        {
            0x72, 0x0d, 0x3b, 0x71, 0x7d, 0xe0, 0xc7, 0x4c, 0x77, 0xdd, 0x9c, 0xaa, 0x9e, 0xba, 0x50, 0x60, 0xdc, 0xbd,
            0x28, 0x8d
        };

        static readonly byte[] _expectedRandomSha256 =
        {
            0x4d, 0x1a, 0x6b, 0x8a, 0x54, 0x67, 0x00, 0xc4, 0x8e, 0xda, 0x70, 0xd3, 0x39, 0x1c, 0x8f, 0x15, 0x8a, 0x8d,
            0x12, 0xb2, 0x38, 0x92, 0x89, 0x29, 0x50, 0x47, 0x8c, 0x41, 0x8e, 0x25, 0xcc, 0x39
        };

        static readonly byte[] _expectedRandomSha384 =
        {
            0xdb, 0x53, 0x0e, 0x17, 0x9b, 0x81, 0xfe, 0x5f, 0x6d, 0x20, 0x41, 0x04, 0x6e, 0x77, 0xd9, 0x85, 0xf2, 0x85,
            0x8a, 0x66, 0xca, 0xd3, 0x8d, 0x1a, 0xd5, 0xac, 0x67, 0xa9, 0x74, 0xe1, 0xef, 0x3f, 0x4d, 0xdf, 0x94, 0x15,
            0x2e, 0xac, 0x2e, 0xfe, 0x16, 0x95, 0x81, 0x54, 0xdc, 0x59, 0xd4, 0xc3
        };

        static readonly byte[] _expectedRandomSha512 =
        {
            0x6a, 0x0a, 0x18, 0xc2, 0xad, 0xf8, 0x83, 0xac, 0x58, 0xe6, 0x21, 0x96, 0xdb, 0x8d, 0x3d, 0x0e, 0xb9, 0x87,
            0xd1, 0x49, 0x24, 0x97, 0xdb, 0x15, 0xb9, 0xfc, 0xcc, 0xb0, 0x36, 0xdf, 0x64, 0xae, 0xdb, 0x3e, 0x82, 0xa0,
            0x4d, 0xdc, 0xd1, 0x37, 0x48, 0x92, 0x95, 0x51, 0xf9, 0xdd, 0xab, 0x82, 0xf4, 0x8a, 0x85, 0x3f, 0x9a, 0x01,
            0xb5, 0xf2, 0x8c, 0xbb, 0x4a, 0xa5, 0x1b, 0x40, 0x7c, 0xb6
        };

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern IntPtr adler32_init();

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int adler32_update(IntPtr ctx, byte[] data, uint len);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int adler32_final(IntPtr ctx, ref uint crc);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern void adler32_free(IntPtr ctx);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern IntPtr fletcher16_init();

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int fletcher16_update(IntPtr ctx, byte[] data, uint len);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int fletcher16_final(IntPtr ctx, ref ushort crc);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern void fletcher16_free(IntPtr ctx);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern IntPtr fletcher32_init();

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int fletcher32_update(IntPtr ctx, byte[] data, uint len);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int fletcher32_final(IntPtr ctx, ref uint crc);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern void fletcher32_free(IntPtr ctx);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern IntPtr crc16_ccitt_init();

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int crc16_ccitt_update(IntPtr ctx, byte[] data, uint len);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int crc16_ccitt_final(IntPtr ctx, ref ushort crc);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern void crc16_ccitt_free(IntPtr ctx);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern IntPtr crc16_init();

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int crc16_update(IntPtr ctx, byte[] data, uint len);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int crc16_final(IntPtr ctx, ref ushort crc);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern void crc16_free(IntPtr ctx);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern IntPtr crc32_init();

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int crc32_update(IntPtr ctx, byte[] data, uint len);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int crc32_final(IntPtr ctx, ref uint crc);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern void crc32_free(IntPtr ctx);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern IntPtr crc64_init();

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int crc64_update(IntPtr ctx, byte[] data, uint len);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int crc64_final(IntPtr ctx, ref ulong crc);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern void crc64_free(IntPtr ctx);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern IntPtr spamsum_init();

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int spamsum_update(IntPtr ctx, byte[] data, uint len);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern int spamsum_final(IntPtr ctx, byte[] result);

        [DllImport("libAaru.Checksums.Native", SetLastError = true)]
        static extern void spamsum_free(IntPtr ctx);

        public static void Fletcher16()
        {
            byte[] data       = new byte[1048576];
            ushort fletcher16 = 0;
            byte[] hash;

            var fs = new FileStream(Path.Combine(Program.Folder, "random"), FileMode.Open, FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();

            IntPtr ctx = fletcher16_init();

            if(ctx == IntPtr.Zero)
                throw new Exception("Could not initialize digest");

            int ret = fletcher16_update(ctx, data, (uint)data.Length);

            if(ret != 0)
                throw new Exception("Could not digest block");

            ret = fletcher16_final(ctx, ref fletcher16);

            if(ret != 0)
                throw new Exception("Could not finalize hash");

            fletcher16_free(ctx);

            fletcher16 = (ushort)((fletcher16 << 8) | (fletcher16 >> 8));

            hash = BitConverter.GetBytes(fletcher16);

            if(hash.Where((t, i) => t != _expectedRandomFletcher16[i]).Any())
                throw new Exception("Invalid hash value");
        }

        public static void Fletcher32()
        {
            byte[] data       = new byte[1048576];
            uint   fletcher32 = 0;
            byte[] hash;

            var fs = new FileStream(Path.Combine(Program.Folder, "random"), FileMode.Open, FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();

            IntPtr ctx = fletcher32_init();

            if(ctx == IntPtr.Zero)
                throw new Exception("Could not initialize digest");

            int ret = fletcher32_update(ctx, data, (uint)data.Length);

            if(ret != 0)
                throw new Exception("Could not digest block");

            ret = fletcher32_final(ctx, ref fletcher32);

            if(ret != 0)
                throw new Exception("Could not finalize hash");

            fletcher32_free(ctx);

            fletcher32 = ((fletcher32 << 8) & 0xFF00FF00) | ((fletcher32 >> 8) & 0xFF00FF);
            fletcher32 = (fletcher32 << 16)               | (fletcher32 >> 16);

            hash = BitConverter.GetBytes(fletcher32);

            if(hash.Where((t, i) => t != _expectedRandomFletcher32[i]).Any())
                throw new Exception("Invalid hash value");
        }

        public static void Adler32()
        {
            byte[] data    = new byte[1048576];
            uint   adler32 = 0;
            byte[] hash;

            var fs = new FileStream(Path.Combine(Program.Folder, "random"), FileMode.Open, FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();

            IntPtr ctx = adler32_init();

            if(ctx == IntPtr.Zero)
                throw new Exception("Could not initialize digest");

            int ret = adler32_update(ctx, data, (uint)data.Length);

            if(ret != 0)
                throw new Exception("Could not digest block");

            ret = adler32_final(ctx, ref adler32);

            if(ret != 0)
                throw new Exception("Could not finalize hash");

            adler32_free(ctx);

            adler32 = ((adler32 << 8) & 0xFF00FF00) | ((adler32 >> 8) & 0xFF00FF);
            adler32 = (adler32 << 16)               | (adler32 >> 16);

            hash = BitConverter.GetBytes(adler32);

            if(hash.Where((t, i) => t != _expectedRandomAdler32[i]).Any())
                throw new Exception("Invalid hash value");
        }

        public static void Crc16Ccitt()
        {
            byte[] data = new byte[1048576];
            ushort crc  = 0;
            byte[] hash;

            var fs = new FileStream(Path.Combine(Program.Folder, "random"), FileMode.Open, FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();

            IntPtr ctx = crc16_ccitt_init();

            if(ctx == IntPtr.Zero)
                throw new Exception("Could not initialize digest");

            int ret = crc16_ccitt_update(ctx, data, (uint)data.Length);

            if(ret != 0)
                throw new Exception("Could not digest block");

            ret = crc16_ccitt_final(ctx, ref crc);

            if(ret != 0)
                throw new Exception("Could not finalize hash");

            crc16_ccitt_free(ctx);

            crc = (ushort)((crc << 8) | (crc >> 8));

            hash = BitConverter.GetBytes(crc);

            if(hash.Where((t, i) => t != _expectedRandomCrc16Ccitt[i]).Any())
                throw new Exception("Invalid hash value");
        }

        public static void Crc16()
        {
            byte[] data = new byte[1048576];
            ushort crc  = 0;
            byte[] hash;

            var fs = new FileStream(Path.Combine(Program.Folder, "random"), FileMode.Open, FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();

            IntPtr ctx = crc16_init();

            if(ctx == IntPtr.Zero)
                throw new Exception("Could not initialize digest");

            int ret = crc16_update(ctx, data, (uint)data.Length);

            if(ret != 0)
                throw new Exception("Could not digest block");

            ret = crc16_final(ctx, ref crc);

            if(ret != 0)
                throw new Exception("Could not finalize hash");

            crc16_free(ctx);

            crc = (ushort)((crc << 8) | (crc >> 8));

            hash = BitConverter.GetBytes(crc);

            if(hash.Where((t, i) => t != _expectedRandomCrc16[i]).Any())
                throw new Exception("Invalid hash value");
        }

        public static void Crc32()
        {
            byte[] data  = new byte[1048576];
            uint   crc32 = 0;
            byte[] hash;

            var fs = new FileStream(Path.Combine(Program.Folder, "random"), FileMode.Open, FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();

            IntPtr ctx = crc32_init();

            if(ctx == IntPtr.Zero)
                throw new Exception("Could not initialize digest");

            int ret = crc32_update(ctx, data, (uint)data.Length);

            if(ret != 0)
                throw new Exception("Could not digest block");

            ret = crc32_final(ctx, ref crc32);

            if(ret != 0)
                throw new Exception("Could not finalize hash");

            crc32_free(ctx);

            crc32 = ((crc32 << 8) & 0xFF00FF00) | ((crc32 >> 8) & 0xFF00FF);
            crc32 = (crc32 << 16)               | (crc32 >> 16);

            hash = BitConverter.GetBytes(crc32);

            if(hash.Where((t, i) => t != _expectedRandomCrc32[i]).Any())
                throw new Exception("Invalid hash value");
        }

        public static void Crc64()
        {
            byte[] data  = new byte[1048576];
            ulong  crc64 = 0;
            byte[] hash;

            var fs = new FileStream(Path.Combine(Program.Folder, "random"), FileMode.Open, FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();

            IntPtr ctx = crc64_init();

            if(ctx == IntPtr.Zero)
                throw new Exception("Could not initialize digest");

            int ret = crc64_update(ctx, data, (uint)data.Length);

            if(ret != 0)
                throw new Exception("Could not digest block");

            ret = crc64_final(ctx, ref crc64);

            if(ret != 0)
                throw new Exception("Could not finalize hash");

            crc64_free(ctx);

            crc64 = ((crc64 & 0x00000000FFFFFFFF) << 32) | ((crc64 & 0xFFFFFFFF00000000) >> 32);
            crc64 = ((crc64 & 0x0000FFFF0000FFFF) << 16) | ((crc64 & 0xFFFF0000FFFF0000) >> 16);
            crc64 = ((crc64 & 0x00FF00FF00FF00FF) << 8)  | ((crc64 & 0xFF00FF00FF00FF00) >> 8);

            hash = BitConverter.GetBytes(crc64);

            if(hash.Where((t, i) => t != _expectedRandomCrc64[i]).Any())
                throw new Exception("Invalid hash value");
        }

        public static void SpamSum()
        {
            byte[] data = new byte[1048576];
            byte[] hash = new byte[256];

            var fs = new FileStream(Path.Combine(Program.Folder, "random"), FileMode.Open, FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();

            IntPtr ctx = spamsum_init();

            if(ctx == IntPtr.Zero)
                throw new Exception("Could not initialize digest");

            int ret = spamsum_update(ctx, data, (uint)data.Length);

            if(ret != 0)
                throw new Exception("Could not digest block");

            ret = spamsum_final(ctx, hash);

            if(ret != 0)
                throw new Exception("Could not finalize block");

            spamsum_free(ctx);
        }
    }
}