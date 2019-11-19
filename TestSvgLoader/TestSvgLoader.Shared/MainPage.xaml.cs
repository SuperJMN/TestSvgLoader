using System;
using System.IO;
using System.Reflection;
using Windows.UI.Xaml.Controls;
using SkiaSharp;
using SkiaSharp.Views.UWP;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestSvgLoader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SkiaSharp.Extended.Svg.SKSvg svg;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private static Stream GetImageStream(string svgName)
        {
            var type = typeof(MainPage).GetTypeInfo();
            var assembly = type.Assembly;

            return assembly.GetManifestResourceStream($"NewSCADA.images.{svgName}");
        }

        private void LoadSvg(string svgName)
        {
            // create a new SVG object
            svg = new SkiaSharp.Extended.Svg.SKSvg();

            // load the SVG document from a stream
            using (var stream = GetImageStream(svgName))
                svg.Load(stream);
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            var page = (SKXamlCanvas)sender;
            LoadSvg(page.Tag as string);

            var surface = e.Surface;
            var canvas = surface.Canvas;

            var width = e.Info.Width;
            var height = e.Info.Height;

            // clear the surface
            canvas.Clear(SKColors.White);

            // the page is not visible yet
            if (svg == null)
                return;

            // calculate the scaling need to fit to screen
            float canvasMin = Math.Min(width, height);
            float svgMax = Math.Max(svg.Picture.CullRect.Width, svg.Picture.CullRect.Height);
            float scale = canvasMin / svgMax;
            var matrix = SKMatrix.MakeScale(scale, scale);

            // draw the svg
            canvas.DrawPicture(svg.Picture, ref matrix);
        }
    }
}