using Presto.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Presto.SWCamp.Lyrics
{
    /// <summary>
    /// LyricsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricsWindow : Window
    {
        public LyricsWindow()
        {
            InitializeComponent();

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += Timer_Tick;
            timer.Start();



            //////////////////
            ///

            string format = @"mm\:ss\.ff";
            string directory = @"C:\Users\cbnu\source\repos\LyricPlugin\Musics\";

            string[] lines = File.ReadAllLines(directory + "숀 (SHAUN) - Way Back Home.lrc");
            for (int index = 3; index < lines.Length; index++)
            {
                var splitData = lines[index].Split(']');
                var time = TimeSpan.ParseExact(splitData[0].Substring(1).Trim(), format, CultureInfo.InvariantCulture);
                MessageBox.Show(time.ToString());
            }
            ////////////

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //lyricBox.Text = 
            //lyricBox.Text = PrestoSDK.PrestoService.Player.Position.ToString();
        }



    }
}


