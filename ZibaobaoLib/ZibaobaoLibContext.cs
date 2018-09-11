namespace ZibaobaoLib
{
    public class ZibaobaoLibContext
    {
        static ZibaobaoLibContext _instance;
        public static ZibaobaoLibContext Instance => _instance ?? (_instance = new ZibaobaoLibContext());

        public string AppName { get; set; }
        public string Platform { get; set; }
        public IPersistentStorage PersistentStorage { get; set; }
        public bool IsFirstTimeStart { get; set; }
    }
}