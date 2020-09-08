using Microsoft.Phone.Controls;
using System.Windows;

namespace Creobe.VoiceMemos.Controls
{
    public class PivotExtensions
    {

        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.RegisterAttached("IsLocked", typeof(bool), typeof(PivotExtensions), new PropertyMetadata(false, IsLockedChanged));

        public static bool GetIsLocked(Pivot obj)
        {
            return (bool)obj.GetValue(IsLockedProperty);
        }

        public static void SetIsLocked(Pivot obj, bool value)
        {
            obj.SetValue(IsLockedProperty, value);
        }

        private static void IsLockedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var pivot = obj as Pivot;

                if (pivot != null)
                {
                    pivot.IsLocked = (bool)e.NewValue;
                }
            }
        }
    }
}
