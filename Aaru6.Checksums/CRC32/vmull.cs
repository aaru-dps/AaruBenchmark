// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : clmul.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//                  Wajdi Feghali    <wajdi.k.feghali@intel.com>
//                  Jim Guilford     <james.guilford@intel.com>
//                  Vinodh Gopal     <vinodh.gopal@intel.com>
//                  Erdinc Ozturk    <erdinc.ozturk@intel.com>
//                  Jim Kukunas      <james.t.kukunas@linux.intel.com>
//                  Marian Beermann
//
// Component      : Checksums.
//
// --[ Description ] ----------------------------------------------------------
//
// Compute the CRC32 using a parallelized folding approach with the PCLMULQDQ
// instruction.
//
// A white paper describing this algorithm can be found at:
// http://www.intel.com/content/dam/www/public/us/en/documents/white-papers/fast-crc-computation-generic-polynomials-pclmulqdq-paper.pdf
//
// --[ License ] --------------------------------------------------------------
//
// This software is provided 'as-is', without any express or implied warranty.
// In no event will the authors be held liable for any damages arising from
// the use of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
//
//   1. The origin of this software must not be misrepresented; you must not
//      claim that you wrote the original software. If you use this software
//      in a product, an acknowledgment in the product documentation would be
//      appreciated but is not required.
//
//   2. Altered source versions must be plainly marked as such, and must not be
//      misrepresented as being the original software.
//
//   3. This notice may not be removed or altered from any source distribution.
//
// ----------------------------------------------------------------------------
// Copyright © 2011-2021 Natalia Portillo
// Copyright (c) 2016 Marian Beermann (add support for initial value, restructuring)
// Copyright (C) 2013 Intel Corporation. All rights reserved.
// ****************************************************************************/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;

namespace Aaru6.Checksums.CRC32
{
    internal static class Vmull
    {
        static readonly uint[] _crcK =
        {
            0xccaa009e, 0x00000000, /* rk1 */ 0x751997d0, 0x00000001, /* rk2 */ 0xccaa009e, 0x00000000, /* rk5 */
            0x63cd6124, 0x00000001, /* rk6 */ 0xf7011640, 0x00000001, /* rk7 */ 0xdb710640, 0x00000001  /* rk8 */
        };

        static readonly Vector128<uint>[] _pshufbShfTable =
        {
            Vector128.Create(0x84838281, 0x88878685, 0x8c8b8a89, 0x008f8e8d),  /* shl 15 (16 - 1)/shr1 */
            Vector128.Create(0x85848382, 0x89888786, 0x8d8c8b8a, 0x01008f8e),  /* shl 14 (16 - 3)/shr2 */
            Vector128.Create(0x86858483, 0x8a898887, 0x8e8d8c8b, 0x0201008f),  /* shl 13 (16 - 4)/shr3 */
            Vector128.Create(0x87868584, 0x8b8a8988, 0x8f8e8d8c, 0x03020100),  /* shl 12 (16 - 4)/shr4 */
            Vector128.Create(0x88878685, 0x8c8b8a89, 0x008f8e8d, 0x04030201),  /* shl 11 (16 - 5)/shr5 */
            Vector128.Create(0x89888786, 0x8d8c8b8a, 0x01008f8e, 0x05040302),  /* shl 10 (16 - 6)/shr6 */
            Vector128.Create(0x8a898887, 0x8e8d8c8b, 0x0201008f, 0x06050403),  /* shl  9 (16 - 7)/shr7 */
            Vector128.Create(0x8b8a8988, 0x8f8e8d8c, 0x03020100, 0x07060504),  /* shl  8 (16 - 8)/shr8 */
            Vector128.Create(0x8c8b8a89, 0x008f8e8d, 0x04030201, 0x08070605),  /* shl  7 (16 - 9)/shr9 */
            Vector128.Create(0x8d8c8b8a, 0x01008f8e, 0x05040302, 0x09080706),  /* shl  6 (16 -10)/shr10*/
            Vector128.Create(0x8e8d8c8b, 0x0201008f, 0x06050403, 0x0a090807),  /* shl  5 (16 -11)/shr11*/
            Vector128.Create(0x8f8e8d8c, 0x03020100, 0x07060504, 0x0b0a0908),  /* shl  4 (16 -12)/shr12*/
            Vector128.Create(0x008f8e8du, 0x04030201, 0x08070605, 0x0c0b0a09), /* shl  3 (16 -13)/shr13*/
            Vector128.Create(0x01008f8eu, 0x05040302, 0x09080706, 0x0d0c0b0a), /* shl  2 (16 -14)/shr14*/
            Vector128.Create(0x0201008fu, 0x06050403, 0x0a090807, 0x0e0d0c0b)  /* shl  1 (16 -15)/shr15*/
        };

        static Vector128<ulong> vmull_p64(Vector64<ulong> a, Vector64<ulong> b)
        {
            if(Aes.IsSupported)
            {
                return Aes.PolynomialMultiplyWideningLower(a, b);
            }

            // Masks
            Vector128<byte> k4832 = Vector128.Create(Vector64.Create(0x0000fffffffffffful),
                                                     Vector64.Create(0x00000000fffffffful)).AsByte();

            Vector128<byte> k1600 = Vector128.Create(Vector64.Create(0x000000000000fffful),
                                                     Vector64.Create(0x0000000000000000ul)).AsByte();

            // Do the multiplies, rotating with vext to get all combinations
            Vector128<byte> d = AdvSimd.PolynomialMultiplyWideningLower(a.AsByte(), b.AsByte()).AsByte(); // D = A0 * B0

            Vector128<byte> e = AdvSimd.
                                PolynomialMultiplyWideningLower(a.AsByte(),
                                                                AdvSimd.ExtractVector64(b.AsByte(), b.AsByte(), 1)).
                                AsByte(); // E = A0 * B1

            Vector128<byte> f = AdvSimd.
                                PolynomialMultiplyWideningLower(AdvSimd.ExtractVector64(a.AsByte(), a.AsByte(), 1),
                                                                b.AsByte()).AsByte(); // F = A1 * B0

            Vector128<byte> g = AdvSimd.
                                PolynomialMultiplyWideningLower(a.AsByte(),
                                                                AdvSimd.ExtractVector64(b.AsByte(), b.AsByte(), 2)).
                                AsByte(); // G = A0 * B2

            Vector128<byte> h = AdvSimd.
                                PolynomialMultiplyWideningLower(AdvSimd.ExtractVector64(a.AsByte(), a.AsByte(), 2),
                                                                b.AsByte()).AsByte(); // H = A2 * B0

            Vector128<byte> i = AdvSimd.
                                PolynomialMultiplyWideningLower(a.AsByte(),
                                                                AdvSimd.ExtractVector64(b.AsByte(), b.AsByte(), 3)).
                                AsByte(); // I = A0 * B3

            Vector128<byte> j = AdvSimd.
                                PolynomialMultiplyWideningLower(AdvSimd.ExtractVector64(a.AsByte(), a.AsByte(), 3),
                                                                b.AsByte()).AsByte(); // J = A3 * B0

            Vector128<byte> k = AdvSimd.
                                PolynomialMultiplyWideningLower(a.AsByte(),
                                                                AdvSimd.ExtractVector64(b.AsByte(), b.AsByte(), 4)).
                                AsByte(); // L = A0 * B4

            // Add cross products
            Vector128<byte> l = AdvSimd.Xor(e, f); // L = E + F
            Vector128<byte> m = AdvSimd.Xor(g, h); // M = G + H
            Vector128<byte> n = AdvSimd.Xor(i, j); // N = I + J

            Vector128<byte> lmP0;
            Vector128<byte> lmP1;
            Vector128<byte> nkP0;
            Vector128<byte> nkP1;

            // Interleave. Using vzip1 and vzip2 prevents Clang from emitting TBL
            // instructions.
            if(AdvSimd.Arm64.IsSupported)
            {
                lmP0 = AdvSimd.Arm64.ZipLow(l.AsUInt64(), m.AsUInt64()).AsByte();
                lmP1 = AdvSimd.Arm64.ZipHigh(l.AsUInt64(), m.AsUInt64()).AsByte();
                nkP0 = AdvSimd.Arm64.ZipLow(n.AsUInt64(), k.AsUInt64()).AsByte();
                nkP1 = AdvSimd.Arm64.ZipHigh(n.AsUInt64(), k.AsUInt64()).AsByte();
            }
            else
            {
                lmP0 = Vector128.Create(l.GetLower(), m.GetLower());
                lmP1 = Vector128.Create(l.GetUpper(), m.GetUpper());
                nkP0 = Vector128.Create(n.GetLower(), k.GetLower());
                nkP1 = Vector128.Create(n.GetUpper(), k.GetUpper());
            }

            // t0 = (L) (P0 + P1) << 8
            // t1 = (M) (P2 + P3) << 16
            Vector128<byte> t0T1Tmp = AdvSimd.Xor(lmP0, lmP1);
            Vector128<byte> t0T1H   = AdvSimd.And(lmP1, k4832);
            Vector128<byte> t0T1L   = AdvSimd.Xor(t0T1Tmp, t0T1H);

            // t2 = (N) (P4 + P5) << 24
            // t3 = (K) (P6 + P7) << 32
            Vector128<byte> t2T3Tmp = AdvSimd.Xor(nkP0, nkP1);
            Vector128<byte> t2T3H   = AdvSimd.And(nkP1, k1600);
            Vector128<byte> t2T3L   = AdvSimd.Xor(t2T3Tmp, t2T3H);

            Vector128<byte> t1;
            Vector128<byte> t0;
            Vector128<byte> t3;
            Vector128<byte> t2;

            // De-interleave
            if(AdvSimd.Arm64.IsSupported)
            {
                t0 = AdvSimd.Arm64.UnzipEven(t0T1L.AsUInt64(), t0T1H.AsUInt64()).AsByte();
                t1 = AdvSimd.Arm64.UnzipOdd(t0T1L.AsUInt64(), t0T1H.AsUInt64()).AsByte();
                t2 = AdvSimd.Arm64.UnzipEven(t2T3L.AsUInt64(), t2T3H.AsUInt64()).AsByte();
                t3 = AdvSimd.Arm64.UnzipOdd(t2T3L.AsUInt64(), t2T3H.AsUInt64()).AsByte();
            }
            else
            {
                t1 = Vector128.Create(t0T1L.GetUpper(), t0T1H.GetUpper());
                t0 = Vector128.Create(t0T1L.GetLower(), t0T1H.GetLower());
                t3 = Vector128.Create(t2T3L.GetUpper(), t2T3H.GetUpper());
                t2 = Vector128.Create(t2T3L.GetLower(), t2T3H.GetLower());
            }

            // Shift the cross products
            Vector128<byte> t0Shift = AdvSimd.ExtractVector128(t0, t0, 15); // t0 << 8
            Vector128<byte> t1Shift = AdvSimd.ExtractVector128(t1, t1, 14); // t1 << 16
            Vector128<byte> t2Shift = AdvSimd.ExtractVector128(t2, t2, 13); // t2 << 24
            Vector128<byte> t3Shift = AdvSimd.ExtractVector128(t3, t3, 12); // t3 << 32

            // Accumulate the products
            Vector128<byte> cross1 = AdvSimd.Xor(t0Shift, t1Shift);
            Vector128<byte> cross2 = AdvSimd.Xor(t2Shift, t3Shift);
            Vector128<byte> mix    = AdvSimd.Xor(d, cross1);
            Vector128<byte> r      = AdvSimd.Xor(mix, cross2);

            return r.AsUInt64();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Vector128<ulong> mm_shuffle_epi8(Vector128<ulong> a, Vector128<ulong> b)
        {
            Vector128<byte> tbl = a.AsByte(); // input a
            Vector128<byte> idx = b.AsByte(); // input b

            Vector128<byte>
                idxMasked = AdvSimd.And(idx, AdvSimd.DuplicateToVector128((byte)0x8F)); // avoid using meaningless bits

            return AdvSimd.Arm64.VectorTableLookup(tbl, idxMasked).AsUInt64();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Fold4(ref Vector128<ulong> qCRC0, ref Vector128<ulong> qCRC1, ref Vector128<ulong> qCRC2,
                          ref Vector128<ulong> qCRC3)
        {
            Vector128<ulong> qFold4 = Vector128.Create(0xc6e41596, 0x00000001, 0x54442bd4, 0x00000001).AsUInt64();

            Vector128<ulong> xTmp0 = qCRC0;
            Vector128<ulong> xTmp1 = qCRC1;
            Vector128<ulong> xTmp2 = qCRC2;
            Vector128<ulong> xTmp3 = qCRC3;

            qCRC0 = vmull_p64(qCRC0.GetUpper(), qFold4.GetLower());
            xTmp0 = vmull_p64(xTmp0.GetLower(), qFold4.GetUpper());
            Vector128<uint> psCRC0 = qCRC0.AsUInt32();
            Vector128<uint> psT0   = xTmp0.AsUInt32();
            Vector128<uint> psRes0 = AdvSimd.Xor(psCRC0, psT0);

            qCRC1 = vmull_p64(qCRC1.GetUpper(), qFold4.GetLower());
            xTmp1 = vmull_p64(xTmp1.GetLower(), qFold4.GetUpper());
            Vector128<uint> psCRC1 = qCRC1.AsUInt32();
            Vector128<uint> psT1   = xTmp1.AsUInt32();
            Vector128<uint> psRes1 = AdvSimd.Xor(psCRC1, psT1);

            qCRC2 = vmull_p64(qCRC2.GetUpper(), qFold4.GetLower());
            xTmp2 = vmull_p64(xTmp2.GetLower(), qFold4.GetUpper());
            Vector128<uint> psCRC2 = qCRC2.AsUInt32();
            Vector128<uint> psT2   = xTmp2.AsUInt32();
            Vector128<uint> psRes2 = AdvSimd.Xor(psCRC2, psT2);

            qCRC3 = vmull_p64(qCRC3.GetUpper(), qFold4.GetLower());
            xTmp3 = vmull_p64(xTmp3.GetLower(), qFold4.GetUpper());
            Vector128<uint> psCRC3 = qCRC3.AsUInt32();
            Vector128<uint> psT3   = xTmp3.AsUInt32();
            Vector128<uint> psRes3 = AdvSimd.Xor(psCRC3, psT3);

            qCRC0 = psRes0.AsUInt64();
            qCRC1 = psRes1.AsUInt64();
            qCRC2 = psRes2.AsUInt64();
            qCRC3 = psRes3.AsUInt64();
        }

        internal static uint Step(byte[] src, long len, uint initialCRC)
        {
            Vector128<ulong> qT0;
            Vector128<ulong> qT1;
            Vector128<ulong> qT2;
            Vector128<ulong> qT3;
            Vector128<ulong> qInitial = AdvSimd.Insert(Vector128<uint>.Zero, 0, initialCRC).AsUInt64();
            Vector128<ulong> qCRC0    = AdvSimd.Insert(Vector128<uint>.Zero, 0, 0x9db42487).AsUInt64();
            Vector128<ulong> qCRC1    = Vector128<ulong>.Zero;
            Vector128<ulong> qCRC2    = Vector128<ulong>.Zero;
            Vector128<ulong> qCRC3    = Vector128<ulong>.Zero;
            int              bufPos   = 0;

            bool first = true;

            /* fold 512 to 32 step variable declarations for ISO-C90 compat. */
            Vector128<uint> qMask  = Vector128.Create(0xFFFFFFFF, 0xFFFFFFFF, 0x00000000, 0x00000000);
            Vector128<uint> qMask2 = Vector128.Create(0x00000000, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF);

            uint             crc;
            Vector128<ulong> xTmp0;
            Vector128<ulong> xTmp1;
            Vector128<ulong> xTmp2;
            Vector128<ulong> crcFold;

            while((len -= 64) >= 0)
            {
                qT0 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                       BitConverter.ToUInt32(src, bufPos + 8), BitConverter.ToUInt32(src, bufPos + 12)).
                                AsUInt64();

                bufPos += 16;

                qT1 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                       BitConverter.ToUInt32(src, bufPos + 8), BitConverter.ToUInt32(src, bufPos + 12)).
                                AsUInt64();

                bufPos += 16;

                qT2 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                       BitConverter.ToUInt32(src, bufPos + 8), BitConverter.ToUInt32(src, bufPos + 12)).
                                AsUInt64();

                bufPos += 16;

                qT3 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                       BitConverter.ToUInt32(src, bufPos + 8), BitConverter.ToUInt32(src, bufPos + 12)).
                                AsUInt64();

                bufPos += 16;

                if(first)
                {
                    first = false;

                    qT0 = AdvSimd.Xor(qT0.AsUInt32(), qInitial.AsUInt32()).AsUInt64();
                }

                Fold4(ref qCRC0, ref qCRC1, ref qCRC2, ref qCRC3);

                qCRC0 = AdvSimd.Xor(qCRC0.AsUInt32(), qT0.AsUInt32()).AsUInt64();
                qCRC1 = AdvSimd.Xor(qCRC1.AsUInt32(), qT1.AsUInt32()).AsUInt64();
                qCRC2 = AdvSimd.Xor(qCRC2.AsUInt32(), qT2.AsUInt32()).AsUInt64();
                qCRC3 = AdvSimd.Xor(qCRC3.AsUInt32(), qT3.AsUInt32()).AsUInt64();
            }

            /* fold 512 to 32 */

            /*
             * k1
             */
            crcFold = Vector128.Create(_crcK[0], _crcK[1], _crcK[2], _crcK[3]).AsUInt64();

            xTmp0 = vmull_p64(qCRC0.GetLower(), crcFold.GetUpper());
            qCRC0 = vmull_p64(qCRC0.GetUpper(), crcFold.GetLower());
            qCRC1 = AdvSimd.Xor(qCRC1.AsUInt32(), xTmp0.AsUInt32()).AsUInt64();
            qCRC1 = AdvSimd.Xor(qCRC1.AsUInt32(), qCRC0.AsUInt32()).AsUInt64();

            xTmp1 = vmull_p64(qCRC1.GetLower(), crcFold.GetUpper());
            qCRC1 = vmull_p64(qCRC1.GetUpper(), crcFold.GetLower());
            qCRC2 = AdvSimd.Xor(qCRC2.AsUInt32(), xTmp1.AsUInt32()).AsUInt64();
            qCRC2 = AdvSimd.Xor(qCRC2.AsUInt32(), qCRC1.AsUInt32()).AsUInt64();

            xTmp2 = vmull_p64(qCRC2.GetLower(), crcFold.GetUpper());
            qCRC2 = vmull_p64(qCRC2.GetUpper(), crcFold.GetLower());
            qCRC3 = AdvSimd.Xor(qCRC3.AsUInt32(), xTmp2.AsUInt32()).AsUInt64();
            qCRC3 = AdvSimd.Xor(qCRC3.AsUInt32(), qCRC2.AsUInt32()).AsUInt64();

            /*
             * k5
             */
            crcFold = Vector128.Create(_crcK[4], _crcK[5], _crcK[6], _crcK[7]).AsUInt64();

            qCRC0 = qCRC3;
            qCRC3 = vmull_p64(qCRC3.GetLower(), crcFold.GetLower());

            Vector128<byte> qCRC0B = qCRC0.AsByte();

            qCRC0 = Vector128.Create(AdvSimd.Extract(qCRC0B, 8), AdvSimd.Extract(qCRC0B, 9),
                                     AdvSimd.Extract(qCRC0B, 10), AdvSimd.Extract(qCRC0B, 11),
                                     AdvSimd.Extract(qCRC0B, 12), AdvSimd.Extract(qCRC0B, 13),
                                     AdvSimd.Extract(qCRC0B, 14), AdvSimd.Extract(qCRC0B, 15), 0, 0, 0, 0, 0, 0, 0, 0).
                              AsUInt64();

            qCRC3 = AdvSimd.Xor(qCRC3.AsUInt32(), qCRC0.AsUInt32()).AsUInt64();

            qCRC0 = qCRC3;

            Vector128<byte> qCRC3B = qCRC3.AsByte();

            qCRC3 = Vector128.Create(0, 0, 0, 0, AdvSimd.Extract(qCRC3B, 0), AdvSimd.Extract(qCRC3B, 1),
                                     AdvSimd.Extract(qCRC3B, 2), AdvSimd.Extract(qCRC3B, 3), AdvSimd.Extract(qCRC3B, 4),
                                     AdvSimd.Extract(qCRC3B, 5), AdvSimd.Extract(qCRC3B, 6), AdvSimd.Extract(qCRC3B, 7),
                                     AdvSimd.Extract(qCRC3B, 8), AdvSimd.Extract(qCRC3B, 9),
                                     AdvSimd.Extract(qCRC3B, 10), AdvSimd.Extract(qCRC3B, 11)).AsUInt64();

            qCRC3 = vmull_p64(qCRC3.GetLower(), crcFold.GetUpper());
            qCRC3 = AdvSimd.Xor(qCRC3.AsUInt32(), qCRC0.AsUInt32()).AsUInt64();
            qCRC3 = AdvSimd.And(qCRC3.AsUInt32(), qMask2.AsUInt32()).AsUInt64();

            /*
             * k7
             */
            qCRC1   = qCRC3;
            qCRC2   = qCRC3;
            crcFold = Vector128.Create(_crcK[8], _crcK[9], _crcK[10], _crcK[11]).AsUInt64();

            qCRC3 = vmull_p64(qCRC3.GetLower(), crcFold.GetLower());
            qCRC3 = AdvSimd.Xor(qCRC3.AsUInt32(), qCRC2.AsUInt32()).AsUInt64();
            qCRC3 = AdvSimd.And(qCRC3.AsUInt32(), qMask.AsUInt32()).AsUInt64();

            qCRC2 = qCRC3;
            qCRC3 = vmull_p64(qCRC3.GetLower(), crcFold.GetUpper());
            qCRC3 = AdvSimd.Xor(qCRC3.AsUInt32(), qCRC2.AsUInt32()).AsUInt64();
            qCRC3 = AdvSimd.Xor(qCRC3.AsUInt32(), qCRC1.AsUInt32()).AsUInt64();

            /*
             * could just as well write q_crc3[2], doing a movaps and truncating, but
             * no real advantage - it's a tiny bit slower per call, while no additional CPUs
             * would be supported by only requiring SSSE3 and CLMUL instead of SSE4.1 + CLMUL
             */
            return ~AdvSimd.Extract(qCRC3.AsUInt32(), 2);
        }
    }
}