using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using StockService.Core;

namespace StockService
{
    [ServiceContract]
    public interface IStockScannerClient
    {
        [OperationContract]
        void PushStockData(StockQuote result);

        [OperationContract]
        void PushCompanyData(CompanyStatistics result);

        [OperationContract]
        void PushSectors(List<Sector> sectors);
    }
}
