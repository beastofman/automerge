using Domain;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sources.TextFile
{
    public class TextFileSource : ISource
    {
        private readonly string m_filename;
        private readonly IList<TextLine> m_lines = new List<TextLine>();
        public int Length => this.m_lines?.Count ?? 0;

        public TextFileSource(string filename)
        {
            if (!File.Exists(filename)) throw new InvalidOperationException("Файл " + filename + " не найден!");
            this.m_filename = filename;
            using (var reader = new StreamReader(filename))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    this.m_lines.Add(new TextLine(line));
                    line = reader.ReadLine();
                }
            }
        }

        public IComparable this[int index] => this.m_lines[index];

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
        /// <summary>
        /// Хэш строки для быстрого сравнения
        /// </summary>
        public int Hash { get; }

        /// <summary>
        /// Строка текста
        /// </summary>
        public string Line { get; }

        public TextLine(string line)
        {
            /*
             * Сохраним исходную строку, но для
             * вычисления хэша очистим ее от окружающих пробелов и табуляций
             */
            this.Line = line;
            this.Hash = line.Trim().GetHashCode();
        }

        public int CompareTo(object obj)
        {
            //Сравниваем только хэши, лексическое равенство не интересует
            var o = obj as TextLine;
            return o == null ? 1 : this.Hash.CompareTo(o.Hash);
        }

        public override bool Equals(object obj)
        {
            //Сравниваем только хэши, лексическое равенство не интересует
            var o = obj as TextLine;
            return o != null && o.Hash == this.Hash;
        }

        public override int GetHashCode()
        {
            //Хэш посчитан в конструкторе, сэкономим время
            return this.Hash;
        }

        public override string ToString()
        {
            return this.Line + Environment.NewLine;
        }

        protected bool Equals(TextLine other)
        {
            //Сравниваем только хэши, лексическое равенство не интересует
            return this.Hash == other.Hash;
        }
    }
}