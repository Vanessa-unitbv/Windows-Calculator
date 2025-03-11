using System;

namespace Calculator
{
    /// <summary>
    /// Clasa care implementează funcționalitățile de calcul și de memorie
    /// </summary>
    public class CalculatorEngine
    {
        // Rezultatul curent al operațiilor
        public double Result { get; private set; }

        // Operația în așteptare
        private string _pendingOperation = "";

        // Valoarea stocată în memoria calculatorului
        private double _memoryValue = 0;

        /// <summary>
        /// Constructor pentru motorul calculatorului
        /// </summary>
        public CalculatorEngine()
        {
            Reset();
        }

        /// <summary>
        /// Resetează calculatorul la starea inițială
        /// </summary>
        public void Reset()
        {
            Result = 0;
            _pendingOperation = "";
        }

        /// <summary>
        /// Setează valoarea curentă a rezultatului
        /// </summary>
        /// <param name="value">Valoarea care trebuie setată</param>
        public void SetValue(double value)
        {
            Result = value;
        }

        /// <summary>
        /// Returnează operația în așteptare
        /// </summary>
        /// <returns>Operația în așteptare</returns>
        public string GetPendingOperation()
        {
            return _pendingOperation;
        }

        /// <summary>
        /// Setează operația în așteptare
        /// </summary>
        /// <param name="operation">Operația care trebuie setată</param>
        public void SetPendingOperation(string operation)
        {
            _pendingOperation = operation;
        }

        #region Basic Operations

        /// <summary>
        /// Efectuează calculul cu valoarea curentă
        /// </summary>
        /// <param name="currentValue">Al doilea operand pentru operație</param>
        public void Calculate(double currentValue)
        {
            if (string.IsNullOrEmpty(_pendingOperation))
                return;

            switch (_pendingOperation)
            {
                case "+":
                    Result += currentValue;
                    break;
                case "-":
                    Result -= currentValue;
                    break;
                case "x":
                    Result *= currentValue;
                    break;
                case "÷":
                    if (currentValue == 0)
                    {
                        throw new DivideByZeroException("Împărțirea la zero nu este permisă!");
                    }
                    Result /= currentValue;
                    break;
                case "%":
                    Result = Result * currentValue / 100;
                    break;
            }

            // Nu resetăm _pendingOperation aici
            // Ca să tratăm corect operațiile "în cascadă"
        }

        #endregion

        #region Special Operations

        /// <summary>
        /// Calculează pătratul unui număr
        /// </summary>
        /// <param name="value">Numărul pentru care se calculează pătratul</param>
        /// <returns>Rezultatul operației</returns>
        public double Square(double value)
        {
            Result = value * value;
            return Result;
        }

        /// <summary>
        /// Calculează radicalul unui număr
        /// </summary>
        /// <param name="value">Numărul pentru care se calculează radicalul</param>
        /// <returns>Rezultatul operației</returns>
        /// <exception cref="ArgumentException">Aruncată când numărul este negativ</exception>
        public double SquareRoot(double value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Nu se poate calcula radicalul unui număr negativ!");
            }

            Result = Math.Sqrt(value);
            return Result;
        }

        /// <summary>
        /// Calculează reciproca unui număr (1/x)
        /// </summary>
        /// <param name="value">Numărul pentru care se calculează reciproca</param>
        /// <returns>Rezultatul operației</returns>
        /// <exception cref="DivideByZeroException">Aruncată când numărul este zero</exception>
        public double Reciprocal(double value)
        {
            if (value == 0)
            {
                throw new DivideByZeroException("Împărțirea la zero nu este permisă!");
            }

            Result = 1 / value;
            return Result;
        }

        #endregion

        #region Memory Operations

        /// <summary>
        /// Șterge memoria (MC)
        /// </summary>
        public void MemoryClear()
        {
            _memoryValue = 0;
        }

        /// <summary>
        /// Returnează valoarea din memorie (MR)
        /// </summary>
        /// <returns>Valoarea stocată în memorie</returns>
        public double MemoryRecall()
        {
            return _memoryValue;
        }

        /// <summary>
        /// Stochează valoarea în memorie (MS)
        /// </summary>
        /// <param name="value">Valoarea care trebuie stocată</param>
        public void MemoryStore(double value)
        {
            _memoryValue = value;
        }

        /// <summary>
        /// Adaugă valoarea la memorie (M+)
        /// </summary>
        /// <param name="value">Valoarea care trebuie adăugată</param>
        public void MemoryAdd(double value)
        {
            _memoryValue += value;
        }

        /// <summary>
        /// Scade valoarea din memorie (M-)
        /// </summary>
        /// <param name="value">Valoarea care trebuie scăzută</param>
        public void MemorySubtract(double value)
        {
            _memoryValue -= value;
        }

        #endregion
    }
}