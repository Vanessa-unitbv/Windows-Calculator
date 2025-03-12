using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    /// <summary>
    /// Clasa care gestionează operațiile de memorie ale calculatorului
    /// </summary>
    public class CalculatorMemoryManager
    {
        // Valoarea curentă din memorie (pentru operațiile clasice MR, MS, M+, M-)
        private double _currentMemoryValue = 0;

        // Stiva de valori din memorie (pentru operația M>)
        private ObservableCollection<MemoryItem> _memoryStack;

        // Referință la ListBox-ul care va afișa valorile din stivă
        private ListBox _memoryListBox;

        // Eveniment pentru a notifica schimbări în memorie
        public event EventHandler MemoryChanged;

        /// <summary>
        /// Constructor pentru managerul de memorie
        /// </summary>
        /// <param name="memoryListBox">ListBox-ul în care se vor afișa valorile din memorie</param>
        public CalculatorMemoryManager(ListBox memoryListBox)
        {
            _memoryListBox = memoryListBox;
            _memoryStack = new ObservableCollection<MemoryItem>();

            // Setăm sursa de date pentru ListBox
            _memoryListBox.ItemsSource = _memoryStack;

            // Ascundem inițial ListBox-ul deoarece nu sunt valori
            _memoryListBox.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Șterge memoria (MC) - șterge valoarea curentă sau elementul selectat
        /// </summary>
        public void MemoryClear()
        {
            // Verifică dacă există un element selectat în ListBox
            if (_memoryListBox.SelectedItem is MemoryItem selectedItem)
            {
                // Șterge doar elementul selectat
                _memoryStack.Remove(selectedItem);

                // Dacă nu mai sunt elemente, ascunde ListBox-ul
                if (_memoryStack.Count == 0)
                {
                    _memoryListBox.Visibility = Visibility.Collapsed;
                }

                // Actualizează valoarea curentă din memorie cu prima valoare din stivă sau 0
                _currentMemoryValue = _memoryStack.Count > 0 ? _memoryStack[0].Value : 0;
            }
            else
            {
                // Comportamentul tradițional - șterge tot
                _currentMemoryValue = 0;
                _memoryStack.Clear();
                _memoryListBox.Visibility = Visibility.Collapsed;
            }

            OnMemoryChanged();
        }

        /// <summary>
        /// Returnează valoarea din memorie (MR)
        /// </summary>
        /// <returns>Valoarea stocată în memorie</returns>
        public double MemoryRecall()
        {
            return _currentMemoryValue;
        }

        /// <summary>
        /// Stochează valoarea în memorie (MS)
        /// </summary>
        /// <param name="value">Valoarea care trebuie stocată</param>
        public void MemoryStore(double value)
        {
            _currentMemoryValue = value;

            // Adăugăm și în stiva de memorie cu timestamp
            AddToMemoryStack(value);

            OnMemoryChanged();
        }

        /// <summary>
        /// Adaugă valoarea la memorie (M+)
        /// </summary>
        /// <param name="value">Valoarea care trebuie adăugată</param>
        public void MemoryAdd(double value)
        {
            // Verifică dacă există un element selectat în stivă
            if (_memoryListBox.SelectedItem is MemoryItem selectedItem)
            {
                // Adaugă valoarea la elementul selectat
                int index = _memoryStack.IndexOf(selectedItem);
                if (index != -1)
                {
                    // Creează un nou MemoryItem cu valoarea actualizată
                    double newValue = selectedItem.Value + value;
                    _memoryStack[index] = new MemoryItem(newValue, DateTime.Now);

                    // Actualizează și valoarea curentă din memorie
                    _currentMemoryValue = newValue;
                }
            }
            else
            {
                // Comportamentul standard dacă nu este selectat niciun element
                _currentMemoryValue += value;

                // Adăugăm rezultatul în stiva de memorie
                AddToMemoryStack(_currentMemoryValue);
            }

            OnMemoryChanged();
        }

        /// <summary>
        /// Scade valoarea din memorie (M-)
        /// </summary>
        /// <param name="value">Valoarea care trebuie scăzută</param>
        public void MemorySubtract(double value)
        {
            // Verifică dacă există un element selectat în stivă
            if (_memoryListBox.SelectedItem is MemoryItem selectedItem)
            {
                // Scade valoarea din elementul selectat
                int index = _memoryStack.IndexOf(selectedItem);
                if (index != -1)
                {
                    // Creează un nou MemoryItem cu valoarea actualizată
                    double newValue = selectedItem.Value - value;
                    _memoryStack[index] = new MemoryItem(newValue, DateTime.Now);

                    // Actualizează și valoarea curentă din memorie
                    _currentMemoryValue = newValue;
                }
            }
            else
            {
                // Comportamentul standard dacă nu este selectat niciun element
                _currentMemoryValue -= value;

                // Adăugăm rezultatul în stiva de memorie
                AddToMemoryStack(_currentMemoryValue);
            }

            OnMemoryChanged();
        }

        /// <summary>
        /// Afișează sau ascunde stiva de memorie (M>) - funcție toggle
        /// </summary>
        public void ToggleMemoryStack()
        {
            if (_memoryStack.Count > 0)
            {
                // Inversează vizibilitatea ListBox-ului
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

        /// <summary>
        /// Ascunde stiva de memorie
        /// </summary>
        public void HideMemoryStack()
        {
            _memoryListBox.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Obține valoarea selectată din stiva de memorie
        /// </summary>
        /// <returns>Valoarea selectată sau 0 dacă nu este selectată nicio valoare</returns>
        public double GetSelectedMemoryValue()
        {
            if (_memoryListBox.SelectedItem is MemoryItem selectedItem)
            {
                return selectedItem.Value;
            }

            return 0;
        }

        /// <summary>
        /// Adaugă o valoare în stiva de memorie
        /// </summary>
        /// <param name="value">Valoarea de adăugat</param>
        private void AddToMemoryStack(double value)
        {
            // Adăugăm la începutul listei pentru a păstra cele mai recente valori în partea de sus
            _memoryStack.Insert(0, new MemoryItem(value, DateTime.Now));

            // Limităm numărul de intrări în stivă la 10 pentru a nu aglomera interfața
            if (_memoryStack.Count > 10)
            {
                _memoryStack.RemoveAt(_memoryStack.Count - 1);
            }

            // Facem ListBox-ul vizibil dacă avem valori
            if (_memoryStack.Count > 0 && _memoryListBox.Visibility != Visibility.Visible)
            {
                _memoryListBox.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Notifică schimbări în memorie
        /// </summary>
        private void OnMemoryChanged()
        {
            MemoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Clasa care reprezintă un element în stiva de memorie
    /// </summary>
    public class MemoryItem
    {
        public double Value { get; }
        public DateTime Timestamp { get; }

        public MemoryItem(double value, DateTime timestamp)
        {
            Value = value;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Returnează reprezentarea ca string a unui element din memorie
        /// </summary>
        public override string ToString()
        {
            return $"{Value} ({Timestamp:HH:mm:ss})";
        }
    }
}