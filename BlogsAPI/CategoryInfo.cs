using CookComputing.XmlRpc;

namespace BlogsAPI
{
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct CategoryInfo
    {
        public string description;
        public string htmlUrl;
        public string rssUrl;
        public string title;
        public string categoryid;
    }
}
