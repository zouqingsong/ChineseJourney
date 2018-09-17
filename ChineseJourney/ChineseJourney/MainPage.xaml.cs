using ChineseJourney.Common.Controller;
using Urho;
using Xamarin.Forms;

namespace ChineseJourney.Common
{
	public partial class MainPage : ContentPage
	{
	    UrhoPackage.Core.ShootingGame urhoApp;
        Slider selectedBarSlider, rotationSlider;
        public MainPage()
		{
		    BindingContext = BaobaoGameController.Instance.DataModel;
			InitializeComponent();
        }
	    protected override async void OnAppearing()
	    {
            
	        if (Device.RuntimePlatform != Device.UWP)
	        {
	            urhoApp = await UrhoSurface.Show<UrhoPackage.Core.ShootingGame>(
	                new ApplicationOptions(assetsFolder: "Data")
	                {
	                    Orientation = ApplicationOptions.OrientationType.LandscapeAndPortrait
	                });
	        }

	        /*
            foreach (var bar in urhoApp.Bars)
            {
                bar.Selected += OnBarSelection;
            }*/
        }
    }
}
