using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogsAPI
{
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct Post
    {
        //必填
        public DateTime dateCreated;
        public string description;
        public string title;
        //可选
        public string[] categories;
        public Enclosure enclosure;
        public string link;
        public string permalink;
        public string postid;
        public Source source;
        public string userid;
        public string mt_allow_comments;
        public string mt_allow_pings;
        public string mt_convert_breaks;
        public string mt_text_more;
        public string mt_excerpt;
        public string mt_keywords;
        public string wp_slug;
    }
}
