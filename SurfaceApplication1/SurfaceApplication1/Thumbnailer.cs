using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace SurfaceApplication1
{
    static class Thumbnailer
    {

        public static Bitmap cropAtRect(this Bitmap b, Rectangle r)
        {
            Bitmap nb = new Bitmap(r.Width, r.Height);
            Graphics g = Graphics.FromImage(nb);
            g.DrawImage(b, -r.X, -r.Y);
            return nb;
        }

        public static Image cropImage(Image img, Rectangle r)
        {
            /**
            Stopwatch sw5a = new Stopwatch();
            sw5a.Start();
            Bitmap nb = new Bitmap(r.Width, r.Height);
            Graphics g = Graphics.FromImage(nb);
            g.DrawImage(img, -r.X, -r.Y);
            sw5a.Stop();
            TimeSpan ts5a = sw5a.Elapsed;
            string elapsedTimea = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts5a.Hours, ts5a.Minutes, ts5a.Seconds,
            ts5a.Milliseconds / 10);
            Console.WriteLine("crop using Graphics: " + elapsedTimea);
            **/

            Stopwatch sw5 = new Stopwatch();
            sw5.Start();

            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(r, bmpImage.PixelFormat);
            sw5.Stop();
            TimeSpan ts5 = sw5.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts5.Hours, ts5.Minutes, ts5.Seconds,
            ts5.Milliseconds / 10);
            Console.WriteLine("cropImage using Clone: " + elapsedTime);



            return (Image)(bmpCrop);
            ///
            ///return nb;
        }

        // Assuming input of Fo1r, Fo1v, etc.
        // Remember other types: Fo23v, Fo28br, Fo28tv
        public static Image getImage(String folio, XmlDocument layoutXml)
        {
            Stopwatch sw6 = new Stopwatch();
            sw6.Start();
            folio = folio.Substring(2);
            String imageName = "/pages/" + getImageName(folio, layoutXml);
            ///String imageName = "Medieval.jpg";
            Image image = Image.FromFile(imageName);

            sw6.Start();
            TimeSpan ts6 = sw6.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts6.Hours, ts6.Minutes, ts6.Seconds,
            ts6.Milliseconds / 10);
            Console.WriteLine("getImage: " + elapsedTime);
            return image;
        }

        public static String getImageName(String folio, XmlDocument layoutXml)
        {
            String imageName = "";
            XmlNode node = layoutXml.DocumentElement.SelectSingleNode("//surface[@id='" + folio + "']");
            imageName = node.FirstChild.SelectSingleNode("graphic").Attributes["url"].Value;

            return imageName;
        }



        public static Rectangle getRect(String tag, XmlDocument layoutXml)
        {
            Rectangle rect = new Rectangle();
            try
            {
                int x = (int)Translate.getPoint(tag, 1, layoutXml).X;
                int y = (int)Translate.getPoint(tag, 1, layoutXml).Y;
                int width = (int)Translate.getPoint(tag, 2, layoutXml).X - x;
                int height = (int)Translate.getPoint(tag, 2, layoutXml).Y - y;

                rect = new Rectangle(x, y, width, height);

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            Console.Write(rect.Width + "\r\n" + rect.Height);

            return rect;
        }

        public static Rectangle getLineRect(String lineNum, XmlDocument layoutXml)
        {
            Stopwatch sw7 = new Stopwatch();
            sw7.Start();

            Rectangle rect = new Rectangle();
            try
            {
                int x = (int)Translate.getLinePoint(lineNum, 1, layoutXml).X;
                int y = (int)Translate.getLinePoint(lineNum, 1, layoutXml).Y;
                int width = (int)Translate.getLinePoint(lineNum, 2, layoutXml).X - x;
                int height = (int)Translate.getLinePoint(lineNum, 2, layoutXml).Y - y;

                rect = new Rectangle(x, y, width, height);

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            Console.Write(rect.Width + "\r\n" + rect.Height);

            sw7.Stop();
            TimeSpan ts7 = sw7.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts7.Hours, ts7.Minutes, ts7.Seconds,
            ts7.Milliseconds / 10);
            Console.WriteLine("getLineRect: " + elapsedTime);

            ///rect = new Rectangle(0, 0, 50, 50);

            return rect;
        }

    }
}
