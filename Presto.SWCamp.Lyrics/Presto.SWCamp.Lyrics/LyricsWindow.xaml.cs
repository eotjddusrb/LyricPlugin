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

        SortedList<double, string> lists = new SortedList<double, string>();

        //전역 변수

        List<double> SplitTime = new List<double>();
        List<string> SplitLyric = new List<string>();
        //음악이 변경되었을때 가사정보에서 시간과 해당하는 가사를 읽어옴.
        

        bool IsMusicPlaying = false;
        //음악이 재생되는지 확인
        bool IsMemberDisplay = false;
        //가수의 파트가 나뉘어 표시될 경우
        bool IsMultiLyric = false;
        //다수의 가사가 출력될경우

        public LyricsWindow()
        {

            /////// 배워가는 곳/////
            
            /*
            if (!lists.ContainsKey(123123))
            {
                lists.Add(300, "asdasd");
            }
            else
            {
                SplitTime[0] == lists.Keys[0];
                SplitLyric[0] == lists.Values[0];
                lists[SplitTime[0]] == SplitLyric[0];

                lists[300] = lists[300] + "\n" + "sadasd";
            }
            */


            //if (PrestoSDK.PrestoService.Player.PlaybackState == Common.PlaybackState.Playing)
            // 노래가 재생중인 상태

            //PrestoSDK.PrestoService.Player.CurrentMusic.Artist.Name
            //PrestoSDK.PrestoService.Player.CurrentMusic.Album.Picture
            //가수와 앨범또한 구조체형태로 되어있음

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

            

        }

        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            
            string songDirect = @"C:\Users\AndyLee\Documents\Gitahead\LyricPlugin\Lyrics\";
            
            //현재 재생중인 음악 파일의 이름을 가져옴.
            string songName = Path.GetFileNameWithoutExtension(PrestoSDK.PrestoService.Player.CurrentMusic.Path);
            
            string[] lines = File.ReadAllLines(songDirect + songName + ".lrc");
            
            // 현재 재생하는 음악과 읽어온 가사가 맞는지 확인
            
            for(int index = 0; index < 3; index++)
            {
                var splitData_1 = lines[index].Split(':');
                var splitData_2 = splitData_1[1].Split(']');
                var playData = splitData_2[0].Trim();
            }


            string format = @"mm\:ss\.ff";
            for (int index = 3; index < lines.Length; index++)
            {
                var splitData = lines[index].Split(']');
                var time_t = TimeSpan.ParseExact(splitData[0].Substring(1).Trim(), format, CultureInfo.InvariantCulture);
                SplitTime.Add( time_t.TotalMilliseconds);
                //if (splitData[2] == null) ;
                if (lines[index].Length > splitData[0].Length + splitData[1].Length + 1)
                {
                    IsMemberDisplay = true; //멤버 파트가 쓰여있는 가사
                    SplitLyric.Add(splitData[2].Trim());
                }
                else
                {
                    SplitLyric.Add(splitData[1].Trim());
                }
                
                if(index>4 && SplitTime[index] == SplitTime[index - 1] && IsMultiLyric != true)
                {
                    IsMultiLyric = true;
                }
                
            }
            //가사가 준비됨
            IsMusicPlaying = true;
            

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (IsMusicPlaying)
            {
                var currentTime = PrestoSDK.PrestoService.Player.Position;


                for (int i = SplitTime.Count - 1;
                     SplitTime[i] > currentTime && SplitTime[0] <= currentTime;
                     i--)
                {

                    lyricBox.Text = SplitLyric[Math.Max(0, i - 1)];
                }




            }
            
        }


    }
}


