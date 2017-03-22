using System.IO;
using System.Windows.Media.Imaging;

namespace 批量生成打印二维码
{
    class Files
    {
        public static void Save(BitmapSource bitmapSource, string fileName)
        {
            BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
            using (Stream stream = File.Create(fileName))
            {
                bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                bitmapEncoder.Save(stream);
            }
        }
    }
}
