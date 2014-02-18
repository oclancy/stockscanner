using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NLog;
using StockService.Core;
using StockService.Core.DTOs;
using StockService.Core.Providers;
using System.Reactive.Linq;
using StockService.Core.Extension;
using StockService;
using System.IO;
using System.Web.Hosting;
using System.Data;
using System.Data.Entity.Infrastructure;

namespace StockService
{
    [ServiceBehavior]
    public class StockServiceAdmin : IStockServiceAdmin, IDisposable
    {
        Logger m_logger = LogManager.GetCurrentClassLogger();
        ConcurrentQueue<Company> m_dataToSave = new ConcurrentQueue<Company>();

        IDisposable m_saveSubscriber;
        CancellationTokenSource m_src = new CancellationTokenSource();

        [Dependency]
        public IDataProviderFactory DataProviderFactory { get; set; }
        
        [OperationBehavior]
        public void Scan()
        {
            m_logger.Info("Starting scan at {0}", DateTime.Now);

            Task.Run( () => SaveCompanies(m_src.Token), m_src.Token);

            DataProviderFactory.GetCompanies()
                     .ToObservable()
                     .Delay(TimeSpan.FromMilliseconds(400))
                     .Subscribe( async company =>
                       {
                           try
                           {
                               var cs = await DataProviderFactory.GetDataProvider<ICompanyDataProvider>(company.Industry.Sector.Market)
                                                           .FetchDataAsync(company);
                               var sq = await DataProviderFactory.GetDataProvider<IStockProvider>(company.Industry.Sector.Market)
                                                           .FetchDataAsync(company);

                               var lstTwoMinutes = DateTime.UtcNow.AddMinutes(-2);

                               if (cs.LastUpdated >= lstTwoMinutes || sq.LastUpdated >= lstTwoMinutes)
                               {
                                   m_dataToSave.Enqueue(company);
                                   m_logger.Info("Enqueued {0} ({1})", company.Symbol, company.CompanyId);
                               }
                           }
                           catch (AggregateException ex)
                           {
                               var newex = ex.Flatten();
                               m_logger.ErrorException(string.Format("Exception whilst getting company details for {0}:{1}: {2}",
                                   company.Name,
                                   company.Symbol,
                                   newex.Message), newex);
                           }
                       });

        }

        [OperationBehavior]
        public void ScanCompanies()
        {
            var markets = MarketDataProvider.FetchData();

            var localPath = HostingEnvironment.ApplicationPhysicalPath;

            var aim = markets.FirstOrDefault(m => m.Name == "AIM");
            var mm = markets.FirstOrDefault(m => m.Name == "Main Market");


            LseDataReader.Read(mm, Path.Combine(localPath, "App_Data", "lsedata.csv"));
            LseDataReader.Read(aim, Path.Combine(localPath, "App_Data", "lsedata.csv"));

            DataProviderFactory.LoadCaches();
        }

        private void SaveCompanies(CancellationToken token)
        {
            m_saveSubscriber = Observable.Interval(new TimeSpan(0, 0, 10))
                      .Subscribe(delegate(long l)
                      {
                          Company cmpny;
                          System.Collections.Generic.List<Company> listToSave = new List<Company>();

                          while (m_dataToSave.TryDequeue(out cmpny))
                              listToSave.Add(cmpny);

                          listToSave.ToObservable()
                                    .Buffer(20)
                                    .ForEachAsync(dataList =>
                                    {
                                        dataList.ForEach(c =>
                                            {
                                                using (var cxt = new StockScannerContext())
                                                {
                                                    try
                                                    {

                                                        if (c.CompanyStatistics.Company == null)
                                                        {
                                                            m_logger.Info("Attaching: {0} ({1}) CompanyStatictics as {2}", c.Symbol, c.CompanyId, EntityState.Added);
                                                            cxt.Entry(c.CompanyStatistics).State = EntityState.Added;
                                                        }
                                                        else
                                                        {
                                                            m_logger.Info("Attaching: {0} ({1}) CompanyStatictics as {2}", c.Symbol, c.CompanyId, EntityState.Modified);
                                                            cxt.Entry(c.CompanyStatistics).State = EntityState.Modified;
                                                        }

                                                        if (c.StockQuote.Company == null)
                                                        {
                                                            m_logger.Info("Attaching: {0} ({1}) StockQuote as {2}", c.Symbol, c.CompanyId, EntityState.Added);
                                                            cxt.Entry(c.StockQuote).State = EntityState.Added;
                                                        }
                                                        else
                                                        {
                                                            m_logger.Info("Attaching: {0} ({1}) StockQuote as {2}", c.Symbol, c.CompanyId, EntityState.Modified);
                                                            cxt.Entry(c.StockQuote).State = EntityState.Modified;
                                                        }
                                                    }

                                                    catch (Exception ex)
                                                    {
                                                        m_logger.ErrorException(string.Format("Exception whilst attaching company {0}:{1}({2}) \r Company details company Id:{3}, assoc. Company Id:{4} \rStock Quote Company Id:{5}, assoc. Company Id:{6} \rException {7}",
                                                            c.Symbol,
                                                            c.Name,
                                                            c.CompanyId,
                                                            c.CompanyStatistics.CompanyId,
                                                            c.CompanyStatistics.Company.CompanyId,
                                                            c.StockQuote.CompanyId,
                                                            c.StockQuote.Company.CompanyId,
                                                            ex.Message),
                                                            ex);
                                                    }

                                                    try
                                                    {
                                                        cxt.SaveChanges();
                                                        m_logger.Info("Saved: {0}", c.Symbol);
                                                    }
                                                    catch (DbUpdateException ex)
                                                    {
                                                        m_logger.ErrorException("DbUpdteException whilst saving company details ", ex);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        m_logger.ErrorException("Exception whilst saving company details ", ex);
                                                    }
                                                }
                                            });
                                    });
                      });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_saveSubscriber != null) m_saveSubscriber.Dispose();
            }
        }
    }
}
