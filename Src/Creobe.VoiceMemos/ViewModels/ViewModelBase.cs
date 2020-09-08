using Coding4Fun.Toolkit.Controls;
using Creobe.VoiceMemos.Data;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Locators;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.GamerServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Creobe.VoiceMemos.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected VoiceMemosUow Uow
        {
            get
            {
                return App.Uow;
            }
        }

        protected ViewLocator ViewLocator
        {
            get
            {
                return App.ViewLocator;
            }
        }

        protected NavigationContext NavigationContext
        {
            get
            {
                return (App.RootFrame.Content as PhoneApplicationPage).NavigationContext;
            }
        }

        protected NavigationService NavigationService
        {
            get
            {
                return (App.RootFrame.Content as PhoneApplicationPage).NavigationService;
            }
        }

        protected PhoneApplicationPage Page
        {
            get
            {
                return (App.RootFrame.Content as PhoneApplicationPage);
            }
        }

        private bool _reset;

        protected bool Reset
        {
            get
            {
                return _reset;
            }
        }

        private bool _isAppBarVisible;

        public virtual bool IsAppBarVisible
        {
            get { return _isAppBarVisible; }
            set
            {
                _isAppBarVisible = value;
                NotifyPropertyChanged("IsAppBarVisible");
            }
        }

        public virtual bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public virtual bool IsBeta
        {
            get
            {
#if BETA
                return true;
#else
                return false;
#endif
            }
        }

        public virtual bool ShowAds
        {
            get { return !LicenseHelper.IsPremium; }
        }


        private ObservableCollection<IApplicationBarMenuItem> _appBarItems;

        public virtual ObservableCollection<IApplicationBarMenuItem> AppBarItems
        {
            get { return _appBarItems; }
            set
            {
                _appBarItems = value;
                NotifyPropertyChanged("AppBarItems");
            }
        }

        public ViewModelBase()
        {
            _appBarItems = new ObservableCollection<IApplicationBarMenuItem>();
            _isAppBarVisible = true;

            App.RootFrame.Obscured += OnObscured;
            App.RootFrame.Unobscured += OnUnobscured;
            App.RootFrame.Navigating += OnNavigating;
            App.RootFrame.Navigated += OnNavigated;
        }

        public virtual void OnUnobscured(object sender, EventArgs e) { }

        public virtual void OnObscured(object sender, ObscuredEventArgs e) { }

        public virtual void OnNavigatedTo(NavigationEventArgs e) { }

        public virtual void OnNavigatedFrom(NavigationEventArgs e) { }

        public virtual void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Reset)
                _reset = true;
        }

        public virtual void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (_reset && e.IsCancelable && e.Uri == ViewLocator.View("Main"))
            {
                e.Cancel = true;
                _reset = false;
            }
        }

        public virtual void OnBackKeyPress(System.ComponentModel.CancelEventArgs e) { }

        public virtual void AddAppBarItem(IApplicationBarMenuItem item)
        {
            if (!AppBarItems.Contains(item))
                AppBarItems.Add(item);
        }

        public virtual void RemoveAppBarItem(IApplicationBarMenuItem item)
        {
            if (AppBarItems.Contains(item))
                AppBarItems.Remove(item);
        }

        protected void BeginInvoke(Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }

        protected MessageBoxResult ShowMessage(string message)
        {
            return MessageBox.Show(message);
        }

        protected MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button)
        {
            return MessageBox.Show(message, caption, button);
        }

        protected string ShowMessage(string message, string caption, IEnumerable<string> buttons)
        {
            var result = Guide.BeginShowMessageBox(caption, message, buttons, 0, MessageBoxIcon.None, null, null);

            result.AsyncWaitHandle.WaitOne();

            var choice = Guide.EndShowMessageBox(result);

            if (choice.HasValue)
                return buttons.ToArray()[choice.Value];

            return null;
        }

        protected async Task<MessageBoxResult> ShowMessageAsync(string message)
        {
            return await ShowMessageAsync(message, null, MessageBoxButton.OK);
        }

        protected async Task<MessageBoxResult> ShowMessageAsync(string message, string caption, MessageBoxButton button)
        {
            string[] buttons;

            if (button == MessageBoxButton.OKCancel)
                buttons = new string[] { "ok", "cancel" };
            else
                buttons = new string[] { "ok" };

            var result = await ShowMessageAsync(message,caption, buttons);

            if (result != null)
            {
                if (button == MessageBoxButton.OKCancel)
                    return result.Equals("ok") ? MessageBoxResult.OK : MessageBoxResult.Cancel;
                else
                    return MessageBoxResult.OK;
            }

            return MessageBoxResult.None;
        }

        protected async Task<string> ShowMessageAsync(string message, string caption, IEnumerable<string> buttons)
        {
            return await Task.Run<string>(() =>
            {
                return ShowMessage(message, caption, buttons);
            });
        }

        protected Task<string> ShowCustomMessageAsync(string message, string caption, string leftButtonText = "", string rightButtonText = "ok", object content = null, bool isFullScreen = true)
        {
            var completion = new TaskCompletionSource<string>();
            EventHandler<DismissedEventArgs> dismissed = null;

            double currentSystemTrayOpacity = SystemTray.Opacity;

            if (currentSystemTrayOpacity >= 0)
                SystemTray.Opacity = 0.999;

            CustomMessageBox msgBox = new CustomMessageBox();
            msgBox.Caption = caption;
            msgBox.Message = message;
            msgBox.Content = content;
            msgBox.IsFullScreen = isFullScreen;
            msgBox.LeftButtonContent = leftButtonText;
            msgBox.RightButtonContent = rightButtonText;

            dismissed = (sender, e) =>
            {
                switch (e.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        completion.SetResult(leftButtonText);
                        break;
                    case CustomMessageBoxResult.RightButton:
                        completion.SetResult(rightButtonText);
                        break;
                    case CustomMessageBoxResult.None:
                        completion.SetResult(string.Empty);
                        break;
                }

                SystemTray.Opacity = currentSystemTrayOpacity;
                msgBox.Dismissed -= dismissed;
                msgBox = null;
            };

            msgBox.Dismissed += dismissed;
            msgBox.Show();

            return completion.Task;
        }

        protected void ShowToast(string title, string message)
        {
            BeginInvoke(() =>
                {
                    ToastPrompt toast = new ToastPrompt();
                    toast.Title = title;
                    toast.Message = message;
                    toast.Show();
                });
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
