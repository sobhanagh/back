namespace GamaEdtech.Backend.Common.Core.Extensions.IO
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;

    public static class StreamExtensions
    {
        public static byte[] GetAllBytes([NotNull] this Stream stream)
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        public static async Task<byte[]> GetAllBytesAsync([NotNull] this Stream stream)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
