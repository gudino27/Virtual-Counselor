using NUnit.Framework;
using System.Collections.Generic;
using VirtualCounselor;
using VirtualCounselor.Backend;

namespace VirtualCounselorTests
{
    /// <summary>
    /// Test fixture for SmartSearch functionality using NUnit
    /// </summary>
    [TestFixture]
    public class SmartSearchTests
    {
        private TestCourseManager courseManager;
        private List<Degree> degrees;
        private List<string> takenCourses;
        private SmartSearch smartSearch;

        /// <summary>
        /// Simple test-specific implementation of CourseManager
        /// </summary>
        private class TestCourseManager : CourseManager
        {
            public void SetupTestCourses(List<Course> testCourses)
            {
                Clear();
                foreach (var course in testCourses)
                {
                    AddCourse(course);
                }
            }
        }

        /// <summary>
        /// Setup method that runs before each test
        /// </summary>
        [SetUp]
        public void Setup()
        {
            courseManager = new TestCourseManager();
            degrees = new List<Degree>
            {
                new Degree
                {
                    Name = "Computer Science",
                    Requirements = new List<(List<string> Options, bool IsRequired)>
                    {
                        (new List<string> { "CS 121", "CS 122" }, true)
                    }
                }
            };
            takenCourses = new List<string> { "CS 121" };
            smartSearch = new SmartSearch(courseManager, degrees, takenCourses);
        }

        /// <summary>
        /// Tests searching courses with an empty query string
        /// </summary>
        [Test]
        public void SearchCourses_EmptyQuery_ReturnsEmptyList()
        {
            var result = smartSearch.SearchCourses("");
            Assert.That(result, Is.Empty);
        }

        /// <summary>
        /// Tests searching courses with a valid query
        /// </summary>
        [Test]
        public void SearchCourses_ValidQuery_ReturnsMatchingCourses()
        {
            var testCourses = new List<Course>
            {
                new Course { CourseCode = "CS 121", Title = "Intro to Programming" },
                new Course { CourseCode = "CS 122", Title = "Advanced Programming" }
            };
            courseManager.SetupTestCourses(testCourses);

            var result = smartSearch.SearchCourses("Programming");

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].CourseCode, Is.EqualTo("CS 122"));
        }

        /// <summary>
        /// Tests recommending UCORE courses that haven't been taken
        /// </summary>
        [Test]
        public void RecommendUCoreCourses_ReturnsUnmetUCoreCourses()
        {
            var testCourses = new List<Course>
            {
                new Course { CourseCode = "CS 121", Title = "Intro to Programming", UCoreCategory = "QUAN" },
                new Course { CourseCode = "HIST 105", Title = "World History", UCoreCategory = "HUM" },
            };
            courseManager.SetupTestCourses(testCourses);

            var result = smartSearch.RecommendUCoreCourses();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].CourseCode, Is.EqualTo("HIST 105"));
        }

        /// <summary>
        /// Tests recommending unmet degree requirements
        /// </summary>
        [Test]
        public void RecommendUnmetDegreeCourses_ReturnsUnmetDegreeCourses()
        {
            var testCourses = new List<Course>
            {
                new Course { CourseCode = "CS 121", Title = "Intro to Programming", UCoreCategory = "QUAN" },
                new Course { CourseCode = "HIST 105", Title = "World History", UCoreCategory = "HUM" },
                new Course { CourseCode = "MATH 101", Title = "College Algebra", UCoreCategory = "QUAN" }
            };
            courseManager.SetupTestCourses(testCourses);

            var result = smartSearch.RecommendUnmetDegreeCourses();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo("CS 122"));
        }

        /// <summary>
        /// Tests getting all recommended courses (both UCORE and degree requirements)
        /// </summary>
        [Test]
        public void GetAllRecommendedCourses_ReturnsAllRecommendedCourses()
        {
            var testCourses = new List<Course>
            {
                new Course { CourseCode = "CS 121", Title = "Intro to Programming", UCoreCategory = "QUAN" },
                new Course { CourseCode = "CS 122", Title = "Advanced Programming" },
                new Course { CourseCode = "HIST 105", Title = "World History", UCoreCategory = "HUM" }
            };
            courseManager.SetupTestCourses(testCourses);

            // Initialize takenCourses
            var takenCourses = new List<string> { "CS 121" };
            smartSearch = new SmartSearch(courseManager, degrees, takenCourses);

            var result = smartSearch.GetAllRecommendedCourses();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Exists(c => c.CourseCode == "CS 122"));
            Assert.That(result.Exists(c => c.CourseCode == "HIST 105"));
        }


        /// <summary>
        /// Tests searching courses with a null query
        /// </summary>
        [Test]
        public void SearchCourses_NullQuery_ReturnsEmptyList()
        {
            var result = smartSearch.SearchCourses(null);
            Assert.That(result, Is.Empty);
        }

        /// <summary>
        /// Tests searching when no courses are available
        /// </summary>
        [Test]
        public void SearchCourses_NoCourses_ReturnsEmptyList()
        {
            courseManager.SetupTestCourses(new List<Course>());
            var result = smartSearch.SearchCourses("Programming");
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void SearchCourses_MultipleKeywords_ReturnsMatchingCourses()
        {
            var testCourses = new List<Course>
        {
            new Course { CourseCode = "CS 121", Title = "Intro Programming" },
            new Course { CourseCode = "CS 122", Title = "Advanced Programming" },
            new Course { CourseCode = "MATH 171", Title = "Calculus Programming" }
        };
            courseManager.SetupTestCourses(testCourses);

            var result = smartSearch.SearchCourses("Advanced Programming");

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].CourseCode, Is.EqualTo("CS 122"));
        }

        [Test]
        public void SearchCourses_PrefixOnly_ReturnsAllMatchingCourses()
        {
            var testCourses = new List<Course>
            {
                new Course { CourseCode = "CS 121", Title = "Intro Programming", Prefix = "CS" },
                new Course { CourseCode = "CS 122", Title = "Advanced Programming", Prefix = "CS" },
                new Course { CourseCode = "MATH 171", Title = "Calculus", Prefix = "MATH" }
            };
            courseManager.SetupTestCourses(testCourses);

            // Clear taken courses for this test
            takenCourses.Clear();
            smartSearch = new SmartSearch(courseManager, degrees, takenCourses);

            var result = smartSearch.SearchCourses("CS");

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Select(c => c.CourseCode), Contains.Item("CS 121"));
            Assert.That(result.Select(c => c.CourseCode), Contains.Item("CS 122"));
        }

        [Test]
        public void RecommendUCoreCourses_MultipleCategories_ReturnsAllUnmetCategories()
        {
            var testCourses = new List<Course>
            {
                new Course { CourseCode = "WRTG 101", Title = "Writing", UCoreCategory = "WRTG", Prefix = "WRTG" },
                new Course { CourseCode = "HIST 105", Title = "History", UCoreCategory = "HUM", Prefix = "HIST" },
                new Course { CourseCode = "MATH 171", Title = "Calculus", UCoreCategory = "QUAN", Prefix = "MATH" }
            };
            courseManager.SetupTestCourses(testCourses);

            // Update taken courses to include WRTG 101
            takenCourses.Add("WRTG 101");
            smartSearch = new SmartSearch(courseManager, degrees, takenCourses);

            var result = smartSearch.RecommendUCoreCourses();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Select(c => c.UCoreCategory), Contains.Item("HUM"));
            Assert.That(result.Select(c => c.UCoreCategory), Contains.Item("QUAN"));
        }

        [Test]
        public void RecommendUnmetDegreeCourses_MultipleDegrees_ReturnsAllRequiredCourses()
        {
            degrees = new List<Degree>
            {
                new Degree
                {
                    Name = "Computer Science",
                    Requirements = new List<(List<string> Options, bool IsRequired)>
                    {
                        (new List<string> { "CS 121", "CS 122" }, true),
                        (new List<string> { "CS 223" }, true)
                    }
                },
                new Degree
                {
                    Name = "Mathematics",
                    Requirements = new List<(List<string> Options, bool IsRequired)>
                    {
                        (new List<string> { "MATH 171", "MATH 172" }, true)
                    }
                }
            };

            var testCourses = new List<Course>
            {
                new Course { CourseCode = "CS 121", Title = "Intro Programming" },
                new Course { CourseCode = "CS 122", Title = "Advanced Programming" },
                new Course { CourseCode = "CS 223", Title = "Advanced Data Structures" },
                new Course { CourseCode = "MATH 171", Title = "Calculus I" },
                new Course { CourseCode = "MATH 172", Title = "Calculus II" }
            };
            courseManager.SetupTestCourses(testCourses);

            smartSearch = new SmartSearch(courseManager, degrees, new List<string> { "CS 121" });
            var result = smartSearch.RecommendUnmetDegreeCourses();

            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result, Contains.Item("CS 122"));
            Assert.That(result, Contains.Item("CS 223"));
            Assert.That(result, Contains.Item("MATH 171"));
            Assert.That(result, Contains.Item("MATH 172"));
        }
    }
}