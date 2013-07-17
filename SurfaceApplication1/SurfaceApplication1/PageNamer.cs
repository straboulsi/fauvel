using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfaceApplication1
{
    public static class PageNamer
    {
        private static String[] pageNames = {"blank" ,"index","0v","1r" ,"1v",
                                         "2r" ,"2v","3r" ,"3v","4r" ,"4v", "5r" ,"5v",
                                         "6r" ,"6v","7r" ,"7v","8r" ,"8v","9r" ,"9v",
                                         "10r","10v","11r","11v","12r","12v","13r","13v",
                                         "14r","14v","15r","15v","16r","16v","17r","17v",
                                         "18r","18v","19r","19v","20r","20v","21r","21v",
                                         "22r","22v","23r","23v","24r","24v","25r","25v",
                                         "26r","26v","27r","27v","28r","28v","28br","28bv",
                                         "28tr","28tv","29r","29v","30r","30v","31r","31v",
                                         "32r","32v","33r","33v","34r","34v","35r","35v",
                                         "36r","36v","37r","37v","38r","38v","39r","39v",
                                         "40r","40v","41r","41v","42r","42v","43r","43v",
                                         "44r","44v","blank"};

        public static String getPageText(int i, bool twopage)
        {
            if (i > SurfaceWindow1.maxPage)
                return "";
            else
            {
                if (twopage)
                    return pageNames[i] + " / " + pageNames[i + 1];
                else
                    return pageNames[i];
            }
        }
    }
}
