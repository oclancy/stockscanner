namespace StockService.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Markets",
                c => new
                    {
                        MarketId = c.Int(nullable: false, identity: true),
                        Symbol = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.MarketId);
            
            CreateTable(
                "dbo.Sectors",
                c => new
                    {
                        SectorId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        MarketId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SectorId)
                .ForeignKey("dbo.Markets", t => t.MarketId, cascadeDelete: true)
                .Index(t => t.MarketId);
            
            CreateTable(
                "dbo.Industries",
                c => new
                    {
                        IndustryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SectorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IndustryId)
                .ForeignKey("dbo.Sectors", t => t.SectorId, cascadeDelete: true)
                .Index(t => t.SectorId);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Symbol = c.String(),
                        IndustryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyId)
                .ForeignKey("dbo.Industries", t => t.IndustryId, cascadeDelete: true)
                .Index(t => t.IndustryId);
            
            CreateTable(
                "dbo.CompanyStatistics",
                c => new
                    {
                        CompanyId = c.Int(nullable: false),
                        LastUpdated = c.DateTime(nullable: false, storeType: "datetime2"),
                        EnterpriseValue = c.Decimal(precision: 18, scale: 2),
                        TrailingPE = c.Decimal(precision: 18, scale: 2),
                        MarketCap = c.Decimal(precision: 18, scale: 2),
                        ForwardPE = c.Decimal(precision: 18, scale: 2),
                        PEGRatio = c.Decimal(precision: 18, scale: 2),
                        PriceSalesRatio = c.Decimal(precision: 18, scale: 2),
                        PriceBookRatio = c.Decimal(precision: 18, scale: 2),
                        EnterpriseValueToRevenueRatio = c.Decimal(precision: 18, scale: 2),
                        EnterpriseValueToEBITDARatio = c.Decimal(precision: 18, scale: 2),
                        ProfitMargin = c.Decimal(precision: 18, scale: 2),
                        OperatingMargin = c.Decimal(precision: 18, scale: 2),
                        ReturnOnAssets = c.Decimal(precision: 18, scale: 2),
                        ReturnOnEquity = c.Decimal(precision: 18, scale: 2),
                        Revenue = c.Decimal(precision: 18, scale: 2),
                        RevenuePerShare = c.Decimal(precision: 18, scale: 2),
                        QtrlyRevenueGrowth = c.Decimal(precision: 18, scale: 2),
                        GrossProfit = c.Decimal(precision: 18, scale: 2),
                        EBITDA = c.Decimal(precision: 18, scale: 2),
                        NetIncomeAvltoCommon = c.Decimal(precision: 18, scale: 2),
                        DilutedEPS = c.Decimal(precision: 18, scale: 2),
                        QtrlyEarningsGrowth = c.Decimal(precision: 18, scale: 2),
                        TotalCash = c.Decimal(precision: 18, scale: 2),
                        TotalCashPerShare = c.Decimal(precision: 18, scale: 2),
                        TotalDebt = c.Decimal(precision: 18, scale: 2),
                        TotalDebtToEquityRatio = c.Decimal(precision: 18, scale: 2),
                        CurrentRatio = c.Decimal(precision: 18, scale: 2),
                        BookValuePerShare = c.Decimal(precision: 18, scale: 2),
                        OperatingCashFlow = c.Decimal(precision: 18, scale: 2),
                        LeveredFreeCashFlow = c.Decimal(precision: 18, scale: 2),
                        TrailingAnnualDividendYield = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CompanyId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.StockQuotes",
                c => new
                    {
                        CompanyId = c.Int(nullable: false),
                        LastUpdated = c.DateTime(nullable: false, storeType: "datetime2"),
                        Ask = c.Decimal(precision: 18, scale: 2),
                        Bid = c.Decimal(precision: 18, scale: 2),
                        AverageDailyVolume = c.Decimal(precision: 18, scale: 2),
                        BookValue = c.Decimal(precision: 18, scale: 2),
                        Change = c.Decimal(precision: 18, scale: 2),
                        DividendShare = c.Decimal(precision: 18, scale: 2),
                        LastTradeDate = c.DateTime(),
                        EarningsShare = c.Decimal(precision: 18, scale: 2),
                        EpsEstimateCurrentYear = c.Decimal(precision: 18, scale: 2),
                        EpsEstimateNextYear = c.Decimal(precision: 18, scale: 2),
                        EpsEstimateNextQuarter = c.Decimal(precision: 18, scale: 2),
                        DailyLow = c.Decimal(precision: 18, scale: 2),
                        DailyHigh = c.Decimal(precision: 18, scale: 2),
                        YearlyLow = c.Decimal(precision: 18, scale: 2),
                        YearlyHigh = c.Decimal(precision: 18, scale: 2),
                        MarketCapitalization = c.Decimal(precision: 18, scale: 2),
                        Ebitda = c.Decimal(precision: 18, scale: 2),
                        ChangeFromYearLow = c.Decimal(precision: 18, scale: 2),
                        PercentChangeFromYearLow = c.Decimal(precision: 18, scale: 2),
                        ChangeFromYearHigh = c.Decimal(precision: 18, scale: 2),
                        LastTradePrice = c.Decimal(precision: 18, scale: 2),
                        PercentChangeFromYearHigh = c.Decimal(precision: 18, scale: 2),
                        FiftyDayMovingAverage = c.Decimal(precision: 18, scale: 2),
                        TwoHunderedDayMovingAverage = c.Decimal(precision: 18, scale: 2),
                        ChangeFromTwoHundredDayMovingAverage = c.Decimal(precision: 18, scale: 2),
                        PercentChangeFromTwoHundredDayMovingAverage = c.Decimal(precision: 18, scale: 2),
                        PercentChangeFromFiftyDayMovingAverage = c.Decimal(precision: 18, scale: 2),
                        Name = c.String(),
                        Open = c.Decimal(precision: 18, scale: 2),
                        PreviousClose = c.Decimal(precision: 18, scale: 2),
                        ChangeInPercent = c.Decimal(precision: 18, scale: 2),
                        PriceSales = c.Decimal(precision: 18, scale: 2),
                        PriceBook = c.Decimal(precision: 18, scale: 2),
                        ExDividendDate = c.DateTime(),
                        PeRatio = c.Decimal(precision: 18, scale: 2),
                        DividendPayDate = c.DateTime(),
                        PegRatio = c.Decimal(precision: 18, scale: 2),
                        PriceEpsEstimateCurrentYear = c.Decimal(precision: 18, scale: 2),
                        PriceEpsEstimateNextYear = c.Decimal(precision: 18, scale: 2),
                        ShortRatio = c.Decimal(precision: 18, scale: 2),
                        OneYearPriceTarget = c.Decimal(precision: 18, scale: 2),
                        Volume = c.Decimal(precision: 18, scale: 2),
                        StockExchange = c.String(),
                        Yield = c.Decimal(precision: 18, scale: 2),
                        Dividend = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CompanyId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.StockQuotes", new[] { "CompanyId" });
            DropIndex("dbo.CompanyStatistics", new[] { "CompanyId" });
            DropIndex("dbo.Companies", new[] { "IndustryId" });
            DropIndex("dbo.Industries", new[] { "SectorId" });
            DropIndex("dbo.Sectors", new[] { "MarketId" });
            DropForeignKey("dbo.StockQuotes", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.CompanyStatistics", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Companies", "IndustryId", "dbo.Industries");
            DropForeignKey("dbo.Industries", "SectorId", "dbo.Sectors");
            DropForeignKey("dbo.Sectors", "MarketId", "dbo.Markets");
            DropTable("dbo.StockQuotes");
            DropTable("dbo.CompanyStatistics");
            DropTable("dbo.Companies");
            DropTable("dbo.Industries");
            DropTable("dbo.Sectors");
            DropTable("dbo.Markets");
        }
    }
}
