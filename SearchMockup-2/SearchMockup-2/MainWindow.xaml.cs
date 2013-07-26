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


namespace SearchMockup_2
{

   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Boolean defaultOptionsChanged = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private TabItem createSearchTab()
        {
            TabItem searchTab = new TabItem();


            return searchTab;
        }


        private void Show_Options(object sender, RoutedEventArgs e)
        {
            More_Options.Visibility = Visibility.Hidden;
            Top_Line.Visibility = Visibility.Hidden;
            Search_Options.Visibility = Visibility.Visible;
        }

        private void Hide_Options(object sender, RoutedEventArgs e)
        {
            Search_Options.Visibility = Visibility.Hidden;
            More_Options.Visibility = Visibility.Visible;
            Top_Line.Visibility = Visibility.Visible;

            checkForChanges();

            if (defaultOptionsChanged == true)
                More_Options.Background = Brushes.MediumTurquoise;

            else
                More_Options.ClearValue(Control.BackgroundProperty);

        }

        private Boolean checkForChanges()
        {
            if (Case_Sensitive.IsChecked == true | Whole_Phrase_Only.IsChecked == true | Whole_Word_Only.IsChecked == true)
                defaultOptionsChanged = true;

            else
                defaultOptionsChanged = false;


            return defaultOptionsChanged;
        }

        
    }
}
