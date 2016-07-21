using CookComputing.XmlRpc;

namespace BlogsAPI
{
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct BlogInfo
    {
        public string blogid;
        public string url;
        public string blogName;
    }
}
