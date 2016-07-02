﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shadowsocks.Model;

namespace Shadowsocks.Controller
{
    /// <summary>
    /// By Lc
    /// </summary>
    public class Data
    {
        public int StartIndex { get; } = 1;

        public int Add { get; } = 12;

        public int EndIndex { get; } = 27;

        private readonly List<string> _urls = new List<string>
        {
            "http://www.ishadowsocks.net",
            "http://i.freevpnss.com"
        };

        public List<Tuple<string, string>> GetHtml_Utf8(List<string> url)
        {
            var wc = new System.Net.WebClient { Encoding = Encoding.UTF8 };
            return (from val in url let valback = val let str = wc.DownloadString(val) select valback.Contains("www.ishadowsocks.net") ? new Tuple<string, string>(str, "h4") : new Tuple<string, string>(str, "p")).ToList();
        }

        public string[] GetData(Tuple<string, string> html)
        {
            string[] rel;
            switch (html.Item2)
            {
                case "h4":
                    rel = html.Item1.Split(new[] { "<h4>", "</h4>" }, StringSplitOptions.RemoveEmptyEntries);
                    break;
                case "p":
                    var test = html.Item1.Split(new[] { "<h1>免费SS帐号-每12小时更换一次密码</h1>" }, StringSplitOptions.RemoveEmptyEntries);
                    rel = test.Length > 0 ? test[1].Split(new[] { "<p>", "</p>" }, StringSplitOptions.RemoveEmptyEntries) : null;
                    break;
                default:
                    rel = null;
                    break;
            }
            return rel;
        }

        public List<Server> GetData()
        {
            var rel = GetHtml_Utf8(_urls);

            var data = rel.Select(GetData).Where(relData => relData != null).ToList();

            var servers = new List<Server>();

            var adds = 0;

            var counter = 0;

            foreach (var val in data)
            {
                if (counter != 0)
                {
                    adds = -2;
                }
                for (var i = StartIndex; i < EndIndex; i = i + Add + adds)
                {
                    var server = new Server
                    {
                        server = SplitSenior(val[i]),
                        server_port = int.Parse(SplitSenior(val[i + 2])),
                        method = SplitSenior(val[i + 6]),
                        password = SplitSenior(val[i + 4]),
                        remarks = "",
                    };
                    if (string.IsNullOrWhiteSpace(server.password))
                    {
                        continue;
                    }
                    servers.Add(server);
                }
                counter++;
            }
            return servers;
        }

        public string SplitSenior(string str)
        {
            var rel = "";
            var extract = false;
            foreach (var item in str)
            {
                if (extract)
                {
                    rel += item;
                }
                if (item == ':' || item == '：')
                {
                    extract = true;
                }
            }
            return rel;
        }
    }
}

