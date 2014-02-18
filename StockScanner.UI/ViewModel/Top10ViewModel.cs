using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using StockScanner.UI.StockScannerService;

namespace StockScanner.UI.ViewModel
{
    public class DividendViewModel : Top10ViewModel 
    {
        public DividendViewModel(IMessenger messenger, StockScannerService.StockScannerServiceClient Client)
            : base(messenger, Client)
        {
        }

        protected override async Task GetData(Sector obj)
        {
            Dividends = await Client.GetDividendsAsync(obj);
        }

        public override string DisplayName { get { return "Dividends"; } }
    }

    public class VolumesViewModel : Top10ViewModel 
    {
        public VolumesViewModel(IMessenger messenger, StockScannerService.StockScannerServiceClient Client)
            : base(messenger, Client)
        {
        }
        
        protected override async Task GetData(Sector obj)
        {
            Dividends = await Client.GetVolumesAsync(obj);
        }

        public override string DisplayName { get { return "Volumes"; } }
    }

    public abstract class Top10ViewModel : ViewModelBase
    {
        protected StockScannerService.StockScannerServiceClient Client;

        protected Sector m_sector;

        IEnumerable<DataTable> m_dividends;
        public IEnumerable<DataTable> Dividends
        {
            get
            {
                return m_dividends;
            }
            set 
            {
                m_dividends = value;
                OnPropertyChanged();
            }
        }

        public Top10ViewModel(IMessenger messenger, StockScannerService.StockScannerServiceClient Client)
            : base(messenger)
        {
            this.Client = Client;

            Activated += async (o, e) => { if (m_sector != null) await GetData(m_sector); };

            messenger.Register<Sector>(this, OnSector);
        }

        private void OnSector(Sector obj)
        {
            m_sector = obj;
            if (Active)
                GetData(obj);
        }

        protected abstract Task GetData(Sector obj);
    }
}
