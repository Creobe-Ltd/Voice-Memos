using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Creobe.VoiceMemos.Controls
{
    [ContentProperty("Buttons")]
    public class BindableApplicationBar : ItemsControl, IApplicationBar
    {
        #region Private Members

        private IApplicationBar _applicationBar;

        #endregion

        #region Properties

        public static readonly DependencyProperty ApplicationBarProperty =
            DependencyProperty.RegisterAttached("ApplicationBar", typeof(BindableApplicationBar), typeof(BindableApplicationBar), new PropertyMetadata(null, ApplicationBarChanged));

        public static BindableApplicationBar GetApplicationBar(PhoneApplicationPage obj)
        {
            return (BindableApplicationBar)obj.GetValue(ApplicationBarProperty);
        }

        public static void SetApplicationBar(PhoneApplicationPage obj, BindableApplicationBar value)
        {
            obj.SetValue(ApplicationBarProperty, value);
        }

        private static void ApplicationBarChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var page = obj as PhoneApplicationPage;

                if (page != null)
                {
                    BindableApplicationBar applicationBar = e.NewValue as BindableApplicationBar;
                    applicationBar.DataContext = page.DataContext;
                    page.ApplicationBar = applicationBar._applicationBar;

                }
            }
        }

        #endregion

        #region Constructors

        public BindableApplicationBar()
        {
            _applicationBar = new ApplicationBar();
            _applicationBar.StateChanged += _applicationBar_StateChanged;
        }

        #endregion

        #region Overrides

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            Invalidate();
        }

        #endregion

        #region Private Methods

        private void _applicationBar_StateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {
            if (StateChanged != null)
                StateChanged(this, e);
        }

        private void Invalidate()
        {
            _applicationBar.Buttons.Clear();
            _applicationBar.MenuItems.Clear();

            foreach (BindableApplicationBarIconButton item in Items.Where(i => i is BindableApplicationBarIconButton))
            {
                _applicationBar.Buttons.Add(item.Button);
            }

            foreach (BindableApplicationBarMenuItem item in Items.Where(i => i is BindableApplicationBarMenuItem))
            {
                _applicationBar.MenuItems.Add(item.MenuItem);
            }
        }

        #endregion

        #region IApplicationBar Members

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(BindableApplicationBar), new PropertyMetadata(true, IsVisibleChanged));

        private static void IsVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (obj as BindableApplicationBar)._applicationBar.IsVisible = (bool)e.NewValue;
            }
        }

        public Color BackgroundColor
        {
            get { return _applicationBar.BackgroundColor; }
            set { _applicationBar.BackgroundColor = value; }
        }

        public IList Buttons
        {
            get { return Items; }
        }

        public double DefaultSize
        {
            get { return _applicationBar.DefaultSize; }
        }

        public Color ForegroundColor
        {
            get { return _applicationBar.ForegroundColor; }
            set { _applicationBar.ForegroundColor = value; }
        }

        public bool IsMenuEnabled
        {
            get { return _applicationBar.IsMenuEnabled; }
            set { _applicationBar.IsMenuEnabled = value; }
        }

        public IList MenuItems
        {
            get { return Items; }
        }

        public double MiniSize
        {
            get { return _applicationBar.MiniSize; }
        }

        public ApplicationBarMode Mode
        {
            get { return _applicationBar.Mode; }
            set { _applicationBar.Mode = value; }
        }

        public new double Opacity
        {
            get { return _applicationBar.Opacity; }
            set { _applicationBar.Opacity = value; }
        }

        public event EventHandler<ApplicationBarStateChangedEventArgs> StateChanged;

        #endregion
    }
}
