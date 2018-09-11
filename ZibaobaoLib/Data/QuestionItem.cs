using System.Collections.Generic;

namespace ZibaobaoLib.Data
{
    public class QuestionItem:ViewModelBase
    {
        private string _question;

        public string Question
        {
            get => _question;
            set
            {
                if (_question != value)
                {
                    _question = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsMultipleChoice { get; set; }
        public List<AnswerItem> Answers { get; set; } = new List<AnswerItem>();
    }
}