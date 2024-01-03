

namespace LeapWoF.Interfaces
{
    /// <summary>
    /// The IInputProvider interface, represents something that provides inputs
    /// </summary>
    public interface IInputProvider
    {
        /// <summary>
        /// Read the input
        /// </summary>
        /// <returns>The input</returns>
        string Read();
    }
}
