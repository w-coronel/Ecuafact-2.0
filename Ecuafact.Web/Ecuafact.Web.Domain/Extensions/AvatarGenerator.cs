using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Extensions
{
    public class Avatar
    {
        static readonly List<string> _BackgroundColours = new List<string> { "#B26126", "#E9341B", "#1B1BE9", "#69C0C8", "#FFC300" };
          
        public static byte[] Generate(string avatarString)
        {  
            var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
            var bgColour = "FFFFFF";
            var textColor = System.Drawing.ColorTranslator.FromHtml(_BackgroundColours[randomIndex]);  
            var bmp = new Bitmap(192, 192);
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var font = new Font("Poppins", 150, FontStyle.Bold, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(bmp);

            graphics.Clear((Color)new ColorConverter().ConvertFromString("#" + bgColour));
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.DrawString(avatarString, font, new SolidBrush(textColor), new  RectangleF(0, 0, 192, 192), sf);
            graphics.Flush();

            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            
            return ms.ToArray();
        }
    }

}
