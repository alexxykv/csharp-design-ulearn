using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Streams.Resources
{
    public class ResourceReaderStream : Stream
    {
        private readonly byte[] key;
        private readonly Stream stream;
        private bool keyFound;
        private bool valueFound;
        private readonly byte[] separator = new byte[2] { 0, 1 };
        private readonly byte[] zeroValue = new byte[2] { 0, 0 };

        public ResourceReaderStream(Stream stream, string key)
        {
            this.key = Encoding.ASCII.GetBytes(key);
            this.stream = new BufferedStream(stream, Constants.BufferSize);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!keyFound)
                SeekValue();
            if (!valueFound)
                return ReadFieldValue(buffer, offset, count);
            return 0;
        }

        private void SeekValue()
        {
            var byteKey = new LinkedList<byte>();
            foreach (var b in stream.ReadAllBytes())
            {
                byteKey.AddLast(b);
                if (byteKey.Last(zeroValue.Length).SequenceEqual(zeroValue))
                    byteKey.RemoveLast();
                if (byteKey.Count > key.Length)
                    byteKey.RemoveFirst();
                if (byteKey.SequenceEqual(key))
                {
                    keyFound = true;
                    if (!stream.ReadAllBytes().Take(separator.Length).SequenceEqual(separator))
                        throw new EndOfStreamException();
                    break;
                }
            }
        }

        private int ReadFieldValue(byte[] buffer, int offset, int count)
        {
            var byteValue = new List<byte>();
            foreach (var b in stream.ReadAllBytes())
            {
                byteValue.Add(b);
                if (byteValue.Last(zeroValue.Length).SequenceEqual(zeroValue))
                {
                    byteValue.RemoveAt(byteValue.Count - 1);
                    byteValue.AddRange(stream.ReadAllBytes().Take(separator.Length));
                }
                if (byteValue.Last(separator.Length).SequenceEqual(separator))
                {
                    valueFound = true;
                    byteValue.RemoveRange(byteValue.Count - separator.Length, separator.Length);
                    break;
                }
                if (byteValue.Count == count) 
                    break;
            }
            return new MemoryStream(byteValue.ToArray()).Read(buffer, offset, count);
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
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
                    throw new EndOfStreamException();
                yield return (byte)readByte;
            }
        }
    }

    public static class IEnumerableExtension
    {
        public static IEnumerable<T> Last<T>(this IEnumerable<T> collection, int count)
        {
            return collection.Skip(collection.Count() - count);
        }
    }
}
