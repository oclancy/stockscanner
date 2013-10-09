using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockScanner.UI.ViewModel
{
    public class DividendViewModel : ViewModelBase
    {
        private GalaSoft.MvvmLight.Messaging.IMessenger m_messenger;
        private StockScannerService.StockScannerServiceClient Client;

        public DividendViewModel(GalaSoft.MvvmLight.Messaging.IMessenger m_messenger)
        {
            // TODO: Complete member initialization
            this.m_messenger = m_messenger;
        }

        public DividendViewModel(GalaSoft.MvvmLight.Messaging.IMessenger m_messenger, StockScannerService.StockScannerServiceClient Client)
        {
            // TODO: Complete member initialization
            this.m_messenger = m_messenger;
            this.Client = Client;
        }
    }
}
