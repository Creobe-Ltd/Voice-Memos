using Creobe.VoiceMemos.ViewModels;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace Creobe.VoiceMemos
{
    public class ViewBase : PhoneApplicationPage
    {
        private ViewModelBase ViewModel
        {
            get
            {
                return DataContext as ViewModelBase;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (ViewModel != null)
                ViewModel.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (ViewModel != null)
                ViewModel.OnNavigatedFrom(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (ViewModel != null)
                ViewModel.OnBackKeyPress(e);
        }
    }
}
