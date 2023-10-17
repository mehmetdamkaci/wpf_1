using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;

namespace wpf_1
{



    public partial class Chart : Window
    {
        //private RealTimeChartViewModel viewModel;
        //private Random random;
        //private DispatcherTimer timer;        
        public Chart()
        {                        
            InitializeComponent();
            RealTimeChartViewModel viewModel = new RealTimeChartViewModel();
            this.DataContext = viewModel;
            
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Collapsed;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private bool IsMaximized = false;
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1080;
                    this.Height = 720;

                    IsMaximized = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;

                    IsMaximized = true;
                }
            }
        }
    }
}
//    public class RealTimeChartViewModel : INotifyPropertyChanged
//    {
//        private ChartValues<double> chartValues;
//        private List<string> labels;
//        private int xAxisCounter;

//        public RealTimeChartViewModel()
//        {
//            chartValues = new ChartValues<double>();
//            labels = new List<string>();
//            xAxisCounter = 0;
//        }

//        public ChartValues<double> ChartValues
//        {
//            get { return chartValues; }
//            set
//            {
//                chartValues = value;
//                OnPropertyChanged("ChartValues");
//            }
//        }

//        public List<string> Labels
//        {
//            get { return labels; }
//            set
//            {
//                labels = value;
//                OnPropertyChanged("Labels");
//            }
//        }

//        public void AddDataPoint(double value)
//        {
//            chartValues.Add(value);
//            labels.Add(xAxisCounter.ToString());
//            xAxisCounter++;
//            OnPropertyChanged("ChartValues");
//            OnPropertyChanged("Labels");
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        protected void OnPropertyChanged(string propertyName)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }
//}






//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Controls.DataVisualization.Charting;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
//using Caliburn.Micro;
//using OxyPlot;
//using OxyPlot.Axes;
//using DateTimeAxis = OxyPlot.Axes.DateTimeAxis;
//using LinearAxis = OxyPlot.Axes.LinearAxis;
//using LineSeries = OxyPlot.Series.LineSeries;
//using System.Windows.Threading;

//namespace wpf_1
//{
//    public partial class Chart : Window
//    {
//        private const int MaxSecondsToShow = 20;
//        private DispatcherTimer? _timer;
//        private Random _randomGenerator;
//        public BindableCollection<SensorData> SensorData { get; set; }
//        public PlotModel SensorPlotModel { get; set; }
//        public Chart()
//        {
//            InitializePlotModel();
//            _randomGenerator = new Random();
//            SensorData = new BindableCollection<SensorData>();
//            SensorData.CollectionChanged += SensorData_CollectionChanged;

//        }

//        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
//        {
//            if (e.ChangedButton == MouseButton.Left)
//            {
//                this.DragMove();
//            }
//        }

//        private bool IsMaximized = false;
//        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
//        {
//            if (e.ClickCount == 2)
//            {
//                if (IsMaximized)
//                {
//                    this.WindowState = WindowState.Normal;
//                    this.Width = 1080;
//                    this.Height = 720;

//                    IsMaximized = false;
//                }
//                else
//                {
//                    this.WindowState = WindowState.Maximized;

//                    IsMaximized = true;
//                }
//            }
//        }


//        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
//        {
//            this.Close();
//            e.Cancel = true;
//            this.Visibility = Visibility.Hidden;
//        }


//        public void InitializePlotModel()
//        {
//            SensorPlotModel = new()
//            {
//                Title = "Demo Live Tracking",
//            };

//            SensorPlotModel.Axes.Add(new DateTimeAxis
//            {
//                Title = "TimeStamp",
//                Position = AxisPosition.Bottom,
//                StringFormat = "HH:mm:ss",
//                IntervalLength = 60,
//                Minimum = DateTimeAxis.ToDouble(DateTime.Now),
//                Maximum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(MaxSecondsToShow)),
//                IsPanEnabled = true,
//                IsZoomEnabled = true,
//                IntervalType = DateTimeIntervalType.Seconds,
//                MajorGridlineStyle = LineStyle.Solid,
//                MinorGridlineStyle = LineStyle.Solid,
//            });

//            SensorPlotModel.Axes.Add(new LinearAxis
//            {
//                Title = "Data Value",
//                Position = AxisPosition.Left,
//                IsPanEnabled = true,
//                IsZoomEnabled = true,
//                Minimum = 0,
//                Maximum = 1
//            });

//            SensorPlotModel.Series.Add(new LineSeries()
//            {
//                MarkerType = MarkerType.Circle,
//            });
//        }

//        private void SensorData_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
//        {
//            if (e.NewItems is null) return;

//            var series = SensorPlotModel.Series.OfType<LineSeries>().First();

//            var dateTimeAxis = SensorPlotModel.Axes.OfType<DateTimeAxis>().First();

//            if (!series.Points.Any())
//            {
//                dateTimeAxis.Minimum = DateTimeAxis.ToDouble(DateTime.Now);
//                dateTimeAxis.Maximum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(MaxSecondsToShow));
//            }

//            foreach (var newItem in e.NewItems)
//            {
//                if (newItem is SensorData sensorData)
//                {
//                    series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(sensorData.TimeStamp), sensorData.Data));
//                }
//            }



//            // if (series.Points.Last().X > dateTimeAxis.Maximum)
//            if (DateTimeAxis.ToDateTime(series.Points.Last().X) > DateTimeAxis.ToDateTime(dateTimeAxis.Maximum))
//            {
//                dateTimeAxis.Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(-1 * MaxSecondsToShow));
//                dateTimeAxis.Maximum = DateTimeAxis.ToDouble(DateTime.Now);
//                dateTimeAxis.Reset();
//            }

//            SensorPlotModel.InvalidatePlot(true);

//            NotifyOfPropertyChange(nameof(SensorPlotModel));
//        }

//        public bool CanStartAcquisition => _timer?.IsEnabled != true;
//        public void StartAcquisition()
//        {
//            if (_timer is null)
//            {
//                _timer = new DispatcherTimer
//                {
//                    Interval = TimeSpan.FromMilliseconds(500),
//                };

//                _timer.Tick += MockSensorRecievedData;
//            }

//            _timer.Start();
//            NotifyOfPropertyChange(nameof(CanStartAcquisition));
//            NotifyOfPropertyChange(nameof(CanStopAcquisition));

//        }

//        private void MockSensorRecievedData(object? sender, EventArgs e)
//        {
//            SensorData.Add(new()
//            {
//                TimeStamp = DateTime.Now,
//                Data = _randomGenerator.NextDouble()
//            });

//        }

//        public bool CanStopAcquisition => _timer?.IsEnabled == true;
//        public void StopAcquisition()
//        {
//            _timer?.Stop();
//            NotifyOfPropertyChange(nameof(CanStartAcquisition));
//            NotifyOfPropertyChange(nameof(CanStopAcquisition));
//        }
//    }


//    public class SensorData
//    {
//        public DateTime TimeStamp { get; set; }
//        public double Data { get; set; }
//    }



//    //public void drawData(List<KeyValuePair<string, int>> kvp)
//    //{
//    //    freqChart.Dispatcher.Invoke(new Action(() =>
//    //    {
//    //        //((AreaSeries)freqChart.Series[0]).ItemsSource = kvp;
//    //    }));

//    //}
//}
//}
