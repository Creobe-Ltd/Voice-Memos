using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Creobe.VoiceMemos.Controls
{
    public class InputExtensions
    {

        public static readonly DependencyProperty UpdateSourceOnChangeProperty =
            DependencyProperty.RegisterAttached("UpdateSourceOnChange", typeof(bool), typeof(InputExtensions), new PropertyMetadata(false, UpdateSourceOnChangeChanged));

        public static bool GetUpdateSourceOnChange(TextBox obj)
        {
            return (bool)obj.GetValue(UpdateSourceOnChangeProperty);
        }

        public static void SetUpdateSourceOnChange(TextBox obj, bool value)
        {
            obj.SetValue(UpdateSourceOnChangeProperty, value);
        }

        private static void UpdateSourceOnChangeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var textBox = obj as TextBox;

                if (textBox != null)
                {
                    if ((bool)e.NewValue)
                        textBox.TextChanged += textBox_TextChanged;
                    else
                        textBox.TextChanged -= textBox_TextChanged;
                }
            }
        }

        static void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BindingExpression expression = (sender as TextBox).GetBindingExpression(TextBox.TextProperty);

            if (expression != null)
                expression.UpdateSource();
        }
    }
}
