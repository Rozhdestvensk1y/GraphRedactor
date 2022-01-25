using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;

namespace GraphRedactor
{
    public class LineSettings : INotifyPropertyChanged
    {
        private Color colorLine;
        private SolidColorBrush brushLine;
        public event PropertyChangedEventHandler PropertyChanged;
        private int thickness { get; set; }
        public Color ColorLine
        {
            get { return colorLine; }
            set
            {
                colorLine = value;
                brushLine = new SolidColorBrush(colorLine);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ColorLine"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BrushLine"));
            }
        }

        public Brush BrushLine
        {
            get { return brushLine; }
            set
            {
                brushLine = (SolidColorBrush)value;
                colorLine = brushLine.Color;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BrushLine"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ColorLine"));
            }
        }

        public int Thickness
        {
            get { return thickness; }
            set
            {
                thickness = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Thickness"));
            }
        }

        public LineSettings(int thicknes, Color c)
        {
            thickness = thicknes;
            colorLine = c;
            brushLine = new SolidColorBrush(c);
        }


    }
}
