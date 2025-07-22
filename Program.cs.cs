using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenHardwareMonitor.Hardware;
using System.IO;

namespace CpuHot
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        private readonly Label _tempLabel;
        private readonly Label _statusLabel;
        private readonly Timer _timer;
        private readonly Computer _computer;

        public MainForm()
        {
            Text = "CPU HOT";
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 30, 30);
            Font = new Font("Segoe UI", 10);
            ClientSize = new Size(520, 410);
            MinimumSize = new Size(220, 140);
	    Microsoft.Win32.Registry.SetValue(
   	    @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
            "CpuHot", Application.ExecutablePath);

// ---- TEMPERATURE ----
_tempLabel = new Label
{
    Dock = DockStyle.Fill,
    Text = "-- °C",
    Font = new Font("Segoe UI", 36, FontStyle.Bold),
    ForeColor = Color.White,
    TextAlign = ContentAlignment.MiddleCenter
};

// ---- STATUS ----
_statusLabel = new Label
{
    Dock = DockStyle.Bottom,
    Height = 250,                      // <-- bigger height
    Text = "CPU IS : --",
    Font = new Font("Segoe UI", 12),  // slightly smaller font
    ForeColor = Color.White,
    TextAlign = ContentAlignment.MiddleCenter,
    Margin = new Padding(0, 0, 0, 10) // push it up from the very edge
};

            Controls.Add(_tempLabel);
            Controls.Add(_statusLabel);

            // Hardware
            _computer = new Computer { IsCpuEnabled = true };
            _computer.Open(false);

            // Timer
            _timer = new Timer { Interval = 1000 };
            _timer.Tick += (_, __) => UpdateTemp();
            _timer.Start();
            UpdateTemp();
        }

        private void UpdateTemp()
        {
            float? t = GetCpuTemp();
            int temp = t.HasValue ? (int)t.Value : 0;

            _tempLabel.Text = $"{temp} °C";
            Color color;
            string status;

            switch (temp)
            {
                case <= 45:
                    color = Color.LimeGreen;
                    status = "COLD";
                    break;
                case <= 55:
                    color = Color.White;
                    status = "NORMAL";
                    break;
                case <= 65:
                    color = Color.Yellow;
                    status = "WARM";
                    break;
		 default:                          // 66-100 °C
   		 color = Color.Red;
   		 status = "HOT";
    		 System.Media.SystemSounds.Beep.Play();
         	// ---- CSV log ----
    		File.AppendAllText("cpu_hot_log.csv",
        	$"{DateTime.Now:yyyy-MM-dd HH:mm:ss},{temp}\n");  
   		 break;
            }

            _tempLabel.ForeColor = color;
            _statusLabel.Text = $"CPU IS : {status}";
            _statusLabel.ForeColor = color;
        }

        private float? GetCpuTemp()
        {
            foreach (var hw in _computer.Hardware.Where(h => h.HardwareType == HardwareType.Cpu))
            {
                hw.Update();
                foreach (var s in hw.Sensors.Where(s => s.SensorType == SensorType.Temperature &&
                                                        s.Name.IndexOf("Package", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    if (s.Value.HasValue)
                        return s.Value.Value;
                }
            }
            return null;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _timer.Stop();
            _computer.Close();
            base.OnFormClosed(e);
        }
    }
}