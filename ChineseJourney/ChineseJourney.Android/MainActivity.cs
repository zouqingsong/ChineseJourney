﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ChineseJourney.Common;
using FFImageLoading;
using FFImageLoading.Svg.Forms;
using SamplyGame.Shared;
using ZibaobaoLib;

namespace ChineseJourney.Droid
{
    [Activity(Label = "ChineseJourney", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            PlatformContext.Init("Android");

            ZibaobaoLibContext.Instance.PersistentStorage.DownloadPath = 
                ApplicationContext.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads).AbsolutePath;

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            var ignore = typeof(SvgCachedImage);

            Rg.Plugins.Popup.Popup.Init(this, bundle);

            //((DownloadManagerImplementation)CrossDownloadManager.Current).IsVisibleInDownloadsUi = false;
            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
            base.OnBackPressed();
        }

        public override void OnLowMemory()
        {
            ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }
    }
}

