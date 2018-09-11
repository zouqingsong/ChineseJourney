using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Forms;
using FFImageLoading.Work;
using SkiaSharp.Extended.Svg;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ImageSource = Xamarin.Forms.ImageSource;

namespace ChineseJourney.Common.Helpers
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {

        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }
            // Do your translation lookup here, using whatever method you require
            var imageSource = FromResource(Source);
            return imageSource;
        }

        public static SKSvg GetSvgImage(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }
            var assembly = typeof(ImageResourceExtension).GetTypeInfo().Assembly;
            if (!source.StartsWith("com."))
            {
                source = "ChineseJourney.Common.Resources.svgs." + source;
            }
            if (!source.EndsWith(".svg"))
            {
                source += ".svg";
            }

            using (var stream = assembly.GetManifestResourceStream(source))
            {
                var svg = new SKSvg();
                if (stream != null)
                {
                    svg.Load(stream);
                }
                return svg;
            }
        }

        public static ImageSource FromResource(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }
            Assembly assembly = typeof(ImageResourceExtension).GetTypeInfo().Assembly;
            if (!source.StartsWith("com."))
            {
                source = "ChineseJourney.Common.Resources." + source;
            }
            if (!source.EndsWith(".png"))
            {
                source += ".png";
            }
            return ImageSource.FromResource(source, assembly);
        }

        public static CachedImage GetImage(string uri, int days = 2)
        {
            return new CachedImage
            {
                Source = uri,
                CacheDuration = TimeSpan.FromDays(days),
                CacheType = CacheType.All
            };
        }

        public static Task DownloadImage(string uri, int days = 2)
        {
            return ImageService.Instance.LoadUrl(uri, TimeSpan.FromDays(days)).WithPriority(LoadingPriority.Highest).WithCache(CacheType.All).PreloadAsync();
        }
    }
}
