using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using StockService.Core;

namespace StockService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(CallbackContract=typeof(IStockScannerClient), 
                     SessionMode=SessionMode.Required)]
    public interface IStockScannerService
    {
        [OperationContract]
        List<Market> GetMarketsData();

        [OperationContract(IsOneWay = true)]
        //[FaultContract(typeof (string))]
        void GetCompanyData(int market, string symbol);

        [OperationContract(IsOneWay = true)]
        //[FaultContract(typeof(string))]
        void GetStockData(int market, string symbol);

        [OperationContract(IsOneWay = true)]
        //[FaultContract(typeof(string))]
        void GetSectorData(int market);

        [OperationContract(IsOneWay=true)]
        //[FaultContract(typeof(string))]
        void GetCompanies(int market, int industry);
    }
}
