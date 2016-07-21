using CookComputing.XmlRpc;

namespace BlogsAPI
{
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public class Source
    {
        public string name;
        public string url;
    }
}