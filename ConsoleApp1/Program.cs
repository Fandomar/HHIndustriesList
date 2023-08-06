using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using System.IO;

namespace ShopParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var spheres = new List<string>();
            var childSpheres = new List<string>();
            var href = "https://hh.ru/employers_company";
            var hrefs = new List<string>();
            
            var requester = new DefaultHttpRequester();
            var config = Configuration.Default
                .With(requester)
                .WithDefaultLoader();
            var context = BrowsingContext.New(config);

            try
            {
                var document = context.OpenAsync(href).Result;
                
                var rows = document.QuerySelectorAll(".employers-company__item");

                foreach (var row in rows)
                {
                    spheres.Add(row.Text()?.Trim());

                    hrefs.Add("https://hh.ru" + $"{row.GetAttribute("Href")}");  
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            foreach (string url in hrefs)
            {
                try
                {
                    var document = context.OpenAsync(url).Result;
                    
                    var rows = document.QuerySelectorAll(".employers-sub-industries__item");

                    foreach (var row in rows)
                    {
                        childSpheres.Add(row.Text()?.Trim());
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }

            using (StreamWriter sw = new StreamWriter("db.txt", true, Encoding.UTF8))
            {
                string line = string.Empty;

                foreach (var sphere in spheres)
                {
                    line += $"{sphere}, ";
                }

                sw.WriteLine(line);

                line = string.Empty;

                foreach (var childSphere in childSpheres)
                {
                    line += $"{childSphere}, ";
                }

                sw.WriteLine(line);
            }
        }
    }
}
