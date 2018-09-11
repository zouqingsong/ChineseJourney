namespace ZibaobaoLib.Model
{
    public class ChineseHanZi
    {
        public string character { get; set; }
        public string definition { get; set; }
        public string[] pinyin { get; set; }
        public string decomposition { get; set; }
        public string radical { get; set; }
        public string[] strokes { get; set; }
        public int[] radStrokes { get; set; }
    }
}