using SamplyGame.Helpers;
using ZibaobaoLib;
using ZibaobaoLib.Helpers;

namespace SamplyGame.Shared
{
    public static class PlatformContext
    {
        public static void Init(string platform)
        {
            ZibaobaoLibContext.Instance.AppName = "ZiBaobaoAdventure";
            ZibaobaoLibContext.Instance.Platform = platform;
            ZibaobaoLibContext.Instance.PersistentStorage = new DiskStorage();
            X1LogHelper.X1ServiceEventLog = new X1LocalLogger();
        }
    }
}
