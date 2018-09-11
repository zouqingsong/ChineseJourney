using ChineseJourney.Common.UrhoPackage.Core;
using Urho.Gui;

namespace ChineseJourney.Common.Controller
{
    public interface ITextStringHelper
    {
        void SetTextString(Text text, string str);
        void SetTextString(Text3D text, string str);
    }
    public class BaobaoGameContextFactory
    {
        static BaobaoGameContextFactory _instance;
        public static BaobaoGameContextFactory Instance => _instance ?? (_instance = new BaobaoGameContextFactory());
        public ITextStringHelper TextStringHelper { get; set; }
        public ShootingGame Game { get; set; }
    }

    public static class TextUnicodeFix
    {
        public static void SetTextFix(this Text text, string str)
        {
            var helper = BaobaoGameContextFactory.Instance.TextStringHelper;
            if (helper != null)
            {
                helper.SetTextString(text, str);
            }
            else
            {
                text.Value = str;
            }
        }

        public static void SetTextFix(this Text3D text, string str)
        {
            var helper = BaobaoGameContextFactory.Instance.TextStringHelper;
            if (helper != null)
            {
                helper.SetTextString(text, str);
            }
            else
            {
                text.Text = str;
            }
        }
    }
}