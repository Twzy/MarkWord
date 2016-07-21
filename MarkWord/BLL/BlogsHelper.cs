using BlogsAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace MarkWord.BLL
{
    public class BlogsHelper
    {
        /// <summary>
        /// 博客传输控制工具
        /// </summary>
        static MetaTools metaTools = new MetaTools();

        /// <summary>
        /// 日志句柄
        /// </summary>
        public static PublishInfoHandler publishHander;

        /// <summary>
        /// 文档上传，包括新增与更新
        /// </summary>
        public static string UploadBlogs(string apiUrl, string BlogId, string userId, string password, string BlogsModel, string postId, string title, string Markdown, bool publish)
        {

            int procIndex = 1;

            SendMsg(5, procIndex, "准备数据中……");
            //转换为html
            string Blogs = string.Format("<!-- edit by MarkWord 墨云软件 -->\r\n{0}", CommonMark.CommonMarkConverter.Convert(Markdown));
            metaTools.Url = apiUrl;


            Post blogsPost = new Post();

            //分类
            List<string> tmpCategories = new List<string>();
            tmpCategories.Add("");//添加空分类，是因为部分博客(如csdn)字段这部分为必填字段不添加会产生异常
            blogsPost.categories = tmpCategories.ToArray();

            //添加时间
            blogsPost.dateCreated = DateTime.Now.ToLocalTime();

            //添加标题
            blogsPost.title = title;


            //指定文章编号
            blogsPost.postid = postId;

            //内容
            blogsPost.description = BlogsModel.Contains("{0}") ?//必须使用{0}占位符
                string.Format(BlogsModel, Blogs) : //根据模板生成数据 主要是为了制定Markdown模板
                BlogsModel + Blogs; //通过前缀方式添加

            //开始查找图片并更新到服务器
            HtmlDocument htmlDoc = new HtmlDocument();
            WebClient webClient = new WebClient();
            htmlDoc.LoadHtml(blogsPost.description);
            var ImgList = htmlDoc.DocumentNode.Descendants("img");

            int procCount = 3 + ImgList.Count();

            SendMsg(procCount, procIndex++, string.Format("数据分析完成，总共需要上传{0}张图片", ImgList.Count()));
            int imgErr = 0;//图片上传错误数量
            foreach (var i in ImgList)
            {
                SendMsg(procCount, procIndex++, "正在上传图片数据……");
                //获取图片文件字符串
                string ImgUrl = i.GetAttributeValue("src", "");
                if (string.IsNullOrEmpty(ImgUrl))
                {
                    imgErr++;
                    continue;
                }
                try
                {
                    var imgeData = webClient.DownloadData(ImgUrl);//下载文件

                    FileData fd = default(FileData);
                    fd.bits = imgeData;//图片数据
                    fd.name = Path.GetExtension(ImgUrl);//文件名
                    fd.type = string.Format("image/{0}", fd.name.Substring(1));

                    UrlData obj = metaTools.newMediaObject(BlogId, userId, password, fd);
                    blogsPost.description = blogsPost.description.Replace(ImgUrl, obj.url);
                }
                catch
                {
                    imgErr++;
                    continue;
                }
            }
            try
            {
                if (string.IsNullOrWhiteSpace(postId))
                {
                    SendMsg(procCount, procIndex++, "开始发布文章……");
                    postId = metaTools.newPost(BlogId, userId, password, blogsPost, publish);
                }
                else
                {
                    SendMsg(procCount, procIndex++, "正在更新文章……");
                    metaTools.editPost(postId, userId, password, blogsPost, publish);
                }
            }
            catch (Exception ex)
            {
                Common.ShowMessage("博客发送失败");
                return postId;
            }

            if (imgErr == 0)
            {
                Common.ShowMessage("博客发送成功");
            }
            else
            {
                Common.ShowMessage(string.Format("博客发送成功了，但是有{0}张图片发送失败", imgErr));
            }
            SendMsg(procCount, procCount, "完成");
            return postId;

        }


        private static void SendMsg(int maxCount, int Count, string msg)
        {
            if (publishHander != null)
            {
                publishHander(maxCount, Count, msg);
            }
        }
    }
}
