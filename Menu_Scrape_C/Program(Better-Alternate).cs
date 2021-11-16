using System;
using System.Collections.Generic;
using CsvHelper;
using HtmlAgilityPack;
using System.IO;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Menu_Scrape_C
{
    class Program
    {
        
        static void Main(string[] args)
        {   //passing a url to the GetMenuLinks method and checking that it can find the links to the actual menus (11-Nov-21)
            var menuLinks = GetMenuLinks("https://www.allmenus.com/ga/atlanta/-/");
            Console.WriteLine("Found {0} links", menuLinks.Count);

            var menu5 = GetMenuDetails(menuLinks);
            

            ExportToCSV(menu5);
        }

        //This method is used to return the webpage that the menu links can be scraped from (11-Nov-21)

        static HtmlDocument GetDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
                return doc;
        }

        //This method is used to grab all the links to the menus that are available on the page, and uses uri to make the paths absolute (11-Nov-21)
        //A break is added later to keep from scraping all 500 menus, perhaps here is a better place for it? (16-Nov-21)
        static List<string> GetMenuLinks(string url)
        {
            var menuLinks = new List<string>();
            HtmlDocument doc = GetDocument(url);
            HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//h4/a");
            var baseUri = new Uri(url);
            foreach (var link in linkNodes)
            {
                string href = link.Attributes["href"].Value;
                menuLinks.Add(new Uri(baseUri, href).AbsoluteUri);
            }
            return menuLinks;
        }

        //This method os resonsible for scraping the menu information from the page (16-Nov-21)
        static List<Menu> GetMenuDetails(List<string> urls)
        {
             IWebDriver driver = new ChromeDriver("C:/Users/Devin Greene/source/repos/Menu_Scrape_C/Menu_Scrape_C");
            var menu = new List<Menu>();
            var counter = 0;
            foreach (var url in urls)
            {
                HtmlDocument document = GetDocument(url);
                driver.Navigate().GoToUrl(url);
                HtmlNodeCollection dishNameXPath = document.DocumentNode.SelectNodes("//*[@id=\"menu\"]/div[3]");
                //These* keep returning Null. I am under the assumption the program as is cannot navigate into the links the menus are actually in.
                //webdriver seems to have been the solution that I was searching for (11-Nov-21)
                //*These refers to a second XPath varible, which was removed, due to the above xpath scraping the entire menu (16-Nov-21)

                var menus = new Menu();

                foreach (var node in dishNameXPath)
                {
                    menu.Add(new Menu { DishName = node.InnerText });
                    //price not showing up in CSV document, only five dish names are shown... I beilive another for loop should be added(12-Nov-21)
                    //After changing the Xpath, the program can now scrape the entirtey of a menu from a single Xpath. (16-Nov-21)

                }

            
                menu.Add(menus);

                driver.Navigate().Back();
                //ensures that the program returns to the page with the list of Menu links (11-Nov-21)

                counter++;
                if (counter == 1) break;
                //I have added this so that I am not bogging down the website by searching all 500 availble menus
            }
            return menu;
        }

        static void ExportToCSV(List<Menu> menu)
        {
            using (var writer = new StreamWriter("C:/Users/Devin Greene/Documents/Menu_Data.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(menu);
                csv.Flush(); 
                //This method did not function correctly without the flush. Unsure of Why. (12-Nov-21)
                //noted that opening files in Notepad displayed data I could not see when opening in Excel (16-Nov-21)
            }
        }

        
    }

    public class Menu
    {
        public string DishName { get; set; }
        public string Price { get; set; }

        
    }

    








}
