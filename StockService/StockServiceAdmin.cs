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

namespace StockService
{
    [ServiceBehavior]
    public class StockServiceAdmin : IStockServiceAdmin
    {
        Logger m_logger = LogManager.GetCurrentClassLogger();
        ConcurrentQueue<Tuple<Company, CompanyStatistics, StockQuote>> m_dataToSave = new ConcurrentQueue<Tuple<Company, CompanyStatistics, StockQuote>>();
        IDisposable m_saveSubscriber;
        CancellationTokenSource m_src = new CancellationTokenSource();

        [Dependency]
        public DataProviderFactory DataProviderFactory { get; set; }
        
        [OperationBehavior]
        public void Scan()
        {
            m_logger.Info("Starting scan at {0}", DateTime.Now);

            SaveCompanies(m_src.Token);

            DataProviderFactory.GetCompanies()
                     .ToObservable()
                     .Delay(TimeSpan.FromMilliseconds(400))
                     .Subscribe(company =>
                       {
                           try
                           {
                               var cs = DataProviderFactory.GetDataProvider<ICompanyDataProvider>(company.Industry.Sector.Market)
                                                           .FetchDataAsync(company);
                               var sq = DataProviderFactory.GetDataProvider<IStockProvider>(company.Industry.Sector.Market)
                                                           .FetchDataAsync(company);

                               Task.WaitAll(cs, sq);

                               var lstTwoMinutes = DateTime.UtcNow.AddMinutes(-2);

                               if (cs.Result.LastUpdated >= lstTwoMinutes || sq.Result.LastUpdated >= lstTwoMinutes)
                               {
                                   m_dataToSave.Enqueue(new Tuple<Company, CompanyStatistics, StockQuote>(company, cs.Result, sq.Result));
                                   m_logger.Info("Enqueued {0}", company.Symbol);
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
            m_saveSubscriber = Observable.Interval(new TimeSpan(0, 0, 30))
                      .Subscribe( delegate (long l)
                      {
                          Tuple<Company, CompanyStatistics, StockQuote> outTuple;
                          System.Collections.Generic.List<Tuple<Company, CompanyStatistics, StockQuote>> listToSave = new List<Tuple<Company, CompanyStatistics, StockQuote>>();

                          while (m_dataToSave.TryDequeue(out outTuple))
                              listToSave.Add(outTuple);

                          listToSave.ToObservable()
                                    .Buffer(20)
                                    .ForEachAsync(dataTupleList =>
                                    {
                                        using (var cxt = new StockScannerContext())
                                        {
                                            foreach( var tuple in dataTupleList)
                                            {
                                                try
                                                {
                                                    cxt.Companies.Attach(tuple.Item1);
                                                    cxt.Entry(tuple.Item1).State = EntityState.Modified;
                                                    //tuple.Item1.CompanyStatistics = tuple.Item2;
                                                    //tuple.Item1.StockQuote = tuple.Item3;
                                                }
                                                catch (Exception ex)
                                                {
                                                    m_logger.ErrorException(string.Format("Exception whilst attaching company details for {0}:{1}: {2}",
                                                        tuple.Item1.Name,
                                                        tuple.Item1.Symbol,
                                                        ex.Message),
                                                        ex);
                                                }
                                            };
                                            try
                                            {
                                                cxt.SaveChanges();
                                                m_logger.Info("Saved: {0}", string.Join(", ", dataTupleList.Select(c => c.Item1.Symbol)));
                                            }
                                            catch (Exception ex)
                                            {
                                                m_logger.ErrorException("Exception whilst saving company details ", ex);
                                            }
                                        }
                                    });
                      });
        }
    }
}
