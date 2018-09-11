using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SamplyGame.Shared;
using ZibaobaoLib.Helpers;
using ZibaobaoLib.Model;

namespace ChineseJourneyCreator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                X1LogHelper.HandlerGlobalException(e.ExceptionObject as Exception, s);
            };

            PlatformContext.Init("Desktop");
        }
    }
}
