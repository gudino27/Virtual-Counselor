using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Collections.Generic;
using System;
using System.Linq;

partial class Program
{
    static int Counter = 0; // Define Counter as an integer
    static ChromeDriver driver; // Declare driver as a class-level variable

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
        using (driver = new ChromeDriver(service,options))
        {
            web();
            Console.Clear();
            coursedata();
        }
    }

    static void web()
    {
        // Navigate to the page
        driver.Navigate().GoToUrl("https://schedules.wsu.edu/sectionList/&campus=Pullman&term=spring&year=2025&prefix=Cpt%20S");

        // Wait for the table to load
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        // Dictionary to store course details
    }
    static void coursedata()
    {
        Dictionary<string, List<string>> CourseDetails = new Dictionary<string, List<string>>();
        string coursename = "";
        int count = 0;
        int last_sectionint=0;
        var last_section = "";

        // Loop through each row of the table (starting from the first course)
        for (int i = 1; i <= 463; i = i + 6)  // Total 463 rows as per your example, adjust if there are more/less
        {
            try
            {
                Counter++;
                // Get the course name from each row (using the XPath for the course)
                var courseXPath = $"/html/body/div[1]/div[3]/div/div[2]/main/div[3]/div/div/table/tbody/tr[{i}]/td";
                var row = driver.FindElement(By.XPath(courseXPath));

                 coursename = row.Text.Trim();

                // Check if the row is a valid course row (contains "CPT_S")
                if (coursename.Contains("CPT_S"))
                {
                    // Add this course to the dictionary if it doesn't exist
                    if (!CourseDetails.ContainsKey(coursename))
                    {
                        CourseDetails[coursename] = new List<string>();
                        
                    }
                }
            }
            catch (Exception)
            {
                //Console.WriteLine($"Error processing row {i}: {ex.Message}");
            }
            // Output the total number of rows processed
            // Console.WriteLine($"Total number of rows processed: {Counter}");
        }

        // Output the results for all courses
        foreach (var course in CourseDetails)
        {
           // Console.WriteLine($"{course.Key}:");
            foreach (var section in course.Value)
            {
               
               //Console.WriteLine($"course: {section}");
            }
        }
        var rows = driver.FindElements(By.XPath("//tr[contains(@class, 'sectionlistdivider')]"));
        Dictionary<string, List<string>> CourseDetail = new Dictionary<string, List<string>>();
        foreach (var row in rows)
        {
            try
            {
                // Increment the counter for each row processed
                Counter++;



                // Extract section details
                var sectionCell = row.FindElement(By.XPath(".//td[@class='sched_sec']"));
                var sectionNumber = sectionCell.Text.Trim();

                var numbercell = row.FindElement(By.XPath(".//td[@class='sched_sln']"));
                var classNumber = numbercell.Text.Trim();

                var maxenrolledcell = row.FindElement(By.XPath(".//td[@class='sched_limit']"));
                var maxenrolled = maxenrolledcell.Text.Trim();

                var enrolledcell = row.FindElement(By.XPath(".//td[@class='sched_enrl']"));
                var enrolled = enrolledcell.Text.Trim();

                var creditcell = row.FindElement(By.XPath(".//td[@class='sched_cr']"));
                var credit = creditcell.Text.Trim();
                int classNumberInt = 0;
                int maxenrolledint = 0;
                int enrolledint = 0;
                int sectionNumberInt = 0;
                if (!int.TryParse(classNumber, out classNumberInt))
                {
                    Console.WriteLine($"Failed to parse Class Number: '{classNumber}' in section '{sectionNumber}'");
                }
                if (!int.TryParse(maxenrolled, out maxenrolledint))
                {
                    Console.WriteLine($"Failed to parse Max Enrolled: '{maxenrolled}' in section '{sectionNumber}'");
                }
                if (!int.TryParse(enrolled, out enrolledint))
                {
                    Console.WriteLine($"Failed to parse Enrolled: '{enrolled}' in section '{sectionNumber}'");
                }
                if (sectionNumber.Contains("Lab"))
                {
                    if (!int.TryParse(sectionNumber.Substring(0, 2), out sectionNumberInt))
                    {
                    }
                }
                else
                {
                    if (!int.TryParse(sectionNumber.Substring(0, 2), out sectionNumberInt))
                    {
                    }
                }
                //if (!int.TryParse(credit, out creditint))
                //{
                //    Console.WriteLine($"Failed to parse Credit: '{credit}' in section '{sectionNumber}'");
                //}

                int spotsleft = Math.Abs(maxenrolledint - enrolledint);
                string waitspot = "";
                string sec = "";
                // Add the course section details to the dictionary under the respective course name
                string status = maxenrolledint == enrolledint ? "Full" : (maxenrolledint > enrolledint ? "Open" : "Waitlisted");
                if (status == "Waitlisted")
                {
                    waitspot = ($"Wait list: {spotsleft}");
                }
                else
                {
                    waitspot = ($"Spots Left: {spotsleft}");
                }
                if (!sectionNumber.Contains("Lab"))
                {
                    sec = ($"Section Number: {sectionNumber}    ");
                }
                else
                {


                    sec = ($"Section Number: {sectionNumber}");

                }



                //Dictionary<string, List<string>> CourseDetails = new Dictionary<string, List<string>>();

                if (sectionNumber == "01" || sectionNumberInt < last_sectionint || sectionNumberInt == 2 && last_sectionint > 3)
                {

                    coursename = CourseDetails.Keys.ElementAt(count);
                    Console.WriteLine($"Course:{coursename}");
                    count++;
                }
                else if (sectionNumber.Contains("Lab"))
                {
                    Console.WriteLine($"{sec} ,Credits: {credit}, Class Number: {classNumberInt:D5}, {waitspot}");
                }
                else if (last_sectionint < sectionNumberInt)
                {

                    coursename = CourseDetails.Keys.ElementAt(count);
                    Console.WriteLine($"Course:{coursename}");
                    Console.WriteLine($"{sec} ,Credits: {credit}, Class Number: {classNumberInt:D5}, {waitspot}");

                    count++;
                }
                else
                {

                    Console.WriteLine($"{sec} ,Credits: {credit}, Class Number: {classNumberInt:D5}, {waitspot}");
                }
                last_sectionint = sectionNumberInt;



            }
            catch (Exception)
            {
                //Console.WriteLine($"Error processing row: {ex.Message}");
            }
        }

        // Output the results
        foreach (var course in CourseDetail)
        {
            Console.WriteLine($"{course.Key}:");
            foreach (var detail in course.Value)
            {
                Console.WriteLine($"  {detail}");
            }
        }

        // Output the total number of rows processed
        //Console.WriteLine($"Total number of rows processed: {Counter}");
    }
}