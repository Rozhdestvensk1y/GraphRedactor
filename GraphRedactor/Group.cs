using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using System.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Runtime.Serialization;

namespace GraphRedactor
{
    [CollectionDataContractAttribute]
    public class Group : IEnumerable, IEnumerator
    {
        [DataMember]
        List<Shape> shapeGroup = new List<Shape>();
        int pos = -1;
        bool selectionMode = false;
        bool morhingMode = false;
        double morphSlider = 0;
        double x_slider_c = 0;
        double y_slider_c = 0;
        double z_slider_c = 0;
        double scale_slider_c = 0;


        public bool selectMode
        {
            get { return selectionMode; }
            set { selectionMode = value; }
        }
        public bool morphMode
        {
            get { return morhingMode; }
            set { morhingMode = value; }
        }
        public double pos_slider
        {
            get { return morphSlider; }
            set { morphSlider = value; }
        }
        public double x_slider
        {
            get { return x_slider_c; }
            set { x_slider_c = value; }
        }
        public double y_slider
        {
            get { return y_slider_c; }
            set { y_slider_c = value; }
        }
        public double z_slider
        {
            get { return z_slider_c; }
            set { z_slider_c = value; }
        }
        public double scale_slider
        {
            get { return scale_slider_c; }
            set { scale_slider_c = value; }
        }
        public int Count()
        {
            return shapeGroup.Count();
        }
        public bool Contains(Shape obj)
        {
            return shapeGroup.Contains(obj);
        }
        public void Add(object obj)
        {
             shapeGroup.Add(obj as Shape);
        }
        public void Remove(Shape obj)
        {
            shapeGroup.Remove(obj);
        }
        public void RemoveAt(int ind)
        {
            shapeGroup.RemoveAt(ind);
        }
        public void Clear()
        {
            shapeGroup.Clear();
        }
        public Shape this[int i]
        {
            get
            {
                return shapeGroup[i];
            }
            set
            {
                shapeGroup[i] = value;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return shapeGroup.GetEnumerator();
        }
        public bool MoveNext()
        {
            if (pos < shapeGroup.Count - 1)
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
            get { return shapeGroup[pos]; }
        }
    }
}
