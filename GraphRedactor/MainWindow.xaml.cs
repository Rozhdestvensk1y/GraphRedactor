using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;


namespace GraphRedactor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        Line line = new Line(); //линия
        public static LineSettings brushes;
        public Color curentBrush;
        public int currentThickness;
        Ellipse el = new Ellipse(); //эллипс
        Point p; //точка
        PointCollection points = new PointCollection();
        List<double> zCoordinates = new List<double>();//z координаты для линии
        List<double> zCoord = new List<double>();//z координаты для фигуры
        bool drawing = false;
        bool canmove = false;
        bool selectionGroup = false;
        double angleX = 0, angleY = 0, angleZ = 0, scale = 0;

        Group myGroup;
        List<Group> MyCanvasList = new List<Group>(); //коллекция листов
        List<Shape> lineGroup = new List<Shape>(); //коллекция передвигаемых линий
        List<Shape> morphingGroup = new List<Shape>(); //коллекция линий для морфинга
        Shape[] previousMorph; //старые позиции морфинга

        public MainWindow()
        {
            InitializeComponent();
            myGroup = new Group();
            MyCanvasList.Add(myGroup);
            for (int i = 0; i < 2; i++)
            {
                zCoordinates.Add(5);
            }
            brushes = new LineSettings(2, Colors.Black);
            LineColorPicker.DataContext = brushes;
            thickness.DataContext = brushes;
            LineSettings.Visibility = Visibility.Hidden;
            MorphingSettings.Visibility = Visibility.Hidden;
            Oper3D.Visibility = Visibility.Hidden;
            
        }
        private void checkPressAnyButton(Button but)
        {
            if (but.Opacity == 0.5)
            {
                Morphing1.Opacity = 1;
                MakeTree.Opacity = 1;
                MakeGroup.Opacity = 1;
                CreateLine.Opacity = 1;
                Operation3D.Opacity = 1;
                myGroup.morphMode = false;
                selectionGroup = false;
                Oper3D.Visibility = Visibility.Hidden;
                LineSettings.Visibility = Visibility.Hidden;
                MorphingSettings.Visibility = Visibility.Hidden;
                TreeSettings.Visibility = Visibility.Hidden;
                edit_line();
            }
            else
            {
                if (but.Opacity == 1)
                {
                    MakeTree.Opacity = 1;
                    MakeGroup.Opacity = 1;
                    CreateLine.Opacity = 1;
                    Operation3D.Opacity = 1;
                    Morphing1.Opacity = 1;
                    but.Opacity = 0.5;
                }
            }

        }
        private void MyCanvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //MyCanvas.Children.Clear();
        }
        #region Линия
        private void CreateLine_Click(object sender, RoutedEventArgs e)
        {
            Oper3D.Visibility = Visibility.Hidden;
            LineSettings.Visibility = Visibility.Visible;
            MorphingSettings.Visibility = Visibility.Hidden;
            TreeSettings.Visibility = Visibility.Hidden;
            LineSettings.Visibility = Visibility.Visible;

            drawing = true;
            myGroup.morphMode = false;
            MyCanvas.Cursor = Cursors.Cross;
            if (morphingGroup != null)
                foreach (Line obj in morphingGroup)
                    obj.Stroke = Brushes.Black;
            if (lineGroup != null)
                foreach (Line obj in lineGroup)
                    obj.Stroke = Brushes.Black;

            Button button = (Button)sender;
            checkPressAnyButton(button);
        }
        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing && MyCanvas.Cursor == Cursors.Cross && line != null)
            {
                line.Stroke = new SolidColorBrush(LineColorPicker.Color);
                line.StrokeThickness = brushes.Thickness;
                Point pointEnd = e.GetPosition(MyCanvas);
                line.X2 = pointEnd.X;
                line.Y2 = pointEnd.Y;
            }
            foreach (Line obj in myGroup)
            {
                Point pointEnd = e.GetPosition(MyCanvas);
                if (((Math.Abs(obj.X2 - pointEnd.X)) < 10 && ((Math.Abs(obj.Y2 - pointEnd.Y)) <10)|| (Math.Abs(obj.X1- pointEnd.X) < 10) && ((Math.Abs(obj.Y1 - pointEnd.Y)) < 10)) && popup.Text == "")//((obj.X1 == pointEnd.X && obj.Y1 == pointEnd.Y) || (obj.X2 == pointEnd.X && obj.Y2 == pointEnd.Y) && popup.Text == "")
                {
                    LineInfo.Background = obj.Stroke;
                    popup.Text += $"({obj.X1};{obj.Y1}) ({obj.X2};{obj.Y2})\n";
                    popup.Text += "Уравнение прямой\n";
                    popup.Text += equation(obj.X1, obj.Y1, obj.X2, obj.Y2);
                    popup1.IsOpen = true;
                } else
                {
                    popup.Text = "";
                    popup1.IsOpen = false;
                }
            }
        }
        private string equation(double X1, double Y1, double X2, double Y2)
        {
            string str;
            double k = (Y1 - Y2) / (X1 - X2);
            double b = Y2 - k * X2;
            str = $"y = {Math.Round(k, 2)}x{Math.Round(b, 2)}";
            return str;
        }
        private void MyCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MyCanvas.Cursor == Cursors.Cross)
            {
                drawing = true;
                p = e.GetPosition(MyCanvas);
                line = new Line();
                line.Stroke = new SolidColorBrush(LineColorPicker.Color);
                line.StrokeThickness = brushes.Thickness;
                line.X1 = line.X2 = p.X;
                line.Y1 = line.Y2 = p.Y;
                MyCanvas.Children.Add(line);
                drawLine(line);
            }
        }
        private void MyCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MyCanvas.Cursor == Cursors.Cross)
            {
                drawing = false;
                line.Tag = 0;

                line.MouseRightButtonDown += Line_MouseRightButtonDown;
                line.PreviewMouseDown += Line_PreviewMouseDown;
                line.MouseMove += Line_MouseMove;
                line.PreviewMouseUp += Line_PreviewMouseUp;
                myGroup.Add(line);
                line = null;
            }
        }
        private void Line_MouseRightButtonDown(object sender, MouseButtonEventArgs e) //Удаление линии
        {
            if (!myGroup.morphMode && !myGroup.selectMode && MyCanvas.Cursor == Cursors.Arrow)
            {
                line = sender as Line;
                deleteLine();
                myGroup.Remove(line);
                canmove = false;
                drawLinesOnMyCanvas();
                MyCanvas.UpdateLayout();
            }
            if (myGroup.selectMode) //режим выделения
            {
                line = sender as Line;
                line.Tag = 0;
                zCoord.Clear();
                line.Stroke = Brushes.Black;
                if (lineGroup.Contains(line))
                {
                    lineGroup.Remove(line);
                }
            }
            if (myGroup.morphMode)
            {
                line = sender as Line;
                line.Stroke = Brushes.Black;
                morphingGroup.Remove(line);
                morhingPrevArray();
            }
        }

        public void drawLine(Line obj)//Отрисовка
        {
            if (drawing)
            {
                Ellipse e1 = new Ellipse() { Width = 5, Height = 5, Stroke = Brushes.Black, Fill = Brushes.White };
                Ellipse e2 = new Ellipse() { Width = 5, Height = 5, Stroke = Brushes.Black, Fill = Brushes.White };

                Binding x1 = new Binding(); x1.Mode = BindingMode.TwoWay; x1.Path = new PropertyPath(Line.X1Property); x1.Converter = new MyConverter(); x1.ConverterParameter = e1;
                Binding y1 = new Binding(); y1.Mode = BindingMode.TwoWay; y1.Path = new PropertyPath(Line.Y1Property); y1.Converter = new MyConverter(); y1.ConverterParameter = e2;
                Binding x2 = new Binding(); x2.Mode = BindingMode.TwoWay; x2.Path = new PropertyPath(Line.X2Property); x2.Converter = new MyConverter(); x2.ConverterParameter = e1;
                Binding y2 = new Binding(); y2.Mode = BindingMode.TwoWay; y2.Path = new PropertyPath(Line.Y2Property); y2.Converter = new MyConverter(); y2.ConverterParameter = e2;

                x1.Source = y1.Source = obj;
                x2.Source = y2.Source = obj;

                e1.SetBinding(Canvas.LeftProperty, x1);
                e1.SetBinding(Canvas.TopProperty, y1);
                e2.SetBinding(Canvas.LeftProperty, x2);
                e2.SetBinding(Canvas.TopProperty, y2);

                e1.PreviewMouseDown += Line_PreviewMouseDown;
                e1.MouseMove += Line_MouseMove;
                e1.PreviewMouseUp += Line_PreviewMouseUp;
                e2.PreviewMouseDown += Line_PreviewMouseDown;
                e2.MouseMove += Line_MouseMove;
                e2.PreviewMouseUp += Line_PreviewMouseUp;

                MyCanvas.Children.Add(e1);
                MyCanvas.Children.Add(e2);
            }
        }
        public void drawLinesOnMyCanvas()//Заполнение холста
        {
            MyCanvas.Children.Clear();
            foreach (Shape obj in myGroup)
            {
                MyCanvas.Children.Add(obj);
                drawLine(obj as Line);
            }
        }
        public void deleteLine()//Удаление
        {
            List<Ellipse> ellist = new List<Ellipse>();
            foreach (Shape obj in ellist)
                if (obj is Ellipse)
                    MyCanvas.Children.Remove(obj);
        }
        private void edit_line()
        {
            drawing = false;
            MyCanvas.Cursor = Cursors.Arrow;
            myGroup.morphMode = false;
            myGroup.selectMode = false;
            LineSettings.Visibility = Visibility.Visible;

            if (morphingGroup != null)
                foreach (Line obj in morphingGroup)
                    obj.Stroke = Brushes.Black;
            if (lineGroup != null)
                foreach (Line obj in lineGroup)
                    obj.Stroke = Brushes.Green;
        }
        private void Line_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (MyCanvas.Cursor == Cursors.Arrow)
                {
                    p = e.GetPosition(MyCanvas);
                    if (sender is Line)
                    {

                        line = sender as Line;
                        line.Tag = 0;
                        line.Stroke = Brushes.Red;
                        Mouse.Capture(line); //захват мыши линией

                    }
                    try
                    {
                        if (sender is Ellipse && !myGroup.morphMode && !myGroup.selectMode)
                        {
                            el = sender as Ellipse;
                            Mouse.Capture(el);
                            el.SetValue(Canvas.LeftProperty, e.GetPosition(MyCanvas).X + (el.Width / 2));
                            el.SetValue(Canvas.TopProperty, e.GetPosition(MyCanvas).Y + (el.Width / 2));
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    canmove = true;
                    if (myGroup.selectMode && e.LeftButton == MouseButtonState.Pressed) //режим выделения
                    {
                        line = sender as Line;
                        line.Tag = 1;  //присвоили тег выделения
                        line.Stroke = Brushes.Green;
                        MyCanvas.UpdateLayout();
                        if (!lineGroup.Contains(line))
                        {
                            lineGroup.Add(line);
                        }
                    }
                    if (myGroup.morphMode && e.LeftButton == MouseButtonState.Pressed && sender is Line)
                    {
                        line = sender as Line;
                        line.Stroke = Brushes.Blue;
                        if (morphingGroup != null && !morphingGroup.Contains(line))
                        {
                            morphingGroup.Add(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (canmove && MyCanvas.Cursor == Cursors.Arrow && !myGroup.selectMode && !myGroup.morphMode)
                {

                    Ellipse tmpel = new Ellipse();
                    Line tmp = new Line();
                    if (Mouse.Captured is Line)
                    {
                        tmp = Mouse.Captured as Line; //присвоили временной линии линию захваченную мышкой
                        Canvas.SetLeft(tmp, e.GetPosition(MyCanvas).X - p.X); //передвижения линии по канвасу 
                        Canvas.SetTop(tmp, e.GetPosition(MyCanvas).Y - p.Y);
                    }
                    try
                    {
                        if (Mouse.Captured is Ellipse)
                        {
                            tmpel = Mouse.Captured as Ellipse;
                            Canvas.SetLeft(tmpel, e.GetPosition(MyCanvas).X + (el.Width / 2));
                            Canvas.SetTop(tmpel, e.GetPosition(MyCanvas).Y + (el.Width / 2));
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    MyCanvas.UpdateLayout();
                }
                if (myGroup.selectMode && e.LeftButton == MouseButtonState.Pressed && Mouse.Captured != null && !myGroup.morphMode && sender is Line) //режим выделения, и мышь зажата
                {
                    foreach (Line obj in myGroup)
                    {
                        if (obj.Tag != null && (int)obj.Tag == 1) //если тег не пустой и в режиме выделения 
                        {
                            Canvas.SetLeft(obj as Line, e.GetPosition(MyCanvas).X - p.X); //перемещаем выделеннные линии
                            Canvas.SetTop(obj as Line, e.GetPosition(MyCanvas).Y - p.Y);
                            MyCanvas.UpdateLayout();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Line_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!myGroup.morphMode && !myGroup.selectMode && Mouse.Captured != null)
            {
                if (Mouse.Captured is Line)
                {
                    line = Mouse.Captured as Line; //получаем захваченную линию
                    Canvas.SetLeft(line, 0);
                    Canvas.SetTop(line, 0);

                    line.X1 = line.X1 + (e.GetPosition(MyCanvas).X - p.X);
                    line.X2 = line.X2 + (e.GetPosition(MyCanvas).X - p.X);
                    line.Y1 = line.Y1 + (e.GetPosition(MyCanvas).Y - p.Y);
                    line.Y2 = line.Y2 + (e.GetPosition(MyCanvas).Y - p.Y);
                    testBorders(line.X1, line.Y1, line.X2, line.Y2);
                    line.Stroke = new SolidColorBrush(LineColorPicker.Color);
                    line.StrokeThickness = brushes.Thickness;
                }
                Mouse.Capture(null); //освобождаем мышь
                canmove = false;
            }
            if (myGroup.selectMode && line != null && sender is Line)
            {
                for (int i = 0; i < myGroup.Count(); i++)
                {
                    if ((myGroup[i] as Line).Tag != null && (int)(myGroup[i] as Line).Tag == 1)
                    {
                        Line tmp = myGroup[i] as Line;
                        Canvas.SetLeft(tmp, 0);
                        Canvas.SetTop(tmp, 0);
                        tmp.X1 = tmp.X1 + (e.GetPosition(MyCanvas).X - p.X);
                        tmp.X2 = tmp.X2 + (e.GetPosition(MyCanvas).X - p.X);
                        tmp.Y1 = tmp.Y1 + (e.GetPosition(MyCanvas).Y - p.Y);
                        tmp.Y2 = tmp.Y2 + (e.GetPosition(MyCanvas).Y - p.Y);
                        testBorders(tmp.X1, tmp.Y1, tmp.X2, tmp.Y2);
                    }
                }
                Mouse.Capture(null);
                MyCanvas.UpdateLayout(); //обнавляем канву
            }
            if (myGroup.morphMode)
                Mouse.Capture(null);
            morhingPrevArray();
            line = null;
        }

        public void testBorders(double x1, double y1, double x2, double y2)//Проверка границ
        {
            //if (x1 > MyCanvas.ActualWidth || x2 > MyCanvas.ActualWidth)
            //{
            //    if (x1 > x2)
            //        MyCanvas.Width = x1 + 20;
            //    else
            //        MyCanvas.Width = x2 + 20;
            //}
            //else if (x1 < scr_view.ActualWidth && x2 < scr_view.ActualWidth)
            //    MyCanvas.Width = scr_view.Width;

            //if (y1 > MyCanvas.ActualHeight || y2 > MyCanvas.ActualHeight)
            //{
            //    if (y1 > y2)
            //        MyCanvas.Height = y1 + 20;
            //    else
            //        MyCanvas.Height = y2 + 20;
            //}
            //else if (y1 < scr_view.ActualHeight && y2 < scr_view.ActualHeight)
            //    MyCanvas.Height = scr_view.Height;
        }
        public void drawLinesOnCanvas()//Заполнение холста
        {
            MyCanvas.Children.Clear();
            foreach (Shape obj in myGroup)
            {
                MyCanvas.Children.Add(obj);
                drawLine(obj as Line);
            }
        }

        #endregion
        #region Морфинг
        private void Morphing_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;


            Oper3D.Visibility = Visibility.Hidden;
            LineSettings.Visibility = Visibility.Hidden;
            MorphingAccept.Visibility = Visibility.Visible;
            TreeSettings.Visibility = Visibility.Hidden;
            MorphingSettings.Visibility = Visibility.Visible;
            morph_slider.Visibility = Visibility.Hidden;

            MorphingCancel.Content = "Отмена";

            drawing = false;

            MyCanvas.Cursor = Cursors.Arrow;
            myGroup.morphMode = true;
            if (morphingGroup != null)
                foreach (Line obj in morphingGroup)
                    obj.Stroke = Brushes.Blue;
            if (lineGroup != null)
                foreach (Line obj in lineGroup)
                    obj.Stroke = Brushes.Black;
            checkPressAnyButton(button);

        }
        public void morhingPrevArray()//Старые положение морфинга
        {
            if (morphingGroup != null)
            {
                previousMorph = new Shape[morphingGroup.Count];
                for (int i = 0; i < morphingGroup.Count; i++)
                {
                    Line tmp = new Line();
                    previousMorph[i] = tmp;
                    (previousMorph[i] as Line).X1 = (morphingGroup[i] as Line).X1;
                    (previousMorph[i] as Line).X2 = (morphingGroup[i] as Line).X2;
                    (previousMorph[i] as Line).Y1 = (morphingGroup[i] as Line).Y1;
                    (previousMorph[i] as Line).Y2 = (morphingGroup[i] as Line).Y2;
                }
            }
        }

        public void Morphing(double now)//морфинг
        {
            if (morphingGroup != null)
                for (int i = 0; i < morphingGroup.Count - 1; i++)
                {
                    if (i % 2 == 0 && myGroup.Contains(morphingGroup[i]))
                    {
                        (morphingGroup[i] as Line).X1 = (morphingGroup[i + 1] as Line).X1 * now + (1 - now) * (previousMorph[i] as Line).X1;
                        (morphingGroup[i] as Line).X2 = (morphingGroup[i + 1] as Line).X2 * now + (1 - now) * (previousMorph[i] as Line).X2;
                        (morphingGroup[i] as Line).Y1 = (morphingGroup[i + 1] as Line).Y1 * now + (1 - now) * (previousMorph[i] as Line).Y1;
                        (morphingGroup[i] as Line).Y2 = (morphingGroup[i + 1] as Line).Y2 * now + (1 - now) * (previousMorph[i] as Line).Y2;
                    }
                }
        }

        private void MorphingAccept_Click(object sender, RoutedEventArgs e)
        {
            MorphingAccept.Visibility = Visibility.Hidden;
            MorphingCancel.Content = "Выход";
            List<Shape> tmp = new List<Shape>();
            if (morphingGroup != null)
                foreach (Line obj in morphingGroup)
                {
                    if (myGroup.Contains(obj))
                        obj.Stroke = Brushes.Black;
                    else
                        tmp.Add(obj);
                }
            //morphingGroup.Clear();
            //morphingGroup = tmp;
            morph_slider.Visibility = Visibility.Visible;
            morhingPrevArray();
            morph_slider.Value = 0;

        }

        private void MorphingCancel_Click(object sender, RoutedEventArgs e)
        {
            MorphingSettings.Visibility = Visibility.Hidden;
            List<Shape> tmp = new List<Shape>();
            Morphing(0);
            if (morphingGroup != null)
                foreach (Line obj in morphingGroup)
                {
                    if (myGroup.Contains(obj))
                        obj.Stroke = Brushes.Black;
                    else
                        tmp.Add(obj);
                }
            morphingGroup.Clear();
            morphingGroup = tmp;
            morhingPrevArray();
            morph_slider.Value = 0;
            Morphing1.Opacity = 1;
            myGroup.morphMode = false;
        }
        private void morph_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Morphing(e.NewValue); //передача параметров морфингу через слайдер
            myGroup.pos_slider = e.NewValue;
        }
        #endregion
        #region 3D операции
        private void Operations_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            Oper3D.Visibility = Visibility.Visible;
            LineSettings.Visibility = Visibility.Hidden;
            MorphingSettings.Visibility = Visibility.Hidden;
            TreeSettings.Visibility = Visibility.Hidden;

            //Morphing1.Opacity = 1;
            //CreateLine.Opacity = 1;
            //MakeTree.Opacity = 1;
            //MakeGroup.Opacity = 1;
            //Operation3D.Opacity = 0.5;

            checkPressAnyButton(button);
        }
        private void x_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RotateAngleX(e.NewValue);
            myGroup.x_slider = e.NewValue;
        }
        private void y_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RotateAngleY(e.NewValue);
            myGroup.y_slider = e.NewValue;
        }

        private void z_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RotateAngleZ(e.NewValue);
            myGroup.z_slider = e.NewValue;
        }
        private void scale_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FigureScale(e.NewValue);
            myGroup.scale_slider = e.NewValue;
        }
        private void mirror_Click(object sender, RoutedEventArgs e)
        {
            morhingPrevArray();
            CheckGroupCount();
            if (lineGroup.Count == 1)
            {
                for (int i = 0; i < lineGroup.Count; i++)
                {
                    points.Add(new Point((lineGroup[i] as Line).X1, (lineGroup[i] as Line).Y1));
                    points.Add(new Point((lineGroup[i] as Line).X2, (lineGroup[i] as Line).Y2));
                    Matrix<double> matrix = MatrixTransfer(((lineGroup[i] as Line).X1 + (lineGroup[i] as Line).X2) / -2, ((lineGroup[i] as Line).Y1 + (lineGroup[i] as Line).Y2) / -2, PointsToMatrix(points, zCoordinates));
                    if (mirroring_comboBox.Text == "ZOX")
                        matrix = MatrixMirrorZOX(matrix);
                    else if (mirroring_comboBox.Text == "XOY")
                        matrix = MatrixMirrorXOY(matrix);
                    else if (mirroring_comboBox.Text == "YOZ")
                        matrix = MatrixMirrorYOZ(matrix);

                    matrix = MatrixTransfer(((lineGroup[i] as Line).X1 + (lineGroup[i] as Line).X2) / 2, ((lineGroup[i] as Line).Y1 + (lineGroup[i] as Line).Y2) / 2, matrix);
                    (lineGroup[i] as Line).X1 = matrix[0, 0];
                    (lineGroup[i] as Line).Y1 = matrix[0, 1];
                    (lineGroup[i] as Line).X2 = matrix[1, 0];
                    (lineGroup[i] as Line).Y2 = matrix[1, 1];
                    zCoordinates[0] = matrix[0, 2];
                    zCoordinates[1] = matrix[1, 2];
                    points.Clear();
                }
            }
            else if (lineGroup.Count > 1)
            {
                for (int i = 0; i < lineGroup.Count; i++)
                {
                    points.Add(new Point((lineGroup[i] as Line).X1, (lineGroup[i] as Line).Y1));
                    points.Add(new Point((lineGroup[i] as Line).X2, (lineGroup[i] as Line).Y2));
                }
                Point centr = FindCentr(points);
                Matrix<double> matrix = MatrixTransfer(-centr.X, -centr.Y, PointsToMatrix(points, zCoord));
                if (mirroring_comboBox.Text == "ZOX")
                    matrix = MatrixMirrorZOX(matrix);
                else if (mirroring_comboBox.Text == "XOY")
                    matrix = MatrixMirrorXOY(matrix);
                else if (mirroring_comboBox.Text == "YOZ")
                    matrix = MatrixMirrorYOZ(matrix);

                matrix = MatrixTransfer(centr.X, centr.Y, matrix);

                for (int i = 0; i < lineGroup.Count; i++)
                {
                    (lineGroup[i] as Line).X1 = matrix[i * 2, 0];
                    (lineGroup[i] as Line).Y1 = matrix[i * 2, 1];
                    (lineGroup[i] as Line).X2 = matrix[i * 2 + 1, 0];
                    (lineGroup[i] as Line).Y2 = matrix[i * 2 + 1, 1];
                    zCoord[i * 2] = matrix[i * 2, 2];
                    zCoord[i * 2 + 1] = matrix[i * 2 + 1, 2];
                }
                points.Clear();
            }
        }
        public void RotateAngleX(double aX)
        {
            morhingPrevArray();
            double a = angleX - aX;
            angleX = aX;
            CheckGroupCount();
            if (lineGroup.Count == 1)
            {
                for (int i = 0; i < lineGroup.Count; i++)
                {
                    points.Add(new Point((lineGroup[i] as Line).X1, (lineGroup[i] as Line).Y1));
                    points.Add(new Point((lineGroup[i] as Line).X2, (lineGroup[i] as Line).Y2));
                    Matrix<double> matrix = MatrixTransfer(((lineGroup[i] as Line).X1 + (lineGroup[i] as Line).X2) / -2, ((lineGroup[i] as Line).Y1 + (lineGroup[i] as Line).Y2) / -2, PointsToMatrix(points, zCoordinates));
                    matrix = MatrixAngleX(a, matrix);
                    matrix = MatrixTransfer(((lineGroup[i] as Line).X1 + (lineGroup[i] as Line).X2) / 2, ((lineGroup[i] as Line).Y1 + (lineGroup[i] as Line).Y2) / 2, matrix);
                    (lineGroup[i] as Line).X1 = matrix[0, 0];
                    (lineGroup[i] as Line).Y1 = matrix[0, 1];
                    (lineGroup[i] as Line).X2 = matrix[1, 0];
                    (lineGroup[i] as Line).Y2 = matrix[1, 1];
                    zCoordinates[0] = matrix[0, 2];
                    zCoordinates[1] = matrix[1, 2];
                    points.Clear();
                }
            }
            else if (lineGroup.Count > 1)
            {
                for (int i = 0; i < lineGroup.Count; i++)
                {
                    points.Add(new Point((lineGroup[i] as Line).X1, (lineGroup[i] as Line).Y1));
                    points.Add(new Point((lineGroup[i] as Line).X2, (lineGroup[i] as Line).Y2));
                }
                Point centr = FindCentr(points);
                Matrix<double> matrix = MatrixTransfer(-centr.X, -centr.Y, PointsToMatrix(points, zCoord));
                matrix = MatrixAngleX(a, matrix);
                matrix = MatrixTransfer(centr.X, centr.Y, matrix);

                for (int i = 0; i < lineGroup.Count; i++)
                {
                    (lineGroup[i] as Line).X1 = matrix[i * 2, 0];
                    (lineGroup[i] as Line).Y1 = matrix[i * 2, 1];
                    (lineGroup[i] as Line).X2 = matrix[i * 2 + 1, 0];
                    (lineGroup[i] as Line).Y2 = matrix[i * 2 + 1, 1];
                    zCoord[i * 2] = matrix[i * 2, 2];
                    zCoord[i * 2 + 1] = matrix[i * 2 + 1, 2];
                }
                points.Clear();
            }
        }
        public void RotateAngleY(double aY)
        {
            morhingPrevArray();
            double a = angleY - aY;
            angleY = aY;
            CheckGroupCount();
            if (lineGroup.Count == 1)
            {
                for (int i = 0; i < lineGroup.Count; i++)
                {
                    points.Add(new Point((lineGroup[i] as Line).X1, (lineGroup[i] as Line).Y1));
                    points.Add(new Point((lineGroup[i] as Line).X2, (lineGroup[i] as Line).Y2));
                    Matrix<double> matrix = MatrixTransfer(((lineGroup[i] as Line).X1 + (lineGroup[i] as Line).X2) / -2, ((lineGroup[i] as Line).Y1 + (lineGroup[i] as Line).Y2) / -2, PointsToMatrix(points, zCoordinates));
                    matrix = MatrixAngleY(a, matrix);
                    matrix = MatrixTransfer(((lineGroup[i] as Line).X1 + (lineGroup[i] as Line).X2) / 2, ((lineGroup[i] as Line).Y1 + (lineGroup[i] as Line).Y2) / 2, matrix);
                    (lineGroup[i] as Line).X1 = matrix[0, 0];
                    (lineGroup[i] as Line).Y1 = matrix[0, 1];
                    (lineGroup[i] as Line).X2 = matrix[1, 0];
                    (lineGroup[i] as Line).Y2 = matrix[1, 1];
                    zCoordinates[0] = matrix[0, 2];
                    zCoordinates[1] = matrix[1, 2];
                    points.Clear();
                }
            }
            else if (lineGroup.Count > 1)
            {
                for (int i = 0; i < lineGroup.Count; i++)
                {
                    points.Add(new Point((lineGroup[i] as Line).X1, (lineGroup[i] as Line).Y1));
                    points.Add(new Point((lineGroup[i] as Line).X2, (lineGroup[i] as Line).Y2));
                }
                Point centr = FindCentr(points);
                Matrix<double> matrix = MatrixTransfer(-centr.X, -centr.Y, PointsToMatrix(points, zCoord));
                matrix = MatrixAngleY(a, matrix);
                matrix = MatrixTransfer(centr.X, centr.Y, matrix);

                for (int i = 0; i < lineGroup.Count; i++)
                {
                    (lineGroup[i] as Line).X1 = matrix[i * 2, 0];
                    (lineGroup[i] as Line).Y1 = matrix[i * 2, 1];
                    (lineGroup[i] as Line).X2 = matrix[i * 2 + 1, 0];
                    (lineGroup[i] as Line).Y2 = matrix[i * 2 + 1, 1];
                    zCoord[i * 2] = matrix[i * 2, 2];
                    zCoord[i * 2 + 1] = matrix[i * 2 + 1, 2];
                }
                points.Clear();
            }
        }
        public void FigureScale(double scaleValue)
        {
            morhingPrevArray();
            double s = scale;
            scale = scaleValue;
            double realize = scale - s + 1;
            CheckGroupCount();
            if (lineGroup.Count == 1)
            {
                for (int i = 0; i < lineGroup.Count; i++)
                {
                    points.Add(new Point((lineGroup[i] as Line).X1, (lineGroup[i] as Line).Y1));
                    points.Add(new Point((lineGroup[i] as Line).X2, (lineGroup[i] as Line).Y2));
                    Matrix<double> matrix = MatrixTransfer(((lineGroup[i] as Line).X1 + (lineGroup[i] as Line).X2) / -2, ((lineGroup[i] as Line).Y1 + (lineGroup[i] as Line).Y2) / -2, PointsToMatrix(points, zCoordinates));
                    matrix = MatrixScale(realize, matrix);
                    matrix = MatrixTransfer(((lineGroup[i] as Line).X1 + (lineGroup[i] as Line).X2) / 2, ((lineGroup[i] as Line).Y1 + (lineGroup[i] as Line).Y2) / 2, matrix);
                    (lineGroup[i] as Line).X1 = matrix[0, 0];
                    (lineGroup[i] as Line).Y1 = matrix[0, 1];
                    (lineGroup[i] as Line).X2 = matrix[1, 0];
                    (lineGroup[i] as Line).Y2 = matrix[1, 1];
                    zCoordinates[0] = matrix[0, 2];
                    zCoordinates[1] = matrix[1, 2];
                    points.Clear();
                }
            }
            else if (lineGroup.Count > 1)
            {
                for (int i = 0; i < lineGroup.Count; i++)
                {
                    points.Add(new Point((lineGroup[i] as Line).X1, (lineGroup[i] as Line).Y1));
                    points.Add(new Point((lineGroup[i] as Line).X2, (lineGroup[i] as Line).Y2));
                }
                Point centr = FindCentr(points);
                Matrix<double> matrix = MatrixTransfer(-centr.X, -centr.Y, PointsToMatrix(points, zCoord));
                matrix = MatrixScale(realize, matrix);
                matrix = MatrixTransfer(centr.X, centr.Y, matrix);


                for (int i = 0; i < lineGroup.Count; i++)
                {
                    (lineGroup[i] as Line).X1 = matrix[i * 2, 0];
                    (lineGroup[i] as Line).Y1 = matrix[i * 2, 1];
                    (lineGroup[i] as Line).X2 = matrix[i * 2 + 1, 0];
                    (lineGroup[i] as Line).Y2 = matrix[i * 2 + 1, 1];
                    zCoord[i * 2] = matrix[i * 2, 2];
                    zCoord[i * 2 + 1] = matrix[i * 2 + 1, 2];
                }
                points.Clear();
            }
        }
        public void RotateAngleZ(double aZ)
        {
            int dsf;
            morhingPrevArray();
            double a = angleZ - aZ;
            angleZ = aZ;
            CheckGroupCount();
            if (lineGroup.Count == 1)
            {
                for (int i = 0; i < lineGroup.Count; i++)
                {
                    points.Add(new Point((lineGroup[i] as Line).X1, (lineGroup[i] as Line).Y1));
                    points.Add(new Point((lineGroup[i] as Line).X2, (lineGroup[i] as Line).Y2));
                    Matrix<double> matrix = MatrixTransfer(((lineGroup[i] as Line).X1 + (lineGroup[i] as Line).X2) / -2, ((lineGroup[i] as Line).Y1 + (lineGroup[i] as Line).Y2) / -2, PointsToMatrix(points, zCoordinates));
                    matrix = MatrixAngleZ(a, matrix);
                    matrix = MatrixTransfer(((lineGroup[i] as Line).X1 + (lineGroup[i] as Line).X2) / 2, ((lineGroup[i] as Line).Y1 + (lineGroup[i] as Line).Y2) / 2, matrix);
                    (lineGroup[i] as Line).X1 = matrix[0, 0];
                    (lineGroup[i] as Line).Y1 = matrix[0, 1];
                    (lineGroup[i] as Line).X2 = matrix[1, 0];
                    (lineGroup[i] as Line).Y2 = matrix[1, 1];
                    zCoordinates[0] = matrix[0, 2];
                    zCoordinates[1] = matrix[1, 2];
                    points.Clear();
                }
            }
            else if (lineGroup.Count > 1)
            {
                for (int i = 0; i < lineGroup.Count; i++)
                {
                    points.Add(new Point((lineGroup[i] as Line).X1, (lineGroup[i] as Line).Y1));
                    points.Add(new Point((lineGroup[i] as Line).X2, (lineGroup[i] as Line).Y2));
                }
                Point centr = FindCentr(points);
                Matrix<double> matrix = MatrixTransfer(-centr.X, -centr.Y, PointsToMatrix(points, zCoord));
                matrix = MatrixAngleZ(a, matrix);
                matrix = MatrixTransfer(centr.X, centr.Y, matrix);

                for (int i = 0; i < lineGroup.Count; i++)
                {
                    (lineGroup[i] as Line).X1 = matrix[i * 2, 0];
                    (lineGroup[i] as Line).Y1 = matrix[i * 2, 1];
                    (lineGroup[i] as Line).X2 = matrix[i * 2 + 1, 0];
                    (lineGroup[i] as Line).Y2 = matrix[i * 2 + 1, 1];
                    zCoord[i * 2] = matrix[i * 2, 2];
                    zCoord[i * 2 + 1] = matrix[i * 2 + 1, 2];
                }
                points.Clear();
            }

        }
        public Matrix<double> MatrixAngleX(double a, Matrix<double> start)
        {
            Matrix<double> rotate = DenseMatrix.OfArray(new double[,] {
                                {1, 0, 0, 0},
                                {0, Math.Cos(a*Math.PI/180), -Math.Sin(a*Math.PI/180), 0},

                                {0, Math.Sin(a*Math.PI/180), Math.Cos(a*Math.PI/180), 0},
                                {0, 0, 0, 1 } });
            return start * rotate;
        }
        public Matrix<double> MatrixAngleY(double a, Matrix<double> start)
        {
            Matrix<double> rotate = DenseMatrix.OfArray(new double[,] {
                                {Math.Cos(a*Math.PI/180), 0, Math.Sin(a*Math.PI/180), 0},
                                {0, 1, 0, 0},

                                {-Math.Sin(a*Math.PI/180), 0, Math.Cos(a*Math.PI/180), 0},
                                {0, 0, 0, 1 } });
            return start * rotate;
        }
        public Matrix<double> MatrixMirrorXOY(Matrix<double> start)
        {
            Matrix<double> rotate = DenseMatrix.OfArray(new double[,] {
                                {1, 0, 0, 0 },
                                {0, 1, 0, 0 },

                                {0, 0,-1, 0 },
                                {0, 0, 0, 1 } });
            return start * rotate;
        }
        public Matrix<double> MatrixMirrorZOX(Matrix<double> start)
        {
            Matrix<double> rotate = DenseMatrix.OfArray(new double[,] {
                                {1, 0, 0, 0 },
                                {0, -1, 0, 0 },

                                {0, 0,1, 0 },
                                {0, 0, 0, 1 } });
            return start * rotate;
        }
        public Matrix<double> MatrixMirrorYOZ(Matrix<double> start)
        {
            Matrix<double> rotate = DenseMatrix.OfArray(new double[,] {
                                {-1, 0, 0, 0 },
                                {0, 1, 0, 0 },

                                {0, 0,1, 0 },
                                {0, 0, 0, 1 } });
            return start * rotate;
        }
        public Matrix<double> MatrixAngleZ(double a, Matrix<double> start)
        {
            Matrix<double> rotate = DenseMatrix.OfArray(new double[,] {
                                {Math.Cos(a*Math.PI/180), -Math.Sin(a*Math.PI/180), 0, 0},
                                {Math.Sin(a*Math.PI/180), Math.Cos(a*Math.PI/180), 0, 0},

                                {0, 0, 1, 0},
                                {0, 0, 0, 1 } });
            return start * rotate;
        }
        public Matrix<double> MatrixScale(double s, Matrix<double> start)
        {
            Matrix<double> scale = DenseMatrix.OfArray(new double[,] {
                                {s, 0, 0, 0},
                                {0, s, 0, 0},
                                {0, 0, s, 0},
                                {0, 0, 0, 1} });
            return start * scale;
        }
        public Matrix<double> MatrixTransfer(double dx, double dy, Matrix<double> Points)
        {
            Matrix<double> transfer = DenseMatrix.OfArray(new double[,] {
                                {1, 0, 0, 0},
                                {0, 1, 0, 0},
                                {0, 0, 1, 0},
                                {dx, dy, 0, 1 } });
            return Points * transfer;
        }
        public Matrix<double> PointsToMatrix(PointCollection col, List<double> zet)
        {
            double[,] arr = new double[col.Count, 4];
            for (int i = 0; i < col.Count; i++)
            {
                arr[i, 0] = col[i].X;
                arr[i, 1] = col[i].Y;
                arr[i, 2] = zet[i];
                arr[i, 3] = 1;
            }
            Matrix<double> start = DenseMatrix.OfArray(arr);

            return start;
        }
        static Point FindCentr(PointCollection points)
        {
            double minX = 10000;
            double minY = 10000;
            double maxX = -10000;
            double maxY = -10000;

            foreach (var item in points)
            {
                if (item.X < minX) minX = item.X;
                if (item.Y < minY) minY = item.Y;
                if (item.X > maxX) maxX = item.X;
                if (item.Y > maxY) maxY = item.Y;

            }
            double centrX = (minX + maxX) / 2;
            double centrY = (minY + maxY) / 2;
            return new Point(centrX, centrY);
        }
        #endregion
        #region Группировка
        private void MakeGroup_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            Oper3D.Visibility = Visibility.Hidden;
            LineSettings.Visibility = Visibility.Hidden;
            MorphingSettings.Visibility = Visibility.Hidden;
            TreeSettings.Visibility = Visibility.Hidden;

            drawing = false;

            //Morphing1.Opacity = 0.5;
            //CreateLine.Opacity = 1;
            //MakeTree.Opacity = 1;
            //MakeGroup.Opacity = 0.5;
            //Operation3D.Opacity=1;

            MyCanvas.Cursor = Cursors.Arrow;
            selectionGroup = true;
            myGroup.selectMode = true; //переход в режим выделения
            myGroup.morphMode = false;
            if (lineGroup != null)
                foreach (Line obj in lineGroup)
                    obj.Stroke = Brushes.Green;
            if (morphingGroup != null)
                foreach (Line obj in morphingGroup)
                    obj.Stroke = Brushes.Black;
            checkPressAnyButton(button);

        }
        public void CheckGroupCount()
        {
            if (lineGroup.Count > 1 && zCoord.Count / lineGroup.Count != 2)
            {
                for (int i = 0; i < lineGroup.Count; i++)
                {
                    zCoord.Add(0);
                    zCoord.Add(0);
                }
            }
        }
        #endregion
        #region Фрактальное дерево
        private void FractalTree_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            Oper3D.Visibility = Visibility.Hidden;
            LineSettings.Visibility = Visibility.Hidden;
            MorphingSettings.Visibility = Visibility.Hidden;
            TreeSettings.Visibility = Visibility.Visible;
            MyCanvas.Visibility = Visibility.Hidden;
            MyCanvasFT.Visibility = Visibility.Visible;
            //Morphing1.Opacity = 1;
            //CreateLine.Opacity = 1;
            //MakeTree.Opacity = 0.5;
            //MakeGroup.Opacity = 1;
            //Operation3D.Opacity = 1;
            checkPressAnyButton(button);
        }

        private void ExitTree_Click(object sender, RoutedEventArgs e)
        {
            TreeSettings.Visibility = Visibility.Hidden;
            MyCanvasFT.Visibility = Visibility.Hidden;
            MyCanvas.Visibility = Visibility.Visible;
            MakeTree.Opacity = 1;
            //Morphing1.Opacity = 1;
            //CreateLine.Opacity = 1;
            //MakeTree.Opacity = 1;
            //MakeGroup.Opacity = 1;
        }
        private void MyCanvasFT_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ChoiceFractal.Text == "Дерево Пифагора")
                DrawBinaryTree((int)iterationsTree.Value, e.GetPosition(MyCanvasFT).X, e.GetPosition(MyCanvasFT).Y, ScaleTree.Value, angleTree.Value);
            else if (ChoiceFractal.Text == "Треугольник, центр масс")
                CreateTriangle((int)iterationsTree.Value);
            else if (ChoiceFractal.Text == "Треугольник Серпинского")
            {
                ScaleTree.IsEnabled = false;
                angleTree.IsEnabled = false;
                Point topPoint = new Point(760 / 2f, 0);
                Point leftPoint = new Point(0, 760);
                Point rightPoint = new Point(760, 760);
                int level = (int)iterationsTree.Value;
                DrawTriangleSerp(level, topPoint, leftPoint, rightPoint);
            }
        }

        private void CreateTriangle(int level)
        {
            int h = 760;
            int w = 760;
            Point A = new Point(w * 3 / 4, h * 3 / 4);
            Point B = new Point(w / 4, h * 3 / 4);
            Point C = new Point(w / 2, h / 4);
            DrawLine(A.X, A.Y, B.X, B.Y);
            DrawLine(B.X, B.Y, C.X, C.Y);
            DrawLine(C.X, C.Y, A.X, A.Y);
            drawtrianglecentr(A, B, C, level);

        }

        private void drawtrianglecentr(Point a, Point b, Point c, int level)
        {
            if (level == 0)
                return;
            Point D = new Point();
            Point v1 = new Point();
            Point v2 = new Point();

            v1.X = b.X - a.X;
            v1.Y = b.Y - a.Y;

            v2.X = c.X - a.X;
            v2.Y = b.Y - a.Y;

            D.X = a.X + (v1.X + v2.X) / 3;
            D.Y = a.Y + (v1.Y + v2.Y) / 3;

            DrawLine(a.X, a.Y, D.X, D.Y);
            DrawLine(b.X, b.Y, D.X, D.Y);
            DrawLine(c.X, c.Y, D.X, D.Y);

            drawtrianglecentr(a, b, D, level - 1);
            drawtrianglecentr(b, c, D, level - 1);
            drawtrianglecentr(a, c, D, level - 1);
        }

        private void ScaleTree_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void iterationsTree_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void angleTree_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void MyCanvasFT_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyCanvasFT.Children.Clear();
        }


        private double lengthScale = 0.75;
        private double deltaTheta = Math.PI / 5;

        private void DrawBinaryTree(int depth, double x, double y, double length, double theta)
        {
            double x1 = x + length * Math.Cos(theta);
            double y1 = y + length * Math.Sin(theta);
            Line line = new Line();
            line.Stroke = Brushes.Green;
            line.X1 = x;
            line.Y1 = y;
            line.X2 = x1;
            line.Y2 = y1;
            MyCanvasFT.Children.Add(line);
            if (depth > 1)
            {
                depth--;
                DrawBinaryTree(depth, x1, y1, length * lengthScale, theta + deltaTheta);
                DrawBinaryTree(depth, x1, y1, length * lengthScale, theta - deltaTheta);
            }
            else
                return;
        }

        private void DrawTriangleSerp(int level, Point top, Point left, Point right)
        { 
            if (level == 0)
            {
                DrawLine(top.X, top.Y, right.X, right.Y);
                DrawLine(right.X, right.Y, left.X, left.Y);
                DrawLine(left.X, left.Y, top.X, top.Y);
            }
            else 
            {
                var leftMid = MidPoint(top, left);
                var rightMid = MidPoint(top, right);
                var topMid = MidPoint(left, right);

                DrawTriangleSerp(level - 1, top, leftMid, rightMid);
                DrawTriangleSerp(level - 1, leftMid, left, topMid);
                DrawTriangleSerp(level - 1, rightMid, topMid, right);
            }
        }
        private Point MidPoint(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2f, (p1.Y + p2.Y) / 2f);
        }
        private void DrawLine(double x1, double y1, double x2, double y2)
        {
            Line line = new Line();
            line.Stroke = Brushes.Red;
            line.StrokeThickness = 1;
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            MyCanvasFT.Children.Add(line);
        }
        public float XX1,YY1,WWidth,Hheight;

        private void SliderZ3D_MouseEnter(object sender, MouseEventArgs e)
        {
            Vector3D vector = new Vector3D(0, 0, 1);
            rotate.Axis = vector;
        }

        private void SliderY3D_MouseEnter(object sender, MouseEventArgs e)
        {
            Vector3D vector = new Vector3D(0, 1, 0);
            rotate.Axis = vector;
        }

        private void SliderX3D_MouseEnter(object sender, MouseEventArgs e)
        {
            Vector3D vector = new Vector3D(1, 0, 0);
            rotate.Axis = vector;
        }


        private void Create3D_Click(object sender, RoutedEventArgs e)
        {
            if(View3D.Visibility == Visibility.Visible)
            {
                View3D.Visibility = Visibility.Hidden;
                StackPanel3D.Visibility = Visibility.Hidden;
                MyCanvas.IsEnabled = true;
                DefaultPanel.Visibility = Visibility.Visible;
                _3DPanel.Visibility = Visibility.Hidden;
                Oper3D.Visibility = Visibility.Hidden;
                LineSettings.Visibility = Visibility.Hidden;
                MorphingSettings.Visibility = Visibility.Hidden;
                TreeSettings.Visibility = Visibility.Hidden;
            }
            else 
            {
                //MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
                MyCanvas.IsEnabled = false;
                Point3DCollection point3D = new Point3DCollection();
                Point3D point3 = new Point3D();
                foreach (Line obj in lineGroup)
                {
                    point3.X = obj.X1;
                    point3.Y = obj.Y1;
                    point3.Z = 5;
                    point3D.Add(point3);
                    point3.X = obj.X2;
                    point3.Y = obj.Y2;
                    point3.Z = 5;
                    point3D.Add(point3);
                    meshGeometry3D.Positions = point3D;
                }
                View3D.Visibility = Visibility.Visible;
                StackPanel3D.Visibility = Visibility.Visible;
                DefaultPanel.Visibility = Visibility.Hidden;
                _3DPanel.Visibility = Visibility.Visible;
                Oper3D.Visibility = Visibility.Hidden;
                LineSettings.Visibility = Visibility.Hidden;
                MorphingSettings.Visibility = Visibility.Hidden;
                TreeSettings.Visibility = Visibility.Hidden;
            }
        }

        private void ChoiceFractal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ChoiceFractal.SelectedItem)
            {
                case "Дерево пифагора":
                    ScaleTree.IsEnabled = true;
                    angleTree.IsEnabled = true;
                    iterationsTree.IsEnabled = true;
                    break;
                case "Треугольник, центр масс":
                    break;
                case "Треугольник Серпинского":
                    ScaleTree.IsEnabled = false;
                    angleTree.IsEnabled = false;
                    break;
                default:
                    break;
            }
        }

        #endregion
        #region Сохранение и открытие
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            Nullable<bool> result = sf.ShowDialog();
            if (result == false)
                return;
            string fn = sf.FileName;

            SaveImage new_save = new SaveImage();
            int number_group = 0;
            foreach (var obj in MyCanvasList)
            {
                for (int i = 0; i < obj.Count(); i++)
                {
                    Save tmp = new Save();
                    tmp.X1 = (obj[i] as Line).X1;
                    tmp.X2 = (obj[i] as Line).X2;
                    tmp.Y1 = (obj[i] as Line).Y1;
                    tmp.Y2 = (obj[i] as Line).Y2;
                    tmp.AngleX = angleX;
                    tmp.AngleY = angleY;
                    tmp.AngleZ = angleZ;
                    tmp.Scale = scale;
                    tmp.Stroke = (obj[i] as Line).Stroke;
                    tmp.StrokeThickness = (obj[i] as Line).StrokeThickness;
                    tmp.Tag = (obj[i] as Line).Tag;
                    tmp.Type = (obj[i] as Line).GetType();
                    tmp.xSlider = myGroup.x_slider;
                    tmp.ySlider = myGroup.y_slider;
                    tmp.zSlider = myGroup.z_slider;
                    tmp.scaleSlider = myGroup.scale_slider;
                    tmp.savedZCoord = zCoord;
                    tmp.savedZCoordinatesLine = zCoordinates;
                    if (lineGroup.Contains((obj[i] as Line)))
                        tmp.Group = true;
                    if (morphingGroup.Contains((obj[i] as Line)))
                    {
                        tmp.Morph = true;
                        tmp.numberMorph = morphingGroup.IndexOf((obj[i] as Line));
                        tmp.countMorph = morphingGroup.Count();
                    }
                    tmp.Number_group = number_group;
                    new_save.Add(tmp);
                }
                number_group++;
            }
            NetDataContractSerializer serializer = new NetDataContractSerializer();
            using (FileStream fs = File.Create(sf.FileName))
            {
                serializer.Serialize(fs, new_save);
            }
            sf.FileName = null;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Text files(*.txt)|*.txt";
            Nullable<bool> result = of.ShowDialog();
            if (result == false)
                return;
            string fn = of.FileName;

            SaveImage open_obj = new SaveImage();
            NetDataContractSerializer serializer = new NetDataContractSerializer();
            using (FileStream fs = File.OpenRead(of.FileName))
            {
                open_obj = (SaveImage)serializer.Deserialize(fs);
            }
            int num_gr = 0;
            Group gsh = new Group();
            MyCanvasList.Clear();
            lineGroup.Clear();
            morphingGroup.Clear();
            previousMorph = null;
            Shape[] groupMorphTemp = null;
            int cnt = 0;
            int real_cnt = 0;
            foreach (Save obj in open_obj)
            {
                Line tmp = new Line();
                tmp.X1 = obj.X1;
                tmp.X2 = obj.X2;
                tmp.Y1 = obj.Y1;
                tmp.Y2 = obj.Y2;
                angleX = obj.AngleX;
                angleY = obj.AngleY;
                angleZ = obj.AngleZ;
                scale = obj.Scale;
                tmp.Stroke = obj.Stroke;
                tmp.StrokeThickness = obj.StrokeThickness;
                tmp.Tag = obj.Tag;
                slider_x.Value = obj.xSlider;
                slider_y.Value = obj.ySlider;
                slider_z.Value = obj.zSlider;
                slider_scale.Value = obj.scaleSlider;
                zCoord = obj.savedZCoord;
                zCoordinates = obj.savedZCoordinatesLine;
                if (obj.Group)
                {
                    lineGroup.Add(tmp);
                    gsh.selectMode = true;
                }
                if (obj.Morph)
                {
                    if (cnt == 0)
                    {
                        groupMorphTemp = new Shape[obj.countMorph];
                        real_cnt = obj.countMorph;
                    }
                    cnt = 1;
                    groupMorphTemp[obj.numberMorph] = tmp;
                }
                morhingPrevArray();
                if (obj.Number_group > num_gr)
                {
                    MyCanvasList.Add(gsh);
                    gsh = new Group();
                    num_gr++;
                }
                tmp.MouseRightButtonDown += Line_MouseRightButtonDown;
                tmp.PreviewMouseDown += Line_PreviewMouseDown;
                tmp.MouseMove += Line_MouseMove;
                tmp.PreviewMouseUp += Line_PreviewMouseUp;
                gsh.Add(tmp);
                if (num_gr == 0)
                    myGroup = gsh;
            }
            for (int i = 0; i < real_cnt; i++)
                morphingGroup.Add(groupMorphTemp[i]);
            morhingPrevArray();
            MyCanvasList.Add(gsh);
            drawLinesOnCanvas();
            edit_line();
        }
        #endregion
    }
}
