using Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sources.TextFile
{
    public class TextFileSource : ISource
    {
        /// <summary>
        /// Максимальная длина одной строки текстового файла
        /// </summary>
        private const int MAX_LINE_LENGTH = 1024;

        private readonly IList<TextLine> m_lines;

        public int Length => this.m_lines?.Count ?? 0;

        public TextFileSource(string filename)
        {
            if (!File.Exists(filename)) throw new InvalidOperationException("Файл " + filename + " не найден!");
            using (var reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                }
            }
        }

        public IComparable this[int index] => this.m_lines?[index];
    }

    public class TextLine : IComparable
    {
        public int Hash { get; }

        public string Line { get; }

        public TextLine(string str)
        {
            this.Line = str.Trim();
            this.Hash = str.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            var o = obj as TextLine;
            return o == null ? -1 : this.Hash.CompareTo(o.Hash);
        }
    }
}