namespace BlazorApp1.Services
{
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;
    using OpenQA.Selenium;
    using System.Text.RegularExpressions;
    using System.Collections.Concurrent;
    using static Major;
    using System.Diagnostics;
    using HtmlAgilityPack;
    using System;
    using System.Threading.Tasks;

    public class Degree
    {
        public string? DegreeDescription { get; set; }
        /**********************************************************************************
        *   function name: PrintDegreInfo                                                 *
        *   precondition: DegreeDescription may be null or contain degree summary text    *
        *   Parameters: none                                                              *
        *   description: Prints the degree description to the console                     *
        *   Post-condition: Degree description is printed                                 *
        *   Returns: void                                                                 *
        ***********************************************************************************/
        public virtual void PrintDegreInfo()
        {
            Console.WriteLine(DegreeDescription);
        }
    }

    public class Major : Degree
    {
        public string Header { get; set; } = "";
        public string Description { get; set; } = "";

        public class CourseRequirement
        {
            public string Name { get; set; } = "";
            public string Credits { get; set; } = "";
        }

        public List<CourseRequirement> CourseRequirements { get; } = new();
        public int? CreditRequirement { get; set; }

        public List<string> StructuredOutput { get; set; } = new();
        public Dictionary<string, string> Footnotes { get; set; } = new();
        public int TotalCredits { get; set; } = 0;

        /***********************************************************************************
         *   function name: PrintDegreInfo                                                 *
         *   precondition: StructuredOutput and Footnotes are populated                    *
         *   Parameters: none                                                              *
         *   description: Prints the degree description, course sequence, and footnotes    *
         *   Post-condition: Degree info is printed                                        *
         *   Returns: void                                                                 *
         ***********************************************************************************/
        public override void PrintDegreInfo()
        {
            Console.WriteLine(Header);
            if (!string.IsNullOrWhiteSpace(Description))
            {
                Console.WriteLine(Description);
            }
            if (CreditRequirement.HasValue)
            {
                Console.WriteLine($"Total credits required: {CreditRequirement.Value}");
            }
            foreach (var line in StructuredOutput)
                Console.WriteLine(line);

            if (Footnotes.Count > 0)
            {
                Console.WriteLine("\n--- Footnotes ---");
                foreach (var kvp in Footnotes)
                    Console.WriteLine($"[{kvp.Key}]: {kvp.Value}");
            }
        }
        public class DegreeTask
        {
            public string Name { get; set; } = "";
            public string Value { get; set; } = "";
        }
    }

    public class Minor : Degree
    {
        public int TotalCredits { get; set; }
        public int UpperDivisionCredits { get; set; }
        public double? MinimumGPA { get; set; }
        public List<string> Courses { get; set; } = new();
        public List<string> Notes { get; set; } = new();
        public List<string> StructuredContent { get; set; } = new();

        public void ParseAdditionalContent(string text)
        {
            // total‐credits
            var m1 = Regex.Match(text, @"(?<c>\d{1,3})\s+credits?", RegexOptions.IgnoreCase);
            if (m1.Success && int.TryParse(m1.Groups["c"].Value, out var tot))
            {
                TotalCredits = tot;
                StructuredContent.Add($"Minimum credits: {tot}");
            }

            // upper‐division
            var m2 = Regex.Match(text, @"(?<u>(Nine|Ten|Eleven|Twelve|\d+))\s+credits.*?(upper-division|300-400-level)", RegexOptions.IgnoreCase);
            if (m2.Success)
            {
                var val = m2.Groups["u"].Value.ToLower();
                var up = val switch
                {
                    "nine" => 9,
                    "ten" => 10,
                    "eleven" => 11,
                    "twelve" => 12,
                    _ => int.TryParse(val, out var x) ? x : 0
                };
                UpperDivisionCredits = up;
                StructuredContent.Add($"upper-division credits: {up}");
            }

            // GPA
            var m3 = Regex.Match(text, @"\b(\d\.\d+)\b");
            if (m3.Success && double.TryParse(m3.Value, out var gpa))
            {
                MinimumGPA = gpa;
                StructuredContent.Add($"minimum Cumalitive GPA: {gpa:F1}");
            }

            // courses
            var courseRx = new Regex(@"([A-Z/&\s]{2,})\s*(\d{3})");
            foreach (Match c in courseRx.Matches(text))
            {
                var course = Regex.Replace(c.Value, @"\s+", " ").Trim();
                if (!Courses.Contains(course))
                    Courses.Add(course);
            }
        }
        public override void PrintDegreInfo()
        {
            Console.WriteLine("[MINOR DEBUG PREVIEW]\n======================");
            Console.WriteLine($"Minor: {DegreeDescription}");

            if (TotalCredits > 0)
                Console.WriteLine($"Minimum credits: {TotalCredits}");
            if (UpperDivisionCredits > 0)
                Console.WriteLine($"upper-division credits: {UpperDivisionCredits}");
            if (MinimumGPA.HasValue)
                Console.WriteLine($"minimum Cumalitive GPA: {MinimumGPA:F1}");

            foreach (var course in Courses.Distinct())
                Console.WriteLine($"Course: {course}");
            foreach (var note in Notes.Distinct())
            {
                if (note.Contains("&nbsp;"))
                {
                    Console.WriteLine("Note:" + note.Replace("&nbsp;", ""));
                }
                else
                {
                    Console.WriteLine("Note:" + note.Trim());
                }
            }

            Console.WriteLine("\n----------------------------\n");
        }
    }

    public class DegreeScrape
    {
        public static List<Degree> degreeList = new();

        /**********************************************************************************
         *   function name: ScrapeAll                                                     *
         *   precondition: WSU catalog site must be online                                *
         *   Parameters: none                                                             *
         *   description: Loads and processes all degrees in parallel using fixed browser *
         *                instances that persist throughout the process                   *
         *   Post-condition: All degree data collected in memory                          *
         *   Returns: void                                                                *
         **********************************************************************************/
        public static void scrapeall()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                // Get all degrees to scrape
                var allTasks = GetAllDegrees();
                Console.WriteLine($"Found {allTasks.Count} degrees to scrape");

                // Determine optimal number of browser instances
                int coreCount = Environment.ProcessorCount;
                int numInstances = Math.Max(2, Math.Min(8, (coreCount / 2)+3));
                Console.WriteLine($"Detected {coreCount} logical processors. Using {numInstances} persistent browser instances.\n");

                // Create a thread-safe collection for results
                var results = new ConcurrentBag<Degree>();
                var failedTasks = new ConcurrentBag<DegreeTask>();

                // Divide degrees among instances
                var taskBatches = CreateBatches(allTasks, numInstances);

                // Process each batch in parallel
                var options = new ParallelOptions { MaxDegreeOfParallelism = numInstances };
                Parallel.For(0, numInstances, options, instanceId =>
                {
                    try
                    {
                        ProcessBatchWithSingleBrowser(taskBatches[instanceId], instanceId, results, failedTasks);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[FATAL ERROR] Instance {instanceId} crashed: {ex.Message}");
                        Console.ResetColor();
                    }
                });

                // Retry failed tasks with fresh browser instances
                if (failedTasks.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n===== RETRYING {failedTasks.Count} FAILED TASKS =====");
                    Console.ResetColor();

                    var retryBatches = CreateBatches(failedTasks.ToList(), numInstances);
                    Parallel.For(0, Math.Min(numInstances, retryBatches.Count), options, instanceId =>
                    {
                        try
                        {
                            ProcessBatchWithSingleBrowser(retryBatches[instanceId], instanceId + 100, results, new ConcurrentBag<DegreeTask>(), true);
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"[RETRY ERROR] Instance {instanceId} crashed: {ex.Message}");
                            Console.ResetColor();
                        }
                    });
                }

                Console.WriteLine("\n===== ALL DONE =====");
                degreeList.AddRange(results);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FATAL ERROR] {e.GetType().Name}: {e.Message}\n{e.StackTrace}");
                Console.ResetColor();
            }
            finally
            {
                stopwatch.Stop();
                string elapsedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[INFO] Program completed in {elapsedTime}");
                Console.ResetColor();
                Console.WriteLine($"Total degrees scraped: {degreeList.Count}");
                Console.WriteLine($"Total majors completed: {degreeList.Count(d => d is Major)}");
                Console.WriteLine($"Total minors completed: {degreeList.Count(d => d is Minor)}");
            }
        }

        /**********************************************************************************
         *   function name: CreateBatches                                                 *
         *   precondition: Tasks list and number of batches are valid                     *
         *   Parameters: List<DegreeTask> tasks, int batchCount                           *
         *   description: Divides tasks into approximately equal batches                  *
         *   Post-condition: Tasks divided into batches                                   *
         *   Returns: List of task batches                                                *
         **********************************************************************************/
        private static List<List<DegreeTask>> CreateBatches(List<DegreeTask> tasks, int batchCount)
        {
            var batches = new List<List<DegreeTask>>();
            int batchSize = (int)Math.Ceiling((double)tasks.Count / batchCount);

            for (int i = 0; i < batchCount; i++)
            {
                int startIndex = i * batchSize;
                int count = Math.Min(batchSize, tasks.Count - startIndex);

                // If we've run out of tasks, break
                if (count <= 0) break;

                var batch = tasks.GetRange(startIndex, count);
                batches.Add(batch);
            }

            return batches;
        }

        /**********************************************************************************
         *   function name: ProcessBatchWithSingleBrowser                                 *
         *   precondition: Browser instance can be created                                *
         *   Parameters: List<DegreeTask> batch, int instanceId, ConcurrentBag<Degree>    *
         *              results, ConcurrentBag<DegreeTask> failedTasks, bool isRetry=false*
         *   description: Processes a batch of degrees with a single browser instance     *
         *   Post-condition: All batch degrees processed and added to results             *
         *   Returns: void                                                                *
         **********************************************************************************/
        private static void ProcessBatchWithSingleBrowser(
            List<DegreeTask> batch,
            int instanceId,
            ConcurrentBag<Degree> results,
            ConcurrentBag<DegreeTask> failedTasks,
            bool isRetry = false)
        {
            // Log batch size
            Console.WriteLine($"[Instance {instanceId}] Starting with {batch.Count} degrees to process");

            // Create persistent browser instance
            using var driver = InitChromeDriver();

            // Navigate to the starting page and wait for it to load properly
            bool initialPageLoaded = false;
            for (int attempt = 1; attempt <= 5 && !initialPageLoaded; attempt++)
            {
                try
                {
                    driver.Navigate().GoToUrl("https://catalog.wsu.edu/Degrees");
                    Thread.Sleep(5000); // Extra time to load

                    // Refresh a few times to ensure everything loads
                    driver.Navigate().Refresh();
                    Thread.Sleep(3000);
                    driver.Navigate().Refresh();
                    Thread.Sleep(3000);

                    // Maximize window for better rendering
                    driver.Manage().Window.Maximize();

                    // Wait for degree selector to appear
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    wait.Until(d => d.FindElement(By.Id("degree-selector")));

                    initialPageLoaded = true;
                    Console.WriteLine($"[Instance {instanceId}] Page initialized successfully");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[Instance {instanceId}] Attempt {attempt} failed to load initial page: {ex.Message}");
                    Console.ResetColor();

                    if (attempt == 5)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[Instance {instanceId}] Failed to initialize browser after 5 attempts. Aborting this batch.");
                        Console.ResetColor();
                        foreach (var task in batch)
                        {
                            failedTasks.Add(task);
                        }
                        return;
                    }

                    Thread.Sleep(5000); // Wait before retrying
                }
            }

            // Process each degree in the batch with the same browser instance
            int completedCount = 0;
            foreach (var task in batch)
            {
                try
                {
                    // Select the degree from dropdown
                    SelectDegreeOption(driver, task.Value);

                    // Extract degree information
                    var localDegrees = new List<Degree>();
                    ProcessScheduleOfStudies(driver, localDegrees);

                    // Add results to the shared collection
                    foreach (var degree in localDegrees)
                    {
                        results.Add(degree);
                    }

                    completedCount++;
                    double progress = (double)completedCount / batch.Count * 100;                    
                    Console.WriteLine($"Thread: {instanceId} has Completed {progress:F1}%");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[Instance {instanceId}] Failed: {task.Name} — {ex.Message}");
                    Console.ResetColor();

                    if (!isRetry)
                    {
                        failedTasks.Add(task);
                    }

                    // Try to reset browser state to prevent subsequent failures
                    try
                    {
                        driver.Navigate().GoToUrl("https://catalog.wsu.edu/Degrees");
                        Thread.Sleep(5000);
                        driver.Navigate().Refresh();
                        Thread.Sleep(3000);
                    }
                    catch { /* Ignore navigation recovery errors */ }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Instance {instanceId}] Batch completed: {completedCount}/{batch.Count} successful");
            Console.ResetColor();
        }
        /**********************************************************************************
         *   function name: SelectDegreeOption                                            *
         *   precondition: Driver is on the degree page and value is valid                *
         *   Parameters: ChromeDriver driver, string value                                *
         *   description: Selects a specific degree from the dropdown                     *
         *   Post-condition: Degree is selected and page is loaded                        *
         *   Returns: void                                                                *
         **********************************************************************************/
        private static void SelectDegreeOption(ChromeDriver driver, string value)
        {
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(30));

            // Wait for the dropdown to be present and interactable
            wait.Until(d => d.FindElement(By.Id("degree-selector")));

            // Use JavaScript to select the option safely
            IJavaScriptExecutor js = driver;
            js.ExecuteScript(@"
                const select = document.getElementById('degree-selector');
                select.value = arguments[0];
                select.dispatchEvent(new Event('change'));
            ", value);

            // Give the page time to load after selection
            Thread.Sleep(5000);
        }
        /**********************************************************************************
         *   function name: InitChromeDriver                                              *
         *   precondition: ChromeDriver binary must be installed and accessible           *
         *   Parameters: none                                                             *
         *   description: Creates and configures a ChromeDriver instance                  *
         *   Post-condition: ChromeDriver instance is initialized                         *
         *   Returns: ChromeDriver                                                        *
        ***********************************************************************************/
        static ChromeDriver InitChromeDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--disable-usb");
            options.AddArgument("--disable-usb-discovery");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-logging");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--headless=new"); // Use newer headless mode
            options.AddArgument("--window-size=1920,1080"); // Set window size to ensure consistent rendering
            options.AddArgument("--log-level=3");
            options.AddArgument("--ignore-certificate-errors");
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2); // Disable images

            var service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            service.EnableVerboseLogging = false;

            var driver = new ChromeDriver(service, options);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            return driver;
        }
        /**********************************************************************************
         *   function name: GetAllDegrees                                                 *
         *   precondition: WSU degree catalog page is accessible                          *
         *   Parameters: none                                                             *
         *   description: Scrapes all degrees from the WSU catalog                        *
         *   Post-condition: All degrees are collected                                    *
         *   Returns: List of DegreeTask objects containing degree label and value        *
         **********************************************************************************/
        static List<DegreeTask> GetAllDegrees()
        {
            using var driver = InitChromeDriver();
            driver.Navigate().GoToUrl("https://catalog.wsu.edu/Degrees");
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(30));

            // Multiple attempts to load the page properly
            for (int i = 0; i < 3; i++)
            {
                driver.Navigate().Refresh();
                Thread.Sleep(3000);
            }

            driver.Manage().Window.Maximize();
            wait.Until(drv => drv.FindElement(By.Id("degree-selector")));

            var dropdown = new SelectElement(driver.FindElement(By.Id("degree-selector")));
            var tasks = new List<DegreeTask>();

            for (int i = 1; i < dropdown.Options.Count; i++)
            {
                var opt = dropdown.Options[i];
                tasks.Add(new DegreeTask
                {
                    Name = opt.Text.Trim(),
                    Value = opt.GetAttribute("value")
                });
            }

            return tasks;
        }
        /**********************************************************************************
         *   function name: ProcessScheduleOfStudies                                      *
         *   precondition: Driver is on the degree page                                   *
         *   Parameters: ChromeDriver driver, List<Degree> localDegrees                   *
         *   description: Processes the schedule of studies and extracts course info      *
         *   Post-condition: Degree data is added to the list                             *
         *   Returns: void                                                                *
         **********************************************************************************/
        static void ProcessScheduleOfStudies(ChromeDriver driver, List<Degree> localDegrees)
        {
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(15));
            try
            {
                wait.Until(d => d.FindElement(By.CssSelector("nav.secondary_nav")));

                var scheduleHeadings = driver.FindElements(By.XPath("//nav[@class='secondary_nav']/h5"));
                string[] targets = { "Schedule of Studies", "Minors" };

                foreach (var heading in scheduleHeadings)
                {
                    string headingText = heading.Text.Trim();
                    if (!targets.Contains(headingText, StringComparer.OrdinalIgnoreCase)) continue;

                    try
                    {
                        IWebElement ulElement = heading.FindElement(By.XPath("following-sibling::ul[1]"));
                        var degreeLinks = ulElement.FindElements(By.CssSelector("li.secnav_li"));

                        foreach (var link in degreeLinks)
                        {
                            string label = link.Text.Trim();

                            // Use JavaScript for more reliable clicks
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", link);
                            Thread.Sleep(4000); // Allow more time for content to load

                            if (headingText.Equals("Minors", StringComparison.OrdinalIgnoreCase))
                                ProcessMinorDegreePage(driver, localDegrees, label);
                            else
                                ProcessDynamicDegreePage(driver, localDegrees);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing {headingText}: {ex.Message}");
                    }
                }
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine($"Timeout waiting for secondary_nav: {ex.Message}");
                // Try to capture what we can see
                var content = driver.PageSource;
                if (content.Contains("unit-fullinfo"))
                {
                    try
                    {
                        // Try to process what we can directly
                        ProcessDynamicDegreePage(driver, localDegrees);
                    }
                    catch { /* Ignore if this fails too */ }
                }
            }
        }
       /**********************************************************************************
        *   function name: ProcessMinorDegreePage                                        *
        *   precondition: Driver is on the minor degree page                             *
        *   Parameters: ChromeDriver driver, List<Degree> degrees, string minorName      *
        *   description: Extracts relevant course and notes data from the minor page     *
        *   Post-condition: Extracted info appended to Minor object and list             *
        *   Returns: void                                                                *
        **********************************************************************************/
        static void ProcessMinorDegreePage(ChromeDriver driver, List<Degree> degrees, string minorName)
        {
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(15));
            wait.Until(d => d.FindElement(By.ClassName("unit-fullinfo")));
            var content = driver.FindElement(By.ClassName("unit-fullinfo"));
            string html = content.GetAttribute("innerHTML");
            Minor minor = ParseMinorHtml(html, minorName);
            degrees.Add(minor);
        }
        /**********************************************************************************
         *   function name: ParseMinorHtml                                                *
         *   precondition: Raw HTML string and minor name are valid                       *
         *   Parameters: string rawHtml, string minorName                                 *
         *   description: Parses the raw HTML to extract course and notes data            *
         *   Post-condition: Minor object is populated with extracted data                *
         *   Returns: Minor object                                                        *
         **********************************************************************************/
        public static Minor ParseMinorHtml(string rawHtml, string minorName)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(rawHtml);

            var minor = new Minor { DegreeDescription = minorName };

            // grab all inner‐divs without a class (your content paragraphs)
            var blocks = doc.DocumentNode
                .SelectNodes("//div[not(@class)]")
                ?.Select(d => d.InnerText.Trim())
                .Where(txt => !string.IsNullOrWhiteSpace(txt))
                .ToList() ?? new List<string>();

            foreach (var block in blocks)
            {
                minor.Notes.Add(block);
                minor.ParseAdditionalContent(block);
            }

            return minor;
        }
        /**********************************************************************************
         *   function name: ProcessDynamicDegreePage                                      *
         *   precondition: Driver is on the degree page                                   *
         *   Parameters: ChromeDriver driver, List<Degree> localDegrees                   *
         *   description: Processes the dynamic degree page and extracts course info      *
         *   Post-condition: Degree data is added to the list                             *
         *   Returns: void                                                                *
         **********************************************************************************/
        static void ProcessDynamicDegreePage(ChromeDriver driver, List<Degree> degrees)
        {
            try
            {
                var content = driver.FindElement(By.ClassName("unit-fullinfo"));
                // 1) grab the header element
                var headerEl = content.FindElement(By.CssSelector("h4.academics_header > span"));
                var rawHeader = headerEl.Text.Trim();
                // E.g. "Accounting (120 Credits)"

                // 2) extract name & credits
                var m = Regex.Match(rawHeader,
                    @"^(?<name>.*?)\s*\(\s*(?<cr>\d+)\s+Credits?\s*\)$",
                    RegexOptions.IgnoreCase);

                var major = new Major();
                if (m.Success)
                {
                    major.Header = rawHeader;
                    major.CreditRequirement = int.Parse(m.Groups["cr"].Value);
                    // store the bare name if you still need it
                    major.DegreeDescription = m.Groups["name"].Value.Trim();
                }
                else
                {
                    major.Header = rawHeader;
                    major.DegreeDescription = rawHeader;
                }

                // 3) grab the very next <div> or <p> that holds the narrative
                try
                {
                    var descNode = content.FindElement(
                        By.XPath("//h4[@class='academics_header']/following-sibling::div[1]"));
                    major.Description = descNode.Text.Trim();
                }
                catch
                {
                    major.Description = "";
                }

                // 4) now your existing sequence parsing:
                var tableRows = driver.FindElements(By.CssSelector("table.degree_sequence_list > tr"));
                foreach (var row in tableRows)
                {
                    var tds = row.FindElements(By.TagName("td"));
                    if (tds.Count == 0) continue;
                    var tdClass = tds[0].GetAttribute("class") ?? "";

                    if (tdClass.Equals("degree_sequence_Year", StringComparison.OrdinalIgnoreCase))
                    {
                        major.StructuredOutput.Add($"====== {tds[0].Text.Trim()} ======");
                        continue;
                    }
                    if (tdClass.Contains("degree_sequence_term"))
                    {
                        major.StructuredOutput.Add($"---- {tds[0].Text.Trim()} ----");
                        continue;
                    }
                    if (tds.Count >= 2)
                    {
                        var courseText = tds[0].Text.Trim();
                        var creditText = tds[1].Text.Trim();
                        if (!string.IsNullOrWhiteSpace(courseText))
                        {
                            major.StructuredOutput.Add($"Course: {courseText}, Credits: {creditText}");
                            major.CourseRequirements.Add(new Major.CourseRequirement
                            {
                                Name = courseText,
                                Credits = creditText
                            });
                            if (int.TryParse(
                                  Regex.Match(creditText, @"\d+").Value,
                                  out var c))
                                major.TotalCredits += c;
                        }
                    }
                    else if (tds.Count == 1)
                    {
                        var messageText = tds[0].Text.Trim();
                        if (!string.IsNullOrWhiteSpace(messageText))
                            major.StructuredOutput.Add($"Message: {messageText}");
                    }
                }

                // 5) footnotes as before …
                var footnoteRows = driver.FindElements(By.CssSelector("table.degree_sequence_footnotes > tr"));
                foreach (var row in footnoteRows)
                {
                    var cells = row.FindElements(By.TagName("td"));
                    if (cells.Count < 2) continue;
                    var key = Regex.Replace(cells[0].Text.Trim(), "[^\\d]", "");
                    var value = cells[1].Text.Trim();
                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                        major.Footnotes[key] = value;
                }

                degrees.Add(major);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Error processing dynamic degree page: {ex.Message}");
                Console.ResetColor();

                // Try to extract what we can from the page source
                try
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(driver.PageSource);

                    // Try to get the header at minimum
                    var headerNode = htmlDoc.DocumentNode.SelectSingleNode("//h4[@class='academics_header']/span");
                    if (headerNode != null)
                    {
                        var major = new Major();
                        major.Header = headerNode.InnerText.Trim();
                        major.DegreeDescription = major.Header;

                        // Try to extract credit requirement from header
                        var m = Regex.Match(major.Header, @"\((?<cr>\d+)\s+Credits?\)", RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            major.CreditRequirement = int.Parse(m.Groups["cr"].Value);
                        }

                        major.StructuredOutput.Add("ERROR: Page could not be fully processed. Partial data only.");
                        degrees.Add(major);
                    }
                }
                catch
                {
                    // If all else fails, at least add something to indicate we tried this degree
                    var fallbackMajor = new Major();
                    fallbackMajor.DegreeDescription = "Error processing page";
                    fallbackMajor.Header = "Error processing page";
                    fallbackMajor.StructuredOutput.Add("ERROR: Could not process this degree page.");
                    degrees.Add(fallbackMajor);
                }
            }
        }
    }
}
