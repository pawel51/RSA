using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSA
{
    public class TableModel : INotifyPropertyChanged
    {
        private string _Variable;

        public string Variable
        {
            get { return _Variable; }
            set { _Variable = value; OnPropertyChanged(nameof(Variable)); }
        }

        private string _Formula;

        public string Formula
        {
            get { return _Formula; }
            set { _Formula = value; OnPropertyChanged(nameof(Formula)); }
        }

        private string _Value;

        public string Value
        {
            get { return _Value; }
            set { _Value = value; OnPropertyChanged(nameof(Value)); }
        }

        public TableModel(string v1, string v2, string v3)
        {
            Variable = v1;
            Formula = v2;
            Value = v3;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
