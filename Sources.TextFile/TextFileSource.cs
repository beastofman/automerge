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

        private readonly string m_filename;
        private readonly IList<TextLine> m_lines;
        public int Length => this.m_lines?.Count ?? 0;

        public TextFileSource(string filename)
        {
            if (!File.Exists(filename)) throw new InvalidOperationException("Файл " + filename + " не найден!");
            this.m_filename = filename;
            using (var reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    this.m_lines.Add(new TextLine(line));
                }
            }
        }

        public IComparable this[int index] => this.m_lines?[index];

        public override string ToString()
        {
            return $"Файл: {this.m_filename}, Кол-во строк: {this.Length}";
        }
    }

    /// <summary>
    /// Строка текстового файла
    /// </summary>
    public class TextLine : IComparable
    {
        public int Hash { get; }

        public string Line { get; }

        public TextLine(string line)
        {
            this.Line = line.Trim();
            this.Hash = line.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            var o = obj as TextLine;
            return o == null ? -1 : this.Hash.CompareTo(o.Hash);
        }
    }
}