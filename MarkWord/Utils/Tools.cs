using Pechkin;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace MarkWord
{
    public class Tools
    {

        #region 判定文件编码类型

        /// <summary> 
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型 
        /// </summary> 
        /// <param name=“FILE_NAME“>文件路径</param> 
        /// <returns>文件的编码类型</returns> 
        public static System.Text.Encoding GetFileType(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            Encoding r = GetFileType(fs);
            fs.Close();
            return r;
        }

        /// <summary> 
        /// 通过给定的文件流，判断文件的编码类型 
        /// </summary> 
        /// <param name=“fs“>文件流</param> 
        /// <returns>文件的编码类型</returns> 
        public static System.Text.Encoding GetFileType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;

        }

        /// <summary> 
        /// 判断是否是不带 BOM 的 UTF8 格式 
        /// </summary> 
        /// <param name=“data“></param> 
        /// <returns></returns> 
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
            byte curByte; //当前分析的字节. 
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前 
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1 
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
        #endregion


//html to Pdf
public static void HtmlToPdf(string filePath, string html, bool isOrientation = false)
{
    if (string.IsNullOrEmpty(html))
        html = "Null";
    // 创建全局信息
    GlobalConfig gc = new GlobalConfig();
    gc.SetMargins(new Margins(50, 50, 60, 60))
        .SetDocumentTitle("MarkWord")
        .SetPaperSize(PaperKind.A4)
        .SetPaperOrientation(isOrientation)
        .SetOutlineGeneration(true);

           
    //页面信息
    ObjectConfig oc = new ObjectConfig();
    oc.SetCreateExternalLinks(false)
        .SetFallbackEncoding(Encoding.UTF8)
        .SetLoadImages(true)
        .SetScreenMediaType(true)
        .SetPrintBackground(true);
    //.SetZoomFactor(1.5);

    var pechkin = new SimplePechkin(gc);
    pechkin.Finished += Pechkin_Finished;
    pechkin.Error += Pechkin_Error;
    pechkin.ProgressChanged += Pechkin_ProgressChanged;
    var buf = pechkin.Convert(oc, html);

    if (buf == null)
    {
        Common.ShowMessage("导出异常");
        return;
    }

    try
    {
        string fn = filePath; //Path.GetTempFileName() + ".pdf";
        FileStream fs = new FileStream(fn, FileMode.Create);
        fs.Write(buf, 0, buf.Length);
        fs.Close();

        //Process myProcess = new Process();
        //myProcess.StartInfo.FileName = fn;
        //myProcess.Start();
    }
    catch { }
}

        private static void Pechkin_ProgressChanged(SimplePechkin converter, int progress, string progressDescription)
        {
            Common.ShowStatusMessage(progressDescription);
        }

        private static void Pechkin_Error(SimplePechkin converter, string errorText)
        {
            Common.ShowStatusMessage("导出失败：" + errorText);
        }

        private static void Pechkin_Finished(SimplePechkin converter, bool success)
        {
            if (success)
            { Common.ShowStatusMessage("导出PDF成功"); }
            else
            {
                Common.ShowStatusMessage("导出失败");
            }
        }



        #region 序列化

        #region 使用XML作为存储

        public static void WriteByXML<T>(T p, string path)
        {
            Stream stream = null;
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));

                stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
                xs.Serialize(stream, p);
                stream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
        public static T ReadByXML<T>(string path)
        {
            Stream stream = null;
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                XmlTextReader xmlread = new XmlTextReader(stream);
                xmlread.Normalization = false;
                T p = (T)xs.Deserialize(xmlread);
                stream.Close();
                return p;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        #endregion

        #region 通过二进制方式存储对象

        public static void WriteByBinary<T>(T p, string path)
        {
            try
            {
                FileStream fileStream = new FileStream(path, FileMode.Create);
                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(fileStream, p);
                fileStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static T ReadByBinary<T>(string path)
        {
            try
            {
                T p;
                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryFormatter b = new BinaryFormatter();
                p = (T)b.Deserialize(fileStream);
                fileStream.Close();
                return p;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion 

        #endregion


        #region 字符串加解密
        private static string DES_KEY = "MarkWord";
        private static string DES_IV = "TWZYFTWA";

        public static string Encrypt(string src)
        {
            if (string.IsNullOrEmpty(src))
                return "";
            string key = DES_KEY;
            string iv = DES_IV;
            byte[] btKey = Encoding.UTF8.GetBytes(key);
            byte[] btIV = Encoding.UTF8.GetBytes(iv);
            string result = "";
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Encoding.UTF8.GetBytes(src);
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                {
                    cs.Write(inData, 0, inData.Length);
                    cs.FlushFinalBlock();
                }
                result = Convert.ToBase64String(ms.ToArray());
            }
            return result;
        }
        public static string Decrypt(string src)
        {
            if (string.IsNullOrEmpty(src))
                return "";
            string key = DES_KEY;
            string iv = DES_IV;
            byte[] btKey = Encoding.UTF8.GetBytes(key);
            byte[] btIV = Encoding.UTF8.GetBytes(iv);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            string result = "";
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Convert.FromBase64String(src);
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                {
                    cs.Write(inData, 0, inData.Length);
                    cs.FlushFinalBlock();
                }
                result = Encoding.UTF8.GetString(ms.ToArray());
            }
            return result;
        }
        #endregion
    }
}
