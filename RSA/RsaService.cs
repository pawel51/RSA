using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RSA
{
    class RsaService
    {
        #region COMMANDS INVOKED BY VIEWMODEL
        public void GenerateAutoPQCommand(RsaModel model)
        {
            model.p = -1;
            model.q = -1;
            Generate_p_and_q(model);
        }
        internal bool GenerateAutoPQCommandCanExecute(RsaModel rsa)
        {
            return true;
        }

        public void CalculateFullMoldelCommand(RsaModel model, ObservableCollection<TableModel> tableData)
        {
            model.n = model.p * model.q;
            model.fiOdN = (model.p - 1) * (model.q - 1);
            BigInteger[] ePossibles = Find_e(model.fiOdN); //możliwe e 
            model.e = -1; // private key d
            model.d = -1; // private key d

            Generate_e_and_d(model, ePossibles);
            if (model.d == -1 || model.e == -1)
            {
                MessageBox.Show("Cannot generate e or d. Try setting P and Q first.");
                return;
            }

            DisplayToTable(model, tableData);

        }

        private void DisplayToTable(RsaModel model, ObservableCollection<TableModel> tableData)
        {
            tableData.Clear();
            tableData.Add(new TableModel("n", "(p * q)", model.n.ToString()));
            tableData.Add(new TableModel("fi(n)", "(p - 1) * (q - 1)", model.fiOdN.ToString()));
            tableData.Add(new TableModel("e", "GCD (fi(n), e) == 1 && e > 2", model.e.ToString()));
            tableData.Add(new TableModel("k", "(k * fi(n) + 1) % e == 0", model.k.ToString()));
            tableData.Add(new TableModel("d", "(k * fi(n) + 1) / e", model.d.ToString()));
        }

        internal bool CalculateFullMoldelCommandCanExecute(RsaModel model)
        {
            return true;
        }

        public void EncryptRsaCommand(string message, RsaModel model)
        {
            int[] converted = ConvertStringToAscii(message);
            BigInteger[] encrypted = encryptRSA(converted, model.n, model.e);
            model.EncryptedStr = ConvertEncryptedAsciiToString(encrypted);
        }

        internal bool EncryptRsaCommandCanExecute(RsaModel model)
        {
            return true;
        }

        public void DecryptRsaCommand(string encryptedStr, RsaModel model)
        {
            BigInteger[] encrypted = ConvertStringToBigInteger(encryptedStr);
            BigInteger[] decrypted = decryptRSA(model.n, model.d, encrypted);
            model.DecryptedStr = ConvertDecryptedAsciiToString(decrypted);
        }

        internal bool DecryptRsaCommandCanExecute(RsaModel model)
        {
            return true;
        }

        #endregion

        #region PRIVATE METHODS

        private static string ConvertDecryptedAsciiToString(BigInteger[] decrypted)
        {
            char[] decrypt = new char[decrypted.Length];
            for (int i = 0; i < decrypted.Length; i++)
            {
                decrypt[i] = (char)decrypted[i];
            }
            string decryptedString = new string(decrypt);
            return decryptedString;
        }

        private static int[] ConvertStringToAscii(string text)
        {
            int[] converted = new int[text.Length];

            for (int i = 0; i < converted.Length; i++)
            {
                converted[i] = Convert.ToInt32(text[i]);
            }

            return converted;
        }

        private BigInteger[] ConvertStringToBigInteger(string str)
        {
            string[] chars = str.Split(',');
            BigInteger[] array = new BigInteger[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                array[i] = new BigInteger(Int64.Parse(chars[i]));
            }
            return array;
        }

        private string ConvertEncryptedAsciiToString(BigInteger[] encrypted)
        {
            string encryptedStr = "";
            for (int i = 0; i < encrypted.Length; i++)
            {
                encryptedStr += encrypted[i].ToString() + ",";
            }
            encryptedStr = encryptedStr.Substring(0, encryptedStr.Length - 1);
            return encryptedStr;
        }


        // pojedynczy zdany test daje statystycznie pewność 75% że liczba jest pierwsza
        // 
        private static bool IsMillerRabinPassed(BigInteger candidate, int numberOfTests)
        {
            int maxDivisionByTwo = 0;
            BigInteger evenComponent = candidate - 1;
            // dzielimy even component maksymalną liczbę razy przez dwa
            while (evenComponent % 2 == 0)
            {
                evenComponent /= 2; // shift left by one bit
                maxDivisionByTwo += 1;
            }
            //if (Math.Pow(2, maxDivisionByTwo) * evenComponent == candidate - 1)

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[candidate.ToByteArray().LongLength];
            BigInteger randomNumber;
            // numberOfTests - im większe tym większa pewność że pierwsza
            for (int i = 0; i < numberOfTests; i++)
            {
                // generujemy losową liczbę większą od 2 ale mniejszą od evenComponent
                do
                {
                    rng.GetBytes(bytes);
                    randomNumber = new BigInteger(bytes);
                }
                while (randomNumber < 2 || randomNumber > candidate - 2);

                // x = randomNumber^evenComponent (mod candidate)
                BigInteger x = BigInteger.ModPow(randomNumber, evenComponent, candidate);
                if (x == 1 || x == candidate - 1)
                    continue; // test zdany pomyślnie

                for (int r = 1; r < maxDivisionByTwo; r++)
                {
                    // x = x^2 (mod candidate)
                    x = BigInteger.ModPow(
                        x
                        , 2
                        , candidate);

                    if (x == 1)
                        return false;
                    if (x == candidate - 1)
                        break; // test zdany pomyślnie
                }

                if (x != candidate - 1)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Generuje pseudolosową liczbę o n bitach
        /// </summary>
        /// <param name="n">liczba bitów do wygenerowania</param>
        /// <returns></returns>
        private static BigInteger GetNBitRandom(int n)
        {
            byte[] rngNum = new byte[n / 8 + 1];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();

            rng.GetBytes(rngNum);

            rngNum[rngNum.Length - 1] = 0; // dodanie wiodącego zera, aby pozbyć się liczb ujemnych

            if (rngNum[rngNum.Length - 2] < 128)
                rngNum[0] = 129; //zawsze wybieramy liczbę w zakresie (2^(n-1) + 1, 2^n - 1) (pierwszy bit zawsze 1)
            if (rngNum[0] % 2 == 0)
                rngNum[0] += 1; //zawsze nieparzysta

            // tworzy biginteger z tablicy bajtów w dopełnieniu U2
            return new BigInteger(rngNum);
        }



        private static void Generate_p_and_q(RsaModel m)
        {
            while (m.p == m.q)
            {
                m.p = FindPrimeNumber(m.size, m.primesList);
                m.q = FindPrimeNumber(m.size, m.primesList);
            }
        }

        private static void Generate_e_and_d(RsaModel m, BigInteger[] ePossibles)
        {
            for (int i = 0; i < ePossibles.Length; i++)
            {
                // dobierz k tak aby k*fi(n) + 1 było podzielne przez e
                for (int k = 0; k < 100; k++) // k może być różne i nie wpływa na trudność złamania szyfru 
                {
                    if ((k * m.fiOdN + 1) % ePossibles[i] == 0)
                    {
                        m.d = (k * m.fiOdN + 1) / ePossibles[i];
                        m.e = ePossibles[i];
                        m.k = k;
                        return;
                    }
                }
            }
        }

        private static BigInteger[] decryptRSA(BigInteger n, BigInteger d, BigInteger[] encrypted)
        {
            BigInteger[] decrypted = new BigInteger[encrypted.Length];

            for (int i = 0; i < encrypted.Length; i++)
            {
                decrypted[i] = BigInteger.ModPow(encrypted[i], d, n);
            }

            return decrypted;
        }

        private static BigInteger[] encryptRSA(int[] converted, BigInteger n, BigInteger e)
        {
            BigInteger[] encrypted = new BigInteger[converted.Length];

            for (int i = 0; i < encrypted.Length; i++)
            {
                encrypted[i] = BigInteger.ModPow(converted[i], e, n);
            }

            return encrypted;
        }

        private static BigInteger[] Find_e(BigInteger fiOdN)
        {
            BigInteger[] possiblesE = new BigInteger[20];
            int iNum = 0;
            for (BigInteger i = new BigInteger(3);
                            iNum < 20 && i < fiOdN;
                            i = i + 2)
            {
                if (GCD(fiOdN, i) == 1)
                    possiblesE[iNum++] = i;
            }
            return possiblesE;
        }

        private static BigInteger GCD(BigInteger A, BigInteger B)
        {
            if (B.CompareTo(0) != 0)
            {
                return GCD(B, A % B);
            }
            return A;
        }

        private static BigInteger FindPrimeNumber(int size, List<int> primesList)
        {
            BigInteger num;
            while (true)
            {
                num = GetNBitRandom(size);
                bool isDivisible = false;
                //check divisibility by pre-generated primes
                foreach (int prime in primesList)
                {
                    if (num % prime == 0 && num < prime)
                    {
                        isDivisible = true;
                        break;
                    }
                }
                if (!isDivisible) // jeżeli nie jest podzielna przez żadną z początkowych liczb pierwszych
                {
                    if (IsMillerRabinPassed(num, 10)) // przeprowadź test millera rabina 20 razy
                        break;
                }
            }

            return num;
        }

        #endregion
    }
}
