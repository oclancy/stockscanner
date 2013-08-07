using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace StockScanner.UI.ViewModel
{
    class StockScannerViewModel : INotifyPropertyChanged
    {
        Lazy<StockScannerService.StockScannerServiceClient> m_lazyLoadClient;

        public ICommand GetData { get; private set; }

        public string m_data;

        public StockScannerViewModel()
        {
            m_lazyLoadClient = new Lazy<StockScannerService.StockScannerServiceClient>(() =>
            {
                var instanceContext = new InstanceContext(new StockScannerCallbackClient(this));
                return new StockScannerService.StockScannerServiceClient(instanceContext);
            });

            GetData = new RelayCommand(() => { Client.GetStockData("YHOO"); });
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


        public StockScannerService.StockQuote StockData { get; set; }

        public StockScannerService.CompanyStatistics CompanyData { get; set; }
    }
}
