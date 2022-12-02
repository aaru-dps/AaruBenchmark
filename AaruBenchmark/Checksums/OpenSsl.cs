using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace AaruBenchmark.Checksums;

public class OpenSsl
{
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

    [DllImport("libssl", SetLastError = true)]
    static extern int EVP_DigestInit(IntPtr ctx, IntPtr type);

    [DllImport("libssl", SetLastError = true)]
    static extern IntPtr EVP_MD_CTX_new();

    [DllImport("libssl", SetLastError = true)]
    static extern IntPtr EVP_md5();

    [DllImport("libssl", SetLastError = true)]
    static extern IntPtr EVP_sha1();

    [DllImport("libssl", SetLastError = true)]
    static extern IntPtr EVP_sha256();

    [DllImport("libssl", SetLastError = true)]
    static extern IntPtr EVP_sha384();

    [DllImport("libssl", SetLastError = true)]
    static extern IntPtr EVP_sha512();

    [DllImport("libssl", SetLastError = true)]
    static extern int EVP_DigestUpdate(IntPtr ctx, byte[] d, ulong cnt);

    [DllImport("libssl", SetLastError = true)]
    static extern int EVP_DigestFinal(IntPtr ctx, byte[] md, ref uint s);

    [DllImport("libssl", SetLastError = true)]
    static extern void EVP_MD_CTX_free(IntPtr ctx);

    public static void Md5()
    {
        byte[] data = new byte[1048576];
        uint   s    = 0;
        byte[] hash = new byte[16];

        var fs = new FileStream(Path.Combine(Program.Folder, "random"), FileMode.Open, FileAccess.Read);

        fs.Read(data, 0, 1048576);
        fs.Close();
        fs.Dispose();

        IntPtr md  = EVP_md5();
        IntPtr ctx = EVP_MD_CTX_new();
        int    ret = EVP_DigestInit(ctx, md);

        if(ret != 1)
            throw new Exception("Could not initialize digest");

        ret = EVP_DigestUpdate(ctx, data, (ulong)data.Length);

        if(ret != 1)
            throw new Exception("Could not digest block");

        ret = EVP_DigestFinal(ctx, hash, ref s);

        if(ret != 1)
            throw new Exception("Could not finalize hash");

        EVP_MD_CTX_free(ctx);

        if(hash.Where((t, i) => t != _expectedRandomMd5[i]).Any())
            throw new Exception("Invalid hash value");
    }

    public static void Sha1()
    {
        byte[] data = new byte[1048576];
        uint   s    = 0;
        byte[] hash = new byte[20];

        var fs = new FileStream(Path.Combine(Program.Folder, "random"), FileMode.Open, FileAccess.Read);

        fs.Read(data, 0, 1048576);
        fs.Close();
        fs.Dispose();

        IntPtr md  = EVP_sha1();
        IntPtr ctx = EVP_MD_CTX_new();
        int    ret = EVP_DigestInit(ctx, md);

        if(ret != 1)
            throw new Exception("Could not initialize digest");

        ret = EVP_DigestUpdate(ctx, data, (ulong)data.Length);

        if(ret != 1)
            throw new Exception("Could not digest block");

        ret = EVP_DigestFinal(ctx, hash, ref s);

        if(ret != 1)
            throw new Exception("Could not finalize hash");

        EVP_MD_CTX_free(ctx);

        if(hash.Where((t, i) => t != _expectedRandomSha1[i]).Any())
            throw new Exception("Invalid hash value");
    }

    public static void Sha256()
    {
        byte[] data = new byte[1048576];
        uint   s    = 0;
        byte[] hash = new byte[32];

        var fs = new FileStream(Path.Combine(Program.Folder, "random"), FileMode.Open, FileAccess.Read);

        fs.Read(data, 0, 1048576);
        fs.Close();
        fs.Dispose();

        IntPtr md  = EVP_sha256();
        IntPtr ctx = EVP_MD_CTX_new();
        int    ret = EVP_DigestInit(ctx, md);

        if(ret != 1)
            throw new Exception("Could not initialize digest");

        ret = EVP_DigestUpdate(ctx, data, (ulong)data.Length);

        if(ret != 1)
            throw new Exception("Could not digest block");

        ret = EVP_DigestFinal(ctx, hash, ref s);

        if(ret != 1)
            throw new Exception("Could not finalize hash");

        EVP_MD_CTX_free(ctx);

        if(hash.Where((t, i) => t != _expectedRandomSha256[i]).Any())
            throw new Exception("Invalid hash value");
    }

    public static void Sha384()
    {
        byte[] data = new byte[1048576];
        uint   s    = 0;
        byte[] hash = new byte[48];

        var fs = new FileStream(Path.Combine(Program.Folder, "random"), FileMode.Open, FileAccess.Read);

        fs.Read(data, 0, 1048576);
        fs.Close();
        fs.Dispose();

        IntPtr md  = EVP_sha384();
        IntPtr ctx = EVP_MD_CTX_new();
        int    ret = EVP_DigestInit(ctx, md);

        if(ret != 1)
            throw new Exception("Could not initialize digest");

        ret = EVP_DigestUpdate(ctx, data, (ulong)data.Length);

        if(ret != 1)
            throw new Exception("Could not digest block");

        ret = EVP_DigestFinal(ctx, hash, ref s);

        if(ret != 1)
            throw new Exception("Could not finalize hash");

        EVP_MD_CTX_free(ctx);

        if(hash.Where((t, i) => t != _expectedRandomSha384[i]).Any())
            throw new Exception("Invalid hash value");
    }

    public static void Sha512()
    {
        byte[] data = new byte[1048576];
        uint   s    = 0;
        byte[] hash = new byte[64];

        var fs = new FileStream(Path.Combine(Program.Folder, "random"), FileMode.Open, FileAccess.Read);

        fs.Read(data, 0, 1048576);
        fs.Close();
        fs.Dispose();

        IntPtr md  = EVP_sha512();
        IntPtr ctx = EVP_MD_CTX_new();
        int    ret = EVP_DigestInit(ctx, md);

        if(ret != 1)
            throw new Exception("Could not initialize digest");

        ret = EVP_DigestUpdate(ctx, data, (ulong)data.Length);

        if(ret != 1)
            throw new Exception("Could not digest block");

        ret = EVP_DigestFinal(ctx, hash, ref s);

        if(ret != 1)
            throw new Exception("Could not finalize hash");

        EVP_MD_CTX_free(ctx);

        if(hash.Where((t, i) => t != _expectedRandomSha512[i]).Any())
            throw new Exception("Invalid hash value");
    }
}