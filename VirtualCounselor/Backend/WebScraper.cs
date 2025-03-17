using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualBasic;
using OpenQA.Selenium.BiDi.Modules.Session;

namespace VirtualCounselor
{
    /// <summary>
    /// The WebScraper class will read all of the data from the WSU cite.
    /// It'll then parse that data into seperate chunks that it can then pass to the Course and Degree Managers.
    /// </summary>
    public class WebScraper
    {
    static ChromeDriver ? driver; // Declare driver as a class-level variable

    static void Main(string[] args)
    {
        // Initialize Chrome WebDriver (you can use Firefox or other browsers as well)
        var options = new ChromeOptions();
        options.AddArgument("--disable-usb");
        options.AddArgument("--disable-usb-discovery");
        options.AddArgument("--headless");  // Run in headless mode (without opening a browser window)
        var service = ChromeDriverService.CreateDefaultService();
        service.SuppressInitialDiagnosticInformation = true;
        service.EnableVerboseLogging = false;

        using (driver = new ChromeDriver(service, options))
        {
            LoadWebPage();
            Console.Clear();
            ProcessCourseData();
        }
    }

    static void LoadWebPage()
    {
        // Navigate to the page
        driver.Navigate().GoToUrl("https://schedules.wsu.edu/sectionList/&campus=Pullman&term=spring&year=2025&prefix=Cpt%20S");

        // Wait for the table to load (adjust the wait condition as needed)
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        wait.Until(drv => drv.FindElement(By.XPath("//table/tbody/tr")));
    }

    static void ProcessCourseData()
    {
        // Get all rows from the table body.
        // We assume that course header rows do NOT have "sectionlistdivider" in their class
        // while section rows DO have that class.
        var allRows = driver.FindElements(By.XPath("//table/tbody/tr"));

        // Keep track of the current course header text.
        string currentCourse = "";

        foreach (var row in allRows)
        {
            // Get the row's class attribute.
            string rowClass = row.GetAttribute("class") ?? "";

            // If the row is a course header row (no "sectionlistdivider")
            if (!rowClass.Contains("sectionlistdivider"))
            {
                // Extract the text.
                string headerText = row.Text.Trim();
                // Only treat header rows that start with "CPT_S" as valid course headers.
                if (headerText.StartsWith("CPT_S"))
                {
                    currentCourse = headerText;
                    Console.WriteLine($"Course:{currentCourse}");
                }
            }
            else
            {
                // Otherwise, process a section row.
                try
                {
                    // Extract section details from the current section row.
                    var sectionCell = row.FindElement(By.XPath(".//td[@class='sched_sec']"));
                    string sectionText = sectionCell.Text.Trim();  // e.g. "01", "01 Lab", "02", etc.

                    var numberCell = row.FindElement(By.XPath(".//td[@class='sched_sln']"));
                    string classNumberText = numberCell.Text.Trim();

                    var maxEnrolledCell = row.FindElement(By.XPath(".//td[@class='sched_limit']"));
                    string maxEnrolledText = maxEnrolledCell.Text.Trim();

                    var enrolledCell = row.FindElement(By.XPath(".//td[@class='sched_enrl']"));
                    string enrolledText = enrolledCell.Text.Trim();

                    var creditCell = row.FindElement(By.XPath(".//td[@class='sched_cr']"));
                    string credit = creditCell.Text.Trim();

                    // Parse the numeric values.
                    int classNumberInt = 0, maxEnrolledInt = 0, enrolledInt = 0;
                    int.TryParse(classNumberText, out classNumberInt);
                    int.TryParse(maxEnrolledText, out maxEnrolledInt);
                    int.TryParse(enrolledText, out enrolledInt);

                    // Calculate available spots.
                    int spotsLeft = Math.Abs(maxEnrolledInt - enrolledInt);
                    string status = (maxEnrolledInt == enrolledInt) ? "Full" :
                                    (maxEnrolledInt > enrolledInt ? "Open" : "Waitlisted");
                    string spotText = (status == "Waitlisted") ? $"Wait list: {spotsLeft}" : $"Spots Left: {spotsLeft}";

                    // Build the section display string using an if/else block.
                    string secDisplay;
                    if (sectionText.Contains("Lab"))
                    {
                        secDisplay = $"Section Number: {sectionText}";
                    }
                    else
                    {
                        // Left-align the section text in a 10-character field.
                        secDisplay = $"Section Number: {sectionText,-10}";
                    }

                    string sectionDetail = $"{secDisplay} ,Credits: {credit}, Class Number: {classNumberInt:D5}, {spotText}";

                    Console.WriteLine(sectionDetail);
                }
                catch (Exception)
                {
                    // Optionally log the error.
                    // Console.WriteLine($"Error processing section row: {ex.Message}");
                }
            }
        }
    }
}
}
