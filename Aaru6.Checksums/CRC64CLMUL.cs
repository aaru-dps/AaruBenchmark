using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Aaru6.Checksums
{
    public class CRC64CLMUL
    {
        static readonly byte[] shuffleMasks =
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x8f, 0x8e,
            0x8d, 0x8c, 0x8b, 0x8a, 0x89, 0x88, 0x87, 0x86, 0x85, 0x84, 0x83, 0x82, 0x81, 0x80
        };

        static void shiftRight128(Vector128<ulong> initial, uint n, out Vector128<ulong> outLeft,
                                  out Vector128<ulong> outRight)
        {
            uint maskPos = 16 - n;

            Vector128<byte> maskA = Vector128.Create(shuffleMasks[maskPos], shuffleMasks[maskPos + 1],
                                                     shuffleMasks[maskPos + 2], shuffleMasks[maskPos + 3],
                                                     shuffleMasks[maskPos + 4], shuffleMasks[maskPos + 5],
                                                     shuffleMasks[maskPos + 6], shuffleMasks[maskPos + 7],
                                                     shuffleMasks[maskPos + 8], shuffleMasks[maskPos + 9],
                                                     shuffleMasks[maskPos + 10], shuffleMasks[maskPos + 11],
                                                     shuffleMasks[maskPos + 12], shuffleMasks[maskPos + 13],
                                                     shuffleMasks[maskPos + 14], shuffleMasks[maskPos + 15]);

            Vector128<byte> maskB = Sse2.Xor(maskA, Sse2.CompareEqual(Vector128<byte>.Zero, Vector128<byte>.Zero));

            outLeft  = Ssse3.Shuffle(initial.AsByte(), maskB).AsUInt64();
            outRight = Ssse3.Shuffle(initial.AsByte(), maskA).AsUInt64();
        }

        static Vector128<ulong> fold(Vector128<ulong> input, Vector128<ulong> foldConstants) =>
            Sse2.Xor(Pclmulqdq.CarrylessMultiply(input, foldConstants, 0x00),
                     Pclmulqdq.CarrylessMultiply(input, foldConstants, 0x11));

        internal static ulong crc64_clmul(ulong crc, byte[] data, uint length)
        {
            int bufPos = 16;

            const ulong k1 = 0xe05dd497ca393ae4; // bitReflect(expMod65(128 + 64, poly, 1)) << 1;
            const ulong k2 = 0xdabe95afc7875f40; // bitReflect(expMod65(128, poly, 1)) << 1;
            const ulong mu = 0x9c3e466c172963d5; // (bitReflect(div129by65(poly)) << 1) | 1;
            const ulong p  = 0x92d8af2baf0e1e85; // (bitReflect(poly) << 1) | 1;

            Vector128<ulong> foldConstants1 = Vector128.Create(k1, k2);
            Vector128<ulong> foldConstants2 = Vector128.Create(mu, p);

            uint             leadOutSize = length % 16;
            Vector128<ulong> initialCrc  = Vector128.Create(~crc, 0);

            Vector128<ulong> R;
            length -= 16;

            // Initial CRC can simply be added to data
            shiftRight128(initialCrc, 0, out Vector128<ulong> crc0, out Vector128<ulong> crc1);

            Vector128<ulong> accumulator =
                Sse2.Xor(fold(Sse2.Xor(crc0, Vector128.Create(BitConverter.ToUInt64(data, 0), BitConverter.ToUInt64(data, 8))), foldConstants1),
                         crc1);

            while(length >= 32)
            {
                accumulator =
                    fold(Sse2.Xor(Vector128.Create(BitConverter.ToUInt64(data, bufPos), BitConverter.ToUInt64(data, bufPos + 8)), accumulator),
                         foldConstants1);

                length -= 16;
                bufPos += 16;
            }

            Vector128<ulong> P;

            if(length == 16)
            {
                P = Sse2.Xor(accumulator,
                             Vector128.Create(BitConverter.ToUInt64(data, bufPos),
                                              BitConverter.ToUInt64(data, bufPos + 8)));
            }
            else
            {
                Vector128<ulong> end0 = Sse2.Xor(accumulator,
                                                 Vector128.Create(BitConverter.ToUInt64(data, bufPos),
                                                                  BitConverter.ToUInt64(data, bufPos + 8)));

                bufPos += 16;

                Vector128<ulong> end1 =
                    Vector128.Create(BitConverter.ToUInt64(data, bufPos), BitConverter.ToUInt64(data, bufPos + 8));

                shiftRight128(end0, leadOutSize, out Vector128<ulong> A, out Vector128<ulong> B);
                shiftRight128(end1, leadOutSize, out Vector128<ulong> C, out Vector128<ulong> D);

                P = Sse2.Xor(fold(A, foldConstants1), Sse2.Or(B, C));
            }

            R = Sse2.Xor(Pclmulqdq.CarrylessMultiply(P, foldConstants1, 0x10), Sse2.ShiftRightLogical128BitLane(P, 8));

            // Final Barrett reduction
            Vector128<ulong> T1 = Pclmulqdq.CarrylessMultiply(R, foldConstants2, 0x00);

            Vector128<ulong> T2 =
                Sse2.Xor(Sse2.Xor(Pclmulqdq.CarrylessMultiply(T1, foldConstants2, 0x10), Sse2.ShiftLeftLogical128BitLane(T1, 8)),
                         R);

            return ~(((ulong)Sse41.Extract(T2.AsUInt32(), 3) << 32) | Sse41.Extract(T2.AsUInt32(), 2));
        }
    }
}