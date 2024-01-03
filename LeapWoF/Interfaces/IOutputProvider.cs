

namespace LeapWoF.Interfaces
{

    /// <summary>
    /// The IOutputProvider interface, represents something that provides outputs
    /// </summary>
    public interface IOutputProvider
    {
        /// <summary>
        /// Write the output
        /// </summary>
        /// <param name="output"></param>
        void Write(string output);

        /// <summary>
        /// Write the output with a new line
        /// </summary>
        /// <param name="output"></param>
        void WriteLine(string output);

        /// <summary>
        /// Write an empty new line
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Clear the output
        /// </summary>
        void Clear();
    }
}
