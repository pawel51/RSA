using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSA
{
    class MainViewModel : INotifyPropertyChanged
    {
        private RsaService service;

        private RsaModel _rsa;

        public RsaModel Rsa
        {
            get { return _rsa; }
            set { _rsa = value; OnPropertyChanged(nameof(Rsa)); }
        }

        private ObservableCollection<TableModel> _tableData = new ObservableCollection<TableModel>();

        public ObservableCollection<TableModel> TableData
        {
            get { return _tableData; }
            set { _tableData = value; OnPropertyChanged(nameof(TableData)); }
        }


        private string _messageToEncrypt = "";
        public string MessageToEncrypt
        {
            get { return _messageToEncrypt; }
            set { _messageToEncrypt = value; OnPropertyChanged(nameof(MessageToEncrypt)); }
        }

        private string _messageToDecrypt = "";
        public string MessageToDecrypt
        {
            get { return _messageToDecrypt; }
            set { _messageToDecrypt = value; OnPropertyChanged(nameof(MessageToDecrypt)); }
        }



        private RelayCommand _GenerateAutoPQCommand;
        public RelayCommand GenerateAutoPQCommand
        {
            get
            {
                if (_GenerateAutoPQCommand == null)
                    _GenerateAutoPQCommand = new RelayCommand(param => service.GenerateAutoPQCommand(Rsa), param => service.GenerateAutoPQCommandCanExecute(Rsa));
                return _GenerateAutoPQCommand;
            }
            set { _GenerateAutoPQCommand = value; OnPropertyChanged(nameof(GenerateAutoPQCommand)); }
        }

        private RelayCommand _CalculateFullMoldelCommand;
        public RelayCommand CalculateFullMoldelCommand
        {
            get
            {
                if (_CalculateFullMoldelCommand == null)
                    _CalculateFullMoldelCommand = new RelayCommand(param => service.CalculateFullMoldelCommand(Rsa, TableData), param => service.CalculateFullMoldelCommandCanExecute(Rsa));
                return _CalculateFullMoldelCommand;
            }
            set { _CalculateFullMoldelCommand = value; OnPropertyChanged(nameof(CalculateFullMoldelCommand)); }
        }

        private RelayCommand _EncryptRsaCommand;
        public RelayCommand EncryptRsaCommand
        {
            get
            {
                if (_EncryptRsaCommand == null)
                    _EncryptRsaCommand = new RelayCommand(param => service.EncryptRsaCommand(MessageToEncrypt, Rsa), param => service.EncryptRsaCommandCanExecute(Rsa));
                return _EncryptRsaCommand;
            }
            set { _EncryptRsaCommand = value; OnPropertyChanged(nameof(EncryptRsaCommand)); }
        }

        private RelayCommand _DecryptRsaCommand;
        public RelayCommand DecryptRsaCommand
        {
            get
            {
                if (_DecryptRsaCommand == null)
                    _DecryptRsaCommand = new RelayCommand(param => service.DecryptRsaCommand(MessageToDecrypt, Rsa), param => service.DecryptRsaCommandCanExecute(Rsa));
                return _DecryptRsaCommand;
            }
            set { _DecryptRsaCommand = value; OnPropertyChanged(nameof(DecryptRsaCommand)); }
        }


        public MainViewModel()
        {
            Rsa = new RsaModel();
            service = new RsaService();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
