using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSTV
{
    class Playlist
    {
        public List<Channel> channelList;

        public Playlist(string location)
        {
            if (System.IO.File.Exists(location))
            {
                this.channelList = parsePlaylist(location);
            }
        }

        private List<Channel> parsePlaylist(string playlistLocation)
        {
            List<Channel> playlistChannels = new List<Channel>();
            string[] ignored_url_keywords = {"weebly", "devimages.apple.com", "thewiz.info", "radio", 
                                                "0.0.0.0", "127.0.0.1"
                                            };

            string[] ignored_extinf_keywords = { "israel", "radio", "test", "brasil", 
                                                   "spain", "argentina", "mexico", "uae", 
                                                   "greece", "french", "bulgaria", "hungary", 
                                                   "sweden", "italy", "italia", "(ita)"
                                               };

            List<string> playlist_data = new List<string>();

            string file_line;
            System.IO.StreamReader file = new System.IO.StreamReader(playlistLocation);
            while ((file_line = file.ReadLine()) != null)
            {
                if (file_line.Trim() != "")
                {
                    playlist_data.Add(file_line);
                }
            }

            file.Close();

            for (int i = 0; i < playlist_data.Count - 1; i++)
            {
                if (playlist_data[i].StartsWith("#EXTINF") && !playlist_data[i + 1].StartsWith("#"))
                {
                    string extinf = playlist_data[i];
                    string url = playlist_data[i + 1];
                    bool valid = true;

                    foreach (string word in ignored_extinf_keywords)
                    {
                        if (extinf.ToLower().Contains(word))
                        {
                            valid = false;
                        }
                    }

                    foreach (string word in ignored_url_keywords)
                    {
                        if (url.ToLower().Contains(word))
                        {
                            valid = false;
                        }
                    }

                    if (valid)
                    {
                        playlistChannels.Add(new Channel(extinf, url));
                    }
                }
            }

            return playlistChannels;
        }

    }
}
