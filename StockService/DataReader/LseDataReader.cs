using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using StockService.Core;

namespace StockService.DataReader
{
    public class LseDataReader
    {
        static NLog.Logger m_log = NLog.LogManager.GetCurrentClassLogger();

        const int Market = 5;
        const int Isin = 11;
        const int Symbol = 13;
        const int Industry = 18;
        const int Sector = 20;
        const int CompanyName = 1;

        internal static async Task Read(Core.Market market, string file)
        {
            await Task.Run(()=>{
                using (var reader = new StreamReader(file))
                {
                    while (!reader.EndOfStream)
                    {
                        reader.ReadLineAsync()
                              .ContinueWith( t => 
                              {
                                  var parts = t.Result.Split(new[]{','});

                                  if(market.Name != parts[Market])return;

                                  Sector sector = market.Sectors.FirstOrDefault( s => s.Name == parts[Sector] );
                                  if(sector == null)
                                      market.Sectors.Add( sector = new Sector(parts[Sector]));

                                  var industry = sector.Industries.FirstOrDefault(i => i.Name == parts[Industry]);
                                  if (industry == null)
                                      sector.Industries.Add(industry = new Industry(0, parts[Industry]));

                                  var company = industry.Companies.FirstOrDefault(c => c.Name == parts[CompanyName]);
                                  if (company == null)
                                      industry.Companies.Add(new Company(parts[CompanyName], parts[Symbol]));
                              
                              }, TaskContinuationOptions.NotOnFaulted )
                              .ContinueWith( t => m_log.ErrorException("Excpetion whilst reading.",t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                    }
                }
            });
        }
    }
}