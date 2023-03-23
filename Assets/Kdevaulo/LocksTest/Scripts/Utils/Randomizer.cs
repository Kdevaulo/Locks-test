using System;
using System.Collections.Generic;
using System.Linq;

namespace Kdevaulo.LocksTest.Scripts.Utils
{
    public class Randomizer
    {
        private readonly int _minValue;

        private readonly int _maxValue;

        private List<int> _values;

        private int _index = 0;

        private Random random = new Random();

        /// <param name="minValue">inclusive</param>
        /// <param name="maxValue">exclusive</param>
        public Randomizer(int minValue, int maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;

            _values = new List<int>(maxValue - minValue);

            Generate();
        }

        public void Generate()
        {
            _index = 0;

            _values.Clear();

            for (int i = _minValue; i < _maxValue; i++)
            {
                _values.Add(i);
            }

            _values = Shuffle(_values);
        }

        public int GetValue()
        {
            if (_index >= _values.Count)
            {
                Generate();
            }

            return _values[_index++];
        }

        private List<int> Shuffle(List<int> values)
        {
            return values.OrderBy(x => random.Next()).ToList();
        }
    }
}