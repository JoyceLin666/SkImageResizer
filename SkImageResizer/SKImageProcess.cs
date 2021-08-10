using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;

namespace SkImageResizer
{
    public class SKImageProcess
    {
        /// <summary>
        /// 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public void ResizeImages(string sourcePath, string destPath, double scale)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            var allFiles = FindImages(sourcePath);
            foreach (var filePath in allFiles)
            {
                var bitmap = SKBitmap.Decode(filePath);
                var imgPhoto = SKImage.FromBitmap(bitmap);
                var imgName = Path.GetFileNameWithoutExtension(filePath);

                var sourceWidth = imgPhoto.Width;
                var sourceHeight = imgPhoto.Height;

                var destinationWidth = (int)(sourceWidth * scale);
                var destinationHeight = (int)(sourceHeight * scale);

                using var scaledBitmap = bitmap.Resize(
                    new SKImageInfo(destinationWidth, destinationHeight),
                    SKFilterQuality.High);
                using var scaledImage = SKImage.FromBitmap(scaledBitmap);
                using var data = scaledImage.Encode(SKEncodedImageFormat.Jpeg, 100);
                using var s = File.OpenWrite(Path.Combine(destPath, imgName + ".jpg"));
                data.SaveTo(s);
            }
        }

        public async Task ResizeImagesAsync(string sourcePath, string destPath, double scale)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            await Task.Yield();

            var allFiles = FindImages(sourcePath);
            foreach (var filePath in allFiles)
            {
                var bitmap = SKBitmap.Decode(filePath);
                var imgPhoto = SKImage.FromBitmap(bitmap);
                var imgName = Path.GetFileNameWithoutExtension(filePath);

                var sourceWidth = imgPhoto.Width;
                var sourceHeight = imgPhoto.Height;

                var destinationWidth = (int)(sourceWidth * scale);
                var destinationHeight = (int)(sourceHeight * scale);
                //using var scaledBitmap = bitmap.Resize(
                //    new SKImageInfo(destinationWidth, destinationHeight),
                //    SKFilterQuality.High);
                using var scaledBitmap = await Task.Run(() =>
               bitmap.Resize(
                   new SKImageInfo(destinationWidth, destinationHeight),
                   SKFilterQuality.High)
               );
                using var scaledImage = SKImage.FromBitmap(scaledBitmap);
                using var data = await Task.Run(() => scaledImage.Encode(SKEncodedImageFormat.Jpeg, 100));
                using var s = File.OpenWrite(Path.Combine(destPath, imgName + ".jpg"));
                data.SaveTo(s);
            }
        }

        public async Task ResizeImagesAsync2(string sourcePath, string destPath, double scale)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            await Task.Yield();

            var allFiles = FindImages(sourcePath);

            ConcurrentDictionary<string, SKBitmap> condic = new ConcurrentDictionary<string, SKBitmap>();
            var arypng = Directory.GetFiles(sourcePath, "*.png", SearchOption.AllDirectories);
            var aryjpg = Directory.GetFiles(sourcePath, "*.jpg", SearchOption.AllDirectories);
            var aryjpeg = Directory.GetFiles(sourcePath, "*.jpeg", SearchOption.AllDirectories);

            foreach(var png in arypng)
            {

                var bitmap = SKBitmap.Decode(png);
                var imgPhoto = SKImage.FromBitmap(bitmap);
                var imgName = Path.GetFileNameWithoutExtension(png);
     
                var sourceWidth = imgPhoto.Width;
                var sourceHeight = imgPhoto.Height;

                var destinationWidth = (int)(sourceWidth * scale);
                var destinationHeight = (int)(sourceHeight * scale);

                var scaledBitmap = await Task.Run(() =>
            bitmap.Resize(
                new SKImageInfo(destinationWidth, destinationHeight),
                SKFilterQuality.High)
             );

                condic.TryAdd(imgName, scaledBitmap);
            }
            foreach (var jpg in aryjpg)
            {

                var bitmap = SKBitmap.Decode(jpg);
                var imgPhoto = SKImage.FromBitmap(bitmap);
                var imgName = Path.GetFileNameWithoutExtension(jpg);

                //await Task.Run(() =>
                //condic.TryAdd(jpg, imgPhoto)
                //);

                var sourceWidth = imgPhoto.Width;
                var sourceHeight = imgPhoto.Height;

                var destinationWidth = (int)(sourceWidth * scale);
                var destinationHeight = (int)(sourceHeight * scale);

                var scaledBitmap = await Task.Run(() =>
            bitmap.Resize(
                new SKImageInfo(destinationWidth, destinationHeight),
                SKFilterQuality.High)
             );

                condic.TryAdd(imgName, scaledBitmap);
            }
            foreach (var jpeg in aryjpeg)
            {

                var bitmap = SKBitmap.Decode(jpeg);
                var imgPhoto = SKImage.FromBitmap(bitmap);
                var imgName = Path.GetFileNameWithoutExtension(jpeg);

                //await Task.Run(() =>
                //condic.TryAdd(jpeg, imgPhoto)
                //);

                var sourceWidth = imgPhoto.Width;
                var sourceHeight = imgPhoto.Height;

                var destinationWidth = (int)(sourceWidth * scale);
                var destinationHeight = (int)(sourceHeight * scale);

                var scaledBitmap = await Task.Run(() =>
            bitmap.Resize(
                new SKImageInfo(destinationWidth, destinationHeight),
                SKFilterQuality.High)
             );

                condic.TryAdd(imgName, scaledBitmap);
            }

            foreach(var condicKey in condic)
            {
                using var scaledImage = SKImage.FromBitmap(condicKey.Value);
                using var data = scaledImage.Encode(SKEncodedImageFormat.Jpeg, 100);
                using var s = File.OpenWrite(Path.Combine(destPath, condicKey.Key + ".jpg"));
                data.SaveTo(s);
            }

            //foreach (var filePath in allFiles)
            //{
            //    var bitmap = SKBitmap.Decode(filePath);
            //    var imgPhoto = SKImage.FromBitmap(bitmap);
            //    var imgName = Path.GetFileNameWithoutExtension(filePath);

            //    var sourceWidth = imgPhoto.Width;
            //    var sourceHeight = imgPhoto.Height;

            //    var destinationWidth = (int)(sourceWidth * scale);
            //    var destinationHeight = (int)(sourceHeight * scale);

            //    var scaledBitmap = await Task.Run(() =>
            //  bitmap.Resize(
            //      new SKImageInfo(destinationWidth, destinationHeight),
            //      SKFilterQuality.High)
            //   );
            //    using var scaledImage = SKImage.FromBitmap(scaledBitmap);
            //    using var data = scaledImage.Encode(SKEncodedImageFormat.Jpeg, 100);
            //    using var s = File.OpenWrite(Path.Combine(destPath, imgName + ".jpg"));
            //    data.SaveTo(s);
            //}
        }

        /// <summary>
        /// 清空目的目錄下的所有檔案與目錄
        /// </summary>
        /// <param name="destPath">目錄路徑</param>
        public void Clean(string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            else
            {
                var allImageFiles = Directory.GetFiles(destPath, "*", SearchOption.AllDirectories);

                foreach (var item in allImageFiles)
                {
                    File.Delete(item);
                }
            }
        }

        /// <summary>
        /// 找出指定目錄下的圖片
        /// </summary>
        /// <param name="srcPath">圖片來源目錄路徑</param>
        /// <returns></returns>
        public List<string> FindImages(string srcPath)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(srcPath, "*.png", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpg", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpeg", SearchOption.AllDirectories));
            return files;
        }
    }
}