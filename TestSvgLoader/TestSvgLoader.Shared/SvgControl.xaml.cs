using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using SkiaSharp;
using SkiaSharp.Views.UWP;
using TestSvgLoader.Shared;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace TestSvgLoader
{
    public sealed partial class SvgControl
    {
        public SvgControl()
        {
            InitializeComponent();
            svg = new SKSvg();
        }

        public static readonly DependencyProperty SourceFromEmbeddedResourceProperty = DependencyProperty.Register(
            "SourceFromEmbeddedResource", typeof(Uri), typeof(SvgControl), new PropertyMetadata(default(Uri), OnSourceFromEmbeddedResourceChanged));

        private static void OnSourceFromEmbeddedResourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (Uri)e.NewValue;
            var oldValue = (Uri)e.OldValue;
            var target = (SvgControl)d;

            target.OnSourceFromEmbeddedResourceChanged(oldValue, newValue);
        }

        private void OnSourceFromEmbeddedResourceChanged(Uri oldValue, Uri newValue)
        {
            var type = typeof(MainPage);

            var identifier = newValue.OriginalString.Replace("ms-resource:///Files/", "");
            
            var assemblyName = type.Assembly.GetName().Name;
            var resourceName = $"{assemblyName}.{identifier}";
            
            //var stream = type.Assembly.GetManifestResourceStream(resourceName);
            var stream = ResourceUtils.GetStreamFromResourceFile(identifier, type);

            svg.Load(stream);
        }

        public Uri SourceFromEmbeddedResource
        {
            get { return (Uri) GetValue(SourceFromEmbeddedResourceProperty); }
            set { SetValue(SourceFromEmbeddedResourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof(Uri), typeof(SvgControl), new PropertyMetadata(default(Uri), OnSourceChanged));

        private readonly SKSvg svg;
        private IDisposable loader;

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (Uri)e.NewValue;
            var oldValue = (Uri)e.OldValue;
            var target = (SvgControl)d;

            target.OnSourceChanged(oldValue, newValue);
        }

        private void OnSourceChanged(Uri oldValue, Uri newValue)
        {
            var fixedUri = new Uri(newValue.OriginalString.Replace("ms-resource:///Files", "ms-appx:///"));

            loader?.Dispose();
            loader = Observable
                .FromAsync(() => GetStream(fixedUri))
                .Subscribe(stream => svg.Load(stream));
        }
        
        private static async Task<Stream> GetStream(Uri uri)
        {
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
            var randomAccessStream = await storageFile.OpenReadAsync();
            var stream = randomAccessStream.AsStreamForRead();

            return stream;
        }

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
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
