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


        public static Image cropImage(Image img, Rectangle r)
        {
            
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(r, bmpImage.PixelFormat);
           
            return (Image)(bmpCrop);
        }

        // Assuming input of Fo1r, Fo1v, etc.
        // Remember other types: Fo23v, Fo28br, Fo28tv
        public static Image getImage(String folio, XmlDocument layoutXml)
        {
            folio = folio.Substring(2);
            String imageName = getImageName(folio, layoutXml);
            Image image = Image.FromFile(@"..\..\pages\" + imageName, true); 

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

            
            return rect;
        }

        public static Rectangle getLineRect(String lineNum, XmlDocument layoutXml)
        {
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

            
            return rect;
        }

    }
}
