using Domain;
using System;

namespace Sources.String
{
    public class StringSource : ISource
    {
        private readonly string m_string;

        public int Length => this.m_string?.Length ?? 0;

        public StringSource(string s)
        {
            this.m_string = s;
        }

        public IComparable this[int index]
        {
            get
            {
                if (this.Length == 0 ||
                    index > this.Length) return null;
                return this.m_string[index];
            }
        }

        public override string ToString()
        {
            return $"Строка: {this.m_string}, Кол-во символов: {this.Length}";
        }
    }
}