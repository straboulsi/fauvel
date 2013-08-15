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

namespace DigitalFauvel
{
    /**
     * Primary Coder: Brendan Chou
     * */
    public class SavedPagesTab : SideBarTab
    {

        public SavedPagesTab(SideBar mySideBar) : base(mySideBar)
        {
            int x = 0;
            int y = 50;
            foreach (SavedPage s in mySideBar.savedPages)
            {
                canvas.Children.Add(s.button);
                Canvas.SetLeft(s.button, x);
                Canvas.SetTop(s.button, y);

                x += 180;
                if (x >= 500)
                {
                    x = 0;
                    y += 129;
                }
            }
        }

    }
}
