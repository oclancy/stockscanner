using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace StockService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IStockServiceAdmin" in both code and config file together.
    [ServiceContract]
    public interface IStockServiceAdmin
    {
        [OperationContract(IsOneWay = true)]
        [WebGet]
        void Scan();

        [OperationContract(IsOneWay = true)]
        [WebGet]
        void ScanCompanies();
    }
}
