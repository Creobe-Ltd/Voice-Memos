using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Input;

namespace Creobe.VoiceMemos.Controls
{
    public class BindableApplicationBarMenuItem : FrameworkElement, IApplicationBarMenuItem
    {
        public ApplicationBarMenuItem MenuItem { get; private set; }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(BindableApplicationBarMenuItem), new PropertyMetadata(null));



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BindableApplicationBarMenuItem), new PropertyMetadata("text", TextChanged));



        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(BindableApplicationBarMenuItem), new PropertyMetadata(true, IsEnabledChanged));


        public BindableApplicationBarMenuItem()
        {
            MenuItem = new ApplicationBarMenuItem();
            MenuItem.Text = "text";
            MenuItem.Click += MenuItem_Click;
        }

        void MenuItem_Click(object sender, EventArgs e)
        {
            if (Command != null)
                Command.Execute(null);

            if (Click != null)
                Click(sender, e);
        }

        private static void TextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (obj as BindableApplicationBarMenuItem).MenuItem.Text = e.NewValue.ToString();
            }
        }

        private static void IsEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (obj as BindableApplicationBarMenuItem).MenuItem.IsEnabled = (bool)e.NewValue;
            }
        }

        public event EventHandler Click;
    }
}
