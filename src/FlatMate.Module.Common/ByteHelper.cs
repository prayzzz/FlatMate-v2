using System.IO;

namespace FlatMate.Module.Common
{
    public static class ByteHelper
    {
        /// <summary>
        ///     https://stackoverflow.com/a/221941
        /// </summary>
        public static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }
    }
}