using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RSA
{
    class RsaModel : INotifyPropertyChanged
    {
        #region PROPS
        private BigInteger _p;

        public BigInteger p
        {
            get { return _p; }
            set { _p = value; OnPropertyChanged(nameof(p)); }
        }

        private BigInteger _q;

        public BigInteger q
        {
            get { return _q; }
            set { _q = value; OnPropertyChanged(nameof(q)); }
        }

        private BigInteger _e;

        public BigInteger e
        {
            get { return _e; }
            set { _e = value; OnPropertyChanged(nameof(e)); }
        }

        private BigInteger _d;

        public BigInteger d
        {
            get { return _d; }
            set { _d = value; OnPropertyChanged(nameof(d)); }
        }

        private BigInteger _k;

        public BigInteger k
        {
            get { return _k; }
            set { _k = value; OnPropertyChanged(nameof(k)); }
        }

        private BigInteger _fiOdN;

        public BigInteger fiOdN
        {
            get { return _fiOdN; }
            set { _fiOdN = value; OnPropertyChanged(nameof(fiOdN)); }
        }

        private BigInteger _n;
        public BigInteger n
        {
            get { return _n; }
            set { _n = value; OnPropertyChanged(nameof(n)); }
        }

        private int _size;

        public int size
        {
            get { return _size; }
            set { _size = value; OnPropertyChanged(nameof(size)); }
        }

        private string _encryptedStr;

        public string EncryptedStr
        {
            get { return _encryptedStr; }
            set { _encryptedStr = value; OnPropertyChanged(nameof(EncryptedStr)); }
        }

        private string decryptedStr;

        public string DecryptedStr
        {
            get { return decryptedStr; }
            set { decryptedStr = value; OnPropertyChanged(nameof(DecryptedStr)); }
        }
        #endregion

        public List<int> primesList = GenerateFirstPrimeNumbers();

        public RsaModel()
        {

        }

        private static List<int> GenerateFirstPrimeNumbers()
        {
            List<int> primesList = new List<int>();
            //generate first few hundred primes
            for (int i = 2; i < 1000; i++)
            {
                int sum = 0;
                for (int j = 2; j < i; j++)
                {
                    if (i % j == 0)
                    {
                        sum += 1;
                        break;
                    }
                }
                if (sum == 0)
                    primesList.Add(i);
            }

            return primesList;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
