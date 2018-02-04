using System.IO;

namespace FlatMate.Module.Common
{
    public static class ByteHelper
    {
        private static readonly uint[] Lookup32 = CreateLookup32();

        /// <summary>
        ///     https://stackoverflow.com/a/221941
        /// </summary>
        public static byte[] ReadToEnd(Stream input)
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

        public static string ToHexString(this byte[] bytes)
        {
            var lookup32 = Lookup32;
            var result = new char[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var val = lookup32[bytes[i]];
                result[2 * i] = (char)val;
                result[(2 * i) + 1] = (char)(val >> 16);
            }
            return new string(result);
        }

        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for (var i = 0; i < 256; i++)
            {
                var s = i.ToString("X2");
                result[i] = s[0] + ((uint)s[1] << 16);
            }
            return result;
        }
    }
}
