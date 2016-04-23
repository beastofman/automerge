using Domain;
using System;

namespace Sources.String
{
    public class StringSource : ISource
    {
        private readonly string m_string;

        public long Length => this.m_string?.Length ?? 0;

        public StringSource(string s)
        {
            this.m_string = s;
        }

        public IComparable this[long index]
        {
            get
            {
                if (this.Length == 0 ||
                    index > this.Length) return null;
                return this.m_string[(int)index];
            }
        }

        public override string ToString()
        {
            return $"Length: {this.Length}, String: {this.m_string}";
        }
    }
}