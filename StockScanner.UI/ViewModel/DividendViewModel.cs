using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using StockScanner.UI.StockScannerService;

namespace StockScanner.UI.ViewModel
{
    public class DividendViewModel : ViewModelBase
    {
        private GalaSoft.MvvmLight.Messaging.IMessenger m_messenger;
        private StockScannerService.StockScannerServiceClient Client;

        private void OnSector(Sector obj)
        {
            Client.GetDividends(obj);
        }

        public DividendViewModel(IMessenger m_messenger, StockScannerService.StockScannerServiceClient Client)
        {
            this.m_messenger = m_messenger;
            this.Client = Client;

            this.m_messenger = m_messenger;

            m_messenger.Register<Sector>(this, OnSector);
        }
    }
}
