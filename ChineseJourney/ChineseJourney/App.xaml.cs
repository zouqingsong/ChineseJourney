using ChineseJourney.Common.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace ChineseJourney.Common
{
	public partial class App : Application
	{
	    private BaobaoGameController _controller;
        public App ()
		{
			InitializeComponent();
		    _controller = BaobaoGameController.Instance;
            //MainPage = new QuestionPage();
            //MainPage = new HanziPage();
            //MainPage = new CreateQuestionPage();
            //MainPage = new ChineseJourneyTaskPage();
		    MainPage = new MasterDetailPage();
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
