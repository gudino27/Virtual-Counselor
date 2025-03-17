namespace VirtualCounselor
{
    /// <summary>
    /// This is a storage class designed to be treated like a struct.
    /// CourseManager will contain a list of these course objects.
    /// </summary>
    public class Course
    {
        public string CourseCode { get; set; }      // e.g. "CS 121"
        public string Title { get; set; }           // e.g. "Intro to Programming"
        public string Prefix { get; set; }          // e.g. "CS"
        public int Credits { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }          // Open, Full, Waitlisted
        public int SpotsLeft { get; set; }
        public int ClassNumberInt { get; set; }     // e.g. 12345
        public string Header { get; set; }          // Like category or section
        public string Section { get; set; }
        public string UCoreCategory { get; set; }   // e.g. "WRTG", "SSCI", "CAPS", etc.

        public string[] GetData()
        {
            return new string[]
            {
                CourseCode,
                Title,
                Prefix,
                Credits.ToString(),
                Description,
                Status,
                SpotsLeft.ToString(),
                ClassNumberInt.ToString(),
                Header,
                Section,
                UCoreCategory
            };
        }

        public override string ToString() =>
            $"{CourseCode} - {Title} ({Credits} credits) | Class#: {ClassNumberInt}";

    }
}
