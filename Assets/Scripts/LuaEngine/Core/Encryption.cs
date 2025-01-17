using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;


namespace LuaCore
{

    /// <summary>
    ///  Summary description for Encryption
    /// </summary>
    public class Encryption
    {

        /// <summary>
        ///  Default Key
        /// </summary>
        public const string Key = "bmc.1001";



        public static string Encrypt(string pToEncrypt)
        {
            return Encrypt(pToEncrypt, Key);
        }


        /// <summary>
        ///  加密方法
        /// </summary>
        public static string Encrypt(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            Encoding coding = new UTF8Encoding(false);

            //把字符串放到byte数组中
            byte[] inputByteArray = coding.GetBytes(pToEncrypt);

            //建立加密对象的密钥和偏移量
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法
            //使得输入密码必须输入英文文本
            des.Key = coding.GetBytes(sKey);// ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = coding.GetBytes(sKey);// ASCIIEncoding.ASCII.GetBytes(sKey);
            //创建其支持存储区为内存的流
            MemoryStream ms = new MemoryStream();
            //将数据流链接到加密转换的流
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            //Write  the  byte  array  into  the  crypto  stream 
            //(It  will  end  up  in  the  memory  stream) 
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            //用缓冲区的当前状态更新基础数据源或储存库，随后清除缓冲区
            cs.FlushFinalBlock();
            //Get  the  data  back  from  the  memory  stream,  and  into  a  string 
            byte[] EncryptData = (byte[])ms.ToArray();
            return System.Convert.ToBase64String(EncryptData, 0, EncryptData.Length);
        }


        public static string Decrypt(string pToDecrypt)
        {
            return Decrypt(pToDecrypt, Key);
        }


        /// <summary>
        ///  解密方法
        /// </summary>
        public static string Decrypt(string pToDecrypt, string sKey)
        {
            Encoding coding = new UTF8Encoding(false);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //Put  the  input  string  into  the  byte  array 
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);

            //建立加密对象的密钥和偏移量，此值重要，不能修改
            des.Key = coding.GetBytes(sKey);// ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = coding.GetBytes(sKey);// ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            //Flush  the  data  through  the  crypto  stream  into  the  memory  stream 
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return coding.GetString(ms.ToArray());
        }


    }

}