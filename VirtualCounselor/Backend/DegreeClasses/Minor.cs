namespace VirtualCounselor
{
    /// <summary>
    /// Child class of Degree that stores different data than a Major.
    /// </summary>
    public class Minor : Degree
    {
        /// <summary>
        /// Apparently, most degree programs in the US typically require 18-24 credits for a minor.With WSU, for our college, it is 20 max.
        /// </summary>
        public int Credits { get; set; } = 20;
    }
}
