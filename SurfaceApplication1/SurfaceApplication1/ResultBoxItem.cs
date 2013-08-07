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
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;


namespace SurfaceApplication1
{

    /**
     * ResultBoxItem is a specialized SurfaceListBoxItem. 
     * It takes many elements from SearchResult objects created in Translate.cs.
     * Each ResultBoxItem is then displayed in a ListBox in the search result tabs.
     * Primary Coder: Alison Y. Chang
     **/
    public class ResultBoxItem : SurfaceListBoxItem
    {
        public StackPanel resultStack, infoStack;
        public List<SpecialString> excerpts;
        public TextBlock folioInfo, lineInfo, resultText;
        public Image resultThumbnail, miniThumbnail;
        public int resultType; // 1 = poetry, 2 = music, 3 = image
        public Point topL, bottomR;


        public ResultBoxItem()
        {
            resultStack = new StackPanel();
            resultStack.Orientation = Orientation.Horizontal;
            Content = resultStack;
            Height = 65;

            infoStack = new StackPanel();
            folioInfo = new TextBlock();
            lineInfo = new TextBlock();
            folioInfo.FontSize = 18;
            lineInfo.FontSize = 18;
            folioInfo.HorizontalAlignment = HorizontalAlignment.Center;
            lineInfo.HorizontalAlignment = HorizontalAlignment.Center;
            infoStack.Children.Add(folioInfo);
            infoStack.Children.Add(lineInfo);

            resultThumbnail = new Image();
            miniThumbnail = new Image(); // miniThumbnails are used for image results
            miniThumbnail.Margin = new Thickness(5);

            resultText = new TextBlock();
            resultText.Width = 350;
            resultText.TextTrimming = TextTrimming.WordEllipsis;

            infoStack.Margin = new Thickness(0, 0, 15, 0);
            infoStack.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            resultText.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            resultStack.Children.Add(infoStack);
            resultStack.Children.Add(miniThumbnail); // for image results
            resultStack.Children.Add(resultText);


        }



    }
}
