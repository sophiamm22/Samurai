using MODI;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using System.Text;

namespace Samurai.MODIReader
{
  public class MODIReader
  {
    public static string Read(string fileName)
    {
      var modiDoc = new MODI.Document();

      var sb = new StringBuilder();
      try
      {
        modiDoc.Create(fileName);
        modiDoc.OCR(MiLANGUAGES.miLANG_SYSDEFAULT, false, false);

        for (int i = 0; i < modiDoc.Images.Count; i++)
        {
          var image = (MODI.Image)modiDoc.Images[i];
          var layout = image.Layout;
          // getting the page's words

          for (int j = 0; j < layout.Words.Count; j++)
          {
            var word = (MODI.Word)layout.Words[j];
            sb.Append(word.Text);
            sb.Append(" ");
          }
        }
      }
      catch
      {
        sb.Append("Error from MODI reader");
      }
      finally
      {
        modiDoc.Close();
      }
      return sb.ToString();
    }
  }
}
