using System;
using ChineseJourney.Common.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChineseJourney.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootPage
    {
        public RootPage()
        {
            InitializeComponent();
            App.NavigationPage = NavigationPage;
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterMenuItem;
            if (item == null)
            {
                return;
            }

            App.NavigationPage.ToolbarItems.Clear();
            if (item.TargetType == null)
            {
                App.NavigationPage.Navigation.PopToRootAsync();
                App.MenuIsPresented = false;
            }
            else
            {
                var page = (Page)Activator.CreateInstance(item.TargetType);
                page.Title = Title;
                App.NavigationPage.Navigation.PushAsync(page);
            }

            App.MenuIsPresented = false;
            MasterPage.ListView.SelectedItem = null;
        }
    }
}