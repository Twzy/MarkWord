using CookComputing.XmlRpc;

namespace BlogsAPI
{
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct UrlData
    {
        public string url;
    }
}
