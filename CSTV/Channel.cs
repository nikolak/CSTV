using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CSTV
{
    class Channel
    {
        public string name { get; set; }
        public string url { get; set; }
        public string stream_params { get; set; }

        public Channel(string extinf, string url)
        {
            this.name = parse_extinf(extinf);
            this.url = url;
            this.stream_params = get_stream();
        }

        private bool is_upper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsLetter(input[i]) && !Char.IsUpper(input[i]))
                    return false;
            }
            return true;
        }

        private string parse_extinf(string extinf)
        {
            string[] invalid_characters = { "?", "!", ".", ",", "-", "»", "«", "USA:", "()" };
            //#EXTINF:-1 tvg-id="2164" tvg-name="BBC World News" tvg-logo="NEWS/BBC_World_News.png", BBC World News
            string[] split_data = extinf.Split(',');
            name = split_data[split_data.Length - 1];

            foreach (string c in invalid_characters)
            {
                name = name.Replace(c, "");
            }

            int s = name.IndexOf('[');
            int e = name.IndexOf(']');

            while ((s != -1) && (e != -1))
            {
                name = name.Replace(name.Substring(s, e + 1 - s), "");
                s = name.IndexOf('[');
                e = name.IndexOf(']');
            }

            name = name.Trim();

            if ((name.Length > 5) && (is_upper(name)))
            {
                return char.ToUpper(name[0]) + name.ToLower().Substring(1);
            }

            return name;
        }

        public string get_stream()
        {
            string source = this.url;

            string[] split_data = source.Split(' ');

            if (split_data.Length < 2)
            {
                return source;
            }

            source = source.Replace("playpath=", "--playpath ");
            source = source.Replace("live=", "--live ");
            source = source.Replace("pageUrl=", "--pageUrl ");
            source = source.Replace("token=", "--token ");
            source = source.Replace("timeout=", "--timeout ");
            source = source.Replace("swfUrl=", "--swfUrl ");

            split_data = source.Split(' ');

            string result = source;
            if (split_data[0].StartsWith("rtmp"))
            {
                result = String.Format("-r \"{0}\" ", split_data[0]);

                string previous_value = "";

                for (int i = 1; i < split_data.Length; i++)
                {
                    string value = split_data[i];
                    if (value.StartsWith("--"))
                    {
                        result += value;

                    }
                    else
                    {
                        if (Regex.IsMatch(value, @"^\d+$") && previous_value == "--live")
                        { //regex: true if string contains only digits
                            result += value;
                        }
                        else
                        {
                            result += String.Format("\"{0}\"", value);
                        }
                    }

                    result += " ";
                    previous_value = value;

                }

            }
            return result;
        }
    }

}
