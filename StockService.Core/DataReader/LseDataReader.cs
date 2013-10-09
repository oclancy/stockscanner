using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using StockService.Core;

namespace StockService.Core
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

        internal static void Read(Core.Market market, string file)
        {
            //Task.Run(()=>{
                string line;
                using (var reader = new StreamReader(file))
                {
                    int industryId;
                    industryId = 0;

                    while ( (line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(new[]{','});

                        if(market.Name != parts[Market])continue;

                        Sector sector = market.Sectors.FirstOrDefault(s => s.Name == parts[Industry]);
                        if(sector == null)
                            market.Sectors.Add(sector = new Sector() { Name=parts[Industry], Market=market });

                        var industry = sector.Industries.FirstOrDefault(i => i.Name == parts[Sector]);
                        if (industry == null)
                            sector.Industries.Add(industry = new Industry() { Name=parts[Sector], Sector = sector });

                        var company = industry.Companies.FirstOrDefault(c => c.Symbol == parts[Symbol]);
                        if (company == null && !string.IsNullOrEmpty(parts[Symbol]))
                            industry.Companies.Add(new Company()
                            {
                                Name = parts[CompanyName],
                                Symbol = string.Format("{0}{1}{2}", parts[Symbol], parts[Symbol].Last() == '.' ? string.Empty : ".", "L"),
                                Industry = industry
                            });
                        else
                        {
                            m_log.Error("Could not construct Company with data: {0}, because {1}", line, company!=null? "Duplicate company":"No Symbol");
                        }
                    }
                }
        }
    }
}