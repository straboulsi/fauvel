using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace SearchSidebar
{
    static class Thumbnailer
    {

        public static Bitmap getThumbnail(Image original, Point topLeft, Point bottomRight)
        {

            Bitmap thumbnail = new Bitmap(original);



            return thumbnail;
        }



        public static Rectangle getRect(String tag, XmlDocument layoutXml)
        {
            Rectangle rect = new Rectangle();
            try
            {
                int x = (int)Class1.getPoint(tag, 1, layoutXml).X;
                int y = (int)Class1.getPoint(tag, 1, layoutXml).Y;
                int width = (int)Class1.getPoint(tag, 2, layoutXml).X - x;
                int height = (int)Class1.getPoint(tag, 2, layoutXml).Y - y;

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
    }
}
