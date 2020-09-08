using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Input;

namespace Creobe.VoiceMemos.Controls
{
    
    public class BindableApplicationBarIconButton : FrameworkElement, IApplicationBarIconButton, IApplicationBarMenuItem
    {
        public ApplicationBarIconButton Button { get; private set; }

        public int Index { get; set; }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(BindableApplicationBarIconButton), new PropertyMetadata(null));


        public Uri IconUri
        {
            get { return (Uri)GetValue(IconUriProperty); }
            set { SetValue(IconUriProperty, value); }
        }

        public static readonly DependencyProperty IconUriProperty =
            DependencyProperty.Register("IconUri", typeof(Uri), typeof(BindableApplicationBarIconButton), new PropertyMetadata(new Uri("/", UriKind.Relative), IconUriChanged));


        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(BindableApplicationBarIconButton), new PropertyMetadata(true, IsEnabledChanged));


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BindableApplicationBarIconButton), new PropertyMetadata("text", TextChanged));


        public BindableApplicationBarIconButton()
        {
            Button = new ApplicationBarIconButton();
            Button.Text = "text";
            Button.IconUri = new Uri("/", UriKind.Relative);
            Button.Click += IconButton_Click;
        }

        void IconButton_Click(object sender, EventArgs e)
        {
            if (Command != null)
                Command.Execute(null);

            if (Click != null)
                Click(sender, e);
        }

        private static void IsEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (obj as BindableApplicationBarIconButton).Button.IsEnabled = (bool)e.NewValue;
            }
        }

        private static void TextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (obj as BindableApplicationBarIconButton).Button.Text = e.NewValue.ToString();
            }
        }

        private static void IconUriChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (obj as BindableApplicationBarIconButton).Button.IconUri = (Uri)e.NewValue;
            }
        }

        public event EventHandler Click;
    }
}
