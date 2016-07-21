using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogsAPI
{
    [XmlRpcMissingMapping(CookComputing.XmlRpc.MappingAction.Ignore)]
    public struct FileData
    {
        public byte[] bits;
        public string name;
        public string type;
    }
}
