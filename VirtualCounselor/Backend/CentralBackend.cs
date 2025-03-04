namespace VirtualCounselor
{
    /// <summary>
    /// This class is going to be the main hub for the backend of our system.
    /// Any comminucation between frontend and backend will happen here. This means that this backend class will have coverage over the entirety of the backend.
    /// It'll serve as a container and communicator for the rest fo the logic.
    /// </summary>
    public class CentralBackend
    {
        // The virtual conselor will contain the rest of the programs logic in itself so it can communicate with them (via function calls or events)
        private CourseManager courseManager = new CourseManager();
        private DegreeManager degreeManager = new DegreeManager();
        private WebScraper webScraper = new WebScraper();
    }
}
