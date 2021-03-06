﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using StockService.Core;
using StockService.Core.DTOs;

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

        public static void Read(Core.Market market, string file)
        {
            using (var cxt = new StockScannerContext())
            {
                cxt.Markets.Attach(market);

                using (var reader = new StreamReader(file))
                {
                    string line;
             
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = ReadCsvLine(line);

                        if (market.Name.ToUpper() != parts[Market].ToUpper()) continue;

                        Sector sector = market.Sectors.FirstOrDefault(s => s.Name == parts[Industry]);
                        if (sector == null)
                            market.Sectors.Add(sector = new Sector() { Name = parts[Industry], Market = market, MarketId = market.MarketId });

                        var industry = sector.Industries.FirstOrDefault(i => i.Name == parts[Sector]);
                        if (industry == null)
                            sector.Industries.Add(industry = new Industry() { Name = parts[Sector], Sector = sector, SectorId = sector.SectorId });

                        if (!string.IsNullOrEmpty(parts[Symbol]))
                        {
                            var symbol = parts[Symbol];
                            var company = industry.Companies.FirstOrDefault(c => c.Symbol == symbol);
                            if (company == null)
                                industry.Companies.Add(new Company()
                                {
                                    Name = parts[CompanyName],
                                    Symbol = symbol,
                                    Industry = industry,
                                    IndustryId = industry.IndustryId
                                });
                        }
                        else
                        {
                            m_log.Error("Could not construct Company with data: {0}, because {1}", line, "No Symbol");
                        }
                    }
                }
                cxt.SaveChanges();
            }
        }

        public static List<string> ReadCsvLine(string line)
        {
            var parts = new List<string>();
            bool inquote=false;
            var posStart = 0;
            var posEnd = 0;
            var trim = new char[] { '"' };

            var length = line.Length;
            for (; posEnd < length; ++posEnd)
            {
                var c = line[posEnd];
                if (c == '"') inquote = !inquote;

                if (c == ',' && !inquote)
                {
                    var entry = line.Substring(posStart, posEnd - posStart);
                    entry.Trim(trim);
                    parts.Add(line.Substring(posStart, posEnd - posStart));
                    posStart = posEnd + 1;
                }
            }
            parts.Add(line.Substring(posStart, posEnd - posStart));

            return parts;
        }
    }
}