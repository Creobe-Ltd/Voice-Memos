using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Data.Models;
using Creobe.VoiceMemos.Models;
using Microsoft.Phone.Maps.Controls;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Creobe.VoiceMemos.ViewModels
{
    public class ListMapViewModel : ViewModelBase
    {
        #region Private Members

        private Tag _tag;

        #endregion

        #region Commands

        public ICommand MemoTapCommand { get; private set; }
        public ICommand MapLoadedCommand { get; private set; }

        #endregion

        #region Properties

        private ObservableCollection<Memo> _all;

        public ObservableCollection<Memo> All
        {
            get { return _all; }
            set
            {
                _all = value;
                NotifyPropertyChanged("All");
            }
        }

        private LocationRectangle _mapBound;

        public LocationRectangle MapBound
        {
            get { return _mapBound; }
            set
            {
                _mapBound = value;
                NotifyPropertyChanged("MapBound");
            }
        }


        #endregion

        #region Constructors

        public ListMapViewModel()
        {
            InitializeCommands();
            InitializeAppBar();
        }

        #endregion

        #region Overrides

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Filter"))
            {
                string filter = NavigationContext.QueryString["Filter"];

                switch (filter)
                {
                    case "Tag":
                        if (NavigationContext.QueryString.ContainsKey("Id"))
                        {
                            int tagId = int.Parse(NavigationContext.QueryString["Id"]);
                            _tag = Uow.TagRepository.Find(tagId);


                            All = new ObservableCollection<Memo>(Uow.MemoRepository.ByTagWithLocation(tagId));
                        }
                        break;
                }
            }
            else
            {
                All = Uow.MemoRepository.AllWithLocation;
            }

        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            MemoTapCommand = new DelegateCommand(MemoTap);
            MapLoadedCommand = new DelegateCommand(MapLoaded);
        }

        private void InitializeAppBar()
        {
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

        private void MapLoaded(object arg)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "3372500c-0f7b-4117-b085-3977a972844f";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "ybxfo_GnI9Y231Eykykl0A";

            if (All.Count > 0)
            {
                MapBound = LocationRectangle.CreateBoundingRectangle(
                    All.Select(m => new GeoCoordinate(m.Latitude.Value, m.Longitude.Value)));
            }
        }

        #endregion
    }
}
