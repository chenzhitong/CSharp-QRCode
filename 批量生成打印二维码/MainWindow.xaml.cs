using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gma.QrCodeNet.Encoding;

namespace 批量生成打印二维码
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //生成很多ID
            int count = 0;
            try
            {
                count = Convert.ToInt32(textBox1.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Input Error");
            }
            var addresses = CreateURL(count);

            //生成很多二维码
            var urlBase = "http://192.168.1.10/Home/Index?id=";
            foreach (var add in addresses)
            {
                var url = $"{urlBase}{add}";
                //using (System.IO.StreamWriter sw = new System.IO.StreamWriter("code.txt",true))
                //{
                //    sw.WriteLine(url);
                //}

                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
                QrCode qrCode = qrEncoder.Encode(url);

                var withBorder = addBorder(qrCode.Matrix.InternalArray, 10, qrCode.Matrix.Width);
                bool[,] qr = zoom(withBorder, 10, (int)Math.Sqrt(withBorder.Length));
                int w = (int)Math.Sqrt(qr.Length);

                byte[] data = new byte[qr.Length];

                for (int j = 0; j < w; j++)
                {
                    for (int i = 0; i < w; i++)
                    {
                        data[j * w + i] = (byte)(qr[i, j] ? 0 : 255);
                    }
                }
                BitmapSource qrBitSource = BitmapSource.Create(w, w, 300, 300, PixelFormats.Gray8, null, data, w);

                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    ////矩形-白底
                    //drawingContext.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(new Point(0, 0), new Point(638, 1010)));
                    ////文字-微信扫一扫
                    //drawingContext.DrawText(
                    //    new FormattedText(
                    //        "微信扫一扫",
                    //        System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    //        new Typeface(new FontFamily("微软雅黑"), FontStyles.Normal, FontWeights.Regular, FontStretches.Normal),
                    //        10, new SolidColorBrush(Colors.Black)
                    //    ),
                    //    new Point(75, 170));
                    ////文字-参加小蚁游戏，大奖等你拿
                    //drawingContext.DrawText(
                    //    new FormattedText(
                    //        "参加小蚁游戏，大奖等你拿",
                    //        System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    //        new Typeface(new FontFamily("微软雅黑"), FontStyles.Normal, FontWeights.Regular, FontStretches.Normal),
                    //        10, new SolidColorBrush(Colors.Black)
                    //    ),
                    //    new Point(40, 185));
                    ////文字-QQ群
                    //drawingContext.DrawText(
                    //    new FormattedText(
                    //        "小蚁，你我的数字资产\n  8月8日小蚁全球ICO\n官方QQ群573816801",
                    //        System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    //        new Typeface(new FontFamily("微软雅黑"), FontStyles.Normal, FontWeights.Regular, FontStretches.Normal),
                    //        10, new SolidColorBrush(Colors.Black)
                    //    ),
                    //    new Point(50, 285));

                    var img = new BitmapImage(new Uri("t.png", UriKind.Relative));
                    drawingContext.DrawImage(img, new Rect(new Point(0, 0), new Point(204.16, 323.2)));

                    drawingContext.DrawText(
                        new FormattedText(
                            add,
                            System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            new Typeface(new FontFamily("微软雅黑"), FontStyles.Normal, FontWeights.Regular, FontStretches.Normal),
                            10, new SolidColorBrush(Colors.White)
                        ),
                        new Point(118, 47));

                    //图片-二维码
                    drawingContext.DrawImage(qrBitSource, new Rect(new Point(42, 120), new Point(160, 238)));
                }

                try
                {
                    RenderTargetBitmap bmp = new RenderTargetBitmap(638, 1010, 300, 300, PixelFormats.Pbgra32);
                    bmp.Render(drawingVisual);
                    //canvas1.Children.Add(new Image());
                    //(canvas1.Children[0] as Image).Source = bmp;
                    var fileName = $"code1/{add}.png";
                    Files.Save(bmp, fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private List<string> CreateURL(int count)
        {
            Random r = new Random();
            List<string> result = new List<string>();
            for (int i = 0; i < count; i++)
            {
                var b = new byte[8];
                r.NextBytes(b);
                var code = Code.Create(b.Sha256().Substring(0, 5));
                if(!result.Contains(code))
                    result.Add(code);
            }
            return result;
        }

        //放大
        private bool[,] zoom(bool[,] input, int X, int width)
        {
            int newWinth = width * X;
            bool[,] result = new bool[newWinth, newWinth];
            for (int i = 0; i < newWinth; i++)
                for (int j = 0; j < newWinth; j++)
                    result[i, j] = input[i * width / newWinth, j * width / newWinth];
            return result;
        }

        //加白边
        private bool[,] addBorder(bool[,] input, int percent, int width)
        {
            int X = width / percent;
            int newWinth = width + X + X;
            bool[,] result = new bool[newWinth, newWinth];

            //生成白底
            for (int i = 0; i < newWinth; i++)
                for (int j = 0; j < newWinth; j++)
                    result[i, j] = false;


            //将二维码写到居中写到白底上
            for (int i = 0; i < width; i++)
                for (int j = 0; j < width; j++)
                    result[i + X, j + X] = input[i,j];

            return result;
        }


    }
}
