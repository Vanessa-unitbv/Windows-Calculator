using System;

namespace Calculator
{
    public class CalculatorEngine
    {
        public double Result { get; private set; }
        private string _pendingOperation = "";
        private double _memoryValue = 0;
        public CalculatorEngine()
        {
            Reset();
        }
        public void Reset()
        {
            Result = 0;
            _pendingOperation = "";
        }
        public void SetValue(double value)
        {
            Result = value;
        }
        public string GetPendingOperation()
        {
            return _pendingOperation;
        }
        public void SetPendingOperation(string operation)
        {
            _pendingOperation = operation;
        }

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
                    currentValue = (Result * currentValue) / 100;
                    Result = currentValue;
                    break;
            }
        }

        public double Square(double value)
        {
            Result = value * value;
            return Result;
        }

        public double SquareRoot(double value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Nu se poate calcula radicalul unui număr negativ!");
            }

            Result = Math.Sqrt(value);
            return Result;
        }

        public double Reciprocal(double value)
        {
            if (value == 0)
            {
                throw new DivideByZeroException("Împărțirea la zero nu este permisă!");
            }

            Result = 1 / value;
            return Result;
        }

        public void MemoryClear()
        {
            _memoryValue = 0;
        }
        public double MemoryRecall()
        {
            return _memoryValue;
        }

        public void MemoryStore(double value)
        {
            _memoryValue = value;
        }

        public void MemoryAdd(double value)
        {
            _memoryValue += value;
        }

        public void MemorySubtract(double value)
        {
            _memoryValue -= value;
        }
    }
}