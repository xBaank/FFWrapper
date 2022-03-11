
using System;

namespace FFmpegWrapper.Utils
{
    public class ThrowUtils
    {
        /// <summary>
        /// Throw an exception if the condition is met
        /// </summary>
        /// <typeparam name="K">Exception type</typeparam>
        /// <param name="condition">Condition to throw</param>
        /// <param name="message">Exception message</param>
        public static void ThrowFor<K>(bool condition, string message) where K : Exception
        {
            Exception? ex = (Exception?)Activator.CreateInstance(typeof(K), message);
            if (condition && ex is not null)
                throw ex;
        }
    }
}
