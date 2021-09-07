using System.CommandLine.IO;
using System.Globalization;

namespace Serious.IO
{
    public static class StreamWriterExtensions
    {
        public static void WriteLine(this IStandardStreamWriter writer, string message, object arg)
        {
            writer.WriteLine(string.Format(CultureInfo.InvariantCulture, message, arg));
        }
        
        public static void WriteLine(this IStandardStreamWriter writer, string message, object arg1, object arg2)
        {
            writer.WriteLine(string.Format(CultureInfo.InvariantCulture, message, arg1, arg2));
        }
    }
}