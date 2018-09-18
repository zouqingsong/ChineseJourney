using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChineseJourney.Common.Model
{
    public class MasterViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MasterMenuItem> MenuItems { get; set; }
        public MasterViewModel()
        {
            MenuItems = new ObservableCollection<MasterMenuItem>(new[]
            {
                new MasterMenuItem { Id = 0, Title = "Spelling", TargetType = typeof(HanziPage)},
                new MasterMenuItem { Id = 1, Title = "Exam", TargetType = typeof(QuestionPage)},
                new MasterMenuItem { Id = 2, Title = "Page 3" },
                new MasterMenuItem { Id = 3, Title = "Page 4" },
                new MasterMenuItem { Id = 4, Title = "Settings" },
            });
        }
            
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}