using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using StockScanner.UI.StockScannerService;

namespace StockScanner.UI
{
    [ServiceBehavior( ConcurrencyMode=ConcurrencyMode.Reentrant)]
    class StockScannerCallbackClient : StockScannerService.IStockScannerServiceCallback
    {
        private ViewModel.StockScannerViewModel m_stockScannerViewModel;
        private IMessenger m_messenger;

        public StockScannerCallbackClient(IMessenger messenger, ViewModel.StockScannerViewModel stockScannerViewModel)
        {
            this.m_stockScannerViewModel = stockScannerViewModel;
            m_messenger = messenger;
        }

        public void PushStockData(StockScannerService.StockQuote result)
        {
            m_messenger.Send<StockQuote>( result );
        }

        public void PushCompanyData(StockScannerService.CompanyStatistics result)
        {
            m_messenger.Send<CompanyStatistics>(result);
        }

        public void PushSectors(StockScannerService.Sector[] sectors)
        {
            m_stockScannerViewModel.SectorData  = new List<StockScannerService.Sector>(sectors);
        }

        public void PushCompanies(StockScannerService.Company[] data)
        {
            m_messenger.Send<Company[]>(data);
        }
    }
}
