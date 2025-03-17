using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    public class CalculatorMemoryManager
    {
        private double _currentMemoryValue = 0;
        private ObservableCollection<MemoryItem> _memoryStack;
        private ListBox _memoryListBox;
        public event EventHandler MemoryChanged;
        public CalculatorMemoryManager(ListBox memoryListBox)
        {
            _memoryListBox = memoryListBox;
            _memoryStack = new ObservableCollection<MemoryItem>();
            _memoryListBox.ItemsSource = _memoryStack;
            _memoryListBox.Visibility = Visibility.Collapsed;
        }
        public void MemoryClear()
        {
            if (_memoryListBox.SelectedItem is MemoryItem selectedItem)
            {
                _memoryStack.Remove(selectedItem);
                if (_memoryStack.Count == 0)
                {
                    _memoryListBox.Visibility = Visibility.Collapsed;
                }
                _currentMemoryValue = _memoryStack.Count > 0 ? _memoryStack[0].Value : 0;
            }
            else
            {
                _currentMemoryValue = 0;
                _memoryStack.Clear();
                _memoryListBox.Visibility = Visibility.Collapsed;
            }

            OnMemoryChanged();
        }
        public double MemoryRecall()
        {
            return _currentMemoryValue;
        }
        public void MemoryStore(double value)
        {
            _currentMemoryValue = value;
            AddToMemoryStack(value);
            OnMemoryChanged();
        }
        public void MemoryAdd(double value)
        {
            if (_memoryListBox.SelectedItem is MemoryItem selectedItem)
            {
                int index = _memoryStack.IndexOf(selectedItem);
                if (index != -1)
                {
                    double newValue = selectedItem.Value + value;
                    _memoryStack[index] = new MemoryItem(newValue, DateTime.Now);
                    _currentMemoryValue = newValue;
                }
            }
            else
            {
                _currentMemoryValue += value;
                AddToMemoryStack(_currentMemoryValue);
            }
            OnMemoryChanged();
        }
        public void MemorySubtract(double value)
        {
            if (_memoryListBox.SelectedItem is MemoryItem selectedItem)
            {
                int index = _memoryStack.IndexOf(selectedItem);
                if (index != -1)
                {
                    double newValue = selectedItem.Value - value;
                    _memoryStack[index] = new MemoryItem(newValue, DateTime.Now);
                    _currentMemoryValue = newValue;
                }
            }
            else
            {
                _currentMemoryValue -= value;
                AddToMemoryStack(_currentMemoryValue);
            }
            OnMemoryChanged();
        }
        public void ToggleMemoryStack()
        {
            if (_memoryStack.Count > 0)
            {
                if (_memoryListBox.Visibility == Visibility.Visible)
                {
                    _memoryListBox.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _memoryListBox.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show("Nu există valori stocate în memorie.", "Memorie goală",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        public void HideMemoryStack()
        {
            _memoryListBox.Visibility = Visibility.Collapsed;
        }
        public double GetSelectedMemoryValue()
        {
            if (_memoryListBox.SelectedItem is MemoryItem selectedItem)
            {
                return selectedItem.Value;
            }

            return 0;
        }
        private void AddToMemoryStack(double value)
        {
            _memoryStack.Insert(0, new MemoryItem(value, DateTime.Now));
            if (_memoryStack.Count > 10)
            {
                _memoryStack.RemoveAt(_memoryStack.Count - 1);
            }
            if (_memoryStack.Count > 0 && _memoryListBox.Visibility != Visibility.Visible)
            {
                _memoryListBox.Visibility = Visibility.Visible;
            }
        }
        private void OnMemoryChanged()
        {
            MemoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public class MemoryItem
    {
        public double Value { get; }
        public DateTime Timestamp { get; }
        public MemoryItem(double value, DateTime timestamp)
        {
            Value = value;
            Timestamp = timestamp;
        }
        public override string ToString()
        {
            return $"{Value} ({Timestamp:HH:mm:ss})";
        }
    }
}