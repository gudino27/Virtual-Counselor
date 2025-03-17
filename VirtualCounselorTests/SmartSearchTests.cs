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
            private Dictionary<string, Course> testCourses = new();

            public void SetupTestCourses(List<Course> courses)
            {
                testCourses.Clear();
                foreach (var course in courses)
                {
                    testCourses[course.CourseCode] = course;
                }
            }

            public new List<Course> GetAllCourses()
            {
                return new List<Course>(testCourses.Values);
            }

            public new Course GetCourse(string courseCode)
            {
                testCourses.TryGetValue(courseCode, out var course);
                return course;
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
    }
}