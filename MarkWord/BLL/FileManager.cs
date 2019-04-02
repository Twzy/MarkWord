using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

using System.IO;

namespace MarkWord.BLL
{
    public class FileManager
    {
        /// <summary>
        /// 打开Markdown文件
        /// </summary>
        /// <returns></returns>
        public static string OpenMarkdownFile()
        {
            OpenFileDialog opFileDialog = new OpenFileDialog();
            opFileDialog.Filter = "Markdown文件|*.md;*.markdown;*.txt|Markdown文件(*.md)|*.md|文本文件(*.txt)|*.txt|其他文件|*.*";
            opFileDialog.FilterIndex = 0;
            try
            {
                if (opFileDialog.ShowDialog() == true)
                {
                    SavePath = opFileDialog.FileName;
                    if (Config.Common.FileList.Contains(opFileDialog.FileName))
                    {
                        Config.Common.FileList.Remove(opFileDialog.FileName);
                        Config.Common.FileList.Insert(0, opFileDialog.FileName);
                    }
                    else
                    {
                        Config.Common.FileList.Insert(0, opFileDialog.FileName);
                    }
                    return File.ReadAllText(opFileDialog.FileName, Tools.GetFileType(opFileDialog.FileName));
                }
            }
            catch (Exception ex)
            {
                Common.ShowMessage(ex.Message);
                return null;
            }
            return null;
        }


        public static string SavePath { get; set; }
        public static bool isChangeFlag { get; set; }//是否更变
        public static bool SaveMarkdownFile(string content)
        {
            SaveFileDialog spFileDialog = new SaveFileDialog();
            spFileDialog.Filter = "Markdown文件(*.md)|*.md|文本文件(*.txt)|*.txt|其他文件|*.*";
            spFileDialog.FilterIndex = 0;
            try
            {
                if (spFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(spFileDialog.FileName, content, Encoding.UTF8);
                    SavePath = spFileDialog.FileName;

                    if (Config.Common.FileList.Contains(spFileDialog.FileName))
                    {
                        Config.Common.FileList.Remove(spFileDialog.FileName);
                        Config.Common.FileList.Insert(0, spFileDialog.FileName);
                    }
                    else
                    {
                        Config.Common.FileList.Insert(0, spFileDialog.FileName);
                    }

                    isChangeFlag = false;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Common.ShowMessage(ex.Message);
                return false;
            }
            return false;
        }

        public static bool SaveMarkdownFileWithoutDialg(string content)
        {
            try
            {
                File.WriteAllText(SavePath, content, Encoding.UTF8);
                isChangeFlag = false;
                return true;
            }
            catch (Exception ex)
            {
                Common.ShowMessage(ex.Message);
                return false;
            }
            return false;
        }



        public static bool OutputPdf(string htmlDoc)
        {
            if (!string.IsNullOrEmpty(htmlDoc))
            {

                SaveFileDialog sp = new SaveFileDialog();
                sp.Filter = "PDF文件|*.pdf";
                var result = sp.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    Tools.HtmlToPdf(sp.FileName, htmlDoc, false);
                    return true;
                }
                return false;
            }
            else
            {
                throw new ArgumentException("无效的参数输入");
            }
            return false;
        }

        public static bool OutputHTLM(string htmlDoc)
        {
            if (!string.IsNullOrEmpty(htmlDoc))
            {

                SaveFileDialog sp = new SaveFileDialog();
                sp.Filter = "HTML文件|*.html";
                var result = sp.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    File.WriteAllText(sp.FileName, htmlDoc, Encoding.UTF8);
                    Common.ShowStatusMessage("导出HTML " + sp.FileName + " 成功");
                    return true;
                }
                return false;
            }
            else
            {
                throw new ArgumentException("无效的参数输入");
            }
            return false;
        }



        public static void GetStyleList()
        {
            Config.StyleList = new Dictionary<string, string>();
            var files = Directory.GetFiles(Config.StyleDir);
            foreach (var f in files)
            {
                Config.StyleList.Add(Path.GetFileNameWithoutExtension(f), f);
            }
        }

        public static void LoadStyle(string styleName)
        {
            var file = Path.Combine(Config.StyleDir, styleName + ".css");
            if (!File.Exists(file))
            {
                Config.Common.StyleName = "DefaultStyle";
                Config.Style = Properties.Resources.DefaultStyle;
                return;
            }
            Config.Common.StyleName = styleName;
            Config.Style = File.ReadAllText(file, Encoding.UTF8);
        }


        public static void CheckMyDocFolder()
        {
            if (!Directory.Exists(Config.MyDocumentFolder))
            {
                Directory.CreateDirectory(Config.MyDocumentFolder);
            }
        }

        public static string GetImgFilePath()
        {
            OpenFileDialog opFileDialog = new OpenFileDialog();
            opFileDialog.Filter = "图片文件|*.jpg;*.bmp;*.png;*.gif";
            try
            {
                if (opFileDialog.ShowDialog() == true)
                {
                    return opFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                Common.ShowMessage(ex.Message);
                return null;
            }
            return null;
        }
    }
}
