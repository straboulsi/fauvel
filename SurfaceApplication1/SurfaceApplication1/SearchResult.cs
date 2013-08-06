using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SurfaceApplication1
{
    /**
     * This class represents a SearchResult created in Translate.cs.
     * The search methods in Translate.cs each return a List of SearchResults.
     * Many elements of SearchResults are then transferred to ResultBoxItems for result display.
     * */
    public class SearchResult
    {
        public String folio, text1, text2, tag, lineRange;
        public List<SpecialString> excerpts = new List<SpecialString>();
        public int lineNum;
        public Image thumbnail, minithumbnail;
        public int resultType; // 1 = poetry, 2 = music, 3 = image

        public SearchResult()
        {



        }
    }
}
