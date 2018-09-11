using Newtonsoft.Json;

namespace ZibaobaoLib.Data
{
    public class AnswerItem: ViewModelBase
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsAnswer { get; set; }

        [JsonIgnore]
        public bool IsSelected { get; set; }
    }
}