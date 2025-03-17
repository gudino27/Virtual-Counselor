using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualBasic;
using OpenQA.Selenium.BiDi.Modules.Session;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Collections.ObjectModel;


public class WebScraper
{
    static ChromeDriver? driver; // Declare driver as a class-level variable
    static void Main(string[] args)
    {

        // Initialize Chrome WebDriver (you can use Firefox or other browsers as well)
        var options = new ChromeOptions();
        options.AddArgument("--disable-usb");
        options.AddArgument("--disable-usb-discovery");
        options.AddArgument("--headless");  // Run in headless mode (without opening a browser window)
        options.AddArgument("--log-level=3");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-logging");
        var service = ChromeDriverService.CreateDefaultService();
        service.SuppressInitialDiagnosticInformation = true;
        service.EnableVerboseLogging = false;

        using (driver = new ChromeDriver(service, options))
        {
            // Load Electrical Engineering Major WebPage
            LoadEECSDegreeWebPage();

            // Process all linked degree WebPages
            ProcessAllDegreeRequirements();
        }
    }

    static void LoadEECSDegreeWebPage()
    {
        // Navigate to the EECS course homepage, we will go to each major and minor via links on this page
        driver.Navigate().GoToUrl("https://catalog.wsu.edu/General/Academics/Info/22");
        Thread.Sleep(2000);
    }

    static void ProcessMajorData()
    {
        // ----------------------------------------Degree Description Section-----------------------------------------------

        // Get degreeInfo (everything, description paragraphs + course table)
        var degreeInfo = driver.FindElement(By.XPath("//*[@id=\"unit-info\"]/div"));

        // Convert everything to one string
        string description = degreeInfo.Text.Trim();

        // Split the string at "First Year". This will separate the description paragraphs from the table of courses below it.
        string[] descriptionArray = description.Split("First Year");

        // Print the description (element 0 of a 2 element array, element 1 is the table of courses, we don't want to print it here)
        Console.WriteLine($"{descriptionArray[0]}");

        // ----------------------------------------Degree Course Requirements Section-----------------------------------------------

        // Get all rows from the table of course requirements
        var tableRows = driver.FindElements(By.XPath("//*[@id=\"unit-info\"]/div/table[1]/tbody/tr"));

        // Get all rows from the course table
        var allRows = driver.FindElements(By.XPath("//table[@class='degree_sequence_list']/tbody/tr"));

        // Print number of rows found
        Console.WriteLine($"Found {allRows.Count} rows");

        // Loop through all rows and parse them
        foreach (var row in allRows)
        {
            // Get row class if it exists
            string rowClass = row.GetAttribute("class") ?? "";

            // Get row text if it exists
            string rowText = row.Text;

            // Extract the academic year
            if (rowClass.Contains("degree_sequence_Year") || rowText.Contains("Year"))
            {
                // Directly get the text from the row
                string yearText = row.Text.Trim();
                if (!string.IsNullOrEmpty(yearText))
                {
                    Console.WriteLine($"\n===== {yearText} =====");
                }
            }
            // Extract the term name
            else if (rowClass.Contains("degree_sequence_term_header"))
            {
                var termElement = row.FindElement(By.XPath(".//td[contains(@class, 'degree_sequence_term')]"));
                string termText = termElement.Text.Trim();
                Console.WriteLine($"\n----- {termText} -----");
            }
            else
            {
                try
                {
                    //  extract the course name
                    var courseElement = row.FindElement(By.XPath(".//td[contains(@class, 'degree_sequence_item')]"));
                    string courseText = courseElement.Text.Trim();

                    //  extract the credit value
                    string creditText = "N/A";
                    var creditElements = row.FindElements(By.XPath(".//td[@style='text-align: right;']"));
                    if (creditElements.Count > 0)
                    {
                        creditText = creditElements[0].Text.Trim();
                    }

                    Console.WriteLine($"Course: {courseText}, Credits: {creditText}");
                }

                // catch statement for NoSuchElementExceptions
                catch (NoSuchElementException ex)
                {
                    Console.WriteLine($"Error processing row: {ex.Message}");
                }
            }
        }

    }

    static void ProcessMinorData()
    {
        var description = driver.FindElement(By.XPath("//*[@id=\"unit-info\"]/div/p"));
        Console.WriteLine($"{description.Text.Trim()}");
    }

    static void ProcessAllDegreeRequirements()
    {
        // Get all major & minor links, ignore all other links
        var degreeLinks = driver.FindElements(By.XPath("//*[@id='secondary']/ul/li/a[contains(@href, '/General/Academics/DegreeProgram') or contains(@href, '/General/Academics/Minor')]"));

        // Counter for which link we are on
        int counter = 0;
        foreach (var degreeLink in degreeLinks)
        {
            // re-fetch the links each time, they become stale after degreeLink.click is ran each loop
            degreeLinks = driver.FindElements(By.XPath("//*[@id='secondary']/ul/li/a[contains(@href, '/General/Academics/DegreeProgram') or contains(@href, '/General/Academics/Minor')]"));
            
            // set current link using counter
            var currentLink = degreeLinks[counter];
            
            // Get href attribute for each link, used for differentiating major vs minor
            string majorOrMinor = currentLink.GetAttribute("href");

            // Link is for a major
            if (majorOrMinor.Contains("DegreeProgram"))
            {
                // Print the major name
                Console.WriteLine($"\n\nMajor: {currentLink.Text.Trim()}");

                // Click the link and wait
                currentLink.Click();
                Thread.Sleep(2000);

                // Process the web page
                ProcessMajorData();

                // Go back to the EECS degrees home page and wait
                driver.Navigate().Back();
                Thread.Sleep(2000);

                counter++;
            }
            // Link is for a minor
            else if (majorOrMinor.Contains("Minor"))
            {
                // Print the minor name
                Console.WriteLine($"\n\nMinor: {currentLink.Text.Trim()}");
                // Click the link and wait
                currentLink.Click();
                Thread.Sleep(2000);

                // Process the web page
                ProcessMinorData();

                // Go back to the EECS degrees home page and wait
                driver.Navigate().Back();
                Thread.Sleep(2000);

                counter++;
            }
        }
    }
}
