using System.Collections.Generic;

namespace ZibaobaoLib.Data
{
    public class ExamPaper
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public BaobaoVersion Version { get; set; }
        public List<QuestionItem> Questions { get; } = new List<QuestionItem>();
    }
}