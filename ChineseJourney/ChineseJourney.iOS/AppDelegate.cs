using System;
using System.Collections.Generic;
using System.Linq;
using ChineseJourney.Common;
using ChineseJourney.Common.Helpers;
using FFImageLoading;
using FFImageLoading.Svg.Forms;
using Foundation;
using Plugin.DownloadManager;
using SamplyGame.Shared;
using UIKit;

namespace ChineseJourney.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            PlatformContext.Init("iOS");

            Rg.Plugins.Popup.Popup.Init();
            Xamarin.Auth.Presenters.XamarinIOS.AuthenticationConfiguration.Init();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            var ignore = typeof(SvgCachedImage);
            global::Xamarin.Forms.Forms.Init();
            AiForms.Layouts.LayoutsInit.Init();  //need to write here
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, Action completionHandler)
        {
            CrossDownloadManager.BackgroundSessionCompletionHandler = completionHandler;
        }

        public override void ReceiveMemoryWarning(UIApplication application)
        {
            ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        public override bool OpenUrl(
           UIApplication application,
           NSUrl url,
           string sourceApplication,
           NSObject annotation)
        {
            // Convert iOS NSUrl to C#/netxf/BCL System.Uri
            var uri_netfx = new Uri(url.AbsoluteString);
            AuthenticationState.Authenticator.OnPageLoading(uri_netfx);
            return true;
        }
    }
}
