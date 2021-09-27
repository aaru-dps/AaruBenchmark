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
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Aaru6.Checksums
{
    public static class CRC32CLMUL
    {
        static readonly uint[] crc_k =
        {
            0xccaa009e, 0x00000000, /* rk1 */ 0x751997d0, 0x00000001, /* rk2 */ 0xccaa009e, 0x00000000, /* rk5 */
            0x63cd6124, 0x00000001, /* rk6 */ 0xf7011640, 0x00000001, /* rk7 */ 0xdb710640, 0x00000001  /* rk8 */
        };

        static readonly Vector128<uint>[] pshufb_shf_table =
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

        static void fold_1(ref Vector128<uint> xmm_crc0, ref Vector128<uint> xmm_crc1, ref Vector128<uint> xmm_crc2,
                           ref Vector128<uint> xmm_crc3)
        {
            Vector128<uint> xmm_fold4 = Vector128.Create(0xc6e41596, 0x00000001, 0x54442bd4, 0x00000001);

            Vector128<uint>  x_tmp3;
            Vector128<float> ps_crc0, ps_crc3, ps_res;

            x_tmp3 = xmm_crc3;

            xmm_crc3 = xmm_crc0;
            xmm_crc0 = Pclmulqdq.CarrylessMultiply(xmm_crc0.AsUInt64(), xmm_fold4.AsUInt64(), 0x01).AsUInt32();
            xmm_crc3 = Pclmulqdq.CarrylessMultiply(xmm_crc3.AsUInt64(), xmm_fold4.AsUInt64(), 0x10).AsUInt32();
            ps_crc0  = xmm_crc0.AsSingle();
            ps_crc3  = xmm_crc3.AsSingle();
            ps_res   = Sse.Xor(ps_crc0, ps_crc3);

            xmm_crc0 = xmm_crc1;
            xmm_crc1 = xmm_crc2;
            xmm_crc2 = x_tmp3;
            xmm_crc3 = ps_res.AsUInt32();
        }

        static void fold_2(ref Vector128<uint> xmm_crc0, ref Vector128<uint> xmm_crc1, ref Vector128<uint> xmm_crc2,
                           ref Vector128<uint> xmm_crc3)
        {
            Vector128<uint> xmm_fold4 = Vector128.Create(0xc6e41596, 0x00000001, 0x54442bd4, 0x00000001);

            Vector128<uint>  x_tmp3,  x_tmp2;
            Vector128<float> ps_crc0, ps_crc1, ps_crc2, ps_crc3, ps_res31, ps_res20;

            x_tmp3 = xmm_crc3;
            x_tmp2 = xmm_crc2;

            xmm_crc3 = xmm_crc1;
            xmm_crc1 = Pclmulqdq.CarrylessMultiply(xmm_crc1.AsUInt64(), xmm_fold4.AsUInt64(), 0x01).AsUInt32();
            xmm_crc3 = Pclmulqdq.CarrylessMultiply(xmm_crc3.AsUInt64(), xmm_fold4.AsUInt64(), 0x10).AsUInt32();
            ps_crc3  = xmm_crc3.AsSingle();
            ps_crc1  = xmm_crc1.AsSingle();
            ps_res31 = Sse.Xor(ps_crc3, ps_crc1);

            xmm_crc2 = xmm_crc0;
            xmm_crc0 = Pclmulqdq.CarrylessMultiply(xmm_crc0.AsUInt64(), xmm_fold4.AsUInt64(), 0x01).AsUInt32();
            xmm_crc2 = Pclmulqdq.CarrylessMultiply(xmm_crc2.AsUInt64(), xmm_fold4.AsUInt64(), 0x10).AsUInt32();
            ps_crc0  = xmm_crc0.AsSingle();
            ps_crc2  = xmm_crc2.AsSingle();
            ps_res20 = Sse.Xor(ps_crc0, ps_crc2);

            xmm_crc0 = x_tmp2;
            xmm_crc1 = x_tmp3;
            xmm_crc2 = ps_res20.AsUInt32();
            xmm_crc3 = ps_res31.AsUInt32();
        }

        static void fold_3(ref Vector128<uint> xmm_crc0, ref Vector128<uint> xmm_crc1, ref Vector128<uint> xmm_crc2,
                           ref Vector128<uint> xmm_crc3)
        {
            Vector128<uint> xmm_fold4 = Vector128.Create(0x54442bd4, 0x00000001, 0xc6e41596, 0x00000001);

            Vector128<uint>  x_tmp3;
            Vector128<float> ps_crc0, ps_crc1, ps_crc2, ps_crc3, ps_res32, ps_res21, ps_res10;

            x_tmp3 = xmm_crc3;

            xmm_crc3 = xmm_crc2;
            xmm_crc2 = Pclmulqdq.CarrylessMultiply(xmm_crc2.AsUInt64(), xmm_fold4.AsUInt64(), 0x01).AsUInt32();
            xmm_crc3 = Pclmulqdq.CarrylessMultiply(xmm_crc3.AsUInt64(), xmm_fold4.AsUInt64(), 0x10).AsUInt32();
            ps_crc2  = xmm_crc2.AsSingle();
            ps_crc3  = xmm_crc3.AsSingle();
            ps_res32 = Sse.Xor(ps_crc2, ps_crc3);

            xmm_crc2 = xmm_crc1;
            xmm_crc1 = Pclmulqdq.CarrylessMultiply(xmm_crc1.AsUInt64(), xmm_fold4.AsUInt64(), 0x01).AsUInt32();
            xmm_crc2 = Pclmulqdq.CarrylessMultiply(xmm_crc2.AsUInt64(), xmm_fold4.AsUInt64(), 0x10).AsUInt32();
            ps_crc1  = xmm_crc1.AsSingle();
            ps_crc2  = xmm_crc2.AsSingle();
            ps_res21 = Sse.Xor(ps_crc1, ps_crc2);

            xmm_crc1 = xmm_crc0;
            xmm_crc0 = Pclmulqdq.CarrylessMultiply(xmm_crc0.AsUInt64(), xmm_fold4.AsUInt64(), 0x01).AsUInt32();
            xmm_crc1 = Pclmulqdq.CarrylessMultiply(xmm_crc1.AsUInt64(), xmm_fold4.AsUInt64(), 0x10).AsUInt32();
            ps_crc0  = xmm_crc0.AsSingle();
            ps_crc1  = xmm_crc1.AsSingle();
            ps_res10 = Sse.Xor(ps_crc0, ps_crc1);

            xmm_crc0 = x_tmp3;
            xmm_crc1 = ps_res10.AsUInt32();
            xmm_crc2 = ps_res21.AsUInt32();
            xmm_crc3 = ps_res32.AsUInt32();
        }

        static void fold_4(ref Vector128<uint> xmm_crc0, ref Vector128<uint> xmm_crc1, ref Vector128<uint> xmm_crc2,
                           ref Vector128<uint> xmm_crc3)
        {
            Vector128<uint> xmm_fold4 = Vector128.Create(0xc6e41596, 0x00000001, 0x54442bd4, 0x00000001);

            Vector128<uint>  x_tmp0,  x_tmp1,  x_tmp2,  x_tmp3;
            Vector128<float> ps_crc0, ps_crc1, ps_crc2, ps_crc3;
            Vector128<float> ps_t0,   ps_t1,   ps_t2,   ps_t3;
            Vector128<float> ps_res0, ps_res1, ps_res2, ps_res3;

            x_tmp0 = xmm_crc0;
            x_tmp1 = xmm_crc1;
            x_tmp2 = xmm_crc2;
            x_tmp3 = xmm_crc3;

            xmm_crc0 = Pclmulqdq.CarrylessMultiply(xmm_crc0.AsUInt64(), xmm_fold4.AsUInt64(), 0x01).AsUInt32();
            x_tmp0   = Pclmulqdq.CarrylessMultiply(x_tmp0.AsUInt64(), xmm_fold4.AsUInt64(), 0x10).AsUInt32();
            ps_crc0  = xmm_crc0.AsSingle();
            ps_t0    = x_tmp0.AsSingle();
            ps_res0  = Sse.Xor(ps_crc0, ps_t0);

            xmm_crc1 = Pclmulqdq.CarrylessMultiply(xmm_crc1.AsUInt64(), xmm_fold4.AsUInt64(), 0x01).AsUInt32();
            x_tmp1   = Pclmulqdq.CarrylessMultiply(x_tmp1.AsUInt64(), xmm_fold4.AsUInt64(), 0x10).AsUInt32();
            ps_crc1  = xmm_crc1.AsSingle();
            ps_t1    = x_tmp1.AsSingle();
            ps_res1  = Sse.Xor(ps_crc1, ps_t1);

            xmm_crc2 = Pclmulqdq.CarrylessMultiply(xmm_crc2.AsUInt64(), xmm_fold4.AsUInt64(), 0x01).AsUInt32();
            x_tmp2   = Pclmulqdq.CarrylessMultiply(x_tmp2.AsUInt64(), xmm_fold4.AsUInt64(), 0x10).AsUInt32();
            ps_crc2  = xmm_crc2.AsSingle();
            ps_t2    = x_tmp2.AsSingle();
            ps_res2  = Sse.Xor(ps_crc2, ps_t2);

            xmm_crc3 = Pclmulqdq.CarrylessMultiply(xmm_crc3.AsUInt64(), xmm_fold4.AsUInt64(), 0x01).AsUInt32();
            x_tmp3   = Pclmulqdq.CarrylessMultiply(x_tmp3.AsUInt64(), xmm_fold4.AsUInt64(), 0x10).AsUInt32();
            ps_crc3  = xmm_crc3.AsSingle();
            ps_t3    = x_tmp3.AsSingle();
            ps_res3  = Sse.Xor(ps_crc3, ps_t3);

            xmm_crc0 = ps_res0.AsUInt32();
            xmm_crc1 = ps_res1.AsUInt32();
            xmm_crc2 = ps_res2.AsUInt32();
            xmm_crc3 = ps_res3.AsUInt32();
        }

        static void partial_fold(long len, ref Vector128<uint> xmm_crc0, ref Vector128<uint> xmm_crc1,
                                 ref Vector128<uint> xmm_crc2, ref Vector128<uint> xmm_crc3,
                                 ref Vector128<uint> xmm_crc_part)
        {
            Vector128<uint> xmm_fold4 = Vector128.Create(0x54442bd4, 0x00000001, 0xc6e41596, 0x00000001);
            Vector128<uint> xmm_mask3 = Vector128.Create(0x80808080);

            Vector128<uint>  xmm_shl,  xmm_shr, xmm_tmp1, xmm_tmp2, xmm_tmp3;
            Vector128<uint>  xmm_a0_0, xmm_a0_1;
            Vector128<float> ps_crc3,  psa0_0, psa0_1, ps_res;

            xmm_shl = pshufb_shf_table[len - 1];
            xmm_shr = xmm_shl;
            xmm_shr = Sse2.Xor(xmm_shr, xmm_mask3);

            xmm_a0_0 = Ssse3.Shuffle(xmm_crc0.AsByte(), xmm_shl.AsByte()).AsUInt32();

            xmm_crc0 = Ssse3.Shuffle(xmm_crc0.AsByte(), xmm_shr.AsByte()).AsUInt32();
            xmm_tmp1 = Ssse3.Shuffle(xmm_crc1.AsByte(), xmm_shl.AsByte()).AsUInt32();
            xmm_crc0 = Sse2.Or(xmm_crc0, xmm_tmp1);

            xmm_crc1 = Ssse3.Shuffle(xmm_crc1.AsByte(), xmm_shr.AsByte()).AsUInt32();
            xmm_tmp2 = Ssse3.Shuffle(xmm_crc2.AsByte(), xmm_shl.AsByte()).AsUInt32();
            xmm_crc1 = Sse2.Or(xmm_crc1, xmm_tmp2);

            xmm_crc2 = Ssse3.Shuffle(xmm_crc2.AsByte(), xmm_shr.AsByte()).AsUInt32();
            xmm_tmp3 = Ssse3.Shuffle(xmm_crc3.AsByte(), xmm_shl.AsByte()).AsUInt32();
            xmm_crc2 = Sse2.Or(xmm_crc2, xmm_tmp3);

            xmm_crc3     = Ssse3.Shuffle(xmm_crc3.AsByte(), xmm_shr.AsByte()).AsUInt32();
            xmm_crc_part = Ssse3.Shuffle(xmm_crc_part.AsByte(), xmm_shl.AsByte()).AsUInt32();
            xmm_crc3     = Sse2.Or(xmm_crc3, xmm_crc_part);

            xmm_a0_1 = Pclmulqdq.CarrylessMultiply(xmm_a0_0.AsUInt64(), xmm_fold4.AsUInt64(), 0x10).AsUInt32();
            xmm_a0_0 = Pclmulqdq.CarrylessMultiply(xmm_a0_0.AsUInt64(), xmm_fold4.AsUInt64(), 0x01).AsUInt32();

            ps_crc3 = xmm_crc3.AsSingle();
            psa0_0  = xmm_a0_0.AsSingle();
            psa0_1  = xmm_a0_1.AsSingle();

            ps_res = Sse.Xor(ps_crc3, psa0_0);
            ps_res = Sse.Xor(ps_res, psa0_1);

            xmm_crc3 = ps_res.AsUInt32();
        }

        internal static uint crc32_clmul(byte[] src, long len, uint initial_crc)
        {
            Vector128<uint> xmm_t0, xmm_t1, xmm_t2, xmm_t3;
            Vector128<uint> xmm_initial = Sse2.ConvertScalarToVector128UInt32(initial_crc);
            Vector128<uint> xmm_crc0    = Sse2.ConvertScalarToVector128UInt32(0x9db42487);
            Vector128<uint> xmm_crc1    = Vector128<uint>.Zero;
            Vector128<uint> xmm_crc2    = Vector128<uint>.Zero;
            Vector128<uint> xmm_crc3    = Vector128<uint>.Zero;
            Vector128<uint> xmm_crc_part;
            int             bufPos = 0;

            bool first = true;

            /* fold 512 to 32 step variable declarations for ISO-C90 compat. */
            Vector128<uint> xmm_mask  = Vector128.Create(0xFFFFFFFF, 0xFFFFFFFF, 0x00000000, 0x00000000);
            Vector128<uint> xmm_mask2 = Vector128.Create(0x00000000, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF);

            uint            crc;
            Vector128<uint> x_tmp0, x_tmp1, x_tmp2, crc_fold;

            if(len < 16)
            {
                if(len == 0)
                    return initial_crc;

                if(len < 4)
                {
                    /*
                     * no idea how to do this for <4 bytes, delegate to classic impl.
                     */
                    crc = ~initial_crc;

                    switch(len)
                    {
                        case 3:
                            crc = (crc >> 8) ^ Crc32Context._isoCrc32Table[0][(crc & 0xFF) ^ src[bufPos++]];
                            goto case 2;
                        case 2:
                            crc = (crc >> 8) ^ Crc32Context._isoCrc32Table[0][(crc & 0xFF) ^ src[bufPos++]];
                            goto case 1;
                        case 1:
                            crc = (crc >> 8) ^ Crc32Context._isoCrc32Table[0][(crc & 0xFF) ^ src[bufPos++]];

                            break;
                    }

                    return ~crc;
                }

                xmm_crc_part = Vector128.Create(BitConverter.ToUInt32(src, 0), BitConverter.ToUInt32(src, 4),
                                                BitConverter.ToUInt32(src, 8), BitConverter.ToUInt32(src, 12));

                if(first)
                {
                    first        = false;
                    xmm_crc_part = Sse2.Xor(xmm_crc_part, xmm_initial);
                }

                goto partial;
            }

            while((len -= 64) >= 0)
            {
                xmm_t0 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                          BitConverter.ToUInt32(src, bufPos                                     + 8),
                                          BitConverter.ToUInt32(src, bufPos                                     + 12));

                bufPos += 16;

                xmm_t1 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                          BitConverter.ToUInt32(src, bufPos                                     + 8),
                                          BitConverter.ToUInt32(src, bufPos                                     + 12));

                bufPos += 16;

                xmm_t2 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                          BitConverter.ToUInt32(src, bufPos                                     + 8),
                                          BitConverter.ToUInt32(src, bufPos                                     + 12));

                bufPos += 16;

                xmm_t3 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                          BitConverter.ToUInt32(src, bufPos                                     + 8),
                                          BitConverter.ToUInt32(src, bufPos                                     + 12));

                bufPos += 16;

                if(first)
                {
                    first  = false;
                    xmm_t0 = Sse2.Xor(xmm_t0, xmm_initial);
                }

                fold_4(ref xmm_crc0, ref xmm_crc1, ref xmm_crc2, ref xmm_crc3);

                xmm_crc0 = Sse2.Xor(xmm_crc0, xmm_t0);
                xmm_crc1 = Sse2.Xor(xmm_crc1, xmm_t1);
                xmm_crc2 = Sse2.Xor(xmm_crc2, xmm_t2);
                xmm_crc3 = Sse2.Xor(xmm_crc3, xmm_t3);
            }

            /*
             * len = num bytes left - 64
             */
            if(len + 16 >= 0)
            {
                len += 16;

                xmm_t0 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                          BitConverter.ToUInt32(src, bufPos                                     + 8),
                                          BitConverter.ToUInt32(src, bufPos                                     + 12));

                bufPos += 16;

                xmm_t1 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                          BitConverter.ToUInt32(src, bufPos                                     + 8),
                                          BitConverter.ToUInt32(src, bufPos                                     + 12));

                bufPos += 16;

                xmm_t2 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                          BitConverter.ToUInt32(src, bufPos                                     + 8),
                                          BitConverter.ToUInt32(src, bufPos                                     + 12));

                bufPos += 16;

                if(first)
                {
                    first  = false;
                    xmm_t0 = Sse2.Xor(xmm_t0, xmm_initial);
                }

                fold_3(ref xmm_crc0, ref xmm_crc1, ref xmm_crc2, ref xmm_crc3);

                xmm_crc1 = Sse2.Xor(xmm_crc1, xmm_t0);
                xmm_crc2 = Sse2.Xor(xmm_crc2, xmm_t1);
                xmm_crc3 = Sse2.Xor(xmm_crc3, xmm_t2);

                if(len == 0)
                    goto done;

                xmm_crc_part = Vector128.Create(BitConverter.ToUInt32(src, bufPos),
                                                BitConverter.ToUInt32(src, bufPos + 4),
                                                BitConverter.ToUInt32(src, bufPos + 8),
                                                BitConverter.ToUInt32(src, bufPos + 12));
            }
            else if(len + 32 >= 0)
            {
                len += 32;

                xmm_t0 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                          BitConverter.ToUInt32(src, bufPos                                     + 8),
                                          BitConverter.ToUInt32(src, bufPos                                     + 12));

                bufPos += 16;

                xmm_t1 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                          BitConverter.ToUInt32(src, bufPos                                     + 8),
                                          BitConverter.ToUInt32(src, bufPos                                     + 12));

                bufPos += 16;

                if(first)
                {
                    first  = false;
                    xmm_t0 = Sse2.Xor(xmm_t0, xmm_initial);
                }

                fold_2(ref xmm_crc0, ref xmm_crc1, ref xmm_crc2, ref xmm_crc3);

                xmm_crc2 = Sse2.Xor(xmm_crc2, xmm_t0);
                xmm_crc3 = Sse2.Xor(xmm_crc3, xmm_t1);

                if(len == 0)
                    goto done;

                xmm_crc_part = Vector128.Create(BitConverter.ToUInt32(src, bufPos),
                                                BitConverter.ToUInt32(src, bufPos + 4),
                                                BitConverter.ToUInt32(src, bufPos + 8),
                                                BitConverter.ToUInt32(src, bufPos + 12));
            }
            else if(len + 48 >= 0)
            {
                len += 48;

                xmm_t0 = Vector128.Create(BitConverter.ToUInt32(src, bufPos), BitConverter.ToUInt32(src, bufPos + 4),
                                          BitConverter.ToUInt32(src, bufPos                                     + 8),
                                          BitConverter.ToUInt32(src, bufPos                                     + 12));

                bufPos += 16;

                if(first)
                {
                    first  = false;
                    xmm_t0 = Sse2.Xor(xmm_t0, xmm_initial);
                }

                fold_1(ref xmm_crc0, ref xmm_crc1, ref xmm_crc2, ref xmm_crc3);

                xmm_crc3 = Sse2.Xor(xmm_crc3, xmm_t0);

                if(len == 0)
                    goto done;

                xmm_crc_part = Vector128.Create(BitConverter.ToUInt32(src, bufPos),
                                                BitConverter.ToUInt32(src, bufPos + 4),
                                                BitConverter.ToUInt32(src, bufPos + 8),
                                                BitConverter.ToUInt32(src, bufPos + 12));
            }
            else
            {
                len += 64;

                if(len == 0)
                    goto done;

                xmm_crc_part = Vector128.Create(BitConverter.ToUInt32(src, bufPos),
                                                BitConverter.ToUInt32(src, bufPos + 4),
                                                BitConverter.ToUInt32(src, bufPos + 8),
                                                BitConverter.ToUInt32(src, bufPos + 12));

                if(first)
                {
                    first        = false;
                    xmm_crc_part = Sse2.Xor(xmm_crc_part, xmm_initial);
                }
            }

            partial:
            partial_fold(len, ref xmm_crc0, ref xmm_crc1, ref xmm_crc2, ref xmm_crc3, ref xmm_crc_part);

            done:

            /* fold 512 to 32 */

            /*
             * k1
             */
            crc_fold = Vector128.Create(crc_k[0], crc_k[1], crc_k[2], crc_k[3]);

            x_tmp0   = Pclmulqdq.CarrylessMultiply(xmm_crc0.AsUInt64(), crc_fold.AsUInt64(), 0x10).AsUInt32();
            xmm_crc0 = Pclmulqdq.CarrylessMultiply(xmm_crc0.AsUInt64(), crc_fold.AsUInt64(), 0x01).AsUInt32();
            xmm_crc1 = Sse2.Xor(xmm_crc1, x_tmp0);
            xmm_crc1 = Sse2.Xor(xmm_crc1, xmm_crc0);

            x_tmp1   = Pclmulqdq.CarrylessMultiply(xmm_crc1.AsUInt64(), crc_fold.AsUInt64(), 0x10).AsUInt32();
            xmm_crc1 = Pclmulqdq.CarrylessMultiply(xmm_crc1.AsUInt64(), crc_fold.AsUInt64(), 0x01).AsUInt32();
            xmm_crc2 = Sse2.Xor(xmm_crc2, x_tmp1);
            xmm_crc2 = Sse2.Xor(xmm_crc2, xmm_crc1);

            x_tmp2   = Pclmulqdq.CarrylessMultiply(xmm_crc2.AsUInt64(), crc_fold.AsUInt64(), 0x10).AsUInt32();
            xmm_crc2 = Pclmulqdq.CarrylessMultiply(xmm_crc2.AsUInt64(), crc_fold.AsUInt64(), 0x01).AsUInt32();
            xmm_crc3 = Sse2.Xor(xmm_crc3, x_tmp2);
            xmm_crc3 = Sse2.Xor(xmm_crc3, xmm_crc2);

            /*
             * k5
             */
            crc_fold = Vector128.Create(crc_k[4], crc_k[5], crc_k[6], crc_k[7]);

            xmm_crc0 = xmm_crc3;
            xmm_crc3 = Pclmulqdq.CarrylessMultiply(xmm_crc3.AsUInt64(), crc_fold.AsUInt64(), 0).AsUInt32();
            xmm_crc0 = Sse2.ShiftRightLogical128BitLane(xmm_crc0, 8);
            xmm_crc3 = Sse2.Xor(xmm_crc3, xmm_crc0);

            xmm_crc0 = xmm_crc3;
            xmm_crc3 = Sse2.ShiftLeftLogical128BitLane(xmm_crc3, 4);
            xmm_crc3 = Pclmulqdq.CarrylessMultiply(xmm_crc3.AsUInt64(), crc_fold.AsUInt64(), 0x10).AsUInt32();
            xmm_crc3 = Sse2.Xor(xmm_crc3, xmm_crc0);
            xmm_crc3 = Sse2.And(xmm_crc3, xmm_mask2);

            /*
             * k7
             */
            xmm_crc1 = xmm_crc3;
            xmm_crc2 = xmm_crc3;
            crc_fold = Vector128.Create(crc_k[8], crc_k[9], crc_k[10], crc_k[11]);

            xmm_crc3 = Pclmulqdq.CarrylessMultiply(xmm_crc3.AsUInt64(), crc_fold.AsUInt64(), 0).AsUInt32();
            xmm_crc3 = Sse2.Xor(xmm_crc3, xmm_crc2);
            xmm_crc3 = Sse2.And(xmm_crc3, xmm_mask);

            xmm_crc2 = xmm_crc3;
            xmm_crc3 = Pclmulqdq.CarrylessMultiply(xmm_crc3.AsUInt64(), crc_fold.AsUInt64(), 0x10).AsUInt32();
            xmm_crc3 = Sse2.Xor(xmm_crc3, xmm_crc2);
            xmm_crc3 = Sse2.Xor(xmm_crc3, xmm_crc1);

            /*
             * could just as well write xmm_crc3[2], doing a movaps and truncating, but
             * no real advantage - it's a tiny bit slower per call, while no additional CPUs
             * would be supported by only requiring SSSE3 and CLMUL instead of SSE4.1 + CLMUL
             */
            crc = Sse41.Extract(xmm_crc3, 2);

            return ~crc;
        }
    }
}