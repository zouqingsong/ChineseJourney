using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.TextToSpeech.Abstractions;

namespace ZibaobaoLib
{
    public class ZibaobaoLibContext
    {
        static ZibaobaoLibContext _instance;
        public static ZibaobaoLibContext Instance => _instance ?? (_instance = new ZibaobaoLibContext());

        public string AppName { get; set; }
        public string Platform { get; set; }
        public ITextToSpeech TextToSpeech { get; set; }
        public IPersistentStorage PersistentStorage { get; set; }
        public bool IsFirstTimeStart { get; set; }

        public async void Speak(string text, CrossLocale? crossLocale = null, float? pitch = null, float? speakRate = null, float? volume = null, CancellationToken cancelToken = default(CancellationToken))
        {
            if(TextToSpeech != null)
            {
                if(crossLocale == null)
                {
                    var localeList  = (await TextToSpeech.GetInstalledLanguages())?.ToArray();
                    if(localeList != null)
                    {
                        crossLocale = localeList.FirstOrDefault(o => o.DisplayName.Contains("zh-CN"));
                    }
                }
                TextToSpeech.Speak(text, crossLocale, pitch, speakRate, volume, cancelToken).Forget();
            }
        }

    }

    static class TaskExtension
    {
        public static void Forget(this Task task)
        {
        }
    }
}