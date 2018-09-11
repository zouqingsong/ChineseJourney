using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace ChineseJourney.Common.View
{
    public class SkiaSvgImage : SKCanvasView, IImageDisplayControl
    {
        public SkiaSvgImage()
        {
            PaintSurface += ImageDisplayControl_PaintSurface;
        }

        public static BindableProperty ImageProperty = BindableProperty.Create(@"Image", typeof(SKSvg), typeof(SkiaSvgImage), null, propertyChanged: ImageChanged);
        public static readonly BindableProperty ForceToLeftProperty = BindableProperty.Create(@"ForceToLeft", typeof(bool), typeof(SkiaSvgImage), false);

        public bool ForceToLeft
        {
            get { return (bool)GetValue(ForceToLeftProperty); }
            set { SetValue(ForceToLeftProperty, value); }
        }

        private SKSvg _image;

        public double ZoomFactor { get; set; } = 1.0f;
        private static void ImageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var c = bindable as SkiaSvgImage;
            if (c != null)
            {
                c._image = newValue as SKSvg;
                c.InvalidateSurface();
            }
        }

        public SKSvg Image => _image;

        public SKRect Bound { get; set; }

        void ImageDisplayControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (_image != null)
            {
                var surface = e.Surface;
                var canvas = surface.Canvas;

                var width = e.Info.Width;
                var height = e.Info.Height;

                // clear the surface
                canvas.Clear(SKColors.White);

                // calculate the scaling need to fit to screen
                float canvasMin = Math.Min(width, height);
                float svgMax = Math.Max(_image.Picture.CullRect.Width, _image.Picture.CullRect.Height);
                float scale = canvasMin / svgMax;
                var matrix = SKMatrix.MakeScale(scale, scale);

                // draw the svg
                canvas.DrawPicture(_image.Picture, ref matrix);
            }
        }

        public event EventHandler<PaintEventArgs> OnPaint;
    }
}