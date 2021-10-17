using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Windows.Forms;
/// 
/// Application monitors CPU usage. You can detect if CPU is on FULL THROTLE or something is limiting its power.
/// It helps to detect if you have TPL (CPU Power) limits or CPU Turbo frequency limits.
/// To overcome TPL limits you can use software like Throttle Stop.
/// 
/// Instead to read CPU temperature, you have to run this app with Administrator privilege. 
/// Otherwise temperature reading will not work. Only CPU freq/usage will work.
/// 
/// Author:     Marcin Kaminski (https://github.com/marcin2048)
/// Data:       2021-10-17

namespace monitorCPU
{
    public partial class Form1 : Form
    {
        private double cpuUsageDouble;  //for chart 
        int cpuUsageInt = 0;            //for label
        private double cpuFreqDouble;   //for chart
        int cpuFreqInt = 0;             //for label
        int temperatureInt = 0;         //for label

        TemperatureReading temperature;
        CpuFreqReading cpuFreq;
        CpuUsageReading cpuUsage;
        
        //you can select temperature rading modes that fits your hardware
        int tempMode = 0;//sposob zbierania danych o temperaturze



        public Form1()
        {
            InitializeComponent();
            temperature = new TemperatureReading();
            cpuFreq = new CpuFreqReading();
            cpuUsage = new CpuUsageReading();
        }

 

        /// <summary>
        /// Procedure updates information on forms
        /// </summary>
        private void updateFormElements_Tick(object sender, EventArgs e)
        {

            //cpu max
            cpuFreqDouble = cpuFreq.getCpuFreq();
            cpuFreqInt = (int)Math.Round(cpuFreqDouble); //w kHz
            label3.Text = cpuFreqInt.ToString();
            //usage
            cpuUsageDouble = cpuUsage.getCpuUsage();
            cpuUsageInt = (int)Math.Round(cpuUsageDouble);
            label4.Text = cpuUsageInt.ToString();

            //update chart
            updateChartData();
        }

        
        /// <summary>
        /// Update chart series values
        /// </summary>
        private void updateChartData()
        {
            if (cartesianChart1.Series.Count == 0)
            {
                initChart();
            }
            //remove old entries, all above 60 
            for (int i = 0; i < cartesianChart1.Series.Count; i++)
                while (cartesianChart1.Series[i].Values.Count > 60)
                {
                    cartesianChart1.Series[i].Values.RemoveAt(0);
                }
            //add fresh data to series
            cartesianChart1.Series[0].Values.Add(cpuUsageDouble * cpuFreqDouble /100);
            cartesianChart1.Series[1].Values.Add(cpuFreqDouble );
        }

        /// <summary>
        /// Chart object inicjalization
        /// </summary>
        private void initChart()
        {
            // X Axis
            cartesianChart1.AxisX.Clear();
            cartesianChart1.AxisX.Add(new Axis
            {
                Title = "CPU",
                ShowLabels = false
            });
            //Y Axis 
            cartesianChart1.AxisY[0].MaxValue = 4500;
            cartesianChart1.AxisY[0].MinValue = 0;
            //Series 0 - CPU Usage
            cartesianChart1.Series.Add(new LineSeries
            {
                Values = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                LineSmoothness = 0, //straight lines, 1 really smooth lines
                PointForeground = System.Windows.Media.Brushes.Gray,
                Title = "CPU Usage (Square)",
                PointGeometry = null
            });
            //Series 1 - CPU MAX Frequency
            cartesianChart1.Series.Add(new LineSeries
            {
                Values = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                LineSmoothness = 0, //straight lines, 1 really smooth lines
                PointForeground = System.Windows.Media.Brushes.Gray,
                Title = "CPU Frequency (Diamond)",
                PointGeometry = null
            });
            //no animiation
            cartesianChart1.AnimationsSpeed = TimeSpan.Zero;
            cartesianChart1.DisableAnimations = true;
            cartesianChart1.BackColor = System.Drawing.Color.Gray;
        }

        /// <summary>
        /// Sets always on top bit
        /// </summary>
        private void setAlwaysOnTop(object sender, EventArgs e)
        {
            //
            this.TopMost = checkBox1.Checked;
        }

 
        /// <summary>
        /// Timer responsible for query
        /// </summary>
        void getTemperature_Tick(object sender, EventArgs e)
        {
            timerCpuTemp.Enabled = false;//stop until reading ends
            //read temperature
            temperatureInt = (int)Math.Round(temperature.getTemperature(tempMode));
            //show description            
            switch (tempMode)
            {
                case 0: label7.Text = "Temp(OH):"; break;
                case 1: label7.Text = "Temp(M1):"; break;
                case 2: label7.Text = "Temp(HP):"; break;
                case 3: label7.Text = "Temp(Li):"; break;
                case 4: label7.Text = "Temp(M2):"; break;
                default: break;
            }
            label8.Text = temperatureInt.ToString();
            //
            timerCpuTemp.Enabled = true;  //reading finished, trigger next query
        }

   
        /// <summary>
        /// When label clicked, we change temperature reading method
        /// </summary>
        private void changeTempMode_Click(object sender, EventArgs e)
        {
            if (tempMode < 4) tempMode++; else tempMode=0;
        }
    }
}
