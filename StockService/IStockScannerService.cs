using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using StockService.Core;

namespace StockService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(CallbackContract=typeof(IStockScannerClient))]
    public interface IStockScannerService
    {
        [OperationContract]
        [ApplyDataContractResolver]
        List<Market> GetMarketsData();

        [OperationContract(IsOneWay = true)]
        //[FaultContract(typeof (string))]
        [ApplyDataContractResolver]
        void GetCompanyData(Company company);

        [OperationContract(IsOneWay = true)]
        //[FaultContract(typeof(string))]
        [ApplyDataContractResolver]
        void GetStockData(Company company);

        [OperationContract]
        //[FaultContract(typeof(string))]
        [ApplyDataContractResolver]
        CalculatedData GetCalculatedCompanyData(Company company);

        [OperationContract(IsOneWay = true)]
        //[FaultContract(typeof(string))]
        [ApplyDataContractResolver]
        void GetSectorData(Market market);

        [OperationContract(IsOneWay=true)]
        //[FaultContract(typeof(string))]
        [ApplyDataContractResolver]
        void GetCompanies(Industry industry);

        [OperationContract]
        Task<DataTable> GetDividends(Sector sector);


    }
}
