using Newtonsoft.Json;

namespace ZibaobaoLib.Data
{
    public class BaobaoBook
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public BaobaoVersion Version { get; set; }

        [JsonIgnore]
        public bool IsDirty { get; set; }
    }
}