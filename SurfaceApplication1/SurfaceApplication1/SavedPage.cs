using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SurfaceApplication1
{
    public class SavedPage
    {
        public Point center;
        public double width;
        public int pageNum;
        public SurfaceWindow1.language language;
        public SavedPage(int p, double w, Point c, SurfaceWindow1.language l)
        {
            center = c;
            width = w;
            pageNum = p;
            language = l;
        }
    }
}
