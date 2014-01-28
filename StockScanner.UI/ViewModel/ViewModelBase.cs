using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace StockScanner.UI.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Active { get; private set; }

        private void OnActivated(ViewModelBase obj)
        {
            Active = obj == this;
        }

        public ViewModelBase(IMessenger m_messenger)
        {
            if (m_messenger == null) return;
            m_messenger.Register<ViewModelBase>(this, OnActivated);
        }

        public void OnPropertyChanged([CallerMemberName] string property = "")
        {
            if (PropertyChanged == null) return;

            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public abstract string DisplayName { get; }

    }
}
