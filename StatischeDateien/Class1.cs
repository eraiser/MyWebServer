﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BIF.SWE1.Interfaces;
using MyWebServer;

namespace StatischeDateien
{
    public class Class1 : IPlugin
    {
        private readonly static object syncObj = new object();
        public float CanHandle(IRequest req)
        {
            if (req.Url.Segments.Contains("StatischeDateien"))
                return 1.0f;
            return 0.0f;
        }

        public IResponse Handle(IRequest req)
        {

            var seg = req.Url.Segments;

            string path = "";
            for( int i=1;i<seg.Length;i++ )
            {
                path = path + "\\" + seg[i];
            }

            Console.WriteLine(".\\" + path);
            IResponse r = new Response();
            r.StatusCode = 200;

            string top;
            string bot;
            lock (syncObj)
            {
                top = File.ReadAllText(@".\res\firstStatischeDatei.html");
                bot = File.ReadAllText(@".\res\secondStatischeDatei.html");
            }
            string mid = "";

            try
            {
                mid = File.ReadAllText(".\\"+path);
                mid = ReplaceSymbols(mid);
            }
            catch(FileNotFoundException e)
            {
                mid = "Could not find: " + ".\\" + path;
            }


            r.SetContent(top + mid + bot);

            return r;
        }

        public static string ReplaceSymbols(string str)
        {
            str = str.Replace("&", "&amp");
            str = str.Replace("<", "&lt");
            str = str.Replace(">", "&gt");
            str = str.Replace("\n", "<br>");
            return str;
        }
    }
    
}
