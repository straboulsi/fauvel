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

public class SearchTab
{
	public SearchTab()
	{
        Canvas searchCanvas = new Canvas();
        searchCanvas.Width = "500";

	}



    /**
     * 
     * <Button Width="70" Height="40" Canvas.Left="476" Canvas.Top="1" FontFamily="Arial" FontSize="35" VerticalAlignment="Top" Click="btnDelete_Click">X</Button>


                            <Label Canvas.Left="32" Canvas.Top="26" FontSize="30">
                                Search:
                            </Label>
                            <TextBox x:Name="Search_query" Canvas.Left="40" Canvas.Top="90" Height="40" Width="315" Foreground="Gray">
                                Enter text
                            </TextBox>
                            <Button Canvas.Left="378" Canvas.Top="90" Height="40" Width="95">
                                Go!
                            </Button>
                            <Button Visibility="Visible" x:Name="More_Options" Click="Show_Options" Canvas.Left="40" Canvas.Top="143" Height="30" Width="30" FontSize="20" VerticalAlignment="Top">
                                ▼
                            </Button>
                            <Line x:Name="Top_Line" X1="72" Y1="163" X2="473" Y2="163" Stroke="Black" StrokeThickness="2" Canvas.Top="-3" />

                            <Canvas x:Name="Search_Options" Visibility="Hidden">
                                <Button x:Name="Less_Options" Canvas.Left="40" Canvas.Top="281" Height="30" Width="30" FontSize="20" VerticalAlignment="Top">
                                    ▲
                                </Button>

                                <CheckBox x:Name="Case_Sensitive" Canvas.Left="40" Canvas.Top="170" FontSize="10" >
                                    <CheckBox.LayoutTransform>
                                        <ScaleTransform ScaleX="2" ScaleY="2" />
                                    </CheckBox.LayoutTransform>
                                    Case sensitive
                                </CheckBox>
                                <ComboBox x:Name="Select_Language" Background="White" Text ="Select language" Canvas.Left="34" Canvas.Top="220" Width="175" Height="40" FontSize="21" HorizontalContentAlignment="Center" SelectedIndex="0">
                                    Select language
                                    <ComboBoxItem>Old French</ComboBoxItem>
                                    <ComboBoxItem>Modern French</ComboBoxItem>
                                    <ComboBoxItem>English</ComboBoxItem>
                                </ComboBox>
                                <Line x:Name="Bottom_Line" Canvas.Left="0" Canvas.Top="113" Stroke="Black" StrokeThickness="2" X1="72" X2="473" Y1="183" Y2="183" />
                                <CheckBox x:Name="Whole_Word_Only" Canvas.Left="243" Canvas.Top="170" FontSize="10">
                                    <CheckBox.LayoutTransform>
                                        <ScaleTransform ScaleX="2" ScaleY="2" />
                                    </CheckBox.LayoutTransform>
                                    Match whole word only
                                </CheckBox>
                                <CheckBox x:Name="Whole_Phrase_Only" Canvas.Left="243" Canvas.Top="227" FontSize="10">
                                    <CheckBox.LayoutTransform>
                                        <ScaleTransform ScaleX="2" ScaleY="2" />
                                    </CheckBox.LayoutTransform>
                                    Match whole phrase only
                                </CheckBox>
                            </Canvas>
     * */
}
