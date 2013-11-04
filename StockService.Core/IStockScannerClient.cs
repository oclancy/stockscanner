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
        [ApplyDataContractResolver]
        void PushStockData(StockQuote result);

        [OperationContract]
        [ApplyDataContractResolver]
        void PushCompanyData(CompanyStatistics result);

        [OperationContract]
        [ApplyDataContractResolver]
        void PushSectors(IList<Sector> sectors);

        [OperationContract]
        [ApplyDataContractResolver]
        void PushCompanies(IList<Company> data);
    }
}
