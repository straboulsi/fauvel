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

        public static Image getThumbnail(String tag)
        {
            checkForCounterparts(tag);

            Image thumbnail = Image.FromFile(@"..\..\thumbnails\" + tag + ".jpg", true);

            return thumbnail;
        }

        public static String checkForCounterparts(String tag)
        {
            if (!tag.StartsWith("Te") && !tag.Contains("Im")) // If it's music
            {
                foreach (Counterpart c in makeCounterpartList())
                {
                    foreach (String s in c.otherNames)
                    {
                        if (tag == s)
                        {
                            tag = c.name;
                            break;
                        }
                    }
                }
                
            }
            return tag;

        }

        public static List<Counterpart> makeCounterpartList()
        {
            List<Counterpart> counterpartList = new List<Counterpart>();
            counterpartList.Add(new Counterpart("10vMo2", new List<String>(new String[] { "11rMo1" })));
            counterpartList.Add(new Counterpart("11vMo1", new List<String>(new String[] { "12rMo1" })));
            counterpartList.Add(new Counterpart("12rPr1", new List<String>(new String[] { "12vPr1" })));
            counterpartList.Add(new Counterpart("14rPr1", new List<String>(new String[] { "14vPr1", "15rPr1" })));
            counterpartList.Add(new Counterpart("15vMo1", new List<String>(new String[] { "16rMo1" })));
            counterpartList.Add(new Counterpart("16vBa2", new List<String>(new String[] { "17rBa1" })));
            counterpartList.Add(new Counterpart("17rLa1", new List<String>(new String[] { "17vLa1", "18rLa1", "18vLa1" })));
            counterpartList.Add(new Counterpart("19rLa1", new List<String>(new String[] { "19vLa1" })));
            counterpartList.Add(new Counterpart("19vRo1", new List<String>(new String[] { "20rRo1" })));
            counterpartList.Add(new Counterpart("22rLa1", new List<String>(new String[] { "22vLa1", "23rLa1" })));
            counterpartList.Add(new Counterpart("23vBa1", new List<String>(new String[] { "24rBa1" })));
            counterpartList.Add(new Counterpart("25vRef4", new List<String>(new String[] { "26rRef1" })));
            counterpartList.Add(new Counterpart("28brLa1", new List<String>(new String[] { "28bvLa1", "28trLa1", "28tvLa1" })));
            counterpartList.Add(new Counterpart("34vLa1", new List<String>(new String[] { "35rLa1", "35vLa1", "36rLa1", "36vLa1" })));
            counterpartList.Add(new Counterpart("37rSe1", new List<String>(new String[] { "37vSe1" })));
            counterpartList.Add(new Counterpart("41vMo1", new List<String>(new String[] { "42rMo1" })));
            counterpartList.Add(new Counterpart("43rMo2", new List<String>(new String[] { "43vMo1" })));

            return counterpartList;
        }



        //public static Image cropImage(Image img, Rectangle r)
        //{
            
        //    Bitmap bmpImage = new Bitmap(img);
        //    Bitmap bmpCrop = bmpImage.Clone(r, bmpImage.PixelFormat);
           
        //    return (Image)(bmpCrop);
        //}

        // Assuming input of Fo1r, Fo1v, etc.
        // Remember other types: Fo23v, Fo28br, Fo28tv
        //public static Image getImage(String folio, XmlDocument layoutXml)
        //{
        //    folio = folio.Substring(2);
        //    String imageName = getImageName(folio, layoutXml);
        //    Image image = Image.FromFile(@"..\..\pages\" + imageName, true); 

        //    return image;
        //}

        //public static String getImageName(String folio, XmlDocument layoutXml)
        //{
        //    String imageName = "";
        //    XmlNode node = layoutXml.DocumentElement.SelectSingleNode("//surface[@id='" + folio + "']");
        //    imageName = node.FirstChild.SelectSingleNode("graphic").Attributes["url"].Value;

        //    return imageName;
        //}



        //public static Rectangle getRect(String tag, XmlDocument layoutXml)
        //{
        //    Rectangle rect = new Rectangle();
        //    try
        //    {
        //        int x = (int)Translate.getPoint(tag, 1, layoutXml).X;
        //        int y = (int)Translate.getPoint(tag, 1, layoutXml).Y;
        //        int width = (int)Translate.getPoint(tag, 2, layoutXml).X - x;
        //        int height = (int)Translate.getPoint(tag, 2, layoutXml).Y - y;

        //        rect = new Rectangle(x, y, width, height);

        //    }
        //    catch (Exception e)
        //    {
        //        Console.Write(e.StackTrace);
        //        Console.Read();
        //    }

            
        //    return rect;
        //}

        //public static Rectangle getLineRect(String lineNum, XmlDocument layoutXml)
        //{
        //    Rectangle rect = new Rectangle();
        //    try
        //    {
        //        int x = (int)Translate.getLinePoint(lineNum, 1, layoutXml).X;
        //        int y = (int)Translate.getLinePoint(lineNum, 1, layoutXml).Y;
        //        int width = (int)Translate.getLinePoint(lineNum, 2, layoutXml).X - x;
        //        int height = (int)Translate.getLinePoint(lineNum, 2, layoutXml).Y - y;

        //        rect = new Rectangle(x, y, width, height);

        //    }
        //    catch (Exception e)
        //    {
        //        Console.Write(e.StackTrace);
        //        Console.Read();
        //    }

            
        //    return rect;
        //}

    }
}
