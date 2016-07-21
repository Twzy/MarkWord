using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogsAPI
{
    /// <summary>
    /// API
    /// 来自博客园
    /// </summary>
    public class MetaTools : XmlRpcClientProtocol
    {


        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="appKey">可以不用填写</param>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="publish">publish - Where applicable, this specifies whether the blog should be republished after the post has been deleted.</param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.deletePost")]
        public bool deletePost(string appKey, string postid, string username, string password, bool publish)
        {
            return (bool)this.Invoke("deletePost", new object[] { appKey, postid, username, password, publish });
        }


        /// <summary>  
        /// 获取博客账户信息
        /// </summary>  
        /// <param name="appKey"> This value is ignored. </param>  
        /// <param name="postid"> The ID of the post to update. </param>  
        /// <param name="username"> The name of the user’s space. </param>  
        /// <param name="password"></param> 
        /// <returns> An array of structs that represents each of the user’s blogs. The array will contain a maximum of one struct, since a user can only have a single space with a single blog. </returns>  
        /// TODO:得到用户的博客清单 
        [XmlRpcMethod("blogger.getUsersBlogs")]
        public BlogInfo[] getUsersBlogs(string appKey, string username, string password)
        {
            return (BlogInfo[])this.Invoke("getUsersBlogs", new object[] { appKey, username, password });
        }


        /// <summary>  
        /// 获取文章分类 
        /// </summary>  
        /// <param name="blogid"> This should be the string MyBlog, which indicates that the post is being created in the user’s blog. </param>  
        /// <param name="username"> The name of the user’s space. </param>  
        /// <param name="password"> The user’s secret word. </param>  
        /// <returns> An array of structs that contains one struct for each category. Each category struct will contain a description field that contains the name of the category. </returns>  
        /// TODO:得到博客分类 
        [XmlRpcMethod("metaWeblog.getCategories")]
        public CategoryInfo[] getCategories(string blogid, string username, string password)
        {
            return (CategoryInfo[])this.Invoke("getCategories", new object[] { blogid, username, password });
        }



        /// <summary>  
        /// 获取一个博客文章  
        /// </summary>  
        /// <param name="postid"> The ID of the post to update. </param>  
        /// <param name="username"> The name of the user’s space. </param>  
        /// <param name="password"> The user’s secret word. </param>  
        /// <returns> Always returns true. </returns>  
        /// TODO:获取一个帖子 
        [XmlRpcMethod("metaWeblog.getPost")]
        public Post getPost(string postid, string username, string password)
        {
            return (Post)this.Invoke("getPost", new object[] { postid, username, password });
        }




        /// <summary>
        /// 获取最新的文章
        /// </summary>
        /// <param name="blogid">博客编号</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="numberOfPosts">获取数量</param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        public Post[] getRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            return (Post[])this.Invoke("getRecentPosts", new object[] { blogid, username, password, numberOfPosts });
        }



        /// <summary>
        /// 上传新媒体对象，如图片等
        /// </summary>
        /// <param name="blogid">博客编号</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="numberOfPosts">获取数量</param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.newMediaObject")]
        public UrlData newMediaObject(string blogid, string username, string password, FileData file)
        {
            return (UrlData)this.Invoke("newMediaObject", new object[] { blogid, username, password, file });
        }



        /// <summary>  
        /// 增加新文章
        /// </summary>  
        /// <param name="blogid"> This should be the string MyBlog, which indicates that the post is being created in the user’s blog. </param>  
        /// <param name="username"> The name of the user’s space. </param>  
        /// <param name="password"> The user’s secret word. </param>  
        /// <param name="post"> A struct representing the content to update. </param>  
        /// <param name="publish"> If false, this is a draft post. </param>  
        /// <returns> The postid of the newly-created post. </returns>  
        /// TODO:增加一个最新的帖子 
        [XmlRpcMethod("metaWeblog.newPost")]
        public string newPost(string blogid, string username, string password, Post content, bool publish)
        {
            return (string)this.Invoke("newPost", new object[] { blogid, username, password, content, publish });
        }

        /// <summary>  
        /// 编辑已经存在的文章 
        /// </summary>  
        /// <param name="postid"> The ID of the post to update. </param>  
        /// <param name="username"> The name of the user’s space. </param>  
        /// <param name="password"> The user’s secret word. </param>  
        /// <param name="post"> A struct representing the content to update. </param>  
        /// <param name="publish"> If false, this is a draft post. </param>  
        /// <returns> Always returns true. </returns>  
        /// TODO:更新一个帖子 
        [XmlRpcMethod("metaWeblog.editPost")]
        public bool editPost(
        string postid,
        string username,
        string password,
        Post content,
        bool publish)
        {
            try
            {
                return (bool)this.Invoke("editPost", new object[] { postid, username, password, content, publish });
            }
            catch (CookComputing.XmlRpc.XmlRpcInvalidXmlRpcException ex)
            {
                if (ex.Message == "response contains invalid boolean value [response : boolean]")//csdn返回值异常，在这边简单处理一下
                {
                    return true;
                }
                else
                {
                    throw ex;
                }
            }
        }
    }
}
