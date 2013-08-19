using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using StockScanner.UI.StockScannerService;

namespace StockScanner.UI.ViewModel
{
    class StockScannerViewModel : INotifyPropertyChanged
    {
        Lazy<StockScannerService.StockScannerServiceClient> m_lazyLoadClient;

        public ICommand Initialise { get; private set; }

        public ICommand IndustryChangedCommand { get; private set; }

        public ICommand MarketChangedCommand { get; private set; }

        public Market SelectedMarket { get; set; }

        public Sector SelectedSector { get; set; }

        Company m_selectedCompany;
        public Company SelectedCompany 
        {
            get
            {
                return m_selectedCompany;
            }
            set
            {
                m_selectedCompany = value;
                Client.GetCompanyData(SelectedMarket.Id, m_selectedCompany.Symbol);
            }
        }

        public StockScannerViewModel()
        {
            m_lazyLoadClient = new Lazy<StockScannerService.StockScannerServiceClient>(() =>
            {
                var instanceContext = new InstanceContext(new StockScannerCallbackClient(this));
                return new StockScannerService.StockScannerServiceClient(instanceContext);
            });

            Initialise = new RelayCommand(()=> {
                MarketData = new List<Market>(Client.GetMarketsData());
                SelectedMarket = MarketData.FirstOrDefault();
            });

            IndustryChangedCommand = new RelayCommand<SelectionChangedEventArgs>((e) =>
            {
                var industry = e.AddedItems.Cast<StockScannerService.Industry>().First();

                Client.GetCompanies( SelectedMarket.Id, industry.Id );
            });

            MarketChangedCommand = new RelayCommand<SelectionChangedEventArgs>((e) =>
            {
                var market = e.AddedItems.Cast<StockScannerService.Market>().First();

                Client.GetSectorData(market.Id);
            });
        }

        public StockScannerService.StockScannerServiceClient Client
        {
            get
            {
                return m_lazyLoadClient.Value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string property ="" )
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }


        StockScannerService.StockQuote m_stockData;
        public StockScannerService.StockQuote StockData 
        {
            get
            {
                return m_stockData;
            }
            set
            {
                m_stockData = value;
                OnPropertyChanged();
            }
        }

        private StockScannerService.CompanyStatistics m_companyData;
        public StockScannerService.CompanyStatistics CompanyData 
        {
            get
            { 
                return m_companyData; 
            }
            set
            {
                m_companyData = value;
                OnPropertyChanged();
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

        private List<StockScannerService.Company> m_companies;
        public List<StockScannerService.Company> Companies
        {
            get { return m_companies; }
            set
            {
                m_companies = value;
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
    }
}
