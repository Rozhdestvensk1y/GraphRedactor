using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GraphRedactor
{
    [Serializable]
    class Save
    {
        double x1;
        double x2;
        double y1;
        double y2;
        double angleX;
        double angleY;
        double angleZ;
        double scale;
        double xValue;
        double yValue;
        double zValue;
        double scaleValue;
        List<double> saveZCoord;
        List<double> zCoordinatesForLine;
        double StrokeT;
        object TG;
        Brush ST;
        Type type;
        bool morhpBool;
        int num_morph;
        int count_morph;
        bool group;
        int number_group;

        public double X1
        {
            get { return x1; }
            set { x1 = value; }
        }
        public double X2
        {
            get { return x2; }
            set { x2 = value; }
        }
        public double Y1
        {
            get { return y1; }
            set { y1 = value; }
        }
        public double Y2
        {
            get { return y2; }
            set { y2 = value; }
        }
        public double AngleX
        {
            get { return angleX; }
            set { angleX = value; }
        }
        public double AngleY
        {
            get { return angleY; }
            set { angleY = value; }
        }
        public double AngleZ
        {
            get { return angleZ; }
            set { angleZ = value; }
        }
        public double Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public double xSlider
        {
            get { return xValue; }
            set { xValue = value; }
        }
        public double ySlider
        {
            get { return yValue; }
            set { yValue = value; }
        }
        public double zSlider
        {
            get { return zValue; }
            set { zValue = value; }
        }
        public double scaleSlider
        {
            get { return scaleValue; }
            set { scaleValue = value; }
        }
        public Brush Stroke
        {
            get { return ST; }
            set { ST = value; }
        }
        public List<double> savedZCoord
        {
            get { return saveZCoord; }
            set { saveZCoord = value; }
        }
        public List<double> savedZCoordinatesLine
        {
            get { return zCoordinatesForLine; }
            set { zCoordinatesForLine = value; }
        }
        public double StrokeThickness
        {
            get { return StrokeT; }
            set { StrokeT = value; }
        }
        public object Tag
        {
            get { return TG; }
            set { TG = value; }
        }
        public Type Type
        {
            get { return type; }
            set { type = value; }
        }
        public bool Morph
        {
            get { return morhpBool; }
            set { morhpBool = value; }
        }
        public int numberMorph
        {
            get { return num_morph; }
            set { num_morph = value; }
        }
        public int countMorph
        {
            get { return count_morph; }
            set { count_morph = value; }
        }
        public bool Group
        {
            get { return group; }
            set { group = value; }
        }
        public int Number_group
        {
            get { return number_group; }
            set { number_group = value; }
        }
    }
}
