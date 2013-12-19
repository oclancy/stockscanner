using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using Microsoft.Practices.Unity;
using NLog;
using StockService.Core;
using StockService.Core.DTOs;
using StockService.Core.Providers;
using System.Reactive.Linq;
using StockService.Core.Extension;

namespace StockService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Multiple)]
    public class StockScannerService : IStockScannerService
    {
        IStockScannerClient m_callback = null;

        Logger m_logger = LogManager.GetCurrentClassLogger();
        ConcurrentQueue<Tuple<Company, CompanyStatistics, StockQuote>> m_dataToSave = new ConcurrentQueue<Tuple<Company, CompanyStatistics, StockQuote>>();
        IDisposable m_saveSubscriber;
        CancellationTokenSource m_src = new CancellationTokenSource();

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
            m_callback.PushSectors(data);
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

        public async Task<System.Data.DataTable> GetDividends(Sector sector)
        {
            DataTable results = new DataTable(sector.Name);
            await Task.Run(() =>
            {
                using (var cxt = new StockScannerContext())
                {
                    Parallel.ForEach(sector.Industries, i =>
                        {
                            var cmd = cxt.Database.Connection.CreateCommand();
                            cmd.CommandText = "exec TopDividends";
                            var industry = cmd.CreateParameter();
                            industry.DbType = System.Data.DbType.Int64;
                            industry.Direction = System.Data.ParameterDirection.Input;
                            industry.ParameterName = "@IndustryId";
                            cmd.Parameters.Add(industry);
                            var reader = cmd.ExecuteReader();
                            DataTable dt = new DataTable(i.Name);
                            dt.Load(reader);
                            results.Merge(dt);
                        });
                }
            });
            return results;
        }
    }
}
