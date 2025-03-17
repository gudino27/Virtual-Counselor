using System.Collections.Generic;
using System.Linq;

namespace VirtualCounselor
{
    /// <summary>
    /// Storage class designed to hold all info on any given degree.
    /// This includes information like course requirements.
    /// </summary>
    public class Degree
    {
        public string Name { get; set; }

        // Tuple: (List of courses to choose from, isRequired)
        public List<(List<string> Options, bool IsRequired)> Requirements { get; set; } = new();

        public void AddRequirement(List<string> options, bool isRequired = true)
        {
            Requirements.Add((options, isRequired));
        }

        public List<string> GetRequiredCourses()
        {
            return Requirements.SelectMany(r => r.Options).Distinct().ToList();
        }

        public bool MarkTaken(string courseCode)
        {
            foreach (var (options, _) in Requirements)
            {
                if (options.Contains(courseCode))
                    return true;
            }
            return false;
        }
    }
}
