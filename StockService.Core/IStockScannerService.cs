using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace StockService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(CallbackContract=typeof(IStockScannerClient), 
                     SessionMode=SessionMode.Required)]
    public interface IStockScannerService
    {

        [OperationContract]
        [FaultContract(typeof (string))]
        void GetCompanyData(string symbol);


        [OperationContract]
        [FaultContract(typeof(string))]
        void GetStockData(string symbol);


        [OperationContract]
        [FaultContract(typeof(string))]
        void GetSetorData();
    }
}
