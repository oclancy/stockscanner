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
        private StockScannerService.StockScannerServiceClient Client;

        public override string DisplayName
        {
            get { return "Dividends"; }
        }

        public DividendViewModel(IMessenger messenger, StockScannerService.StockScannerServiceClient Client)
            : base(messenger)
        {
            this.Client = Client;

            messenger.Register<Sector>(this, OnSector);
            
        }

        private void OnSector(Sector obj)
        {
            if(Active)
                Client.GetDividends(obj);
        }
    }
}
