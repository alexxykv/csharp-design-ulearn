using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Streams.Compression
{
    public class CustomCompressionStream : Stream
    {
        private readonly bool read;
        private readonly Stream baseStream;
        private readonly ICompression compression;
        private readonly List<byte> compressedBytes = new List<byte>();
        private readonly List<byte> decompressedBytes = new List<byte>();

        public CustomCompressionStream(Stream baseStream, bool read)
        {
            this.read = read;
            this.baseStream = baseStream;
            compression = new RunLengthEncoding();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead) throw new InvalidOperationException();
            foreach (var b in baseStream.ReadAllBytes())
            {
                compressedBytes.Add(b);
                if (compressedBytes.Count > 1)
                {
                    compression.Decompress(compressedBytes)
                        .ToList()
                        .ForEach(z => decompressedBytes.Add(z));
                    compressedBytes.Clear();
                }
                if (decompressedBytes.Count >= count) 
                    break;
            }
            if (decompressedBytes.Count < count && compressedBytes.Count != 0) 
                throw new InvalidOperationException();
            var cnt = new MemoryStream(decompressedBytes.ToArray()).Read(buffer, offset, count);
            decompressedBytes.RemoveRange(0, cnt);
            return cnt;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite) throw new InvalidOperationException();
            var compressedBytes = compression.Compress(buffer.Skip(offset).Take(count)).ToArray();
            baseStream.Write(compressedBytes, 0, compressedBytes.Length);
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead => read;
        public override bool CanSeek => false;
        public override bool CanWrite => !read;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
    }

    public interface ICompression
    {
        IEnumerable<byte> Compress(IEnumerable<byte> bytes);
        IEnumerable<byte> Decompress(IEnumerable<byte> bytes);
    }

    public class RunLengthEncoding : ICompression
    {
        public IEnumerable<byte> Compress(IEnumerable<byte> bytes)
        {
            byte repeat = 0;
            byte previous = 0;
            foreach (var b in bytes)
            {
                if (repeat == 0) previous = b;
                if (previous != b || repeat == byte.MaxValue)
                {
                    yield return repeat;
                    yield return previous;
                    //
                    repeat = 0;
                    previous = b;
                }
                repeat++;
            }
            if (repeat != 0)
            {
                yield return repeat;
                yield return previous;
            }
        }

        public IEnumerable<byte> Decompress(IEnumerable<byte> bytes)
        {
            byte repeat = 0;
            foreach (var b in bytes)
            {
                if (repeat == 0)
                {
                    repeat = b;
                    continue;
                }
                for (var i = 0; i < repeat; i++)
                    yield return b;
                repeat = 0;
            }
        }
    }

    public static class StreamExtension
    {
        public static IEnumerable<byte> ReadAllBytes(this Stream stream)
        {
            while (true)
            {
                var readByte = stream.ReadByte();
                if (readByte == -1)
                    yield break;
                yield return (byte)readByte;
            }
        }
    }
}
