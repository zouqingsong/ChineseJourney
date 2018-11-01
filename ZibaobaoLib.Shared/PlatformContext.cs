using System.Linq;
using Plugin.TextToSpeech;
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
            ZibaobaoLibContext.Instance.TextToSpeech = CrossTextToSpeech.Current;
            X1LogHelper.X1ServiceEventLog = new X1LocalLogger();
        }
    }
}
