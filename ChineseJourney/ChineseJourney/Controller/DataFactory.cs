using System.Collections.Generic;
using System.IO;
using ZibaobaoLib;
using ZibaobaoLib.Data;
using ZibaobaoLib.Helpers;

namespace ChineseJourney.Common.Controller
{
    public class DataFactory
    {
        public static DataFactory _instance;

        public static DataFactory Instance => _instance ?? (_instance = new DataFactory());
        protected DataFactory()
        {
        }

        public void InitData()
        {
            var questionList = GeneratePaper();

            SaveQuestion(questionList, "test1.que");
        }

        public static ExamPaper GeneratePaper()
        {
            return new ExamPaper
            {
                Description = "作文《如何面对挫折》上的相关语料",
                Questions =
                {
                    new QuestionItem
                    {
                        IsMultipleChoice = true,
                        Question = "Q1 以下哪些要点可以作为如何面对挫折的Sub-point(分论点)？",
                        Answers =
                        {
                            new AnswerItem {Value = "1.挫折是不可避免的。", IsAnswer = true},
                            new AnswerItem {Value = "2.调整自己的情绪。", IsAnswer = true},
                            new AnswerItem {Value = "3.听音乐、睡觉。"},
                            new AnswerItem {Value = "4.乐观面对。"},
                            new AnswerItem {Value = "5.换一种方法来解决遇到的困难或问题。"},
                            new AnswerItem {Value = "6.不要放弃，应该坚持到底。"},
                            new AnswerItem {Value = "7.勇敢地去寻求帮助。"},
                            new AnswerItem {Value = "8.勇敢、坚强地面对问题，而不是逃避问题。"},
                            new AnswerItem {Value = "9.可以和他人分享。"},
                        }
                    },
                    new QuestionItem
                    {
                        IsMultipleChoice = false,
                        Question = "Q2 人生的道路不一定平坦，我们在生活上都会遇到种种困难，这就是所谓的挫折。挫折在人的一生当中是无法避免的。我们不要抱怨自己为什么总是遇到挫折了，总是遇到不如意的事情。其实每个人都会碰到困境，只是大小而已。那遇到了就要以开阔的心胸来迎接挫折，全力以赴。也就是说，做任何事情都得付出代价，而遇到挫折和失败是所付出代价的一部分。这些并不可怕，关键是你怎么去面对他。也不能因为失败就丧失向上的意志力。\n\n这一段作为文章开头存在什么问题？",
                        Answers =
                        {
                            new AnswerItem {Value = "1.开头太长。", IsAnswer = true},
                            new AnswerItem {Value = "2.和主题无关。"},
                            new AnswerItem {Value = "3.缺少例子。"},
                        }
                    },
                }
            };
        }

        public static void SaveQuestion(ExamPaper examPaper, string name)
        {
            FileSettingsHelper<ExamPaper>.SaveSetting(examPaper, Path.Combine(ZibaobaoLibContext.Instance.PersistentStorage.DataPath, name));
        }

        public static ExamPaper LoadQuestion(string name)
        {
            return FileSettingsHelper<ExamPaper>.LoadSetting(Path.Combine(ZibaobaoLibContext.Instance.PersistentStorage.DataPath, name));
        }

        public static List<string> ListOfQuestionPapers =>
            ZibaobaoLibContext.Instance.PersistentStorage.GetAllFiles(
                ZibaobaoLibContext.Instance.PersistentStorage.DataPath, "*.que");
    }
}