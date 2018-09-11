using ChineseJourney.Common.Controller;
using Urho;
using Urho.Forms;
using Xamarin.Forms;

namespace ChineseJourney.Common
{
	public partial class QuestionPage : ContentPage
	{
	    private BaobaoGameController _controller;
        public QuestionPage()
		{
		    _controller = BaobaoGameController.Instance;
		    BindingContext = _controller.DataModel;
			InitializeComponent();
        }
    }
}
