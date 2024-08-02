using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace InternalProcessCommunicationWithTCPDemo
{
    public class KonsoleProgressManager
    {
        public List<KonsoleProgressBar> KonsoleProgressBars { get; }

        internal int XBasePosi { get; }

        internal int YBasePosi { get; }

        public KonsoleProgressManager(int XBasePosi, int YBasePosi)
        {
            KonsoleProgressBars = new List<KonsoleProgressBar>();

            this.XBasePosi = XBasePosi;
            this.YBasePosi = YBasePosi;
        }

        public void Refresh()
        {
            var recordCursorVisible = Console.CursorVisible;

            Console.CursorVisible = false;

            for(int index = 0; index < KonsoleProgressBars.Count; index++)
            {
                var progressBar = KonsoleProgressBars[index];
                Console.SetCursorPosition(XBasePosi, YBasePosi + index); // 设置光标位置

                Console.WriteLine(progressBar.BarString);
            }

            Console.CursorVisible = recordCursorVisible;
        }

        public int FPS { get; set; } = 30;

        private Timer RefreshTimer;

        public void Start()
        {
            if (RefreshTimer is null) RefreshTimer = new Timer();
            RefreshTimer.Interval = 1.0 / FPS;
            RefreshTimer.AutoReset = false;
            RefreshTimer.Elapsed += RefreshTimer_Elapsed;
            RefreshTimer.Start();
        }

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Refresh();

            RefreshTimer.Start();
        }

        public void Stop()
        {
            if (RefreshTimer is null || !RefreshTimer.Enabled) return;
            RefreshTimer.Elapsed -= RefreshTimer_Elapsed;
            RefreshTimer.Stop();
            Refresh();
        }
    }


    public class KonsoleProgressBar
    {
        public KonsoleProgressBar(string name)
        {
            this.Name = name;
            BarString = $"{Name}: [  ] {0}%";
        }


        public void Update(int progress, int total)
        {
            int percent = (int)((double)progress / total * 100);

            // 构建进度条
            int barSize = 50; // 进度条长度
            int position = progress * barSize / total;
            string bar = new string('#', position) + new string(' ', barSize - position);
            BarString = $"{Name}: [{bar}] {percent}%";

        }

        public string Name { get; }

        internal string BarString { get; private set; }

    }
}
