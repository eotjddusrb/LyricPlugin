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
using System.Windows.Threading;

namespace Presto.SWCamp.Lyrics
{
    /// <summary>
    /// LyricsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricsWindow : Window
    {
        
        double[] SplitTime;
        string[] SplitLyric;
        //음악이 변경되었을때 가사정보에서 시간과 해당하는 가사를 읽어옴.
        //

        public LyricsWindow()
        {

            /////// 배워가는 곳/////
            ///

            //PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;
            //presto에서 현재 재생중인 음악이 변경되었을 때
            //MessageBox.Show( PrestoSDK.PrestoService.Player.CurrentMusic.Path);

            //Path.GetFileNameWithoutExtension

            ///////////////////////

            InitializeComponent();

            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += Timer_Tick;
            timer.Start();



            //////////////////
            ///

            if (PrestoSDK.PrestoService.Player.Position != 0) //작성중 필요한 내용 if문 수행하지않음
            {

                
                string directory = @"C:\Users\cbnu\source\repos\LyricPlugin\Musics\";
                string title = PrestoSDK.PrestoService.Player.CurrentMusic.Title;
                string[] lines = File.ReadAllLines(directory + title + ".lrc");
                string format = @"mm\:ss\.ff";
                for (int index = 3; index < lines.Length; index++)
                {
                    var splitData = lines[index].Split(']');
                    var time = TimeSpan.ParseExact(splitData[0].Substring(1).Trim(), format, CultureInfo.InvariantCulture);
                    SplitTime[index - 3] = time.TotalMilliseconds;

                    //MessageBox.Show(lines[index]);
                }
            }
            ////////////

        }

        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            
            string directory = @"C:\Users\cbnu\source\repos\LyricPlugin\Lyrics\";
            //현재 재생중인 음악 파일의 이름을 가져옴.
            string songName = Path.GetFileNameWithoutExtension(PrestoSDK.PrestoService.Player.CurrentMusic.Path);
            MessageBox.Show(songName);

            string[] lines = File.ReadAllLines(directory + songName + ".lrc");
            
            string format = @"mm\:ss\.ff";
            for (int index = 3; index < lines.Length; index++)
            {
                var splitData = lines[index].Split(']');
                var time = TimeSpan.ParseExact(splitData[0].Substring(1).Trim(), format, CultureInfo.InvariantCulture);
                //MessageBox.Show(time.ToString());
            }

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //lyricBox.Text = 
            //lyricBox.Text = PrestoSDK.PrestoService.Player.Position.ToString();
        }


    }
}


