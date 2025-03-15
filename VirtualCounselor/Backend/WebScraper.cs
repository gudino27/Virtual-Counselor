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
                /*
                LoadWebPage();
                Console.Clear();
                ProcessCourseData();
                */
                ProcessDegreeData();
            }
        }
        /*
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
        */

        // -----------------------------------------Degree Scrapping Stuff--------------------------------------------------
        // School of Electrical Engineering and Computer Science is page 200-212, 3 columns
        // Majors are page 202-205, Minors are page 205, only 4, 

        static void LoadDegreeWebPage()
        {
            // Navigate to the page
            driver.Navigate().GoToUrl("https://catalog.wsu.edu/General/Academics/DegreeProgram/10561");
            Thread.Sleep(2000);
        }

        static void ProcessDegreeData()
        {
            // Load computer engineering page
            LoadDegreeWebPage();

            // Get all rows from the table of course requirements
            var tableRows = driver.FindElements(By.XPath("//*[@id=\"unit-info\"]/div/table[1]/tbody/tr"));

            // Lists to store class requirements, 1 per year
            List<string> yearOneClasses = new List<string>();
            List<string> yearTwoClasses = new List<string>();
            List<string> yearThreeClasses = new List<string>();
            List<string> yearFourClasses = new List<string>();

            // current year variable
            var currentYear = yearOneClasses;

            foreach (var row in tableRows)
            {
                // Get the class of the current row
                string rowClass = row.GetAttribute("class") ?? "";

                // Get the text of the current row, only used to set currentYear
                string rowTerm = row.Text;

                // If class is "degree_sequence_Year", change the current year to the new one
                if (rowClass.Contains("degree_sequence_Year"))
                {
                    if (rowTerm.Contains("First Year"))
                    {
                        currentYear = yearOneClasses;
                    }
                    else if (rowTerm.Contains("Second Year"))
                    {
                        currentYear = yearTwoClasses;
                    }
                    else if (rowTerm.Contains("Third Year"))
                    {
                        currentYear = yearThreeClasses;
                    }
                    else if (rowTerm.Contains("Fourth Year"))
                    {
                        currentYear = yearTwoClasses;
                    }
                }

                // If rowClass is "degree_sequence_term_header", ignore it go to next row
                if (rowClass.Contains("degree_sequence_term"))
                {
                    continue;
                }

                // If rowClass is "degree_sequence_item" same the item to the list of the current year
                if (rowClass.Contains("degree_sequence_item"))
                {
                    // Separate the course name from the course credits
                    var columns = row.FindElements(By.TagName("td"));

                    // Check for two columns, one for the course name one for the credits
                    if (columns.Count >= 2)
                    {
                        string courseName = columns[0].Text.Trim();
                        string credits = columns[1].Text.Trim();

                        // Print the course and credits to the console
                        Console.WriteLine($"Course: {courseName}, Credits: {credits}");

                        // Store in current year list
                        currentYear.Add($"{courseName} ({credits} credits)");
                    }


                }

                // tell driver to click link to next degree w/ xpath

                Thread.Sleep(2000);

                // Parse degree info into list

                //repeat for all degrees, major and minor
            }
        }
    }
}
