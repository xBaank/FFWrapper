using System;
using System.Collections.Generic;
using System.Text;

namespace WebmOpus.Exceptions
{
    /// <summary>
    /// Exceptions for WebmToOpus
    /// </summary>
    public class YtStreamException : Exception
    {
        /// <summary>
        /// Creates an exception
        /// </summary>
        /// <param name="message"></param>
        public YtStreamException(string message)  : base(message) { }
    }
}
