using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace StockScanner.UI
{
    [ServiceBehavior( ConcurrencyMode=ConcurrencyMode.Reentrant)]
    class StockScannerCallbackClient : StockScannerService.IStockScannerServiceCallback
    {
        private ViewModel.StockScannerViewModel m_stockScannerViewModel;

        public StockScannerCallbackClient(ViewModel.StockScannerViewModel stockScannerViewModel)
        {
            this.m_stockScannerViewModel = stockScannerViewModel;
        }


        public void PushStockData(StockScannerService.StockQuote result)
        {
            m_stockScannerViewModel.StockData = result;
        }

        public void PushCompanyData(StockScannerService.CompanyStatistics result)
        {
            m_stockScannerViewModel.CompanyData = result;
        }
    }
}
