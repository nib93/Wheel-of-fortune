using System;
using LeapWoF.Interfaces;

namespace LeapWoF
{
    /// <summary>
    /// Ths ConsoleInputProvider class, provides inputs from the Console
    /// </summary>
    class ConsoleInputProvider : IInputProvider
    {
        /// <summary>
        /// Read a line from the console
        /// </summary>
        /// <returns></returns>
        public string Read()
        {
            return Console.ReadLine();
        }
    }
}
