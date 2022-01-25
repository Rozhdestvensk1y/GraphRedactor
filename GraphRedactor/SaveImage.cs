using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace GraphRedactor
{
    [Serializable]
    class SaveImage : IEnumerable, IEnumerator
    {
        List<Save> save = new List<Save>();
        int pos = -1;
        public void Add(object obj)
        {
            save.Add(obj as Save);
        }
        public void Clear()
        {
            save.Clear();
        }
        public int Count()
        {
            return save.Count();
        }
        public Save this[int i]
        {
            get
            {
                return save[i];
            }
            set
            {
                save[i] = value;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return save.GetEnumerator();
        }
        public bool MoveNext()
        {
            if (pos < save.Count - 1)
            {
                pos++;
                return true;
            }
            else
            {
                return false;
            }
        }

        // Установить указатель (position) перед началом набора.
        public void Reset()
        {
            pos = -1;
        }

        // Получить текущий элемент набора. 
        public object Current
        {
            get { return save[pos]; }
        }
    }
}
