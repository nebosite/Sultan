using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sultan
{
    class Permutator<T>
    {
        private int _size;
        T[] _set;

        public Permutator(T[] set)
        {
            _size = set.Length;
            _set = set;
        }

        public IEnumerable<T[]> Permutations{ get{ foreach (var gotIt in RecurseNextPermutation(_set, 0)) yield return _set; } }

        IEnumerable<bool> RecurseNextPermutation(T[] set, int position)
        {
            if (position == _size) yield return true;
            else
            {
                for (int i = position; i < _size; i++)
                {
                    Swap(set, i, position);
                    foreach (var gotIt in RecurseNextPermutation(set, position + 1)) yield return true;
                    Swap(set, i, position);
                }
            }
        }

        private void Swap(T[] set, int i, int position)
        {
            T temp = set[i];
            set[i] = set[position];
            set[position] = temp;
        }
    }

}
