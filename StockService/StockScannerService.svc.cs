using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using NLog;
using StockService.Core;

namespace StockService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Reentrant)]
    public class StockScannerService : IStockScannerService
    {
        IStockScannerClient m_callback = null;

        Logger m_logger = LogManager.GetCurrentClassLogger();

        public StockScannerService()
        {
            m_callback = OperationContext.Current.GetCallbackChannel<IStockScannerClient>();
        }

        public async void GetCompanyData(string symbol)
        {
            string.Format("Getting company data for: {0}", symbol);

            CompanyStatistics data = await YahooCompanyDataProvider.FetchDataAsync(symbol);
            m_callback.PushCompanyData(data);
        }

        public async void GetStockData(string symbol)
        {
            string.Format("Getting stock data for: {0}", symbol);

            StockQuote data = await YahooStockDataProvider.FetchDataAsync(symbol);
            m_callback.PushStockData(data);
        }

        public async void GetSetorData()
        {
            var data = await SectorIndustryDataProvider.FetchDataAsync();
            m_callback.PushSectors(data);
        }
    }
}
