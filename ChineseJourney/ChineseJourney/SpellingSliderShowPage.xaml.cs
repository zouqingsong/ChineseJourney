using ChineseJourney.Common.Controller;
using Urho;
using Urho.Forms;
using Xamarin.Forms;

namespace ChineseJourney.Common
{
	public partial class SpellingSliderShowPage : ContentPage
	{
        public SpellingSliderShowPage()
		{
		    BindingContext = BaobaoGameController.Instance.DataModel;
			InitializeComponent();
        }
    }
}
