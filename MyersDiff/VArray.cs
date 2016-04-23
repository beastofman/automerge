using System;

namespace MyersDiff
{
    /// <summary>
    /// Массив V — согласно Майерсу, среди элементов V[−D], V[−D+2], ... , V[D-2], V[D] список максимально удаленных конечных точек D-путей.
    /// Или — максимальное количество шагов, на которые можно продвинуться по любой из диагоналей K от теущей позиции в матрице.
    /// Подробнее см. http://xmailserver.org/diff2.pdf, лемма 2, с. 5-6
    /// </summary>
    internal sealed class VArray
    {
        private long[] m_array;
        private long m_max;

        public VArray(long n, long m)
        {
            this.m_max = n + m;
            this.m_array = new long[this.m_max * 2 + 1];
        }

        private VArray()
        {
        }

        public long this[long k]
        {
            get { return this.m_array[k + this.m_max]; }
            set { this.m_array[k + this.m_max] = value; }
        }

        public VArray Copy(int d)
        {
            if (d == 0) d++;
            var v = new VArray
            {
                m_max = d,
                m_array = new long[2 * d + 1]
            };

            if (d <= this.m_max)
            {
                Array.Copy(this.m_array, this.m_max - v.m_max, v.m_array, 0, v.m_array.Length);
            }

            return v;
        }

        public override string ToString()
        {
            return $"V [-{this.m_max}..{this.m_max}]";
        }

        public long Y(long k)
        {
            return this[k] - k;
        }
    }
}