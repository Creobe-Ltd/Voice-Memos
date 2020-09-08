using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Data.Models;
using Creobe.VoiceMemos.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Creobe.VoiceMemos.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        #region Private Members

        public ICommand MemoTapCommand { get; private set; }

        #endregion

        #region Commands

        #endregion

        #region Properties

        public ObservableCollection<Memo> Filtered
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_searchText))
                {
                    return new ObservableCollection<Memo>(Uow.MemoRepository.All
                        .Where(m => m.Title.ToLower().Contains(_searchText.ToLower())));
                }

                return Uow.MemoRepository.All;
            }
        }

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                NotifyPropertyChanged("Filtered");
            }
        }

        #endregion

        #region Constructors

        public SearchViewModel()
        {
            InitializeCommands();
        }

        private void OnFilter(object sender, FilterEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchText))
                e.Accepted = (e.Item as Memo).Title.Contains(SearchText);
            else
                e.Accepted = true;
        }

        #endregion

        #region Overrides

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            NotifyPropertyChanged("Filtered");
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            MemoTapCommand = new DelegateCommand(MemoTap);
        }

        #endregion

        #region Command Handlers

        public void MemoTap(object arg)
        {
            var memo = arg as Memo;

            if (memo != null)
            {
                NavigationService.Navigate(ViewLocator.View("Memo", new { Id = memo.Id }));
            }
        }

        #endregion

    }
}
