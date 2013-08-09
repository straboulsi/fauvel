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
using System.Windows.Media.Imaging;


namespace SurfaceApplication1
{

    /**
     * This class fetches thumbnails from the thumbnails folder for search results.
     * Primary Coder: Alison Y. Chang
     * */
    static class Thumbnailer
    {

        /**
         * Returns the image when given the tag of a chunk of poetry, music object, or image.
         * For music lyric results, starts by removing the _t at end of tag (which meant text).
         * */
        public static BitmapImage getThumbnail(String tag)
        {
            if (tag.EndsWith("_t"))
                tag = tag.Substring(0, tag.Length - 2);

            tag = Counterpart.checkForCounterparts(tag);

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("thumbnails/" + tag + ".jpg", UriKind.Relative);
            image.EndInit();
            image.Freeze();

            return image;
        }


    }
}
