using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace wpf_1
{
    public class RealTimeChartViewModel : INotifyPropertyChanged
    {

        ChartValues<double> chartValues;
        private ObservableCollection<string> labels;

        public RealTimeChartViewModel()
        {
            chartValues = new ChartValues<double>();
            labels = new ObservableCollection<string>();
        }

        public ChartValues<double> ChartValues
        {
            get { return chartValues; }
            set
            {
                chartValues = value;
                OnPropertyChanged("ChartValues");
            }
        }

        public ObservableCollection<string> Labels
        {
            get { return labels; }
            set
            {
                labels = value;
                OnPropertyChanged("Labels");
            }
        }

        public void AddDataPoint(double value)
        {
            chartValues.Add(value);
            labels.Add(DateTime.Now.ToString("HH:mm:ss"));
            OnPropertyChanged("ChartValues");
            OnPropertyChanged("Labels");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
