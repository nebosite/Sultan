using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sultan
{
    public class EricRule : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string Value
        {
            get
            {
                if (_resultCount == 0) return "---";
                else return (_resultTotal / _resultCount).ToString(".000");
            }
        }

        Func<double[], double> _pick;

        double _resultTotal = 0;
        int _resultCount = 0;

        public EricRule(string name, Func<double[], double> pick)
        {
            Name = name;
            _pick = pick;
        }

        public double Pick(double[] a)
        {
            return _pick(a);
        }

        public void AddResult(double result)
        {
            _resultCount++;
            _resultTotal += result;
            RaisePropertyChanged("Value");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        internal void Clear()
        {
            _resultTotal = 0;
            _resultCount = 0;
            RaisePropertyChanged("Value");
        }
    }

}
