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
		    App.NavigationPage.ToolbarItems.Add(new ToolbarItem { Text = "Test",Order = ToolbarItemOrder.Primary,Priority = 0});
            BindingContext = BaobaoGameController.Instance.DataModel.BaobaoSpellingModel;
			InitializeComponent();
        }
    }
}
