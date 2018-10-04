using ZibaobaoLib.Data;

namespace ChineseJourney.Common.Model
{
    public class HanZiModel:ViewModelBase
    {
        string _zi;
        bool _isHighlighRadialEnabled = true;
        bool _isAnimationEnabled;
 
        public bool IsHighlighRadialEnabled
        {
            get => _isHighlighRadialEnabled;
            set
            {
                if (_isHighlighRadialEnabled != value)
                {
                    _isHighlighRadialEnabled = value;
                    OnPropertyChanged();
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
                }
            }
        }
        public string Zi
        {
            get { return _zi; }
            set
            {
                if (_zi != value)
                {
                    _zi = value;
                    OnPropertyChanged();
                    TextToSpeakCommand.Execute(_zi);
                }
            }
        }
    }
}