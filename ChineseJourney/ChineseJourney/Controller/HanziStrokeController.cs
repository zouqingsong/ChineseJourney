using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ChineseJourney.Common.Helpers;
using SkiaSharp.Extended.Svg;
using ZibaobaoLib;
using ZibaobaoLib.Helpers;
using ZibaobaoLib.Model;

namespace ChineseJourney.Common.Controller
{
    public class HanziStrokeController
    {
        static HanziStrokeController _instance;
        public static HanziStrokeController Instance => _instance ?? (_instance = new HanziStrokeController());
        public Dictionary<string, ChineseHanZi> HanZi { get; set;} = new Dictionary<string, ChineseHanZi>();
        public string HanziSvgTemplate { get; set; }
        protected HanziStrokeController()
        {
            HanziSvgTemplate = LoadTextFromResource("hanzi.svg");

            using (var stream = LoadStreamFromResource("all.txt"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string allZi = reader.ReadToEnd();
                        HanZi = NewtonJsonSerializer.ParseJSON<Dictionary<string, ChineseHanZi>>(allZi);
                    }
                }
            }

            using (var stream = LoadStreamFromResource("dictionary.txt"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            var hanzi = NewtonJsonSerializer.ParseJSON<ChineseHanZi>(line);
                            if (HanZi.ContainsKey(hanzi.character))
                            {
                                HanZi[hanzi.character].character = hanzi.character;
                                HanZi[hanzi.character].decomposition = hanzi.decomposition;
                                HanZi[hanzi.character].definition = hanzi.definition;
                                HanZi[hanzi.character].pinyin = hanzi.pinyin;
                                HanZi[hanzi.character].radical = hanzi.radical;
                            }
                        }
                    }
                }
            }
        }

        public int StrokeCount(string source)
        {
            return HanZi.ContainsKey(source)?HanZi[source].strokes.Length:0;
        }
        public SKSvg GetSvgImage(string source, bool highlightRadical=false, int numStroke=-1)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            var zi = HanZi[source];

            string strokes = string.Empty;
            for (var index = 0; index < zi.strokes.Length && (numStroke < 0 || index <= numStroke); index++)
            {
                string color = "black";
                if (highlightRadical && (zi.radStrokes == null || zi.radStrokes.Contains(index)))
                {
                    color = "blue";
                }
                var ziStroke = zi.strokes[index];
                strokes += "<path d=\"" + ziStroke + "\" fill=\"" + color + "\"/>" + System.Environment.NewLine;
            }

            using (var stream = HanziSvgTemplate.Replace("STROKE_PATH", strokes).ToStream())
            {
                var svg = new SKSvg();
                svg.Load(stream);
                return svg;
            }
        }

        public static string LoadTextFromResource(string source)
        {
            using (var stream = LoadStreamFromResource(source))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }

            return string.Empty;
        }

        public static Stream LoadStreamFromResource(string source)
        {
            Assembly assembly = typeof(HanziStrokeController).GetTypeInfo().Assembly;
            if (!source.StartsWith("com."))
            {
                source = "ChineseJourney.Common.Resources." + source;
            }
            return assembly.GetManifestResourceStream(source);
          }
    }
}