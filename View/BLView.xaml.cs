using sldc.Model;
using sldc.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using sldc.Themes.ElementThemes;
using System.Collections.ObjectModel;
using sldc.Converter.CovenantConverters;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Net;

namespace sldc.View
{
    /// <summary>
    /// Interaction logic for BLView.xaml
    /// </summary>
    public partial class BLView : UserControl
    {
        private Dictionary<string, UIElement> StackpanelChildren = new Dictionary<string, UIElement>();
        public BLView()
        {
            InitializeComponent();
            //PopulateChangePlaythroughs();
            //BLViewModel.UpdatePlaythroughList += UpdatePlaythroughListFlagSetter;
        }

        //public void PopulateChangePlaythroughs()
        //{
        //    // create entries for playthroughs
        //    Dictionary<string,int> playthroughs = GameDeathDataSerialiser.LoadData(GameDeathDataSerialiser.NON_HOOKABLE_GAME.BLOODBORNE);
        //    foreach (var kvp in playthroughs)
        //    {
        //        CreateEntry(kvp.Key, kvp.Value);
        //    }
        //    // Sorting keys alphabetically
        //    var sortedKeys = StackpanelChildren.Keys.OrderBy(key => key);
        //    // Creating a new sorted dictionary
        //    var sortedDictionary = sortedKeys.ToDictionary(key => key, key => StackpanelChildren[key]);

        //    foreach (var kvp in sortedDictionary)
        //    {
        //        PlaythroughList.Children.Add(sortedDictionary[kvp.Key]);
        //    }

        //}
        //public void CreateEntry(string playthroughName, int playthroughDeaths)
        //{
        //    // Set Border properties
        //    Border border = new Border();
        //    border.Background = (Brush)Application.Current.FindResource("Background_Level_3");
        //    border.Height = 30;
        //    border.CornerRadius = new CornerRadius(10);
        //    border.Margin = new Thickness(0, 0, 0, 10);
        //    string borderName = FormatBorderName(playthroughName);
        //    border.Name = borderName;

        //    // Create DockPanel
        //    DockPanel dockPanel = new DockPanel();
        //    dockPanel.Margin = new Thickness(10, 0, 0, 0);

        //    // Create Grid
        //    Grid grid = new Grid();
        //    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(280) });
        //    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(235) });

        //    // Create first TextBlock
        //    TextBlock textBlock1 = new TextBlock();
        //    textBlock1.Text = playthroughName;
        //    textBlock1.FontFamily = (FontFamily)Application.Current.FindResource("Inter");
        //    textBlock1.FontWeight = FontWeights.Regular;
        //    textBlock1.FontSize = 15;
        //    textBlock1.Foreground = (Brush)Application.Current.FindResource("TextColour");
        //    textBlock1.Opacity = 0.8;
        //    textBlock1.HorizontalAlignment = HorizontalAlignment.Left;
        //    textBlock1.VerticalAlignment = VerticalAlignment.Center;

        //    // Create second TextBlock
        //    TextBlock textBlock2 = new TextBlock();
        //    textBlock2.Text = playthroughDeaths.ToString();
        //    textBlock2.FontFamily = (FontFamily)Application.Current.FindResource("Inter");
        //    textBlock2.FontWeight = FontWeights.Regular;
        //    textBlock2.FontSize = 15;
        //    textBlock2.Foreground = (Brush)Application.Current.FindResource("TextColour");
        //    textBlock2.Opacity = 0.8;
        //    textBlock2.HorizontalAlignment = HorizontalAlignment.Left;
        //    textBlock2.VerticalAlignment = VerticalAlignment.Center;
        //    Grid.SetColumn(textBlock2, 1);
        //    // create select button
        //    Button selectButton = new Button();
        //    selectButton.Width = 520;
        //    selectButton.Height = 25;
        //    selectButton.Visibility = Visibility.Visible;
        //    selectButton.Opacity = 0;
        //    selectButton.Cursor = Cursors.Hand;
        //    selectButton.HorizontalAlignment = HorizontalAlignment.Center;
        //    selectButton.VerticalAlignment = VerticalAlignment.Center;
        //    selectButton.Command = new RelayCommand(SelectPlaythrough);
        //    selectButton.CommandParameter = playthroughName;
        //    Grid.SetColumnSpan(selectButton, 2);
        //    // Add TextBlocks to Grid
        //    grid.Children.Add(textBlock1);
        //    grid.Children.Add(textBlock2);
        //    grid.Children.Add(selectButton);
            
        //    // Create Button
        //    Button closeButton = new Button();
        //    closeButton.Style = (Style)Application.Current.FindResource("PrimaryButtonStyle");
        //    closeButton.Background = (Brush)Application.Current.FindResource("Background");
        //    closeButton.Command = new RelayCommand(DeletePlaythrough); // Assume RelayCommand implementation for ICommand
        //    closeButton.CommandParameter = playthroughName;
        //    closeButton.Content = "X";
        //    closeButton.HorizontalAlignment = HorizontalAlignment.Right;
        //    closeButton.VerticalAlignment = VerticalAlignment.Center;
        //    closeButton.Width = 25;
        //    closeButton.Height = 25;
        //    closeButton.Padding = new Thickness(0, 3, 0, 0);
        //    // Apply custom property
        //    closeButton.SetValue(PrimaryButtonStyleHelper.CornerRadiusProperty, new CornerRadius(12.5));

        //    // Add Grid and Button to DockPanel
        //    dockPanel.Children.Add(grid);
        //    dockPanel.Children.Add(closeButton);
        //    border.Child = dockPanel;
        //    // Set DockPanel as the child of the Border
        //    //PlaythroughList.Children.Add(border);
        //    StackpanelChildren.Add(borderName, border);
        //}
        //public void DeletePlaythrough(object parameter)
        //{
        //    BLViewModel vm = (BLViewModel)this.DataContext;
        //    vm.DeletePlaythrough(parameter.ToString());
        //    PlaythroughList.Children.Remove(StackpanelChildren[FormatBorderName(parameter.ToString())]);
        //}
        //public void SelectPlaythrough(object parameter)
        //{
        //    BLViewModel vm = (BLViewModel)this.DataContext;
        //    vm.SelectPlaythrough(parameter.ToString());
        //    vm.TogglePlaythroughDialogueCommand();
        //}

        //private void UpdatePlaythroughListFlagSetter()
        //{
        //    if (PlaythroughList.Children.Count > 0)
        //    {
        //        PlaythroughList.Children.Clear();
        //        StackpanelChildren.Clear();
        //    }
        //    PopulateChangePlaythroughs();
        //}

        //private static string FormatBorderName(string name)
        //{
        //    return name.Replace(" ", "_").Replace("-", "_");
        //}


    }
}
