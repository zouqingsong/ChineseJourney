using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ChineseJourney.Common.Controller;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using ZibaobaoLib;

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

        public HanZiImage()
        {
            EnableTouchEvents = true;
            Touch += SkiaSvgImage_Touch;
            this.OnAnimationChanged += HanZiImageOnAnimationChanged;
        }

        private void HanZiImageOnAnimationChanged(object sender, AnimationStateArgs e)
        {
            Debug.Print(e.Started?"Animation started": "Animation stopped");
        }

        void SkiaSvgImage_Touch(object sender, SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Entered:
                    break;
                case SKTouchAction.Pressed:
                    IsAnimationEnabled = !IsAnimationEnabled;
                    ZibaobaoLibContext.Instance.Speak(Zi);
                    break;
                case SKTouchAction.Moved:
                    break;
                case SKTouchAction.Released:
                    break;
                case SKTouchAction.Cancelled:
                    break;
                case SKTouchAction.Exited:
                    break;
            }
        }

        static void ZiChanged(BindableObject bindable, object oldValue, object newValue)
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
                    OnAnimationChanged?.Invoke(this, new AnimationStateArgs(_isAnimationEnabled));
                    RefreshDisplay();
                }
            }
        }

        public event EventHandler<AnimationStateArgs> OnAnimationChanged;

        public void RefreshHZImage()
        {
            if (Image != null && !IsAnimationEnabled)
            {
                return;
            }

            if (Image != null && (IsAnimationEnabled && (_strokeCount > 0 && _currentStroke >= _strokeCount)))
            {
                _isAnimationEnabled = false;
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
                    });
                }
            }
        }

        void RefreshDisplay()
        {
            _currentStroke = 0;
            Image = null;
            RefreshHZImage();
        }
    }

    public class AnimationStateArgs : EventArgs
    {
        public AnimationStateArgs(bool started)
        {
            Started = started;
        }

        public bool Started { get; set; }
    }
}