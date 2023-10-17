using Caliburn.Micro;
using LiveCharts.Wpf;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Syncfusion.Data.Extensions;
using LiveCharts;
using System.Reflection;
using static wpf_1.Enums;

namespace wpf_1
{
    
    public partial class MainWindow : Window
    {
        
        
        // -------------------- CHART TANIMLAMALARI --------------------
        private Random random;
        private DispatcherTimer timer;

        //Her paket için view model tanımlamaları
        RealTimeChartViewModel viewModel1;
        RealTimeChartViewModel viewModel2;
        RealTimeChartViewModel viewModel3;
        RealTimeChartViewModel viewModel4;
        RealTimeChartViewModel viewModel5;

        //Her paket için chart tanımlamaları
        Chart chart1;
        Chart chart2;
        Chart chart3;
        Chart chart4;
        Chart chart5;

        //Her paket için frekans hesabında kullanılan frekans değişkenleri
        public int privFreq1 = 0;
        public int privFreq2 = 0;
        public int privFreq3 = 0;
        public int privFreq4 = 0;
        public int privFreq5 = 0;

        SeriesCollection piechartPaket;

        ChartValues<double> piePaket1;
        ChartValues<double> piePaket2;
        ChartValues<double> piePaket3;
        ChartValues<double> piePaket4;
        ChartValues<double> piePaket5;
        //-------------------- CHART TANIMLAMALARI --------------------


        //Her paket için proje ID sayısının tutulduğu arrayler
        public int[] packet_1 = { 0, 0, 0, 0, 0 };
        public int[] packet_2 = { 0, 0, 0, 0, 0 };
        public int[] packet_3 = { 0, 0, 0, 0, 0 };
        public int[] packet_4 = { 0, 0, 0, 0, 0 };
        public int[] packet_5 = { 0, 0, 0, 0, 0 };

        ObservableCollection<Packets> _packets = new ObservableCollection<Packets>();


        public List<Type> enums;

        public MainWindow()
        {
            InitializeComponent();

            //Namespace'teki Enum'ların listelenmesi
            Type[] typesInNamespace = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => type.Namespace == "wpf_1").ToArray();

            enums = new List<Type>();

            foreach (Type type in typesInNamespace)
            {
                if (type.IsEnum)
                {                  
                    enums.Add(type);
                }
            }

            // -------------------- FREKANS CHART TANIMLAMALARI --------------------
            chart1 = new Chart();
            chart2 = new Chart();
            chart3 = new Chart();
            chart4 = new Chart();
            chart5 = new Chart();

            viewModel1 = (RealTimeChartViewModel)chart1.DataContext;           
            viewModel2 = (RealTimeChartViewModel)chart2.DataContext;            
            viewModel3 = (RealTimeChartViewModel)chart3.DataContext;            
            viewModel4 = (RealTimeChartViewModel)chart4.DataContext;            
            viewModel5 = (RealTimeChartViewModel)chart5.DataContext;


            // -------------------- PIE CHART TANIMLAMALARI --------------------
            piePaket1 = new ChartValues<double> { 0 };
            piePaket2 = new ChartValues<double> { 0 };
            piePaket3 = new ChartValues<double> { 0 };
            piePaket4 = new ChartValues<double> { 0 };
            piePaket5 = new ChartValues<double> { 0 };


            Func<ChartPoint, string> labelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            piechartPaket = new SeriesCollection
            {
                new PieSeries
                {
                    Title = Convert.ToString((Enums.Paket_ID)1),
                    Values = piePaket1,
                    DataLabels = true,
                    LabelPoint = labelPoint,
                    Fill = Brushes.DarkBlue,
                    FontSize=12
                },
                new PieSeries
                {
                    Title = Convert.ToString((Enums.Paket_ID)2),
                    Values = piePaket2,
                    DataLabels = true,
                    LabelPoint = labelPoint,
                    Fill = Brushes.Gray,
                    FontSize=12
                },
                new PieSeries
                {
                    Title = Convert.ToString((Enums.Paket_ID)3),
                    Values = piePaket3,
                    DataLabels = true,
                    LabelPoint = labelPoint,
                    Fill = Brushes.Brown,
                    FontSize=12
                },
                new PieSeries
                {
                    Title = Convert.ToString((Enums.Paket_ID)4),
                    Values = piePaket4,
                    DataLabels = true,
                    LabelPoint = labelPoint,
                    Fill = Brushes.DarkGreen,
                    FontSize=12
                },
                new PieSeries
                {
                    Title = Convert.ToString((Enums.Paket_ID)5),
                    Values = piePaket5,
                    DataLabels = true,
                    LabelPoint = labelPoint,
                    Fill = Brushes.Orange,
                    FontSize=12
                }
            };

            pieChart.Series = piechartPaket;

            // -------------------- 1 SANİYELİK TIMER --------------------
            random = new Random();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += UpdateChartData;
            timer.Start();


            // -------------------- ZERO MQ DATA ALMA THREAD --------------------
            Thread subscriber = new Thread(new ThreadStart(receiveData));
            subscriber.IsBackground = true;
            subscriber.Start();
        }

        //Enum To Enum dönüşümü
        public string EnumToEnum(string enumName, int id)
        {
            string result = null;
            foreach (Type type in enums)
            {
                if (enumName == type.Name)
                {
                    result = Enum.Parse(type, id.ToString()).ToString();
                }
            }

            return result;
        }

        private void UpdateChartData(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                piePaket1[0] = packet_1.Sum();
                viewModel1.AddDataPoint(Convert.ToDouble(packet_1.Sum() - privFreq1));
                privFreq1 = packet_1.Sum();
                piechartPaket[0].Values = piePaket1;

                piePaket2[0] = packet_2.Sum();
                viewModel2.AddDataPoint(Convert.ToDouble(packet_2.Sum() - privFreq2));
                privFreq2= packet_2.Sum();
                piechartPaket[1].Values = piePaket2;

                piePaket3[0] = packet_3.Sum();
                viewModel3.AddDataPoint(Convert.ToDouble(packet_3.Sum() - privFreq3));
                privFreq3 = packet_3.Sum();
                piechartPaket[2].Values = piePaket3;

                piePaket4[0] = packet_4.Sum();
                viewModel4.AddDataPoint(Convert.ToDouble(packet_4.Sum() - privFreq4));
                privFreq4 = packet_4.Sum();
                piechartPaket[3].Values = piePaket4;

                piePaket5[0] = packet_5.Sum();
                viewModel5.AddDataPoint(Convert.ToDouble(packet_5.Sum() - privFreq5));
                privFreq5 = packet_5.Sum();
                piechartPaket[4].Values = piePaket5;
            });

            CommandManager.InvalidateRequerySuggested();
        }


        // -------------------- DATA GRID DETAY BLOKLARI --------------------
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }

        // -------------------- PENCERE MOUSE EVENTLERİ --------------------
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

        // -------------------- FREKANS BUTONU FONKSİYONU --------------------
        public void ButtonFrekans(object sender, RoutedEventArgs e)
        {
            
            Packets _tempPacket = (Packets)dataGrid.SelectedItem;
            string paketIDName = _tempPacket.paketID.ToString();

            if (paketIDName == Convert.ToString((Enums.Paket_ID)1))
            {
                if (!chart1.IsVisible) chart1.Visibility = Visibility.Visible;
                chart1.labelPaket.Content = paketIDName.Replace("_", " ") + " Frekans Grafiği";
            }
            else if (paketIDName == Convert.ToString((Enums.Paket_ID)2))
            {
                if (!chart2.IsVisible) chart2.Visibility = Visibility.Visible;
                chart2.labelPaket.Content = paketIDName.Replace("_", " ") + " Frekans Grafiği";
            }
            else if (paketIDName == Convert.ToString((Enums.Paket_ID)3))
            {
                if (!chart3.IsVisible) chart3.Visibility = Visibility.Visible;
                chart3.labelPaket.Content = paketIDName.Replace("_", " ") + " Frekans Grafiği";
            }
            else if (paketIDName == Convert.ToString((Enums.Paket_ID)4))
            {
                if (!chart4.IsVisible) chart4.Visibility = Visibility.Visible;
                chart4.labelPaket.Content = paketIDName.Replace("_", " ") + " Frekans Grafiği";
            }
            else if (paketIDName == Convert.ToString((Enums.Paket_ID)5))
            {
                if (!chart5.IsVisible) chart5.Visibility = Visibility.Visible;
                chart5.labelPaket.Content = paketIDName.Replace("_", " ") + " Frekans Grafiği";
            }
        }

        // --------------- PAKETİN ALINMA SÜRESİNİ HESAPLAYAN FONKSİYON ---------------
        public double getTime(TimeSpan timespan)
        {
            string time = timespan.ToString();
            string[] times = time.Split(':');

            double hour = Convert.ToDouble(times[0]);
            double minute = Convert.ToDouble(times[1]);
            double second = Convert.ToDouble(times[2]);

            return hour * 60000 * 60 + minute * 1000 * 60 + second * 1000;
        }

        // -------------------- FİLTRE FONKSİYONU --------------------
        List<Packets> filterModeList = new List<Packets>();
        public void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            filterModeList.Clear();
            if (searchBox.Text.Equals(""))
            {
                filterModeList.AddRange(_packets.ToList());
            }
            else
            {
                foreach (Packets packet in _packets)
                {
                    if (packet.imza.Contains(searchBox.Text) || packet.kaynak.Contains(searchBox.Text)
                        || packet.hedef.Contains(searchBox.Text) || packet.projeID.Contains(searchBox.Text)
                        || packet.paketID.Contains(searchBox.Text)
                        || packet.imza.Contains(searchBox.Text.ToUpper()) || packet.kaynak.Contains(searchBox.Text.ToUpper())
                        || packet.hedef.Contains(searchBox.Text.ToUpper()) || packet.projeID.Contains(searchBox.Text.ToUpper())
                        || packet.paketID.Contains(searchBox.Text.ToUpper()))
                    {
                        filterModeList.Add(packet);
                    }
                }
            }
            dataGrid.ItemsSource = filterModeList.ToList();
        }

        // -------------------- PAKETİN PROJE DETAYLARI --------------------
        public void packetDetails(byte packet, int projectNum)
        {
            switch (packet)
            {
                case (byte)1:
                    packet_1[projectNum - 1] += 1;
                    break;
                case (byte)2:
                    packet_2[projectNum - 1] += 1;
                    break;
                case (byte)3:
                    packet_3[projectNum - 1] += 1;
                    break;
                case (byte)4:
                    packet_4[projectNum - 1] += 1;
                    break;
                case (byte)5:
                    packet_5[projectNum - 1] += 1;
                    break;
            }
        }

        // -------------------- HANGİ PAKETİN GELDİĞİNİ DETECT EDEN FONKSİYON --------------------
        public int[] selectPacket(byte packet)
        {
            int[] result = null;
            switch (packet)
            {
                case (byte)1:
                    result = packet_1; break;
                case (byte)2:
                    result = packet_2; break;
                case (byte)3:
                    result = packet_3; break;
                case (byte)4:
                    result = packet_4; break;
                case (byte)5:
                    result = packet_5; break;
            }

            return result;
        }

        // -------------------- PAKET ALMA FONKSİYONU --------------------
        public void receiveData()
        {
            byte[] bytes = new byte[6];

            using (var subSocket = new SubscriberSocket())
            {

                subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect("tcp://127.0.0.1:12345");
                subSocket.SubscribeToAnyTopic();

                double time = 0;

                while (true)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    TimeSpan ts = new TimeSpan();
                    stopwatch.Start();
                    bytes = ReceivingSocketExtensions.ReceiveFrameBytes(subSocket);
                    stopwatch.Stop();

                    ts = stopwatch.Elapsed;
                    time = getTime(ts);

                    Packets packets = new Packets();

                    packets.imza = Convert.ToString((Enums.Imza)bytes[0]);
                    packets.kaynak = Convert.ToString((Enums.Kaynak)bytes[1]);
                    packets.hedef = Convert.ToString((Enums.Hedef)bytes[2]);
                    packets.paketID = Convert.ToString((Enums.Paket_ID)bytes[3]);
                    //packets.projeID = Convert.ToString((Enums.Proje_ID)bytes[4]);

                    packets.projeID = EnumToEnum(packets.paketID, (int)bytes[4]);

                    packetDetails(bytes[3], (int)bytes[4]);

                    int[] projectNum = selectPacket(bytes[3]);
                    string[] packetProject = projectNum.Select(x => x.ToString()).ToArray();

                    string totalPacket = "\nToplam Alınan " + packets.paketID + " Sayısı : " + projectNum.Sum();

                    packets.details = "Paket Alınma Süresi :  " + Convert.ToString(Math.Round(time, 2)) + " ms";
                    packets.details += "\t\t" + packets.paketID + " =>\tPROJE_ID_1 : " + packetProject[0]
                                                                + totalPacket + "\t\t\tPROJE_ID_2 : " + packetProject[1]
                                                                    + "\n\t\t\t\t\t\t\tPROJE_ID_3 : " + packetProject[2]
                                                                    + "\n\t\t\t\t\t\t\tPROJE_ID_4 : " + packetProject[3]
                                                                    + "\n\t\t\t\t\t\t\tPROJE_ID_5 : " + packetProject[4] + "\n";                                       
                    
                    dataGrid.Dispatcher.Invoke(new System.Action(() =>
                    {
                        _packets.Add(packets);
                        //dataGrid.Items.Add(packets);
                        dataGrid.ItemsSource = _packets;
                    }));
                }
            }
        }
    }
}
