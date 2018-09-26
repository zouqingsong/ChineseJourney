using ChineseJourney.Common.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace ChineseJourney.Common
{
	public partial class App : Application
	{
	    public static NavigationPage NavigationPage { get; set; }
	    private static RootPage RootPage;
	    public static bool MenuIsPresented
	    {
	        get => RootPage.IsPresented;
	        set => RootPage.IsPresented = value;
	    }

        private BaobaoGameController _controller;
        public App ()
		{
			InitializeComponent();
		    _controller = BaobaoGameController.Instance;
            //MainPage = new QuestionPage();
            //MainPage = new HanziPage();
            //MainPage = new CreateQuestionPage();
            //MainPage = new ChineseJourneyTaskPage();
		    RootPage = new RootPage();
            //var menuPage = new MenuPage();
		    //NavigationPage = new NavigationPage(new HomePage());
		    //RootPage.Master = menuPage;
		    //ootPage.Detail = NavigationPage;
		    MainPage = RootPage;
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
