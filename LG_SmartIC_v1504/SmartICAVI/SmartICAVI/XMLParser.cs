using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartICAVI
{
    class XMLParser
    {
        public static string GetText(string body, string tag)
        {
            int start = body.IndexOf("<" + tag + ">");		        // IndexOfing <Tag
            start = body.IndexOf(">", start + 1);		                // IndexOfing <Tag ...>

            int end = body.IndexOf("</" + tag + ">", start + 1);	// IndexOfing </Tag>

            if (start == -1 || end == -1)
                return "";
            else
                return body.Substring(start + 1, end - start - 1);
        }

        public static string GetAttribute(string body, string tag)
        {
            int start = body.IndexOf(tag + "=\"");
            if (start == -1) return "";

            if (start + tag.Length > body.Length) return "";

            int end = body.IndexOf("\"", start + tag.Length);
            if (end == -1) return "";

            return body.Substring(start + tag.Length, end - start - tag.Length);
        }

        public static string GetElement(string body, string tag)
        {
            int start = body.IndexOf("<" + tag + ">");
            int end = body.IndexOf("</" + tag + ">", start + 1);

            if (start == -1 || end == -1) return "";

            return body.Substring(start, end + tag.Length + 3 - start);
        }

        public static List<string> GetElements(string body, string tag)
        {
            int start = 0;
            int end = 0;
            List<string> elements = new List<string>();

            do
            {
                start = body.IndexOf("<" + tag + ">", start);
                end = body.IndexOf("</" + tag + ">", start + 1);

                if (start == -1 || end == -1) break;

                string tmp = body.Substring(start, end + tag.Length + 3 - start);
                elements.Add(tmp);

                start = end + 1;
            } while (start != -1 || end != -1);

            return elements;
        }

        public static string SetText(string data, string tag)
        {
            return string.Format("<{0}>{1}</{2}>", tag, data, tag);
        }
    }
}
