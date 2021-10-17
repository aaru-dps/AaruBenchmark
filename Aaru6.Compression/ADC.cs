﻿//
// ADC.cs
//
// Author:
//       Natalia Portillo <claunia@claunia.com>
//
// Copyright (c) 2016 © Claunia.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Runtime.CompilerServices;

namespace Aaru6.Compression
{
    /// <summary>Implements the Apple version of RLE</summary>
    public class ADC
    {
        const int PLAIN      = 1;
        const int TWO_BYTE   = 2;
        const int THREE_BYTE = 3;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int GetChunkType(byte byt) => (byt & 0x80) == 0x80
                                                 ? PLAIN
                                                 : (byt & 0x40) == 0x40
                                                     ? THREE_BYTE
                                                     : TWO_BYTE;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int GetChunkSize(byte byt) => GetChunkType(byt) switch
        {
            PLAIN      => (byt & 0x7F)        + 1,
            TWO_BYTE   => ((byt & 0x3F) >> 2) + 3,
            THREE_BYTE => (byt & 0x3F)        + 4,
            _          => -1
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int GetOffset(ReadOnlySpan<byte> chunk) => GetChunkType(chunk[0]) switch
        {
            PLAIN      => 0,
            TWO_BYTE   => ((chunk[0] & 0x03) << 8) + chunk[1],
            THREE_BYTE => (chunk[1]          << 8) + chunk[2],
            _          => -1
        };

        /// <summary>Decompresses a byte buffer that's compressed with ADC</summary>
        /// <param name="source">Compressed buffer</param>
        /// <param name="destination">Buffer to hold decompressed data</param>
        /// <returns>How many bytes are stored on <paramref name="destination" /></returns>
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static int DecodeBuffer(byte[] source, byte[] destination)
        {
            int        inputPosition = 0;
            int        chunkSize;
            int        offset;
            int        chunkType;
            int        outPosition = 0;
            Span<byte> temp        = stackalloc byte[3];

            while(inputPosition < source.Length)
            {
                byte readByte = source[inputPosition++];

                chunkType = GetChunkType(readByte);

                switch(chunkType)
                {
                    case PLAIN:
                        chunkSize = GetChunkSize(readByte);

                        if(outPosition + chunkSize > destination.Length)
                            goto finished;

                        Array.Copy(source, inputPosition, destination, outPosition, chunkSize);
                        outPosition   += chunkSize;
                        inputPosition += chunkSize;

                        break;
                    case TWO_BYTE:
                        chunkSize = GetChunkSize(readByte);
                        temp[0]   = readByte;
                        temp[1]   = source[inputPosition++];
                        offset    = GetOffset(temp);

                        if(outPosition + chunkSize > destination.Length)
                            goto finished;

                        if(offset == 0)
                        {
                            byte lastByte = destination[outPosition - 1];

                            for(int i = 0; i < chunkSize; i++)
                            {
                                destination[outPosition] = lastByte;
                                outPosition++;
                            }
                        }
                        else
                        {
                            for(int i = 0; i < chunkSize; i++)
                            {
                                destination[outPosition] = destination[outPosition - offset - 1];
                                outPosition++;
                            }
                        }

                        break;
                    case THREE_BYTE:
                        chunkSize = GetChunkSize(readByte);
                        temp[0]   = readByte;
                        temp[1]   = source[inputPosition++];
                        temp[2]   = source[inputPosition++];
                        offset    = GetOffset(temp);

                        if(outPosition + chunkSize > destination.Length)
                            goto finished;

                        if(offset == 0)
                        {
                            byte lastByte = destination[outPosition - 1];

                            for(int i = 0; i < chunkSize; i++)
                            {
                                destination[outPosition] = lastByte;
                                outPosition++;
                            }
                        }
                        else
                        {
                            for(int i = 0; i < chunkSize; i++)
                            {
                                destination[outPosition] = destination[outPosition - offset - 1];
                                outPosition++;
                            }
                        }

                        break;
                }
            }

            finished:

            return outPosition;
        }
    }
}