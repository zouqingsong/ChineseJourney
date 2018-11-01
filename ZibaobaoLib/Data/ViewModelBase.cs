using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Newtonsoft.Json;
using Plugin.TextToSpeech;
using SQLite;
using ZibaobaoLib.Annotations;
using ZibaobaoLib.Command;

namespace ZibaobaoLib.Data
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        [JsonIgnore]
        public ICommand TextToSpeakCommand => new Command<string>(text =>
        {
            if (!string.IsNullOrEmpty(text))
            {
                ZibaobaoLibContext.Instance.Speak(text);
            }
        });

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}