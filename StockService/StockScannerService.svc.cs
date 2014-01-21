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

            using (var cxt = new StockScannerContext())
            {
                var data = cxt.Companies.FirstOrDefault(c => c.CompanyId == company.CompanyId);
                if(data!=null)
                    m_callback.PushCompanyData(data.CompanyStatistics);
            }
        }

        public async void GetStockData(Company company)
        {
            string.Format("Getting stock data for: {0}", company.Symbol);

            using (var cxt = new StockScannerContext())
            {
                var data = cxt.Companies.FirstOrDefault(c => c.CompanyId == company.CompanyId);
                if (data != null)
                    m_callback.PushStockData(data.StockQuote);
            }
        }

        public async void GetSectorData(Market market)
        {
            using (var cxt = new StockScannerContext())
            {
                var market2 = cxt.Markets.FirstOrDefault(m => m.MarketId == market.MarketId);
                m_callback.PushSectors(market2.Sectors);
            }
        }

        public async void GetCompanies(Industry industry)
        {
            using (var cxt = new StockScannerContext())
            {
                var market = cxt.Markets.FirstOrDefault(m => m.MarketId == industry.Sector.Market.MarketId);
                var sector = market.Sectors.FirstOrDefault(s => s.SectorId == industry.SectorId);
                var ind = sector.Industries.FirstOrDefault(i => i.IndustryId == industry.IndustryId);
                if(ind != null)
                   m_callback.PushCompanies(ind.Companies);
            }
        }

        public List<Market> GetMarketsData()
        {
            using (var cxt = new StockScannerContext())
            {
                return cxt.Markets.ToList();
            }
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
