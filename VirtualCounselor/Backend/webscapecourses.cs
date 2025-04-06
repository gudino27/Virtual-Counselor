

    /// <summary>
    /// The WebScraper class will read all of the data from the WSU cite.
    /// It'll then parse that data into seperate chunks that it can then pass to the Course and Degree Managers.

    /// </summary>
namespace BlazorApp1.Services
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


    public class Campus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Term> Terms { get; set; } = new List<Term>();
    }

    public class Term
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<CourseData> Courses { get; set; } = new List<CourseData>();
    }

    public class CourseData
    {
        public string? CourseName { get; set; }
        public string? Title { get; set; }
        public List<SectionData> Sections { get; set; } = new List<SectionData>();
    }

    public class SectionData
    {
        public string? SectionNumber { get; set; }
        public string? Credits { get; set; }
        public string? ClassNumber { get; set; }
        public int SpotsLeft { get; set; }
        public string? Status { get; set; }
        public string? Days { get; set; }
        public string? Time { get; set; }
        public string? Location { get; set; }
        public string? Instructor { get; set; }
        public List<string> CourseDetails { get; set; } = new List<string>();
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
        public static List<Campus> CampusesList { get; set; } = new List<Campus>();

        public static void Runall()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
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

            var terms = new Dictionary<int, string>
    {
        { 1, "Fall 2025" },
        { 2, "Spring 2025" },
        { 3, "Summer 2025" }
    };

            using (mainDriver = new ChromeDriver(service, options))
            {
                mainDriver.Navigate().GoToUrl("https://schedules.wsu.edu");

                for (int i = 1; i <= 1; i++)
                {
                    for (int j = 1; j <= 1; j++)
                    {
                        Console.WriteLine($"{campuses[i]} {terms[j]}");
                        ClickEvent(campuses[i], terms[j], mainDriver);
                        Thread.Sleep(5000);
                        CourseLoadParallel(campuses[i], terms[j]);
                        mainDriver.Navigate().Back();
                    }

                }

            }
            watch.Stop();
            var elapsedminutes = watch.ElapsedMilliseconds / 60000;
            var reminderelapsedseconds = (watch.ElapsedMilliseconds % 60000) / 1000;
            Console.WriteLine($"Time elapsed: {elapsedminutes} minutes and {reminderelapsedseconds} seconds");         
        }
        public static void AddCourseData(string campusName, string termCode, string termDescription, List<CourseData> scrapedCourses)
        {
            Campus? campus = CampusesList.FirstOrDefault(c =>
                c.Name.Equals(campusName, StringComparison.OrdinalIgnoreCase));
            if (campus == null)
            {
                campus = new Campus { Id = CampusesList.Count + 1, Name = campusName };
                CampusesList.Add(campus);
            }

            Term? term = campus.Terms.FirstOrDefault(t =>
                t.Description.Equals(termDescription, StringComparison.OrdinalIgnoreCase));
            if (term == null)
            {
                term = new Term { Code = termCode, Description = termDescription };
                campus.Terms.Add(term);
            }

            term.Courses.AddRange(scrapedCourses);
        }
        public static void DateLoad()
        {
            DateTime current = DateTime.Now;
            DateTime february10 = new DateTime(2025, 2, 10);
            DateTime march5 = new DateTime(2025, 3, 5);
            DateTime june1 = new DateTime(2025, 6, 1);



            var watch = System.Diagnostics.Stopwatch.StartNew();
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

            var terms = new Dictionary<int, string>
    {
        { 1, "Fall 2025" },
        { 2, "Spring 2025" },
        { 3, "Summer 2025" }
    };

            using (mainDriver = new ChromeDriver(service, options))
            {
                mainDriver.Navigate().GoToUrl("https://schedules.wsu.edu");
                if (current.Year == 2025)
                {
                    if (current > march5)
                    {
                        for (int i = 1; i <= 6; i++)
                        {
                           // Console.WriteLine($"{campuses[i]} {terms[1]}");
                            ClickEvent(campuses[i], terms[1], mainDriver);
                            Thread.Sleep(5000);
                            CourseLoadParallel(campuses[i], terms[1]);
                            mainDriver.Navigate().Back();
                        }
                    }
                    else if (current > june1)
                    {
                        for (int i = 1; i <= 6; i++)
                        {
                           // Console.WriteLine($"{campuses[i]} {terms[2]}");
                            ClickEvent(campuses[i], terms[2], mainDriver);
                            Thread.Sleep(5000);
                            CourseLoadParallel(campuses[i], terms[2]);
                            mainDriver.Navigate().Back();
                        }
                    }

                    else if (current > february10)
                    {
                        for (int i = 1; i <= 6; i++)
                        {
                            //Console.WriteLine($"{campuses[i]} {terms[3]}");
                            ClickEvent(campuses[i], terms[3], mainDriver);
                            Thread.Sleep(5000);
                            CourseLoadParallel(campuses[i], terms[3]);
                            mainDriver.Navigate().Back();
                        }
                    }
                }

            }
            watch.Stop();
            var elapsedminutes = watch.ElapsedMilliseconds / 60000;
            var reminderelapsedseconds = (watch.ElapsedMilliseconds % 60000) / 1000;
            Console.WriteLine($"Time elapsed: {elapsedminutes} minutes and {reminderelapsedseconds} seconds");
        }
        public static void ClickEvent(string campus, string term, ChromeDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            try
            {
                wait.Until(d => d.FindElements(By.ClassName("header_wrapper")).Count > 0);

                var campusContainers = driver.FindElements(By.ClassName("header_wrapper"));
                foreach (var container in campusContainers)
                {
                    var cityElement = container.FindElement(By.ClassName("City"));
                    if (cityElement.Text.Trim().Equals(campus, StringComparison.OrdinalIgnoreCase))
                    {
                        var semesterLinks = wait.Until(d => container.FindElements(By.ClassName("nav-item")));
                        foreach (var link in semesterLinks)
                        {
                            if (link.Text.Contains(term, StringComparison.OrdinalIgnoreCase))
                            {
                                wait.Until(d => link.FindElement(By.TagName("a"))).Click();

                                Thread.Sleep(10000);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ClickEvent: {ex.Message}");
            }
        }
        public static void CourseLoadParallel(string campus, string term)
        {
            if (mainDriver == null)
            {
                throw new InvalidOperationException("The mainDriver is not initialized.");
            }

            var wait = new WebDriverWait(mainDriver, TimeSpan.FromSeconds(20));
            string tableBodyXPath = "/html/body/div[1]/div[3]/div/div[2]/main/div[2]/div/div/div/table/tbody";
            string rowXPath = tableBodyXPath + "/tr";

            int rowCount = 0;
            wait.Until(driver =>
            {
                var rows = driver.FindElements(By.XPath(rowXPath));
                if (rows.Count > 0)
                {
                    rowCount = rows.Count;
                    return true;
                }
                return false;
            });
            // Console.WriteLine($"Found {rowCount} rows in the zebra table.");

            var expectedSubjects = new HashSet<string>();
            var mainRows = mainDriver.FindElements(By.XPath(rowXPath));
            foreach (var row in mainRows)
            {
                try
                {
                    var subjElement = row.FindElement(By.XPath("./td[2]/a"));
                    expectedSubjects.Add(subjElement.Text.Trim());
                }
                catch { }
            }
            // Console.WriteLine($"Expected subjects count: {expectedSubjects.Count}");

            int partitionCount = Math.Min(20, rowCount);
            int basePartitionSize = rowCount / partitionCount;
            int remainder = rowCount % partitionCount;
            var partitions = new List<(int start, int end)>();
            int startIndex = 1;
            for (int i = 0; i < partitionCount; i++)
            {
                int partitionSize = basePartitionSize + (i < remainder ? 1 : 0);
                int endIndex = startIndex + partitionSize - 1; 
                partitions.Add((startIndex, endIndex));
                startIndex = endIndex + 1;
            }
            if (startIndex <= rowCount)
            {
                partitions.Add((startIndex, rowCount));
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
                Thread.Sleep(10000);
                ClickEvent(campus, term, driver);
                wait.Until(d => d.FindElements(By.XPath(rowXPath)).Count == rowCount);

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

                        wait.Until(d => subjectElement.Displayed);
                        subjectElement.Click();
                        wait.Until(d => d.FindElements(By.XPath("//table/tbody/tr")).Count > 0);
                        var courseData = ProcessCourseData(driver, subjectText, subjectTitleText);

                        if (courseData.Count > 0)
                        {
                            results.AddOrUpdate(subjectText, courseData, (key, existing) =>
                            {
                                existing.AddRange(courseData);
                                return existing;
                            });
                        }
                        driver.Navigate().Back();
                        Thread.Sleep(5000);
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine($"Error processing row {i}: {ex.Message}");
                    }
                }
                driver.Quit();
            })).ToArray();

            Task.WhenAll(tasks).Wait();

            //  Missing Rows Check & Retry
            var processedSubjects = new HashSet<string>(results.Keys);
            var missingSubjects = expectedSubjects.Except(processedSubjects).ToList();
            if (missingSubjects.Count > 0)
            {
                Console.WriteLine($"Retrying missing subjects: {string.Join(", ", missingSubjects)}");
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

                using var missingDriver = new ChromeDriver(courseService, courseOptions);
                missingDriver.Navigate().GoToUrl("https://schedules.wsu.edu");
                Thread.Sleep(3000);
                ClickEvent(campus, term, missingDriver);
                wait.Until(d => d.FindElements(By.XPath(rowXPath)).Count == rowCount);
                foreach (var subject in missingSubjects)
                {
                    try
                    {
                        Thread.Sleep(2000);
                        var rowElement = missingDriver.FindElement(By.XPath($"//td/a[normalize-space(text())='{subject}']/ancestor::tr"));
                        var subjectTitle = rowElement.FindElement(By.XPath("./td[3]"));
                        string subjectTitleText = subjectTitle.Text.Trim();
                        var subjectElement = rowElement.FindElement(By.XPath("./td[2]/a"));
                        subjectElement.Click();
                        wait.Until(d => d.FindElements(By.XPath("//table/tbody/tr")).Count > 0);
                        var courseData = ProcessCourseData(missingDriver, subject, subjectTitleText);
                        if (courseData.Count > 0)
                        {
                            results.AddOrUpdate(subject, courseData, (key, existing) =>
                            {
                                existing.AddRange(courseData);
                                return existing;
                            });
                        }
                        missingDriver.Navigate().Back();
                        Thread.Sleep(5000);
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine($"Failed to retry subject {subject}: {ex.Message}");
                    }
                }
                missingDriver.Quit();
            }


            foreach (var kvp in results.OrderBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase))
            {
                //Console.WriteLine($"\n===========================================================  Subject: {kvp.Key}  ===========================================================");

                foreach (var course in kvp.Value.OrderBy(c => c.CourseName))  // Sort courses by name
                {
                    //Console.WriteLine($"{course.CourseName}");

                    foreach (var section in course.Sections.OrderBy(s => s.SectionNumber))  // Sort sections numerically
                    {
                        //Console.WriteLine($"*   Section: {section.SectionNumber}, Credits: {section.Credits}, Class Number: {section.ClassNumber},Status: {section.Status}, Spots Left: {section.SpotsLeft}");
                        //Console.WriteLine($"    *   Days: {section.Days}, Time: {section.Time}, Location: {section.Location}, Instructor: {section.Instructor}");
                        if (section.CourseDetails.Any())
                        {
                            foreach (var detail in section.CourseDetails)
                            {
                                // Console.WriteLine($"         {detail}");
                            }
                        }
                    }
                }
            }
            //Console.WriteLine($"Total courses processed: {processedSubjects.Count()}");
            // Console.WriteLine($"Missing numbers: {expectedSubjects.Count - processedSubjects.Count}");
            Thread.Sleep(5000);
            // Add the scraped courses to the campus and term
            var allScrapedCourses = results.Values.SelectMany(courses => courses).ToList();
            AddCourseData(campus, term, term, allScrapedCourses);
        }
        static List<CourseData> ProcessCourseData(ChromeDriver driverInstance, string degree, string title)
        {
            Thread.Sleep(7000);
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
                        if (rowClass.Contains("comment-text"))
                        {
                            continue;
                        }
                        else
                        {
                            currentCourse = new CourseData { CourseName = headerText, Title = title };
                            courses.Add(currentCourse);
                        }

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
                        var dayscell = row.FindElement(By.XPath(".//td[@class='sched_days']"));
                        string fullText = dayscell.Text.Trim();


                        var locationCell = row.FindElement(By.XPath(".//td[@class='sched_loc']"));
                        string location = locationCell.Text.Trim();
                        var instructorCell = row.FindElement(By.XPath(".//td[@class='sched_instructor']"));
                        string instructor = instructorCell.Text.Trim();
                        _ = int.TryParse(classNumberText, out int classNumberInt);
                        _ = int.TryParse(maxEnrolledText, out int maxEnrolledInt);
                        _ = int.TryParse(enrolledText, out int enrolledInt);
                        int spotsLeft = maxEnrolledInt - enrolledInt;
                        string status = "";
                        if (spotsLeft > 0)
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
                        classNumberText = classNumberInt.ToString().PadLeft(5, '0');
                        string daysText = "";
                        string timeText = "";

                        if (string.IsNullOrWhiteSpace(fullText) || fullText.Equals("ARRGT", StringComparison.OrdinalIgnoreCase))
                        {
                            // Handle  no set schedule
                            daysText = "ARRGT";
                            timeText = "";
                        }
                        else if (fullText.Contains(' '))
                        {
                            int spaceIndex = fullText.IndexOf(' ');
                            daysText = fullText.Substring(0, spaceIndex);
                            timeText = fullText.Substring(spaceIndex + 1);
                        }
                        else
                        {
                            // treats the whole thing as days if random thing shows up
                            daysText = fullText;
                            timeText = "";
                        }
                        SectionData section = new()
                        {
                            SectionNumber = sectionText,
                            Credits = credit,
                            ClassNumber = classNumberText,
                            SpotsLeft = spotsLeft,
                            Status = status,
                            Days = daysText,
                            Time = timeText,
                            Location = location,
                            Instructor = instructor

                        };
                        CourseData course = new()
                        {
                            Title = title
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

 }
}
