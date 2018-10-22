using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChineseJourney.Common.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChineseJourney.Common
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreateSpellingPage : ContentPage
	{
		public CreateSpellingPage()
		{
			InitializeComponent ();
		    BindingContext = BaobaoGameController.Instance.DataModel.BaobaoSpellingModel;
		}
	}
}