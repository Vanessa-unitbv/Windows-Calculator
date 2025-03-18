using System;
using System.Collections.Generic;

namespace Calculator
{
    public class CalculatorEngine
    {
        public double Result { get; private set; }
        private string _pendingOperation = "";
        private double _memoryValue = 0;

        private bool _useOrderOfOperations = false;
        private List<double> _operands = new List<double>();
        private List<string> _operators = new List<string>();

        public CalculatorEngine()
        {
            Reset();
        }

        public void Reset()
        {
            Result = 0;
            _pendingOperation = "";
            _operands.Clear();
            _operators.Clear();
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

        // Operații cu memoria
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
        public void SetUseOrderOfOperations(bool useOrderOfOperations)
        {
            _useOrderOfOperations = useOrderOfOperations;
            if (!_useOrderOfOperations)
            {
                _operands.Clear();
                _operators.Clear();
            }
        }
        public void AddToExpression(double operand, string nextOperator)
        {
            if (_useOrderOfOperations)
            {
                _operands.Add(operand);

                if (!string.IsNullOrEmpty(nextOperator) && nextOperator != "=")
                {
                    _operators.Add(nextOperator);
                }
            }
        }
        public double EvaluateExpression()
        {
            if (!_useOrderOfOperations || _operands.Count == 0)
            {
                return Result;
            }
            List<double> values = new List<double>(_operands);
            List<string> ops = new List<string>(_operators);
            string debugExpr = "";
            for (int j = 0; j < values.Count; j++)
            {
                debugExpr += values[j].ToString();
                if (j < ops.Count)
                {
                    debugExpr += " " + ops[j] + " ";
                }
            }
            System.Diagnostics.Debug.WriteLine("Evaluating: " + debugExpr);
            int i = 0;
            while (i < ops.Count)
            {
                if (ops[i] == "x" || ops[i] == "÷" || ops[i] == "%")
                { 
                    double result = 0;
                    switch (ops[i])
                    {
                        case "x":
                            result = values[i] * values[i + 1];
                            break;
                        case "÷":
                            if (values[i + 1] == 0)
                            {
                                throw new DivideByZeroException("Împărțirea la zero nu este permisă!");
                            }
                            result = values[i] / values[i + 1];
                            break;
                        case "%":
                            result = (values[i] * values[i + 1]) / 100;
                            break;
                    }
                    values[i] = result;
                    values.RemoveAt(i + 1);
                    ops.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            double finalResult = values[0];
            for (i = 0; i < ops.Count; i++)
            {
                switch (ops[i])
                {
                    case "+":
                        finalResult += values[i + 1];
                        break;
                    case "-":
                        finalResult -= values[i + 1];
                        break;
                }
            }
            _operands.Clear();
            _operators.Clear();
            Result = finalResult;
            return Result;
        }
    }
}