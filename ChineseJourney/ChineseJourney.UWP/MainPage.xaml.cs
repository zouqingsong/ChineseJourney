using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ChineseJourney.Common;
using ChineseJourney.Common.Controller;
using ChineseJourney.Common.Helpers;
using SamplyGame.Helpers;
using SamplyGame.Shared;
using Urho;
using Urho.Gui;
using Urho.UWP;
using Application = Urho.Application;

namespace ChineseJourney.UWP
{
    public sealed partial class MainPage
    {
        private Application currentApplication;
        public MainPage()
        {
            this.InitializeComponent();
            BaobaoGameContextFactory.Instance.TextStringHelper = new TextUnicodeFix();
            LoadApplication(new Common.App());
        }

        public class TextUnicodeFix : ITextStringHelper
        {
            [DllImport("mono-urho", CallingConvention = CallingConvention.Cdecl)]
            static extern void Text_SetText(IntPtr handle, byte[] text);

            [DllImport("mono-urho", CallingConvention = CallingConvention.Cdecl)]
            internal static extern void Text3D_SetText(IntPtr handle, byte[] text);
            public void SetTextString(Text text, string str)
            {
                try
                {
                    Text_SetText(text.Handle, Encoding.UTF8.GetBytes(str));
                }
                catch (Exception e)
                {
                    text.Value = str;
                }
            }

            public void SetTextString(Text3D text, string str)
            {
                try
                {
                    Text3D_SetText(text.Handle, Encoding.UTF8.GetBytes(str));
                }
                catch (Exception e)
                {
                    text.Text = str;
                }
            }
        }

    }
}
