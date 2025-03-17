namespace VirtualCounselor
{
    /// <summary>
    /// Child class of Degree that stores different data than a Minor.
    /// </summary>
    public class Major : Degree
    {
        public int Credits { get; set; } = 120;
    }
}
