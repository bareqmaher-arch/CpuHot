using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using OpenHardwareMonitor.Hardware;

namespace CpuHot
{
    public partial class MainWindow : Window
    {
        private readonly Computer _computer;
        private readonly DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            // Hardware initialization (API of 0.9.4-alpha)
            _computer = new Computer
            {
                IsCpuEnabled = true
            };
            _computer.Open(/*portable=*/false);

            // Timer for 1-second refresh
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (_, __) => UpdateTemp();
            _timer.Start();

            // Initial update
            UpdateTemp();
        }

        private void UpdateTemp()
        {
            var temp = GetCpuTemp();
            TempText.Text = temp.HasValue
                ? $"{temp.Value:0} °C"
                : "-- °C";
        }

        private float? GetCpuTemp()
        {
            foreach (var hw in _computer.Hardware.Where(h => h.HardwareType == HardwareType.Cpu))
            {
                hw.Update();
                foreach (var sensor in hw.Sensors.Where(s => s.SensorType == SensorType.Temperature
                                                             && s.Name.IndexOf("Package", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    if (sensor.Value.HasValue)
                        return sensor.Value.Value;
                }
            }
            return null;
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();
            _computer.Close();
            base.OnClosed(e);
        }

        // Allow dragging the border-less window
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}