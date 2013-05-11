using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;

//using Samurai.MODIReader;  
/* Removed from the release version as I have no need for an OCR reader.
 * This was used for the castrol index / finktank predictions where all they would provide
 * was an image with % probabilities.  The project is still in the github repo so you can
 * resurect it if you want.
 */

namespace Samurai.Core
{
  public sealed class ImageReader<T> where T : IReadableImage
  {
    public T GetBitmaps(T obj, Image imageFile)
    {
      var statReadRectangle = new Rectangle(0, 0, 512, 512);

      var fullScreen = (Bitmap)imageFile;

      foreach (PropertyInfo p in WebUtils.PropertyInfos<T>())
      {
        Position att = null;
        foreach (var attCheck in p.GetCustomAttributes(false))
        {
          att = attCheck as Position;
          if (att != null)
          {
            string statRead = string.Empty;
            Guid g = obj.FileGuid;
            var cropRectangle = new Rectangle(att.X, att.Y, att.Width, att.Height);
            using (var croppedImage = FrameText(fullScreen, cropRectangle, statReadRectangle, att.Colour))
            {
              croppedImage.Save(string.Format(@"{0}{1}{2}.tif", Path.GetTempPath(), p.Name, g.ToString()), ImageFormat.Tiff);
              //statRead = MODIReader.MODIReader.Read(string.Format(@"{0}{1}{2}.tif", Path.GetTempPath(), p.Name, g.ToString()));
            }
            object[] update = new object[1];
            update[0] = statRead.Trim();
            typeof(T).InvokeMember(p.Name, BindingFlags.SetProperty, null, obj, update);
          }
        }

      }
      fullScreen.Dispose();
      imageFile.Dispose();
      return obj;
    }

    public T ConvertReadItems(T conv)
    {
      foreach (PropertyInfo p in WebUtils.PropertyInfos<T>())
      {
        Position att = null;
        if (p.GetCustomAttributes(false).Length > 0)
          att = p.GetCustomAttributes(false)[0] as Position;
        if (att != null)
        {
          var regString = typeof(T).GetProperty(p.Name).GetValue(conv, null) as string;
          if (regString != null)
          {
            regString = conv.StringManipulation(regString);
            foreach (var reg in conv.Regexs)
            {
              if (reg.IsMatch(regString))
              {
                foreach (var append in reg.GetGroupNames())
                {
                  double val = 0.0;
                  if (append == "0")
                    continue;
                  if (double.TryParse(reg.Match(regString).Groups[append].ToString(), out val))
                  {
                    object[] update = new object[1];
                    update[0] = val;
                    typeof(T).InvokeMember(p.Name + append, BindingFlags.SetProperty, null, conv, update);
                  }
                }
              }
            }
          }
        }
      }
      return conv;
    }


    private Bitmap FrameText(Bitmap img, Rectangle cropArea, Rectangle resizeArea, int backGround)
    {
      var destBitmap = new Bitmap(resizeArea.Width, resizeArea.Height);

      using (var imgBitmap = new Bitmap(img))
      {
        using (var cropBitmap = imgBitmap.Clone(cropArea, imgBitmap.PixelFormat))
        {
          using (var g = Graphics.FromImage(destBitmap))
          {
            g.Clear(System.Drawing.Color.FromArgb(backGround, backGround, backGround));
            g.DrawImage(cropBitmap, new Rectangle((resizeArea.Width / 2) - (cropArea.Width / 2),
              (resizeArea.Height / 2) - (cropArea.Height / 2), cropArea.Width, cropArea.Height));
          }
          return new Bitmap(destBitmap);
        }
      }
    }
  }
}
