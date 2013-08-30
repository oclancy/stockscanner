using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using NLog;
using StockService.Core;
using StockService.Core.Providers;

namespace StockService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Reentrant)]
    public class StockScannerService : IStockScannerService
    {
        IStockScannerClient m_callback = null;

        Logger m_logger = LogManager.GetCurrentClassLogger();

        DataProviderFactory m_dataProviderFactory = new DataProviderFactory(HostingEnvironment.ApplicationPhysicalPath);

        public StockScannerService()
        {
            m_callback = OperationContext.Current.GetCallbackChannel<IStockScannerClient>();
        }

        public async void GetCompanyData(int market, string symbol)
        {
            string.Format("Getting company data for: {0}", symbol);

            CompanyStatistics data = await m_dataProviderFactory.GetDataProvider<ICompanyDataProvider>(market).FetchDataAsync(symbol);
            m_callback.PushCompanyData(data);
        }

        public async void GetStockData(int market,string symbol)
        {
            string.Format("Getting stock data for: {0}", symbol);

            StockQuote data = await m_dataProviderFactory.GetDataProvider<IStockProvider>(market).FetchDataAsync(symbol);
            m_callback.PushStockData(data);
        }

        public async void GetSectorData(int market)
        {
            var data = await m_dataProviderFactory.GetDataProvider<ISectorDataProvider>(market).FetchDataAsync();
            m_callback.PushSectors(data);
        }

        public async void GetCompanies(int market, int industry)
        {
            var data = await m_dataProviderFactory.GetDataProvider<ICompanyProvider>(market).FetchDataAsync(industry);
            m_callback.PushCompanies(data);
        }

        public List<Market> GetMarketsData()
        {
            return m_dataProviderFactory.GetMarketsData();
        }
    }
}
