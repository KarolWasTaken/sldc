using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace sldc.Themes.ElementThemes
{
    public class ModernScrollBarBackgroundHelper : DependencyObject
    {
        // Define a DependencyProperty for the ScrollBar background.
        public static readonly DependencyProperty ScrollBarThumbBackgroundProperty =
            DependencyProperty.RegisterAttached(
                "ScrollBarThumbBackground",
                typeof(Brush),
                typeof(ModernScrollBarBackgroundHelper),
                new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits)
            );

        public static void SetScrollBarThumbBackground(UIElement element, Brush value)
        {
            element.SetValue(ScrollBarThumbBackgroundProperty, value);
        }

        public static Brush GetScrollBarThumbBackground(UIElement element)
        {
            return (Brush)element.GetValue(ScrollBarThumbBackgroundProperty);
        }
    }
}
