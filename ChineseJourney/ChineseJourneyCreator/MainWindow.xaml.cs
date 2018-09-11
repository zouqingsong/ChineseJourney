using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZibaobaoLib.Model;

namespace ChineseJourneyCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string BookListName = "BaobaoBookList.txt";
        private BaobaoModel Model;

        public MainWindow()
        {
            InitializeComponent();
            Model = new BaobaoModel(BookListName);
            DataContext = Model;
        }
    }
}
