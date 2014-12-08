using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Core;

namespace CSTV
{
    public partial class MainForm : Form
    {

        private Process rtmpdumpProcess = new Process();
        private bool rtmp_stream = false;
        private Playlist channelPlaylist;

        #if DEBUG
        private static string rtmpdumpPath = @"C:\Users\Nikola\Downloads\rtmpdump-2.3\rtmpdump.exe";
        #else
        private string rtmpdump_path = @"rtmpdump.exe";
        #endif

        public MainForm()
        {
            InitializeComponent();

            this.channelPlaylist = new Playlist("playlist.m3u");
            updateListBox(channelPlaylist.channelList);

        }

        private void updateListBox(List<Channel> channelList)
        {
            channelListBox.DataSource = channelList;
            channelListBox.DisplayMember = "name";
            channelListBox.ValueMember = "stream_params";
        }

        private void startRTMPStream(string stream)
        {

            rtmpdumpProcess.StartInfo.FileName = rtmpdumpPath;
            rtmpdumpProcess.StartInfo.Arguments = " " + stream + " -o output";
            rtmpdumpProcess.StartInfo.CreateNoWindow = true;
            rtmpdumpProcess.EnableRaisingEvents = true;
            rtmpdumpProcess.Exited += new EventHandler(rtmpdumpProcess_Exited);
            rtmpdumpProcess.Start();
            rtmp_stream = true;

        }

        private void rtmpdumpProcess_Exited(object sender, System.EventArgs e)
        {
            rtmp_stream = false;
            stopPlaying();
        }

        private void playChannel()
        {

            string stream = channelListBox.SelectedValue.ToString();

            if (stream.StartsWith("-r"))
            {
                startRTMPStream(stream);
                Thread.Sleep(4000);
                if (rtmp_stream)
                {
                    vlcPlayer.Media = new PathMedia("output");
                    vlcPlayer.Play();
                }
            }
            else
            {
                vlcPlayer.Media = new LocationMedia(stream);
                vlcPlayer.Play();
            }

        }

        private void stopPlaying()
        {
            vlcPlayer.Stop();
            if (rtmp_stream)
                rtmpdumpProcess.Kill();
        }

        private void watchButton_Click(object sender, EventArgs e)
        {
            playChannel();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            vlcPlayer.Play();
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            if (rtmp_stream)
                vlcPlayer.Pause();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            stopPlaying();
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            string query = searchTextBox.Text;
            if (query == "")
            {
                updateListBox(this.channelPlaylist.channelList);
                return;
            }

            List<Channel> queryChannelList = new List<Channel>();

            foreach (Channel channel in this.channelPlaylist.channelList)
            {
                if (channel.name.ToLower().Contains(query.ToLower()))
                {
                    queryChannelList.Add(channel);
                }
            }

            updateListBox(queryChannelList);

        }

        private void vlcPlayer_Stopped(Vlc.DotNet.Forms.VlcControl sender, EventArgs e)
        {
            Console.WriteLine("VLC STOPPED!");
        }

        private void vlcPlayer_Buffering(Vlc.DotNet.Forms.VlcControl sender, EventArgs e)
        {
        }

        private void vlcPlayer_Paused(Vlc.DotNet.Forms.VlcControl sender, EventArgs e)
        {
            Console.WriteLine("PAUSED");
        }

        private void vlcPlayer_EndReached(Vlc.DotNet.Forms.VlcControl sender, EventArgs e)
        {
            
            Console.WriteLine("EOF");
        }

        private void vlcPlayer_EncounteredError(Vlc.DotNet.Forms.VlcControl sender, EventArgs er)
        {
            Console.WriteLine("Error");
        }

        private void vlcPlayer_TimeChanged(Vlc.DotNet.Forms.VlcControl sender, VlcEventArgs<TimeSpan> e)
        {
            if (vlcPlayer.Media == null)
                return;
            label1.Text = string.Format(
                "{0:00}:{1:00}:{2:00}",
                e.Data.Hours,
                e.Data.Minutes,
                e.Data.Seconds);
        }

        private void vlcPlayer_Playing(Vlc.DotNet.Forms.VlcControl sender, EventArgs e)
        {
            Console.WriteLine("Playing!");
        }

    }
}
