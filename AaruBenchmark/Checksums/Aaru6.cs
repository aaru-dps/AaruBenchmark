using System;
using System.IO;
using System.Linq;
using Aaru.CommonTypes.Interfaces;
using Aaru6.Checksums;

namespace AaruBenchmark.Checksums
{
    public class Aaru6
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

        public static void Fletcher16()
        {
            byte[] data = new byte[1048576];

            var fs = new FileStream(Path.Combine("/mnt/DiscImageChef", "Checksum test files", "random"), FileMode.Open,
                                    FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            IChecksum ctx = new Fletcher16Context();
            ctx.Update(data);
            byte[] result = ctx.Final();

            if(result?.Length != _expectedRandomFletcher16.Length)
                throw new Exception("Invalid hash length");

            if(result.Where((t, i) => t != _expectedRandomFletcher16[i]).Any())
                throw new Exception("Invalid hash value");
        }

        public static void Fletcher32()
        {
            byte[] data = new byte[1048576];

            var fs = new FileStream(Path.Combine("/mnt/DiscImageChef", "Checksum test files", "random"), FileMode.Open,
                                    FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            IChecksum ctx = new Fletcher32Context();
            ctx.Update(data);
            byte[] result = ctx.Final();

            if(result?.Length != _expectedRandomFletcher32.Length)
                throw new Exception("Invalid hash length");

            if(result.Where((t, i) => t != _expectedRandomFletcher32[i]).Any())
                throw new Exception("Invalid hash value");
        }

        public static void Adler32()
        {
            byte[] data = new byte[1048576];

            var fs = new FileStream(Path.Combine("/mnt/DiscImageChef", "Checksum test files", "random"), FileMode.Open,
                                    FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            IChecksum ctx = new Adler32Context();
            ctx.Update(data);
            byte[] result = ctx.Final();

            if(result?.Length != _expectedRandomAdler32.Length)
                throw new Exception("Invalid hash length");

            if(result.Where((t, i) => t != _expectedRandomAdler32[i]).Any())
                throw new Exception("Invalid hash value");
        }

        public static void Crc16Ccitt()
        {
            byte[] data = new byte[1048576];

            var fs = new FileStream(Path.Combine("/mnt/DiscImageChef", "Checksum test files", "random"), FileMode.Open,
                                    FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            IChecksum ctx = new CRC16CCITTContext();
            ctx.Update(data);
            byte[] result = ctx.Final();
            /*
                        if(result?.Length != _expectedRandomCrc16Ccitt.Length)
                            throw new Exception("Invalid hash length");

                        if(result.Where((t, i) => t != _expectedRandomCrc16Ccitt[i]).Any())
                            throw new Exception("Invalid hash value");*/
        }

        public static void Crc16()
        {
            byte[] data = new byte[1048576];

            var fs = new FileStream(Path.Combine("/mnt/DiscImageChef", "Checksum test files", "random"), FileMode.Open,
                                    FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            IChecksum ctx = new CRC16IBMContext();
            ctx.Update(data);
            byte[] result = ctx.Final();

            if(result?.Length != _expectedRandomCrc16.Length)
                throw new Exception("Invalid hash length");

            if(result.Where((t, i) => t != _expectedRandomCrc16[i]).Any())
                throw new Exception("Invalid hash value");
        }

        public static void Crc32()
        {
            byte[] data = new byte[1048576];

            var fs = new FileStream(Path.Combine("/mnt/DiscImageChef", "Checksum test files", "random"), FileMode.Open,
                                    FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            IChecksum ctx = new Crc32Context();
            ctx.Update(data);
            byte[] result = ctx.Final();

            if(result?.Length != _expectedRandomCrc32.Length)
                throw new Exception("Invalid hash length");

            if(result.Where((t, i) => t != _expectedRandomCrc32[i]).Any())
                throw new Exception("Invalid hash value");
        }

        public static void Crc64()
        {
            byte[] data = new byte[1048576];

            var fs = new FileStream(Path.Combine("/mnt/DiscImageChef", "Checksum test files", "random"), FileMode.Open,
                                    FileAccess.Read);

            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            IChecksum ctx = new Crc64Context();
            ctx.Update(data);
            byte[] result = ctx.Final();

            if(result?.Length != _expectedRandomCrc64.Length)
                throw new Exception("Invalid hash length");

            if(result.Where((t, i) => t != _expectedRandomCrc64[i]).Any())
                throw new Exception("Invalid hash value");
        }
    }
}