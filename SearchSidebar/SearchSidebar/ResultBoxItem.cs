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
   

    
    class ResultBoxItem : ListBoxItem
    {
        public StackPanel resultStack, infoStack;
        public String excerpt;
        public TextBlock folioInfo, lineInfo, resultText;
        public Image resultThumbnail;
        public int resultType; // 1 = poetry, 2 = music, 3 = image


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
            resultThumbnail.Height = 50;
            resultThumbnail.Width = 50;
            resultThumbnail.Margin = new Thickness(5);

            resultText = new TextBlock();
            resultText.Width = 350;
            resultText.TextTrimming = TextTrimming.WordEllipsis;

            resultStack.Children.Add(infoStack);
            resultStack.Children.Add(resultThumbnail);
            resultStack.Children.Add(resultText);


        }



    }
}
