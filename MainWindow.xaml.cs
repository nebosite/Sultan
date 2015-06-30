using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sultan
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        int _count = 0;
        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                RaisePropertyChanged("Count");
            }
        }

        private int _depth;
        private int _logDepth;
        public int Depth
        {
            get { return _depth; }
            set
            {
                _depth = value;
                RaisePropertyChanged("Depth");
                _logDepth = (int) Math.Log(_depth, 2);
            } 
        }

        
        /// <summary>
        /// Obervable property: PurelyRandom
        /// </summary>
        private bool _purelyRandom;
        public bool PurelyRandom
        {
            get { return _purelyRandom; }
            set
            {
                _purelyRandom = value;
                RaisePropertyChanged("PurelyRandom");
            }
        }
        
        /// <summary>
        /// Obervable property: Normalize
        /// </summary>
        private bool _normalize;
        public bool Normalize
        {
            get { return _normalize; }
            set
            {
                _normalize = value;
                RaisePropertyChanged("Normalize");
            }
        }
        

        public string Output { get; set; }

        public ObservableCollection<JulienRule> Rules { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Depth = 10;
            Rules = new ObservableCollection<JulienRule>();

            Rules.Add(new JulienRule("PickFirst", (t, i, r, b) =>
            {
                return true;
            }));

            Rules.Add(new JulienRule("Confidence .3", (t, i, r, b) =>
            {
                if (i < 3) return false;
                var confidence = i / (double)t;
                if (r * confidence > i * .3) return true;
                return false;
            }));

            Rules.Add(new JulienRule("Confidence .35", (t, i, r, b) =>
            {
                if (i < 3) return false;
                var confidence = i / (double)t;
                if (r * confidence > i * .35) return true;
                return false;
            }));

            Rules.Add(new JulienRule("Confidence .4", (t, i, r, b) =>
            {
                if (i < 3) return false;
                var confidence = i / (double)t;
                if (r * confidence > i * .4) return true;
                return false;
            }));

            Rules.Add(new JulienRule("Confidence .45", (t, i, r, b) =>
            {
                if (i < 3) return false;
                var confidence = i / (double)t;
                if (r * confidence > i * .45) return true;
                return false;
            }));

            Rules.Add(new JulienRule("Julien", (t, i, r, b) =>
            {
                switch (i - 1)
                {
                    case 0:
                    case 1:
                    case 2:
                        break;
                    case 3:
                        if (r > 3) return true;
                        break;
                    case 4:
                        if (r > 4) return true;
                        break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        if (r > 5) return true;
                        break;
                    case 9:
                        return true;
                }

                return false;
            }));

            DataContext = this;

        }


        void Worker()
        {
            Random rand = new Random();
            
            while(true)
            {

                double[] set = new double[Depth];

                if (PurelyRandom)
                {
                    for (int i = 0; i < Depth; i++) set[i] = rand.NextDouble();
                }
                else
                {
                    // regularly spaced set of numbers, shuffled
                    for (int i = 0; i < Depth; i++)
                    {
                        var value = (i + 1.0);
                        set[i] = Normalize ? value/Depth : value;
                        if (i == 0) continue;
                        int index = rand.Next(i - 1);
                        var temp = set[index];
                        set[index] = set[i];
                        set[i] = temp;
                    }
                }

                foreach (var rule in Rules)
                {
                    for (int i = 0; i < Depth; i++)
                    {
                        var lower = 0;
                        var thisItem = set[i];
                        var compares = new bool[i];


                        for (int j = 0; j < i; j++)
                        {
                            double rankCheckItem = set[j];
                            compares[j] = rankCheckItem < thisItem;
                            if (rankCheckItem < thisItem) lower++;
                        }
                        double previousItem = 0;
                        if (i > 0) previousItem =  set[i - 1];
                        bool betterThanLast = thisItem > previousItem;
                        if (rule.Pick(Depth, i+1, lower + 1, compares))
                        {
                            rule.AddResult(thisItem);
                            break;
                        }

                        if (i == Depth - 1)
                        {
                            rule.AddResult(thisItem);
                        }
                    }
                }
                Count++;
            }
        }

        Thread workerThread = null;
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (workerThread != null) return;
            workerThread = new Thread(Worker);
            workerThread.Start();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (workerThread == null) return;
            workerThread.Abort();
            workerThread = null;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var rule in Rules) rule.Clear();
            Count = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Stop_Click(null, null);
        }

    }
}

//var result = new List<int[]>();
//var choices = new int[Depth];
//var start = new int[Depth];
//for (int i = 0; i < Depth; i++)
//{
//    choices[i] = i + 1;
//}

//GenerateSets(start, choices, result, 0);
//var newOutput = new StringBuilder();

//foreach (var ruleName in rules.Keys)
//{
//    var rule = rules[ruleName];
//    int total = 0;
//    foreach (var set in result)
//    {
//        total += rule(set);
//    }
//    newOutput.AppendLine("Rule: " + ruleName + ":  " + (double) total/result.Count);
//}

////foreach (var set in result)
////{
////    newOutput.AppendLine(string.Join(",", set));
////}

//Output = newOutput.ToString();
//RaisePropertyChanged("Output");
            
        //private void GenerateSets(int[] start, int[] choices, List<int[]> result, int digit)
        //{
        //    for (int i = 0; i < choices.Length; i++)
        //    {
        //        if (choices[i] == 0) continue;
        //        var newStart = new int[Depth];
        //        start.CopyTo(newStart,0);
        //        newStart[digit] = choices[i];
        //        if (digit == Depth - 1)
        //        {
        //            result.Add(newStart);   
        //        }
        //        else
        //        {
        //            var subChoices = new int[Depth];
        //            choices.CopyTo(subChoices,0);
        //            subChoices[i] = 0;
        //            GenerateSets(newStart, subChoices, result, digit + 1);
        //        }
        //    }
        //}