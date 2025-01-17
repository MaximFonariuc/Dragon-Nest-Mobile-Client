using System;
using System.Text;
using System.IO;
using Unity.IO.Compression;


namespace com.tencent.pandora
{
    internal class Zlib
    {
        private static byte[] _decompressBuffer = new byte[512];

        public static byte[] Compress(string content)
        {
            byte[] array = Encoding.UTF8.GetBytes(content);
            return Compress(array, 0, array.Length);
        }

        /// <summary>
        /// 输入参数：原始数据
        /// 返回参数：用deflate算法压缩后的zlib格式数据，出错则返回空
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] array, int offset, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (array.Length - offset < count)
            {
                throw new ArgumentException("Invalid argument offset count");
            }

            MemoryStream memoryStream = new MemoryStream();
            using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true))
            {
                deflateStream.Write(array, offset, count);
            }
            byte[] compressedArray = new byte[memoryStream.Length];
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.Read(compressedArray, 0, compressedArray.Length);
            memoryStream.Dispose();

            // 计算zlib校验值
            ulong adler = 1;
            adler = Adler32(adler, array);

            // 填充zlib头2字节与为四字节
            byte[] header = new byte[] { 0x78, 0x01 };            // 头二字节
            byte[] tail = BitConverter.GetBytes((UInt32)adler);   // 尾四字节
            Array.Reverse(tail);  // 字节序转换

            byte[] zlibData = new byte[header.Length + compressedArray.Length + tail.Length];
            header.CopyTo(zlibData, 0);
            compressedArray.CopyTo(zlibData, header.Length);
            tail.CopyTo(zlibData, header.Length + compressedArray.Length);

            return zlibData;
        }

        public static string Decompress(byte[] array, int length)
        {
            byte[] decompressed = Decompress(array, 0, length);
            return Encoding.UTF8.GetString(decompressed);
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] array, int offset, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (array.Length - offset < count)
            {
                throw new ArgumentException("Invalid argument offset count");
            }
                
            // zlib有两字节头和4字节尾
            int headerLen = 2;
            int tailLen = 4;
            // 校验头尾
            if (CheckZlibHead(array, offset, count) != 0)
            {
                // 抛出异常
                throw new Exception("invalid zlib head");
            }

            // 去掉头尾，只取压缩数据
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(array, offset + headerLen, count - headerLen - tailLen); // 去掉头和尾，写入压缩数据
            memoryStream.Seek(0, SeekOrigin.Begin);

            MemoryStream outputStream = new MemoryStream();
            using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress, true))
            {
                while(true)
                {
                    int len = deflateStream.Read(_decompressBuffer, 0, _decompressBuffer.Length);
                    if(len <= 0)
                    {
                        break;
                    }
                    outputStream.Write(_decompressBuffer, 0, len);
                }
                outputStream.Seek(0, SeekOrigin.Begin);
                byte[] uncompressData = outputStream.ToArray();
                
                // 校验头尾
                if (CheckZlibTail(array, offset, count, uncompressData) != 0)
                {
                    // 抛出异常
                    throw new Exception("invalid zlib tail");
                }
                return uncompressData;
            }
        }

        // 对zlib源数据进行Adler32算法计算，生成校验码
        private static ulong Adler32(ulong adler, byte[] array)
        {
            ulong bufLen = (ulong)array.Length;
            ulong i;
            ulong s1 = (adler & 0xffff);
            ulong s2 = (adler >> 16);
            ulong blockLen = bufLen % 5552;
            ulong offset = 0;
            if (array.Length == 0) return 1;
            while (bufLen != 0)
            {
                for (i = 0; i + 7 < blockLen; i += 8, offset += 8)
                {
                    s1 += array[offset + 0];
                    s2 += s1;
                    s1 += array[offset + 1];
                    s2 += s1;
                    s1 += array[offset + 2];
                    s2 += s1;
                    s1 += array[offset + 3];
                    s2 += s1;

                    s1 += array[offset + 4];
                    s2 += s1;
                    s1 += array[offset + 5];
                    s2 += s1;
                    s1 += array[offset + 6];
                    s2 += s1;
                    s1 += array[offset + 7];
                    s2 += s1;
                }

                for (; i < blockLen; ++i)
                {
                    s1 += array[offset];
                    offset++;
                    s2 += s1;
                }

                s1 %= 65521U;
                s2 %= 65521U;
                bufLen -= blockLen;
                blockLen = 5552;
            }

            return (s2 << 16) + s1;
        }

        private static int CheckZlibHead(byte[] array, int offset, int count)
        {
            if (array[offset] != 0x78 || array[offset + 1] != 0x01)
            {
                // 头两字节无效
                return -1;
            }

            return 0;
        }

        private static int CheckZlibTail(byte[] array, int offset, int count, byte[] uncompressData)
        {
            // 计算zlib校验值
            ulong adler = 1;
            adler = Adler32(adler, uncompressData);

            byte[] tail = BitConverter.GetBytes((UInt32)adler);   // 尾四字节
            Array.Reverse(tail);  // 字节序转换

            // 比较tail是否相等
            for (int i = 0; i < 4; i++)
            {
                if (array[offset + count - 1 - i] != tail[4 - 1 - i])
                {
                    // 接收的校验值与算出来的校验值不相等
                    return -1;
                }
            }

            return 0;
        }
    }
}
