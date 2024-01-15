using System;

namespace LibBundle3
{
    public static class StringExtensions
    {
        public static LineSplitEnumerator Split_ZeroAlloc(this string str, char separator)
        {
            return new LineSplitEnumerator(str.AsSpan(), separator);
        }

        public ref struct LineSplitEnumerator
        {
            private ReadOnlySpan<char> _str;
            private readonly char _separator;

            public LineSplitEnumerator(ReadOnlySpan<char> str, char separator)
            {
                _str = str;
                Current = default;
                _separator = separator;
            }

            public LineSplitEnumerator GetEnumerator() => this;

            public bool MoveNext()
            {
                var span = _str;
                if (span.Length == 0)
                    return false;

                var index = span.IndexOf(_separator);
                if (index == -1)
                {
                    _str = ReadOnlySpan<char>.Empty;
                    Current = new LineSplitEntry(span, ReadOnlySpan<char>.Empty);
                    return true;
                }

                if (index < span.Length - 1 && span[index] == _separator)
                {
                    Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 1));
                    _str = span.Slice(index + 1);
                    return true;
                }

                Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 1));
                _str = span.Slice(index + 1);
                return true;
            }

            public LineSplitEntry Current { get; private set; }
        }

        public readonly ref struct LineSplitEntry
        {
            public LineSplitEntry(ReadOnlySpan<char> line, ReadOnlySpan<char> separator)
            {
                Line = line;
                Separator = separator;
            }

            public ReadOnlySpan<char> Line { get; }
            public ReadOnlySpan<char> Separator { get; }

            public static implicit operator ReadOnlySpan<char>(LineSplitEntry entry) => entry.Line;
        }
    }
}