

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
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using HtmlAgilityPack;


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
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public List<string> CourseDetails { get; set; } = new List<string>();
        public List<CourseDescriptionDetails> CourseDescriptionDetails { get; set; } = new List<CourseDescriptionDetails>();

    }
    public class CourseDescriptionDetails
    {

        public string CourseDescription { get; set; } = string.Empty;
        public string CoursePrerequisite { get; set; } = string.Empty;
        public string CourseCredit { get; set; } = string.Empty;
        public string SpecialCourseFee { get; set; } = string.Empty;
        public string ConsentRequired { get; set; } = string.Empty;
        public string CrosslistedCourses { get; set; } = string.Empty;
        public string ConjointCourses { get; set; } = string.Empty;
        public string UCORE { get; set; } = string.Empty;
        public string GraduateCapstone { get; set; } = string.Empty;
        public string GERCode { get; set; } = string.Empty;
        public string WritingInTheMajor { get; set; } = string.Empty;
        public string Cooperative { get; set; } = string.Empty;
        public List<string> MeetingsInfo { get; set; } = new List<string>();
        public List<string> Instructors { get; set; } = new List<string>();
        public string InstructionMode { get; set; } = string.Empty;
        public string EnrollmentLimit { get; set; } = string.Empty;
        public string CurrentEnrollment { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string Footnotes { get; set; } = string.Empty;

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

            int partitionCount = Math.Min(30, rowCount);
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

                using var driver = new ChromeDriver(courseService, courseOptions,TimeSpan.FromSeconds(120));
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
                        /*
                         foreach (var detail in section.CourseDescriptionDetails)
                        {
                            int maxWidth =80;
                            string wrappedDescription = WrapText(detail.CourseDescription, maxWidth);
                            Console.WriteLine($"        *Course Description: {detail.CourseDescription}, Course PreRecs: {detail.CoursePrerequisite}, Credits: {detail.CourseCredit}");
                            Console.WriteLine($"        *Special Fee: {detail.SpecialCourseFee}, Consent: {detail.ConsentRequired}, Crosslisted: {detail.CrosslistedCourses}, Conjoint: {detail.ConjointCourses}");
                            Console.WriteLine($"        *UCORE: {detail.UCORE}, , GER: {detail.GERCode}, Writing: {detail.WritingInTheMajor}");
                            Console.WriteLine($"        *Cooperative: {detail.Cooperative}, Instr. Mode: {detail.InstructionMode}");
                            Console.WriteLine($"        *Comment: {detail.Comment}, Footnotes: {detail.Footnotes}");
                        }
                         */
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
            var allScrapedCourses = results.Values.SelectMany(courses => courses).ToList();
            AddCourseData(campus, term, term, allScrapedCourses);
        }
        static List<CourseData> ProcessCourseData(ChromeDriver driverInstance, string degree, string title)
        {
            Thread.Sleep(5000);
            List<CourseData> courses = new List<CourseData>();
            Dictionary<string, SectionData> sectionMap = new Dictionary<string, SectionData>();

            var allRows = driverInstance.FindElements(By.XPath("//table/tbody/tr"));
            if (allRows == null || allRows.Count == 0)
            {
                return courses;
            }

            CourseData? currentCourse = null;

            // First try block: Process sections class number all that good stuff and store mapping by class number as it is special.
            try
            {
                foreach (var row in allRows)
                {
                    string rowClass = row.GetAttribute("class") ?? "";
                    if (!rowClass.Contains("sectionlistdivider"))
                    {
                        string headerText = row.Text.Trim();
                        if (headerText.StartsWith(degree))
                        {
                            if (rowClass.Contains("comment-text"))
                                continue;
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
                            var daysCell = row.FindElement(By.XPath(".//td[@class='sched_days']"));
                            string fullText = daysCell.Text.Trim();
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
                                status = "Open";
                            else if (spotsLeft == 0)
                                status = "Full";
                            else { spotsLeft = Math.Abs(spotsLeft); status = "Waitlist"; }

                            if (!sectionText.Contains("Lab"))
                                sectionText += "    ";
                            classNumberText = classNumberInt.ToString().PadLeft(5, '0');

                            string daysText = "";
                            string timeText = "";
                            if (string.IsNullOrWhiteSpace(fullText) || fullText.Equals("ARRGT", StringComparison.OrdinalIgnoreCase))
                            {
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
                                daysText = fullText;
                                timeText = "";
                            }

                            SectionData section = new SectionData()
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

                            // each classNumberText is unique to its section number and course name
                            sectionMap[classNumberText] = section;
                            currentCourse.Sections.Add(section);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error processing basic section info: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in first phase: " + ex.Message);
            }

            // Second try block: For  class number, click the corresponding section button,
            // click it, extract details, attach them to the correct SectionData, then navigate back
            try
            {
                foreach (string key in sectionMap.Keys.ToList())
                {
                    try
                    {
                        var sectionNumberElement = driverInstance.FindElement(
                            By.XPath($"//td[@class='sched_sln' and normalize-space(text())='{key}']"));
                        var sectionRow = sectionNumberElement.FindElement(By.XPath("./ancestor::tr"));
                        var button = sectionRow.FindElement(By.XPath(".//td[@class='sched_sec']/a"));

                        button.Click();
                        Thread.Sleep(5000);

                        var extraDetailsObj = LoadCourseDescriptionDetails(driverInstance);
                        List<string> extraDetailsList = ConvertDetailsToStringList(extraDetailsObj);

                        if (sectionMap.ContainsKey(key))
                        {
                            sectionMap[key].CourseDescriptionDetails.Add(extraDetailsObj);
                            //Console.WriteLine($"DEBUG: Added extra details to class number {key}. Count now: {sectionMap[key].CourseDescriptionDetails.Count}");

                        }

                        driverInstance.Navigate().Back();
                        Thread.Sleep(7000);
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("Error processing extra details for class number " + key + ": " + ex2.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in second try block: " + ex.Message);
            }

            return courses;
        }
        public static CourseDescriptionDetails LoadCourseDescriptionDetails(ChromeDriver driverInstance)
        {
            CourseDescriptionDetails details = new CourseDescriptionDetails();
            try
            {
                Thread.Sleep(5000);
                var dlElement = driverInstance.FindElement(By.XPath("/html/body/div[1]/div[3]/div/div[2]/main/div[4]/div/div/dl"));
                string? detailsHtml = dlElement.GetAttribute("outerHTML");
                if(string.IsNullOrEmpty(detailsHtml))
                {
                    Console.WriteLine("No details found.");
                    return details;
                }
                var doc = new HtmlDocument();
                doc.LoadHtml(detailsHtml);

                var dtNodes = doc.DocumentNode.SelectNodes("//dt");
                if (dtNodes != null)
                {
                    foreach (var dt in dtNodes)
                    {
                        var dd = dt.SelectSingleNode("following-sibling::dd[1]");
                        string key = dt.InnerText.Trim();
                        string value = dd != null ? dd.InnerText.Trim() : string.Empty;

                        switch (key)
                        {
                            case "Course Description":
                                details.CourseDescription = value;
                                break;
                            case "Course Prerequisite":
                                details.CoursePrerequisite = value;
                                break;
                            case "Course Credit":
                                details.CourseCredit = value;
                                break;
                            case "Special Course Fee":
                                details.SpecialCourseFee = value;
                                break;
                            case "Consent required:":
                                details.ConsentRequired = value;
                                break;
                            case "Crosslisted Courses":
                                details.CrosslistedCourses = value;
                                break;
                            case "Conjoint Courses":
                                details.ConjointCourses = value;
                                break;
                            case "UCORE":
                                details.UCORE = value;
                                break;
                            case "[GRADCAPS] Graduate Capstone":
                                details.GraduateCapstone = value;
                                break;
                            case "GER Code":
                                details.GERCode = value;
                                break;
                            case "Writing in the Major":
                                details.WritingInTheMajor = value;
                                break;
                            case "Cooperative":
                                details.Cooperative = value;
                                break;
                            case "Meetings Info":
                                details.MeetingsInfo.Add(value);
                                break;
                            case "Instructor(s)":
                                details.Instructors.Add(value);
                                break;
                            case "Instruction Mode":
                                details.InstructionMode = value;
                                break;
                            case "Enrollment Limit":
                                details.EnrollmentLimit = value;
                                break;
                            case "Current Enrollment":
                                details.CurrentEnrollment = value;
                                break;
                            case "Comment":
                                details.Comment = value;
                                break;
                            case "Start Date":
                                details.StartDate = value;
                                break;
                            case "End Date":
                                details.EndDate = value;
                                break;
                            case "Footnotes":
                                details.Footnotes = value;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading course details: {ex.Message}");
            }
            return details;
        }
        public static List<string> ConvertDetailsToStringList(CourseDescriptionDetails details)
        {
            var list = new List<string>();

            if (!string.IsNullOrWhiteSpace(details.CourseDescription))
                list.Add(details.CourseDescription);
            if (!string.IsNullOrWhiteSpace(details.CoursePrerequisite))
                list.Add(details.CoursePrerequisite);
            if (!string.IsNullOrWhiteSpace(details.CourseCredit))
                list.Add(details.CourseCredit);
            if (!string.IsNullOrWhiteSpace(details.SpecialCourseFee))
                list.Add(details.SpecialCourseFee);
            if (!string.IsNullOrWhiteSpace(details.ConsentRequired))
                list.Add(details.ConsentRequired);
            if (!string.IsNullOrWhiteSpace(details.CrosslistedCourses))
                list.Add(details.CrosslistedCourses);
            if (!string.IsNullOrWhiteSpace(details.ConjointCourses))
                list.Add(details.ConjointCourses);
            if (!string.IsNullOrWhiteSpace(details.UCORE))
                list.Add(details.UCORE);
            if (!string.IsNullOrWhiteSpace(details.GraduateCapstone))
                list.Add(details.GraduateCapstone);
            if (!string.IsNullOrWhiteSpace(details.GERCode))
                list.Add(details.GERCode);
            if (!string.IsNullOrWhiteSpace(details.WritingInTheMajor))
                list.Add(details.WritingInTheMajor);
            if (!string.IsNullOrWhiteSpace(details.Cooperative))
                list.Add(details.Cooperative);
            if (!string.IsNullOrWhiteSpace(details.InstructionMode))
                list.Add(details.InstructionMode);
            if (!string.IsNullOrWhiteSpace(details.EnrollmentLimit))
                list.Add(details.EnrollmentLimit);
            if (!string.IsNullOrWhiteSpace(details.CurrentEnrollment))
                list.Add(details.CurrentEnrollment);
            if (!string.IsNullOrWhiteSpace(details.Comment))
                list.Add(details.Comment);
            if (!string.IsNullOrWhiteSpace(details.StartDate))
                list.Add(details.StartDate);
            if (!string.IsNullOrWhiteSpace(details.EndDate))
                list.Add(details.EndDate);
            if (!string.IsNullOrWhiteSpace(details.Footnotes))
                list.Add(details.Footnotes);

            return list;
        }
        public static string WrapText(string text, int maxLineWidth)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var words = text.Split(' ');
            var sb = new StringBuilder();

            int currentLineLength = 0;
            foreach (var word in words)
            {
                if (currentLineLength + word.Length + 1 > maxLineWidth)
                {
                    sb.AppendLine();
                    currentLineLength = 9; 
                }
                sb.Append(word + " ");
                currentLineLength += word.Length + 1;
            }
            return sb.ToString();
        }

    }
}
