using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StockScanner.UI.Messages;
using StockScanner.UI.StockScannerService;

namespace StockScanner.UI.ViewModel
{
    class StockScannerViewModel : ViewModelBase
    {
        IMessenger m_messenger = new Messenger();

        Lazy<StockScannerService.StockScannerServiceClient> m_lazyLoadClient;

        public ICommand Initialise { get; private set; }

        public ICommand IndustryChangedCommand { get; private set; }

        public ICommand MarketChangedCommand { get; private set; }

        public ICommand ViewChangedCommand { get; private set; }

        public IEnumerable<string> ViewNames { get { return m_views.Select(vm => vm.DisplayName); } }

        private IList<ViewModelBase> m_views = new List<ViewModelBase>();

        Market m_selectedMarket;
        public Market SelectedMarket
        {
            get
            {
                return m_selectedMarket;
            }
            set
            {
                m_selectedMarket = value;
                OnPropertyChanged();
            }
        }


        public INotifyPropertyChanged View
        {
            get;
            set;
        }

        string m_selectedViewName;
        public string SelectedViewName
        {
            get
            {
                return m_selectedViewName;
            }
            set
            {
                m_selectedViewName = value;
                View = m_views.First( vm => vm.DisplayName == value);
                OnPropertyChanged("View");
            }

        }

        public StockScannerViewModel()
            :base(null)
        {
            m_lazyLoadClient = new Lazy<StockScannerService.StockScannerServiceClient>(() =>
            {
                var instanceContext = new InstanceContext(new StockScannerCallbackClient(m_messenger, this));
                return new StockScannerService.StockScannerServiceClient(instanceContext);
            });

            Initialise = new RelayCommand(()=> {
                MarketData = new List<Market>(Client.GetMarketsData());
                Client.GetSectorData(MarketData.First());
            });

            IndustryChangedCommand = new RelayCommand<SelectionChangedEventArgs>((e) =>
            {
                var industry = e.AddedItems.Cast<StockScannerService.Industry>().FirstOrDefault();

                if (industry == null) return;

                m_messenger.Send<Industry>(industry);
            });
            
            SectorChangedCommand = new RelayCommand<SelectionChangedEventArgs>((e) =>
            {
                var sector = e.AddedItems.Cast<StockScannerService.Sector>().FirstOrDefault();

                if (sector == null) return;

                m_messenger.Send<Sector>(sector);
            });

            MarketChangedCommand = new RelayCommand<SelectionChangedEventArgs>((e) =>
            {
                var market = e.AddedItems.Cast<StockScannerService.Market>().First();

                Client.GetSectorData(market);
            });

            ViewChangedCommand = new RelayCommand<SelectionChangedEventArgs>((e) =>
            {
                var viewModel = m_views.First(vm=> vm.DisplayName == e.AddedItems.Cast<string>().First());

                m_messenger.Send<ViewModelBase>(viewModel);
            });

            m_views.Add(new DetailsViewModel(m_messenger, Client));
            m_views.Add(new DividendViewModel(m_messenger, Client));
            m_views.Add(new VolumesViewModel(m_messenger, Client));

            SelectedViewName = m_views.First().DisplayName;
        }

        public StockScannerService.StockScannerServiceClient Client
        {
            get
            {
                return m_lazyLoadClient.Value;
            }
        }

        private List<StockScannerService.Sector> m_sectorData;
        public List<StockScannerService.Sector> SectorData
        {
            get { return m_sectorData; }
            set 
            {
                m_sectorData = value;
                OnPropertyChanged();
            }
        }

        private List<Market> m_marketData;
        public List<Market> MarketData
        {
            get { return m_marketData; }
            set 
            { 
                m_marketData = value;
                OnPropertyChanged();
            }
        }

        public override string DisplayName
        {
            get { throw new NotImplementedException(); }
        }

        public RelayCommand<SelectionChangedEventArgs> SectorChangedCommand { get; set; }
    }
}
