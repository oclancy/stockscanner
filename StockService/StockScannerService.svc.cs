using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using Microsoft.Practices.Unity;
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

        [Dependency]
        public ICalculatedCompanyDataProvider CalculatedStaticticsProvider { get; set; }

        [Dependency]
        public DataProviderFactory DataProviderFactory { get; set; }

        public StockScannerService()
        {
            m_callback = OperationContext.Current.GetCallbackChannel<IStockScannerClient>();
        }

        public async void GetCompanyData(Company company)
        {
            string.Format("Getting company data for: {0}", company.Symbol);

            CompanyStatistics data = await DataProviderFactory.GetDataProvider<ICompanyDataProvider>(company.Industry.Sector.Market).FetchDataAsync(company);

            m_callback.PushCompanyData(data);
        }

        public async void GetStockData(Company company)
        {
            string.Format("Getting stock data for: {0}", company.Symbol);

            StockQuote data = await DataProviderFactory.GetDataProvider<IStockProvider>(company.Industry.Sector.Market).FetchDataAsync(company);
            
            m_callback.PushStockData(data);
        }

        public async void GetSectorData(Market market)
        {
            var data = await DataProviderFactory.GetDataProvider<ISectorDataProvider>(market).FetchDataAsync(market);
            try
            {
                m_callback.PushSectors(data);
            }
            catch (FaultException fe)
            {
                throw fe;
            }
            catch (CommunicationException ce)
            {
                throw ce;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async void GetCompanies(Industry industry)
        {
            var data = await DataProviderFactory.GetDataProvider<ICompanyProvider>(industry.Sector.Market).FetchDataAsync(industry);
            try
            {
                m_callback.PushCompanies(data);
            }
            catch (FaultException fe)
            {
                throw fe;
            }
            catch (CommunicationException ce)
            {
                throw ce;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<Market> GetMarketsData()
        {
            return DataProviderFactory.GetMarketsData();
        }

        public CalculatedData GetCalculatedCompanyData(Company company)
        {
            return CalculatedStaticticsProvider.FetchData(company);
        }
    }
}
