using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChineseJourney.Common.Controller;
using ChineseJourney.Common.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChineseJourney.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPageMaster : ContentPage
    {
        public ListView ListView;

        public MasterPageMaster()
        {
            InitializeComponent();

            BindingContext = BaobaoGameController.Instance.DataModel;
            ListView = MenuItemsListView;
        }
    }
}