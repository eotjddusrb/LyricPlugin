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
        //전역 변수

        SortedList<double, string> SplitLists = new SortedList<double, string>();
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
            
           

            //if (PrestoSDK.PrestoService.Player.PlaybackState == Common.PlaybackState.Playing)
            // 노래가 재생중인 상태

            //PrestoSDK.PrestoService.Player.CurrentMusic.Artist.Name
            //PrestoSDK.PrestoService.Player.CurrentMusic.Album.Picture
            //가수와 앨범또한 구조체형태로 되어있음

            //PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;
            //presto에서 현재 재생중인 음악이 변경되었을 때
            //MessageBox.Show( PrestoSDK.PrestoService.Player.CurrentMusic.Path);
            //Path.GetFileNameWithoutExtension

            //String.IsNullOrWhiteSpace()
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
        
        // 재생하는 노래가 바뀔 때마다 가사를 불러올 수 있도록 함
        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            //전역변수로 설정된 시간과 가사 변수를 초기화 시킨다.
            SplitLists.Clear();
            //현재 가사창 또한 초기화 시킨다.
            //prelyric.Text = "";
            //lyricBox.Text = "";
            //postlyric.Text = "";

            // 다른 컴터에서 사용시 변경해야함
            string songDirect = @"C:\Users\AndyLee\Documents\Gitahead\LyricPlugin\Lyrics\";
            
            //현재 재생중인 음악 파일의 이름을 가져옴.
            string songName = Path.GetFileNameWithoutExtension(PrestoSDK.PrestoService.Player.CurrentMusic.Path);
            
            string[] lines = File.ReadAllLines(songDirect + songName + ".lrc");
            
            string format = @"mm\:ss\.ff";
            for (int index = 3; index < lines.Length; index++)
            {
                var splitData = lines[index].Split(']');
                var time_t = TimeSpan.ParseExact(splitData[0].Substring(1).Trim(), format, CultureInfo.InvariantCulture);

                // 현재 가사의 시간과 중복된 가사가 없을 경우
                if (!SplitLists.ContainsKey(time_t.TotalMilliseconds))
                {
                    //가사파일의 한줄의 길이와 ']'를 기준으로 나눈 길이를 비교하여 ']'가 한번 더 생겼는지 확인
                    if (lines[index].Length > splitData[0].Length + splitData[1].Length + 1)
                    {
                        IsMemberDisplay = true; //멤버 파트가 쓰여있는 가사
                        SplitLists.Add(time_t.TotalMilliseconds, splitData[2].Trim());
                    }
                    else
                    {
                        SplitLists.Add(time_t.TotalMilliseconds, splitData[1].Trim());
                    }
                }
                // 현재 가사의 시간과 중복된 시간이 있고, 유의미한 가사가 있는 경우
                else if (!String.IsNullOrWhiteSpace(splitData[1])) 
                {
                    if (lines[index].Length > splitData[0].Length + splitData[1].Length + 1)
                    {
                        IsMemberDisplay = true; //멤버 파트가 쓰여있는 가사
                        SplitLists[time_t.TotalMilliseconds] = SplitLists[time_t.TotalMilliseconds] + "\n" + splitData[2].Trim();
                    }
                    else
                    {
                        SplitLists[time_t.TotalMilliseconds] = SplitLists[time_t.TotalMilliseconds] + "\n" + splitData[1].Trim();
                        //가사에 한 줄 띄고 가사 추가
                    }
                }


            }
            //가사가 준비됨
            IsMusicPlaying = true;

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (IsMusicPlaying)
            {
                var currentTime = PrestoSDK.PrestoService.Player.Position+10;
                //Need to make a sync

                for (int i = SplitLists.Count - 1;
                     SplitLists.Keys[i] > currentTime && SplitLists.Keys[0] <= currentTime;
                     i--)
                {
                    //gonna use binary search
                    if(Math.Max(0, i-1) == 0)
                    {
                        prelyric.Text = "";
                        lyricBox.Text = SplitLists.Values[0];
                        postlyric.Text = SplitLists.Values[1] + "\n" + SplitLists.Values[2];
                    }
                    else
                    {
                        prelyric.Text = SplitLists.Values[Math.Max(0, i - 2)];
                        lyricBox.Text = SplitLists.Values[Math.Max(0, i - 1)];
                        if (i != SplitLists.Count - 1)
                            postlyric.Text = SplitLists.Values[Math.Max(0, i)] + "\n" + SplitLists.Values[i + 1];
                        else
                            postlyric.Text = SplitLists.Values[Math.Max(0, i)];
                    } 
                }
                if(SplitLists.Keys[SplitLists.Count-1] <= currentTime)
                {
                    prelyric.Text = SplitLists.Values[SplitLists.Count - 2];
                    lyricBox.Text = SplitLists.Values[SplitLists.Count - 1];
                    postlyric.Text = "감사합니다";
                }
                else if(SplitLists.Keys[0] >= currentTime)
                {
                    prelyric.Text = "";
                    lyricBox.Text = "";
                    postlyric.Text = SplitLists.Values[0]+"\n"+SplitLists.Values[1];
                }


            }
            
        }


    }
}


