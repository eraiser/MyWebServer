﻿using BIF.SWE1.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebServer
{
    public class Request : IRequest
    {
        public Request(System.IO.Stream network)
        {
            this.Headers = new Dictionary<string, string>();

            StreamReader rStream = new StreamReader(network);

            
            string temp = rStream.ReadLine();

            if( !string.IsNullOrEmpty(temp) )
            {
                string[] splitStr = temp.Split(' ');
                int l = splitStr.Length;
                if( l>2 )
                {
                    this.Method = splitStr[0].ToUpper();
                    this.Url = new Url(splitStr[1]);
                }
            }
            else
            {
                this.Url = new Url("");
            }



            while (!string.IsNullOrEmpty((temp = rStream.ReadLine())))
            {
                string[] splitStr = temp.Split(':');
                if (splitStr.Length == 2)
                {
                    string key = splitStr[0].ToLower().Trim();
                    string val = splitStr[1].Trim();

                    if (this.Headers.ContainsKey(key))
                    {
                        this.Headers[key] = val;
                    }
                    else
                    {
                        this.Headers.Add(key, val);
                    }
                }
            }

            string testTemp;
            if (this.Headers.TryGetValue("content-length" , out testTemp))
            {
                int contentlengthInt = -1;
                try
                {
                    contentlengthInt = Int32.Parse(testTemp.Trim());
                }
                catch (Exception e)
                {
                    contentlengthInt = -1;
                }
                if (contentlengthInt > 0)
                {
                    byte[] bodyStream = new byte[contentlengthInt];
                    char[] buffer = new char[contentlengthInt];
                    rStream.Read(buffer, 0, contentlengthInt);
                    bodyStream = Encoding.UTF8.GetBytes(buffer);
                    contentlengthInt = -1;
                    this.ContentBytes = bodyStream;
                }

            }
        }
        public bool IsValid  { 
            get {
                if(this.Method != "GET" && this.Method != "POST")
                {
                    return false;
                }
                if(string.IsNullOrEmpty(this.Url.RawUrl))
                {
                    return false;
                }
                return true;
            } 
        }

        public string Method { get; }

        public IUrl Url { get; }

        public IDictionary<string, string> Headers { get; }

        public string UserAgent { get { return this.Headers["user-agent"]; } }

        public int HeaderCount { get{
                return this.Headers.Count(); 
            }
         }

        public int ContentLength { get {
                string testTemp;
                if (this.Headers.TryGetValue("content-length", out testTemp))
                {
                    int contentlengthInt = 0;
                    try
                    {
                        contentlengthInt = Int32.Parse(testTemp.Trim());
                    }
                    catch (Exception e)
                    {
                        contentlengthInt = 0;
                    }
                    return contentlengthInt;
                }
                return 0;
            } }

        public string ContentType => throw new NotImplementedException();

        public Stream ContentStream { get; }

        public string ContentString
        {
            get {
                if (this.ContentBytes == null)
                { return ""; }
                else
                {
                    return Encoding.UTF8.GetString(this.ContentBytes);
                }
            }
            
        }

        public byte[] ContentBytes { get; }

        public string toString()
        {
            string s = "URL: \n";
            s += this.Url.ToString();
            s += "Headers: \n";
            foreach (KeyValuePair<string, string> kvp in Headers)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                s += string.Format("Key = {0}, Value = {1} \n", kvp.Key, kvp.Value);
            }
            s += "Content: \n";
            s += ContentString;
            return s;
        }

    }
}
