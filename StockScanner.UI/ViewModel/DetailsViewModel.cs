using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockScanner.UI.Messages;
using StockScanner.UI.StockScannerService;

namespace StockScanner.UI.ViewModel
{
    public class DetailsViewModel : ViewModelBase
    {
        private GalaSoft.MvvmLight.Messaging.IMessenger m_messenger;
        private StockScannerService.StockScannerServiceClient Client;

        public DetailsViewModel(GalaSoft.MvvmLight.Messaging.IMessenger m_messenger, StockScannerService.StockScannerServiceClient Client)
        {
            m_messenger.Register<Industry>(this, OnIndustry);

            m_messenger.Register<Company[]>(this, OnCompany);

            m_messenger.Register<StockQuote>(this, OnStockQuote);
            m_messenger.Register<CompanyStatistics>(this, OnCompanyStatistics);


            this.Client = Client;
        }

        private void OnCompanyStatistics(CompanyStatistics msg)
        {
            CompanyData = msg;
        }

        private void OnStockQuote(StockQuote msg)
        {
            StockData = msg;
        }

        private void OnCompany(Company[] msg)
        {
            Companies = msg;
        }

        private void OnIndustry(Industry msg)
        {
            Client.GetCompanies(msg);
        }

        Company m_selectedCompany;
        public Company SelectedCompany
        {
            get
            {
                return m_selectedCompany;
            }
            set
            {
                m_selectedCompany = value;
                if (m_selectedCompany == null) return;
                Client.GetCompanyData(m_selectedCompany);
                Client.GetStockData(m_selectedCompany);
            }
        }


        StockScannerService.Company[] m_companies;
        public StockScannerService.Company[] Companies
        {
            get
            {
                return m_companies;
            }
            set
            {
                m_companies = value;
                OnPropertyChanged();
            }
        }

        StockScannerService.StockQuote m_stockData;
        public StockScannerService.StockQuote StockData
        {
            get
            {
                return m_stockData;
            }
            set
            {
                m_stockData = value;
                OnPropertyChanged();
            }
        }

        private StockScannerService.CompanyStatistics m_companyData;
        public StockScannerService.CompanyStatistics CompanyData
        {
            get
            {
                return m_companyData;
            }
            set
            {
                m_companyData = value;
                OnPropertyChanged();
            }
        }
    }
}
