using Microsoft.Phone.Maps.Controls;
using System.Collections;
using System.Linq;
using System.Windows;

namespace Creobe.VoiceMemos.Controls
{
    public class MapExtensions
    {
        public static readonly DependencyProperty BoundProperty =
            DependencyProperty.RegisterAttached("Bound", typeof(LocationRectangle), typeof(MapExtensions), new PropertyMetadata(null, BoundChanged));

        public static LocationRectangle GetBound(Map obj)
        {
            return (LocationRectangle)obj.GetValue(BoundProperty);
        }

        public static void SetBound(Map obj, LocationRectangle value)
        {
            obj.SetValue(BoundProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached("ItemsSource", typeof(IEnumerable), typeof(MapExtensions), new PropertyMetadata(ItemsSourceChanged));

        public static IEnumerable GetItemsSource(Map obj)
        {
            return (IEnumerable)obj.GetValue(ItemsSourceProperty);
        }

        public static void SetItemsSource(Map obj, IEnumerable value)
        {
            obj.SetValue(ItemsSourceProperty, value);
        }


        private static void BoundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var map = obj as Map;

                if (map != null)
                {
                    map.SetView((LocationRectangle)e.NewValue);
                }
            }
        }

        private static void ItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var map = obj as Map;

                if (map != null)
                {
                    var itemsControl = Microsoft.Phone.Maps.Toolkit.MapExtensions.GetChildren(map)
                        .OfType<Microsoft.Phone.Maps.Toolkit.MapItemsControl>()
                        .FirstOrDefault();

                    if (itemsControl != null)
                    {

                        if (itemsControl.ItemsSource == null)
                            itemsControl.ItemsSource = (IEnumerable)e.NewValue;
                    }
                }
            }
        }

    }
}
