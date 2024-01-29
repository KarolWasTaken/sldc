using sldc.Model;
using sldc.Themes.ElementThemes;
using sldc.ViewModel;
using sldc.ViewModel.PlaythroughSubViewModels;
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

namespace sldc.View.PlaythroughSubViews
{
    /// <summary>
    /// Interaction logic for SelectPlaythroughView.xaml
    /// </summary>
    public partial class SelectPlaythroughView : UserControl
    {
        private Dictionary<string, UIElement> StackpanelChildren = new Dictionary<string, UIElement>();
        private SelectPlaythroughViewModel _spvm;
        public SelectPlaythroughView()
        {
            InitializeComponent();
            Loaded += SelectPlaythroughView_Loaded;
        }

        private void SelectPlaythroughView_Loaded(object sender, RoutedEventArgs e)
        {
            SelectPlaythroughViewModel spvm = (SelectPlaythroughViewModel)this.DataContext;
            PopulateChangePlaythroughs(spvm.PlaythroughsToShow);
            _spvm = spvm;
        }

        public void PopulateChangePlaythroughs(GameDeathDataSerialiser.NON_HOOKABLE_GAME game)
        {
            // create entries for playthroughs
            Dictionary<string, int> playthroughs = GameDeathDataSerialiser.LoadData(game);
            foreach (var kvp in playthroughs)
            {
                CreateEntry(kvp.Key, kvp.Value);
            }
            // Sorting keys alphabetically
            var sortedKeys = StackpanelChildren.Keys.OrderBy(key => key);
            // Creating a new sorted dictionary
            var sortedDictionary = sortedKeys.ToDictionary(key => key, key => StackpanelChildren[key]);

            foreach (var kvp in sortedDictionary)
            {
                PlaythroughList.Children.Add(sortedDictionary[kvp.Key]);
            }

        }

        public void CreateEntry(string playthroughName, int playthroughDeaths)
        {
            // Set Border properties
            Border border = new Border
            {
                Background = (Brush)Application.Current.FindResource("Background_Level_3"),
                Height = 30,
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(0, 0, 0, 10)
            };
            string borderName = FormatBorderName(playthroughName);
            border.Name = borderName;

            // Create DockPanel
            DockPanel dockPanel = new DockPanel();
            dockPanel.Margin = new Thickness(10, 0, 0, 0);

            // Create Grid
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(280) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(235) });

            // Create first TextBlock
            TextBlock textBlock1 = new TextBlock
            {
                Text = playthroughName,
                FontFamily = (FontFamily)Application.Current.FindResource("Inter"),
                FontWeight = FontWeights.Regular,
                FontSize = 15,
                Foreground = (Brush)Application.Current.FindResource("TextColour"),
                Opacity = 0.8,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Create second TextBlock
            TextBlock textBlock2 = new TextBlock
            {
                Text = playthroughDeaths.ToString(),
                FontFamily = (FontFamily)Application.Current.FindResource("Inter"),
                FontWeight = FontWeights.Regular,
                FontSize = 15,
                Foreground = (Brush)Application.Current.FindResource("TextColour"),
                Opacity = 0.8,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(textBlock2, 1);
            // create select button
            Button selectButton = new Button
            {
                Width = 520,
                Height = 25,
                Visibility = Visibility.Visible,
                Opacity = 0,
                Cursor = Cursors.Hand,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Command = new RelayCommand(SelectPlaythrough),
                CommandParameter = playthroughName
            };
            Grid.SetColumnSpan(selectButton, 2);
            // Add TextBlocks to Grid
            grid.Children.Add(textBlock1);
            grid.Children.Add(textBlock2);
            grid.Children.Add(selectButton);

            // Create Button
            Button closeButton = new Button
            {
                Style = (Style)Application.Current.FindResource("PrimaryButtonStyle"),
                Background = (Brush)Application.Current.FindResource("Background"),
                Command = new RelayCommand(DeletePlaythrough), // Assume RelayCommand implementation for ICommand
                CommandParameter = playthroughName,
                Content = "X",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 25,
                Height = 25,
                Padding = new Thickness(0, 3, 0, 0)
            };
            // Apply custom property
            closeButton.SetValue(PrimaryButtonStyleHelper.CornerRadiusProperty, new CornerRadius(12.5));

            // Add Grid and Button to DockPanel
            dockPanel.Children.Add(grid);
            dockPanel.Children.Add(closeButton);
            border.Child = dockPanel;
            // Set DockPanel as the child of the Border
            //PlaythroughList.Children.Add(border);
            StackpanelChildren.Add(borderName, border);
        }

        public void DeletePlaythrough(object parameter)
        {
            PlaythroughList.Children.Remove(StackpanelChildren[FormatBorderName(parameter.ToString())]);
            _spvm.DeletePlaythrough(parameter.ToString());
            PlaythroughList.Children.Remove(StackpanelChildren[FormatBorderName(parameter.ToString())]);
        }
        public void SelectPlaythrough(object parameter)
        {
            _spvm.SelectPlaythrough(parameter.ToString());
            _spvm.CloseDialogueCommand();
        }
        private static string FormatBorderName(string name)
        {
            return name.Replace(" ", "_").Replace("-", "_");
        }
    }
}
