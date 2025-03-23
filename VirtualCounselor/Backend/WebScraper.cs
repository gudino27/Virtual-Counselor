

namespace VirtualCounselor
{
    /// <summary>
    /// The WebScraper class will read all of the data from the WSU cite.
    /// It'll then parse that data into seperate chunks that it can then pass to the Course and Degree Managers.
    /// </summary>
 using OpenQA.Selenium;
 using OpenQA.Selenium.Chrome;
 using OpenQA.Selenium.Support.UI;
 using System;
 using System.Collections.Concurrent;
 using System.Collections.Generic;
 using System.Linq;
 using System.Text;
 using System.Threading;
 using System.Threading.Tasks;
     public class CourseData
 {
     public string? CourseName { get; set; }
     public string? Title { get; set; }
     public List<SectionData> Sections { get; set; } = [];
 }

 public class SectionData
 {
     public string? SectionNumber { get; set; }
     public string? Credits { get; set; }
     public int ClassNumber { get; set; }
     public int SpotsLeft { get; set; }
     public string? Status { get; set; }
 }
 public class CampusData
 {
     public int Id { get; set; }
     public string Name { get; set; } = string.Empty;
 }

 public class TermData
 {
     public string Code { get; set; } = string.Empty;
     public string Description { get; set; } = string.Empty;
 }

 public class StaticDataService
 {
     public List<CampusData> Campuses { get; } = new List<CampusData>
 {
     new CampusData { Id = 1, Name = "Everett" },
     new CampusData { Id = 2, Name = "Global" },
     new CampusData { Id = 3, Name = "Pullman" },
     new CampusData { Id = 4, Name = "Spokane" },
     new CampusData { Id = 5, Name = "Tri-Cities" },
     new CampusData { Id = 6, Name = "Vancouver" }
 };

     public List<TermData> Terms { get; } = new List<TermData>
 {
     new TermData { Code = "fall", Description = "Fall 2025" },
     new TermData { Code = "spring", Description = "Spring 2025" },
     new TermData { Code = "summer", Description = "Summer 2025" }
 };
 }

 partial class Sprint4
 {
     static ChromeDriver? mainDriver;

     static void Main()
     {
         var options = new ChromeOptions();
         options.AddArgument("--disable-usb");
         options.AddArgument("--disable-usb-discovery");
         options.AddArgument("--headless");
         options.AddArgument("--log-level=3");
         options.AddArgument("--disable-gpu");
         options.AddArgument("--disable-logging");

         var service = ChromeDriverService.CreateDefaultService();
         service.SuppressInitialDiagnosticInformation = true;
         service.HideCommandPromptWindow = true;
         service.EnableVerboseLogging = false;
         var campuses = new Dictionary<int, string>
         {
             { 1, "Everett" },
             { 2, "Global" },
             { 3, "Pullman" },
             { 4, "Spokane" },
             { 5, "Tri-Cities" },
             { 6, "Vancouver" }
         };

         var terms = new Dictionary<string, string>
         {
             { "fall", "Fall 2025" },
             { "spring", "Spring 2025" },
             { "summer", "Summer 2025" }
         };

         using (mainDriver = new ChromeDriver(service, options))
         {
             mainDriver.Navigate().GoToUrl("https://schedules.wsu.edu");
             for (int i = 1; i <= 6; i++)
             {
                 string selectedTerm = terms["fall"];
                 string selectedCampus = campuses[i];
                 Console.WriteLine(campuses[i]);
                 ClickEvent(selectedCampus, selectedTerm, mainDriver);
                 CourseLoadParallel(selectedCampus, selectedTerm);
                 mainDriver.Navigate().Back();
                 Thread.Sleep(5000);
                 selectedTerm = terms["spring"];
                 ClickEvent(selectedCampus, selectedTerm, mainDriver);
                 CourseLoadParallel(selectedCampus, selectedTerm);
                 mainDriver.Navigate().Back();
                 Thread.Sleep(5000);
                 selectedTerm = terms["summer"];
                 ClickEvent(selectedCampus, selectedTerm, mainDriver);
                 CourseLoadParallel(selectedCampus, selectedTerm);
                 mainDriver.Navigate().Back();
                 Thread.Sleep(5000);
             }
         }
     }
     public static void ClickEvent(string campus, string term, ChromeDriver driver)
     {
         Console.WriteLine($"Clicking on {campus} and {term}.");
         try
         {
             var campusContainers = driver.FindElements(By.ClassName("header_wrapper"));
             foreach (var container in campusContainers)
             {
                 var cityElement = container.FindElement(By.ClassName("City"));
                 string cityName = cityElement.Text.Trim();
                 if (cityName.Equals(campus, StringComparison.OrdinalIgnoreCase))
                 {
                     var semesterLinks = container.FindElements(By.ClassName("nav-item"));
                     foreach (var link in semesterLinks)
                     {
                         if (link.Text.Contains(term, StringComparison.OrdinalIgnoreCase))
                         {
                             link.FindElement(By.TagName("a")).Click();
                             Thread.Sleep(8000);
                             return;
                         }
                     }
                 }
             }
             Console.WriteLine("Semester link not found.");
         }
         catch (NoSuchElementException ex)
         {
             Console.WriteLine("Element not found: " + ex.Message);
         }
     }
     static int previousRowCount = -1;

     public static void CourseLoadParallel(string campus, string term)
     {
         if (mainDriver == null)
         {
             throw new InvalidOperationException("The mainDriver is not initialized.");
         }

         var wait = new WebDriverWait(mainDriver, TimeSpan.FromSeconds(20));
         string tableBodyXPath = "/html/body/div[1]/div[3]/div/div[2]/main/div[2]/div/div/div/table/tbody";
         string rowXPath = tableBodyXPath + "/tr";

         // Wait until there are rows and they differ from the previous term's count (if available)
         int rowCount = 0;
         wait.Until(driver =>
         {
             var rows = driver.FindElements(By.XPath(rowXPath));
             if (rows.Count > 0 && (previousRowCount == -1 || rows.Count != previousRowCount))
             {
                 rowCount = rows.Count;
                 return true;
             }
             return false;
         });
         previousRowCount = rowCount; // Update for next term check
         Console.WriteLine($"Found {rowCount} rows in the zebra table.");

         int partitionCount = 15;
         int partitionSize = rowCount / partitionCount;
         int remainder = rowCount % partitionCount;

         var partitions = new List<(int start, int end)>();
         int startIndex = 1;

         for (int i = 0; i < partitionCount; i++)
         {
             int extraRow = (i < remainder) ? 1 : 0;
             int endIndex = startIndex + partitionSize + extraRow - 1;
             partitions.Add((startIndex, endIndex));
             startIndex = endIndex + 1;
         }

         var results = new ConcurrentDictionary<string, List<CourseData>>();

         var tasks = partitions.Select(partition => Task.Run(() =>
         {
             var courseOptions = new ChromeOptions();
             courseOptions.AddArgument("--disable-usb");
             courseOptions.AddArgument("--disable-usb-discovery");
             courseOptions.AddArgument("--headless");
             courseOptions.AddArgument("--log-level=3");
             courseOptions.AddArgument("--disable-gpu");
             courseOptions.AddArgument("--disable-logging");

             var courseService = ChromeDriverService.CreateDefaultService();
             courseService.SuppressInitialDiagnosticInformation = true;
             courseService.HideCommandPromptWindow = true;
             courseService.EnableVerboseLogging = false;

             using var driver = new ChromeDriver(courseService, courseOptions);
             driver.Navigate().GoToUrl("https://schedules.wsu.edu");
             Thread.Sleep(8000);
             ClickEvent(campus, term, driver);

             for (int i = partition.start; i <= partition.end; i++)
             {
                 try
                 {
                     string currentRowXPath = $"{tableBodyXPath}/tr[{i}]";
                     var rowElement = driver.FindElement(By.XPath(currentRowXPath));
                     var subjectElement = rowElement.FindElement(By.XPath($"{currentRowXPath}/td[2]/a"));
                     string subjectText = subjectElement.Text.Trim();
                     var subjectTitle = rowElement.FindElement(By.XPath($"{currentRowXPath}/td[3]"));
                     string subjectTitleText = subjectTitle.Text.Trim();

                     subjectElement.Click();
                     Thread.Sleep(5000);

                     var courseData = ProcessCourseData(driver, subjectText, subjectTitleText);

                     if (courseData.Count > 0)
                     {
                         results[subjectText] = courseData;
                     }

                     driver.Navigate().Back();
                     Thread.Sleep(5000);
                 }
                 catch (Exception)
                 {
                     // Handle errors appropriately
                 }
             }
             driver.Quit();
         })).ToArray();

         Task.WhenAll(tasks).Wait();

         foreach (var kvp in results.OrderBy(kvp => kvp.Key))
         {
             var course = kvp.Value.FirstOrDefault();
             string title = course?.Title ?? "No Title";
             Console.WriteLine($"\n======================================================================================\n\t\tTitle: {title} subject: {kvp.Key}\n======================================================================================");
             foreach (var courseData in kvp.Value)
             {
                 Console.WriteLine($"{courseData.CourseName}");
                 foreach (var section in courseData.Sections)
                 {
                     Console.WriteLine($"*Section: {section.SectionNumber}, Credits: {section.Credits}, Class Number: {section.ClassNumber}, Status: {section.Status}, Spots Left: {section.SpotsLeft}");
                 }
             }
         }
         Console.WriteLine($"\n\n");
         Thread.Sleep(5000);
     }

     static List<CourseData> ProcessCourseData(ChromeDriver driverInstance, string degree, string title)
     {
         Thread.Sleep(2000);
         List<CourseData> courses = [];
         var allRows = driverInstance.FindElements(By.XPath("//table/tbody/tr"));

         if (allRows == null || allRows.Count == 0)
         {
             return courses;
         }

         CourseData? currentCourse = null;

         foreach (var row in allRows)
         {
             string rowClass = row.GetAttribute("class") ?? "";

             if (!rowClass.Contains("sectionlistdivider"))
             {
                 string headerText = row.Text.Trim();
                 if (headerText.StartsWith(degree))
                 {
                     currentCourse = new CourseData { CourseName = headerText, Title = title }; 
                     courses.Add(currentCourse);
                 }
             }
             else if (currentCourse != null)
             {
                 try
                 {
                     var sectionCell = row.FindElement(By.XPath(".//td[@class='sched_sec']"));
                     string sectionText = sectionCell.Text.Trim();
                     var numberCell = row.FindElement(By.XPath(".//td[@class='sched_sln']"));
                     string classNumberText = numberCell.Text.Trim();
                     var maxEnrolledCell = row.FindElement(By.XPath(".//td[@class='sched_limit']"));
                     string maxEnrolledText = maxEnrolledCell.Text.Trim();
                     var enrolledCell = row.FindElement(By.XPath(".//td[@class='sched_enrl']"));
                     string enrolledText = enrolledCell.Text.Trim();
                     var creditCell = row.FindElement(By.XPath(".//td[@class='sched_cr']"));
                     string credit = creditCell.Text.Trim();

                     _ = int.TryParse(classNumberText, out int classNumberInt);
                     _ = int.TryParse(maxEnrolledText, out int maxEnrolledInt);
                     _ = int.TryParse(enrolledText, out int enrolledInt);
                     int spotsLeft = maxEnrolledInt - enrolledInt;
                     string status = "";
                     if (spotsLeft>0)
                     {
                          status = "Open";
                     }
                     else if (spotsLeft == 0)
                     {
                          status = "Full";
                     }
                     else if (spotsLeft < 0)
                     {
                         spotsLeft = Math.Abs(spotsLeft);
                          status = "Waitlist";
                     }
                     if (!sectionText.Contains("Lab"))
                     {
                         sectionText += "    ";
                     }
                     SectionData section = new()
                     {
                         SectionNumber = sectionText,
                         Credits = credit,
                         ClassNumber = classNumberInt,
                         SpotsLeft = spotsLeft,
                         Status = status
                     };

                     currentCourse.Sections.Add(section);
                 }
                 catch (Exception)
                 {
                     // Handle errors
                 }
             }
         }

         return courses;
     }

 }
}
