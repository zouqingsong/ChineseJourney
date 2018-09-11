using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace ChineseJourney.Common.Helpers
{
    public class DrawingHelper
    {
        public static void Draw(SKPaintSurfaceEventArgs args, SKBitmap webBitmap, bool forceToLeft, out SKRect rect)
        {
			SKImageInfo info = args.Info;
			SKSurface surface = args.Surface;
			SKCanvas canvas = surface.Canvas;
			canvas.Clear();
            rect = new SKRect();

            if (webBitmap != null)
			{
				float x = 0, y = 0, width = info.Width, height = info.Height;
				float ratio1 = (webBitmap.Width + 0.0f) / webBitmap.Height;
				float ratio2 = (info.Width + 0.0f) / info.Height;
				if (ratio1 >= ratio2)
				{
                    height = info.Width / ratio1;
				    if (!forceToLeft)
				    {
				        y = (info.Height - height) / 2.0f;
				    }
				}
				else
				{
                    width = info.Height * ratio1;
				    if (!forceToLeft)
				    {
				        x = (info.Width - width) / 2.0f;
				    }
				}

			    rect = new SKRect(x, y, x + width, y + height);
                canvas.DrawBitmap(webBitmap, rect);
			}
        }

		public static SKColor[] SkColorList = {
			SKColors.AliceBlue,
			SKColors.PaleGreen,
			SKColors.PaleGoldenrod,
			SKColors.Orchid,
			SKColors.OrangeRed,
			SKColors.Orange,
			SKColors.OliveDrab,
			SKColors.Olive,
			SKColors.OldLace,
			SKColors.Navy,
			SKColors.NavajoWhite,
			SKColors.Moccasin,
			SKColors.MistyRose,
			SKColors.MintCream,
			SKColors.MidnightBlue,
			SKColors.MediumVioletRed,
			SKColors.MediumTurquoise,
			SKColors.MediumSpringGreen,
			SKColors.LightSlateGray,
			SKColors.LightSteelBlue,
			SKColors.LightYellow,
			SKColors.Lime,
			SKColors.LimeGreen,
			SKColors.Linen,
			SKColors.PaleTurquoise,
			SKColors.Magenta,
			SKColors.MediumAquamarine,
			SKColors.MediumBlue,
			SKColors.MediumOrchid,
			SKColors.MediumPurple,
			SKColors.MediumSeaGreen,
			SKColors.MediumSlateBlue,
			SKColors.Maroon,
			SKColors.PaleVioletRed,
			SKColors.PapayaWhip,
			SKColors.PeachPuff,
			SKColors.Snow,
			SKColors.SpringGreen,
			SKColors.SteelBlue,
			SKColors.Tan,
			SKColors.Teal,
			SKColors.Thistle,
			SKColors.SlateGray,
			SKColors.Tomato,
			SKColors.Turquoise,
			SKColors.Violet,
			SKColors.Wheat,
			SKColors.White,
			SKColors.WhiteSmoke,
			SKColors.Yellow,
			SKColors.Transparent,
			SKColors.LightSkyBlue,
			SKColors.SlateBlue,
			SKColors.Silver,
			SKColors.Peru,
			SKColors.Pink,
			SKColors.Plum,
			SKColors.PowderBlue,
			SKColors.Purple,
			SKColors.Red,
			SKColors.SkyBlue,
			SKColors.RosyBrown,
			SKColors.SaddleBrown,
			SKColors.Salmon,
			SKColors.SandyBrown,
			SKColors.SeaGreen,
			SKColors.SeaShell,
			SKColors.Sienna,
			SKColors.RoyalBlue,
			SKColors.LightSeaGreen,
			SKColors.LightSalmon,
			SKColors.LightPink,
			SKColors.Crimson,
			SKColors.Cyan,
			SKColors.DarkBlue,
			SKColors.DarkCyan,
			SKColors.DarkGoldenrod,
			SKColors.DarkGray,
			SKColors.Cornsilk,
			SKColors.DarkGreen,
			SKColors.DarkMagenta,
			SKColors.DarkOliveGreen,
			SKColors.DarkOrange,
			SKColors.DarkOrchid,
			SKColors.DarkRed,
			SKColors.DarkSalmon,
			SKColors.DarkKhaki,
			SKColors.DarkSeaGreen,
			SKColors.CornflowerBlue,
			SKColors.Chocolate,
			SKColors.AntiqueWhite,
			SKColors.Aqua,
			SKColors.Aquamarine,
			SKColors.Azure,
			SKColors.Beige,
			SKColors.Bisque,
			SKColors.Coral,
			SKColors.Black,
			SKColors.Blue,
			SKColors.BlueViolet,
			SKColors.Brown,
			SKColors.BurlyWood,
			SKColors.CadetBlue,
			SKColors.Chartreuse,
			SKColors.BlanchedAlmond,
			SKColors.YellowGreen,
			SKColors.DarkSlateBlue,
			SKColors.DarkTurquoise,
			SKColors.IndianRed,
			SKColors.Indigo,
			SKColors.Ivory,
			SKColors.Khaki,
			SKColors.Lavender,
			SKColors.LavenderBlush,
			SKColors.HotPink,
			SKColors.LawnGreen,
			SKColors.LightBlue,
			SKColors.LightCoral,
			SKColors.LightCyan,
			SKColors.LightGoldenrodYellow,
			SKColors.LightGray,
			SKColors.LightGreen,
			SKColors.LemonChiffon,
			SKColors.DarkSlateGray,
			SKColors.Honeydew,
			SKColors.Green,
			SKColors.DarkViolet,
			SKColors.DeepPink,
			SKColors.DeepSkyBlue,
			SKColors.DimGray,
			SKColors.DodgerBlue,
			SKColors.Firebrick,
			SKColors.GreenYellow,
			SKColors.FloralWhite,
			SKColors.Fuchsia,
			SKColors.Gainsboro,
			SKColors.GhostWhite,
			SKColors.Gold,
			SKColors.Goldenrod,
			SKColors.Gray,
			SKColors.ForestGreen
        };
	}
}
