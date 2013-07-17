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

namespace SearchSidebar
{
    class SearchResult
    {
        public String folio, text1, text2, excerpt, tag;
        public int lineNum;
        public Image thumbnail;
        public int resultType; // 1 = poetry, 2 = music, 3 = image

        public SearchResult()
        {
            


        }
    }
}
