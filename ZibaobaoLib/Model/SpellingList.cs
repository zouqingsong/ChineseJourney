using System.Collections.Generic;

namespace ZibaobaoLib.Model
{
    public class SpellingList
    {
        public string Title { get; set; }
        public List<string> Words { get; set; }=new List<string>();
    }

    public static class SpellingListHelper
    {
        public static List<SpellingList> GenerateDemo()
        {
            var sp1 = new SpellingList {Title = "P5 L1", Words = {"天天", "怒气冲冲", "辗转反侧", "勃然大怒"}};
            var sp2 = new SpellingList {Title = "P5 L2", Words = {"怒发冲冠", "脸色铁青", "急得像热锅上的蚂蚁"}};
            return new List<SpellingList> {sp1, sp2};
        }
    }
}