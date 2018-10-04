using System.Threading.Tasks;
using ChineseJourney.Common.Controller;
using SkiaSharp.Extended.Svg;
using Xamarin.Forms;

namespace ChineseJourney.Common.View
{
    public class HanZiImage: SkiaSvgImage
    {
        public static BindableProperty ZiProperty = BindableProperty.Create(@"Zi", typeof(string), typeof(string), null, propertyChanged: ZiChanged);
        public static BindableProperty IsHighlighRadialEnabledProperty = BindableProperty.Create(@"IsHighlighRadialEnabled", typeof(bool), typeof(bool), null, propertyChanged: IsHighlighRadialEnabledChanged);
        public static BindableProperty IsAnimationEnabledProperty = BindableProperty.Create(@"IsAnimationEnabled", typeof(bool), typeof(bool), null, propertyChanged: IsAnimationEnabledChanged);

        string _zi;
        int _currentStroke = 0;
        int _strokeCount = -1;
        bool _isHighlighRadialEnabled = true;
        bool _isAnimationEnabled;

        private static void ZiChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var c = bindable as HanZiImage;
            if (c != null)
            {
                c.Zi = newValue as string;
            }
        }

        static void IsHighlighRadialEnabledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var c = bindable as HanZiImage;
            if (c != null)
            {
                c.IsHighlighRadialEnabled = (bool)newValue;
            }
        }

        static void IsAnimationEnabledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var c = bindable as HanZiImage;
            if (c != null)
            {
                c.IsAnimationEnabled = (bool)newValue;
            }
        }
        
        public string Zi
        {
            get { return _zi; }
            set
            {
                if (_zi != value && HanziStrokeController.Instance.HanZi.ContainsKey(value))
                {
                    _zi = value;
                    _strokeCount = HanziStrokeController.Instance.StrokeCount(_zi);
                    RefreshDisplay();
                }
            }
        }

        public bool IsHighlighRadialEnabled
        {
            get => _isHighlighRadialEnabled;
            set
            {
                if (_isHighlighRadialEnabled != value)
                {
                    _isHighlighRadialEnabled = value;
                    RefreshDisplay();
                }
            }
        }
        public bool IsAnimationEnabled
        {
            get => _isAnimationEnabled;
            set
            {
                if (_isAnimationEnabled != value)
                {
                    _isAnimationEnabled = value;
                    RefreshDisplay();
                }
            }
        }
        public void RefreshHZImage()
        {
            if (Image != null && (!IsAnimationEnabled || (_strokeCount > 0 && _currentStroke >= _strokeCount)))
            {
                return;
            }

            Image = HanziStrokeController.Instance.GetSvgImage(Zi, IsHighlighRadialEnabled,
                IsAnimationEnabled ? _currentStroke : -1);

            if (IsAnimationEnabled)
            {
                if (_currentStroke <= _strokeCount)
                {
                    _currentStroke++;
                    Task.Run(async () =>
                    {
                        await Task.Delay(500);
                        RefreshHZImage();
                        InvalidateSurface();
                    });
                }
            }
        }
        void RefreshDisplay()
        {
            _currentStroke = 0;
            Image = null;
            RefreshHZImage();
            InvalidateSurface();
        }
    }
}