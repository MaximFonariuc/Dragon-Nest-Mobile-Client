using System;
using System.Net;

namespace com.tencent.pandora
{
    class MsdkTea
{
    private static uint kDelta = 0x9e3779b9;
    private static int kRounds = 16;
    private static int kLogRounds = 4;
    private static int kSaltLen = 2;
    private static int kZeroLen = 7;

    /************************************************************************************************
	对称加密底层函数
    ************************************************************************************************/
    //inBuf、outBuf均为8byte, key为16byte
    private static void TeaEncryptECB(byte[] inBuf, byte[] key, byte[] outBuf, int outBufStartPos)
    {
        uint y, z;
        uint sum;
        uint[] k = new uint[4];
        int i;

        y = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inBuf, 0));
        z = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inBuf, 4));

        for (i = 0; i < 4; ++i)
        {
            k[i] = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(key, i*4));
        }

        sum = 0;
        for (i = 0; i < kRounds; ++i)
        {
            sum += kDelta;
            y += ((z << 4) + k[0]) ^ (z + sum) ^ ((z >> 5) + k[1]);
            z += ((y << 4) + k[2]) ^ (y + sum) ^ ((y >> 5) + k[3]);
        }

        byte[] yBytes = BitConverter.GetBytes((uint)IPAddress.HostToNetworkOrder((int)y));
        byte[] zBytes = BitConverter.GetBytes((uint)IPAddress.HostToNetworkOrder((int)z));
        Array.Copy(yBytes, 0, outBuf, outBufStartPos, 4);
        Array.Copy(zBytes, 0, outBuf, outBufStartPos + 4, 4);
    }

    //inBuf、outBuf均为8byte, key为16byte
    private static void TeaDecryptECB(byte[] inBuf, byte[] key, byte[] outBuf)
    {
        uint y, z, sum;
        uint[] k = new uint[4];
        int i;

        y = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inBuf, 0));
        z = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inBuf, 4));

        for (i = 0; i < 4; ++i)
        {
            k[i] = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(key, i*4));
        }

        sum = kDelta << kLogRounds;
        for (i = 0; i < kRounds; ++i)
        {
            z -= ((y << 4) + k[2]) ^ (y + sum) ^ ((y >> 5) + k[3]);
            y -= ((z << 4) + k[0]) ^ (z + sum) ^ ((z >> 5) + k[1]); 
            sum -= kDelta;
        }

        byte[] yBytes = BitConverter.GetBytes((uint)IPAddress.HostToNetworkOrder((int)y));
        byte[] zBytes = BitConverter.GetBytes((uint)IPAddress.HostToNetworkOrder((int)z));
        Array.Copy(yBytes, 0, outBuf, 0, 4);
        Array.Copy(zBytes, 0, outBuf, 4, 4);
    }

    /************************************************************************************************
        QQ对称加密第二代函数
    ************************************************************************************************/

    /*key为16byte*/
    /*
        输入:inBufLen为需加密的明文部分(Body)长度;
        输出:返回为加密后的长度(是8byte的倍数);
    */
    /*TEA加密算法,CBC模式*/
    /*密文格式:PadLen(1byte)+Padding(var,0-7byte)+Salt(2byte)+Body(var byte)+Zero(7byte)*/
    public static int oi_symmetry_encrypt2_len(int nInBufLen)
    {
        int nPadSaltBodyZeroLen/*PadLen(1byte)+Salt+Body+Zero的长度*/;
        int nPadlen;

        /*根据Body长度计算PadLen,最小必需长度必需为8byte的整数倍*/
        nPadSaltBodyZeroLen = nInBufLen/*Body长度*/+ 1 + kSaltLen + kZeroLen/*PadLen(1byte)+Salt(2byte)+Zero(7byte)*/;
        if ((nPadlen = nPadSaltBodyZeroLen % 8) != 0) /*len=nSaltBodyZeroLen%8*/
        {
            /*模8余0需补0,余1补7,余2补6,...,余7补1*/
            nPadlen = 8 - nPadlen;
        }

        return nPadSaltBodyZeroLen + nPadlen;
    }

    /*key为16byte*/
    /*
        输入:inBuf为需加密的明文部分(Body),inBufLen为inBuf长度;
        输出:outBuf为密文格式,outBufLen为outBuf的长度是8byte的倍数,至少应预留inBufLen+17;
    */
    /*TEA加密算法,CBC模式*/
    /*密文格式:PadLen(1byte)+Padding(var,0-7byte)+Salt(2byte)+Body(var byte)+Zero(7byte)*/
    public static void oi_symmetry_encrypt2(byte[] inBuf, int inBufLen, byte[] key, byte[] outBuf, ref int outBufLen)
    {
        int inBufPos = 0, outBufPos = 0;// C# 不支持指针
	    int nPadSaltBodyZeroLen/*PadLen(1byte)+Salt+Body+Zero的长度*/;
	    int nPadlen;
        byte[] src_buf = new byte[8];
        byte[] iv_plain = new byte[8];
        byte[] iv_crypt = new byte[8];
	    int src_i, i, j;

        /*根据Body长度计算PadLen,最小必需长度必需为8byte的整数倍*/
        nPadSaltBodyZeroLen = inBufLen/*Body长度*/+ 1 + kSaltLen + kZeroLen/*PadLen(1byte)+Salt(2byte)+Zero(7byte)*/;
        if ((nPadlen = nPadSaltBodyZeroLen % 8) != 0) /*len=nSaltBodyZeroLen%8*/
        {
            /*模8余0需补0,余1补7,余2补6,...,余7补1*/
            nPadlen = 8 - nPadlen;
        }

        Random rnd = new Random();
        src_buf[0] = (byte)((rnd.Next(256) & 0x0f8)/*最低三位存PadLen,清零*/ | nPadlen);
        //src_buf[0] = (byte)((1 & 0x0f8)/*最低三位存PadLen,清零*/ | nPadlen);
        src_i = 1; /*src_i指向src_buf下一个位置*/

        while (nPadlen-- != 0)
            src_buf[src_i++] = (byte)rnd.Next(256); /*Padding*/
            //src_buf[src_i++] = (byte)2; /*Padding*/

        /*come here, src_i must <= 8*/

        for (i = 0; i < 8; i++)
            iv_plain[i] = 0;
        //byte[] iv_crypt = iv_plain; /*make zero iv*/

        outBufLen = 0; /*init OutBufLen*/

        for (i = 1; i <= kSaltLen; ) /*Salt(2byte)*/
        {
            if (src_i < 8)
            {
                src_buf[src_i++] = (byte)rnd.Next(256);
                //src_buf[src_i++] = (byte)3;
                i++; /*i inc in here*/
            }

            if (src_i == 8)
            {
                /*src_i==8*/

                for (j = 0; j < 8; j++) /*加密前异或前8个byte的密文(iv_crypt指向的)*/
                    src_buf[j] ^= iv_crypt[j];

                /*pOutBuffer、pInBuffer均为8byte, pKey为16byte*/
                /*加密*/
                TeaEncryptECB(src_buf, key, outBuf, outBufPos);

                for (j = 0; j < 8; j++) /*加密后异或前8个byte的明文(iv_plain指向的)*/
                    outBuf[outBufPos+j] ^= iv_plain[j];

                /*保存当前的iv_plain*/
                for (j = 0; j < 8; j++)
                    iv_plain[j] = src_buf[j];

                /*更新iv_crypt*/
                src_i = 0;
                Array.Copy(outBuf, outBufPos, iv_crypt, 0, 8);
                outBufLen += 8;
                outBufPos += 8;
            }
        }

        /*src_i指向src_buf下一个位置*/

        while (inBufLen != 0)
        {
            if (src_i < 8)
            {
                src_buf[src_i++] = inBuf[inBufPos++];
                inBufLen--;
            }

            if (src_i == 8)
            {
                /*src_i==8*/

                for (j = 0; j < 8; j++) /*加密前异或前8个byte的密文(iv_crypt指向的)*/
                    src_buf[j] ^= iv_crypt[j];
                /*outBuffer、inBuffer均为8byte, key为16byte*/
                TeaEncryptECB(src_buf, key, outBuf, outBufPos);

                for (j = 0; j < 8; j++) /*加密后异或前8个byte的明文(iv_plain指向的)*/
                    outBuf[outBufPos+j] ^= iv_plain[j];

                /*保存当前的iv_plain*/
                for (j = 0; j < 8; j++)
                    iv_plain[j] = src_buf[j];

                src_i = 0;
                Array.Copy(outBuf, outBufPos, iv_crypt, 0, 8);
                outBufLen += 8;
                outBufPos += 8;
            }
        }

        /*src_i指向src_buf下一个位置*/

        for (i = 1; i <= kZeroLen; )
        {
            if (src_i < 8)
            {
                src_buf[src_i++] = 0;
                i++; /*i inc in here*/
            }

            if (src_i == 8)
            {
                /*src_i==8*/

                for (j = 0; j < 8; j++) /*加密前异或前8个byte的密文(iv_crypt指向的)*/
                    src_buf[j] ^= iv_crypt[j];
                /*pOutBuffer、pInBuffer均为8byte, pKey为16byte*/
                TeaEncryptECB(src_buf, key, outBuf, outBufPos);

                for (j = 0; j < 8; j++) /*加密后异或前8个byte的明文(iv_plain指向的)*/
                    outBuf[outBufPos+j] ^= iv_plain[j];

                /*保存当前的iv_plain*/
                for (j = 0; j < 8; j++)
                    iv_plain[j] = src_buf[j];

                src_i = 0;
                Array.Copy(outBuf, outBufPos, iv_crypt, 0, 8);
                outBufLen += 8;
                outBufPos += 8;
            }
        }
    }


    /*key为16byte*/
    /*
        输入:inBuf为密文格式,inBufLen为inBuf的长度是8byte的倍数; outBufLen为接收缓冲区的长度
            特别注意outBufLen应预置接收缓冲区的长度!
        输出:outBuf为明文(Body),outBufLen为outBuf的长度,至少应预留inBufLen-10;
        返回值:如果格式正确返回TRUE;
    */
    /*TEA解密算法,CBC模式*/
    /*密文格式:PadLen(1byte)+Padding(var,0-7byte)+Salt(2byte)+Body(var byte)+Zero(7byte)*/
    public static int oi_symmetry_decrypt2(byte[] inBuf, int inBufLen, byte[] key, byte[] outBuf, ref int outBufLen)
    {
        int inBufPos = 0, outBufPos = 0;// C# 不支持指针
        int nPadLen, nPlainLen;
        byte[] dest_buf = new byte[8];
        byte[] zero_buf = new byte[8];
        byte[] iv_pre_crypt = new byte[8];
        byte[] iv_cur_crypt = new byte[8];

	    int dest_i, i, j;
	    int nBufPos = 0;

        if ((inBufLen % 8) != 0 || (inBufLen < 16))
            return -1;

        TeaDecryptECB(inBuf, key, dest_buf);
        nPadLen = dest_buf[0] & 0x7/*只要最低三位*/;

        /*密文格式:PadLen(1byte)+Padding(var,0-7byte)+Salt(2byte)+Body(var byte)+Zero(7byte)*/
        i = inBufLen - 1/*PadLen(1byte)*/- nPadLen - kSaltLen - kZeroLen; /*明文长度*/
        if ((outBufLen < i) || (i < 0))
            return -1;
        outBufLen = i;

        for (i = 0; i < 8; i++)
            zero_buf[i] = 0;

        Array.Copy(inBuf, 0, iv_cur_crypt, 0, 8);
        inBufPos += 8;
        nBufPos += 8;

        dest_i = 1; /*dest_i指向dest_buf下一个位置*/

        /*把Padding滤掉*/
        dest_i += nPadLen;
        /*dest_i must <=8*/

        /*把Salt滤掉*/
        for (i = 1; i <= kSaltLen; )
        {
            if (dest_i < 8)
            {
                dest_i++;
                i++;
            }
            else if (dest_i == 8)
            {
                /*解开一个新的加密块*/

                /*改变前一个加密块的指针*/
                Array.Copy(iv_cur_crypt, 0, iv_pre_crypt, 0, 8);
                Array.Copy(inBuf, inBufPos, iv_cur_crypt, 0, 8);

                /*异或前一块明文(在dest_buf[]中)*/
                for (j = 0; j < 8; j++)
                {
                    if ((nBufPos + j) >= inBufLen)
                        return -1;
                    dest_buf[j] ^= inBuf[inBufPos + j];
                }

                /*dest_i==8*/
                TeaDecryptECB(dest_buf, key, dest_buf);

                /*在取出的时候才异或前一块密文(iv_pre_crypt)*/


                inBufPos += 8;
                nBufPos += 8;

                dest_i = 0; /*dest_i指向dest_buf下一个位置*/
            }
        }


        /*还原明文*/

        nPlainLen = outBufLen;
        while (nPlainLen != 0)
        {
            if (dest_i < 8)
            {
                outBuf[outBufPos++] = (byte)(dest_buf[dest_i] ^ iv_pre_crypt[dest_i]);
                dest_i++;
                nPlainLen--;
            }
            else if (dest_i == 8)
            {
                /*dest_i==8*/

                /*改变前一个加密块的指针*/
                Array.Copy(iv_cur_crypt, 0, iv_pre_crypt, 0, 8);
                Array.Copy(inBuf, inBufPos, iv_cur_crypt, 0, 8);

                /*解开一个新的加密块*/

                /*异或前一块明文(在dest_buf[]中)*/
                for (j = 0; j < 8; j++)
                {
                    if ((nBufPos + j) >= inBufLen)
                        return -1;
                    dest_buf[j] ^= inBuf[inBufPos + j];
                }

                TeaDecryptECB(dest_buf, key, dest_buf);

                /*在取出的时候才异或前一块密文(iv_pre_crypt)*/


                inBufPos += 8;
                nBufPos += 8;

                dest_i = 0; /*dest_i指向dest_buf下一个位置*/
            }
        }

        /*校验Zero*/
        for (i = 1; i <= kZeroLen; )
        {
            if (dest_i < 8)
            {
                if ((dest_buf[dest_i] ^ iv_pre_crypt[dest_i]) != 0)
                    return -1;
                dest_i++;
                i++;
            }
            else if (dest_i == 8)
            {
                /*改变前一个加密块的指针*/
                Array.Copy(iv_cur_crypt, 0, iv_pre_crypt, 0, 8);
                Array.Copy(inBuf, inBufPos, iv_cur_crypt, 0, 8);

                /*解开一个新的加密块*/

                /*异或前一块明文(在dest_buf[]中)*/
                for (j = 0; j < 8; j++)
                {
                    if ((nBufPos + j) >= inBufLen)
                        return -1;
                    dest_buf[j] ^= inBuf[inBufPos + j];
                }

                TeaDecryptECB(dest_buf, key, dest_buf);

                /*在取出的时候才异或前一块密文(iv_pre_crypt)*/


                inBufPos += 8;
                nBufPos += 8;
                dest_i = 0; /*dest_i指向dest_buf下一个位置*/
            }

        }

        return 0;
    }

    // 加密
    public static string Encode(string rawData)
    {
        byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(rawData);
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes("msdkmsdkmsdkmsdk");

        int iLength = 0;
        byte[] buf = new byte[dataBytes.Length + 18];       
        MsdkTea.oi_symmetry_encrypt2(dataBytes, dataBytes.Length, keyBytes, buf, ref iLength);
        return Convert.ToBase64String(buf, 0, iLength);
    }

    // 解密
    public static string Decode(byte[] encodedDataBytes)
    {
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes("msdkmsdkmsdkmsdk");
        int iLength = encodedDataBytes.Length;
        byte[] buf = new byte[iLength + 1];
        int decryptRet = MsdkTea.oi_symmetry_decrypt2(encodedDataBytes, 
            encodedDataBytes.Length, keyBytes, buf, ref iLength);
        if (decryptRet != 0)
        {
            return "";
        }
        else
        {
            return System.Text.Encoding.UTF8.GetString(buf, 0, iLength);
        }
    }

    /*public static void Test()
    {
            byte[] data = System.Text.Encoding.UTF8.GetBytes("hello world!");
            byte[] key = System.Text.Encoding.UTF8.GetBytes("msdkmsdkmsdkmsdk");
            byte[] buf = new byte[key.Length + 18];
            int iLength = 0;
            MsdkTea.oi_symmetry_encrypt2(data, data.Length, key, buf, ref iLength);
            buf[iLength] = 0;

            for (int k = 0; k < iLength; k++)
                Console.Write(buf[k].ToString() + " ");
            Console.WriteLine("");
            Console.WriteLine(iLength);

            int iLength2 = iLength;
            byte[] buf2 = new byte[iLength + 1];
            MsdkTea.oi_symmetry_decrypt2(buf, iLength, key, buf2, ref iLength2);
            buf2[iLength2] = 0;

            for (int k = 0; k < iLength2; k++)
                Console.Write(buf2[k].ToString() + " ");
            Console.WriteLine("");
            Console.WriteLine(iLength2);

            Console.Read();
    }*/
}
}
