using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ChineseJourney.Common.Controller;
using ChineseJourney.Common.Helpers;
using SkiaSharp.Extended.Svg;
using Xamarin.Auth;
using Xamarin.Forms;
using ZibaobaoLib;
using ZibaobaoLib.Data;

namespace ChineseJourney.Common.Model
{
    public class HanZiModel:ViewModelBase
    {
        SKSvg _hanZiImage;
        string _zi;
        int _currentStroke = 0;
        int _strokeCount = -1;
        bool _isHighlighRadialEnabled = true;
        bool _isAnimationEnabled;

        public SKSvg HanZiImage
        {
            get
            {
                if(_hanZiImage != null && (!IsAnimationEnabled || (_strokeCount > 0 && _currentStroke >= _strokeCount)))
                {
                    return _hanZiImage;
                }
                _hanZiImage = HanziStrokeController.Instance.GetSvgImage(Zi, IsHighlighRadialEnabled, IsAnimationEnabled?_currentStroke:-1);
                if (IsAnimationEnabled)
                {
                    if (_currentStroke <= _strokeCount)
                    {
                        _currentStroke++;
                        Task.Run(async () =>
                        {
                            await Task.Delay(500);
                            OnPropertyChanged(nameof(HanZiImage));
                        });
                    }
                }
                return _hanZiImage;
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                    RefreshDisplay();
                }
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
                    OnPropertyChanged();
                    TextToSpeakCommand.Execute(_zi);
                    RefreshDisplay();
                }
            }
        }

        void RefreshDisplay()
        {
            _currentStroke = 0;
            _hanZiImage = null;
            OnPropertyChanged(nameof(HanZiImage));
        }
    }
}