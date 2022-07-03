using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Lab5
{
    public partial class Form1 : Form
    {
        double[,] tetrahedron = new double[5, 4];                     // Матрица фигуры
        double[,] axes = new double[8, 4];                          // Матрица осей
        double[,] convertMatrix = new double[4, 4];                 // Матрица преобразования
        double[,] rotationMatrix = new double[4, 4];                // Матрица поворота
        Color color = Color.Black;                                  // Цвет основного контура фигуры
        Pen myPen;                                                  // Перо для рисования фигуры
        int k, l, m, alpha = 5;                                     // Элементы, отвечающие за перенос и угол поворота
        double sizeX = 1;                                           // Коэффициент размера по X
        double sizeY = 1;                                           // Коэффициент размера по Y
        double sizeZ = 1;                                           // Коэффициент размера по Z
        int rotationSpeed = 0;                                      // Скорость вращения
        bool rotationCheck = false;                                 // Условие запуска вращения относительно координатной оси
        bool rotationCheckArbitaryAxis = false;                     // Условие запуска вращения относительно произвольной оси
        bool keepMoving = false;                                    // Условие запуска непрерывного сдвига
        bool figureExist = false;                                   // Условие существования фигуры
        double k1 = 0;                                              // Сдвиг относительно OX
        double l1 = 0;                                              // Сдвиг относительно OY
        double m1 = 0;                                              // Сдвиг относительно OZ
        double[,] worldCoordinateMatrix = new double[4, 4];         // Матрица преобразования для сдвига относительно осей
        double[,] newShiftMatrix = new double[4, 4];                // Матрица сдвига
        
        /// <summary>
        /// Матрица сдвига
        /// </summary>
        private void InitializationShiftMatrix()
        {
            newShiftMatrix[0, 0] = 1; newShiftMatrix[0, 1] = 0; newShiftMatrix[0, 2] = 0; newShiftMatrix[0, 3] = 0;
            newShiftMatrix[1, 0] = 0; newShiftMatrix[1, 1] = 1; newShiftMatrix[1, 2] = 0; newShiftMatrix[1, 3] = 0;
            newShiftMatrix[2, 0] = 0; newShiftMatrix[2, 1] = 0; newShiftMatrix[2, 2] = 1; newShiftMatrix[2, 3] = 0;
            newShiftMatrix[3, 0] = k; newShiftMatrix[3, 1] = l; newShiftMatrix[3, 2] = m; newShiftMatrix[3, 3] = 1;

        }

        /// <summary>
        /// Матрица Фигуры
        /// </summary>
        private void InitializationBipyramid()
        {
            tetrahedron[0, 0] = 0; tetrahedron[0, 1] = -100; tetrahedron[0, 2] = 0; tetrahedron[0, 3] = 1;
            tetrahedron[1, 0] = 100; tetrahedron[1, 1] = 0; tetrahedron[1, 2] = 0; tetrahedron[1, 3] = 1;
            tetrahedron[2, 0] = 0; tetrahedron[2, 1] = 0; tetrahedron[2, 2] = 100; tetrahedron[2, 3] = 1;
            tetrahedron[3, 0] = 0; tetrahedron[3, 1] = 0; tetrahedron[3, 2] = -100; tetrahedron[3, 3] = 1;
        }

        /// <summary>
        /// Матрица Осей
        /// </summary>
        private void InitializationAxes()
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Введите сдвиг произвольной оси вращения");
                return;
            }
            int shift = -Convert.ToInt32(textBox1.Text);
            axes[0, 0] = -500;  axes[0, 1] = 0;     axes[0, 2] = 0;     axes[0, 3] = 1;
            axes[1, 0] = 500;   axes[1, 1] = 0;     axes[1, 2] = 0;     axes[1, 3] = 1;
            axes[2, 0] = 0;     axes[2, 1] = 500;   axes[2, 2] = 0;     axes[2, 3] = 1;
            axes[3, 0] = 0;     axes[3, 1] = -500;  axes[3, 2] = 0;     axes[3, 3] = 1;
            axes[4, 0] = 0;     axes[4, 1] = 0;     axes[4, 2] = 3000;  axes[4, 3] = 1;
            axes[5, 0] = 0;     axes[5, 1] = 0;     axes[5, 2] = -3000; axes[5, 3] = 1;
            axes[6, 0] = -500;  axes[6, 1] = shift; axes[6, 2] = 0;     axes[6, 3] = 1;
            axes[7, 0] = 500;   axes[7, 1] = shift; axes[7, 2] = 0;     axes[7, 3] = 1;
        }


        /// <summary>
        /// Матрица смещения относительно центра нарисованных координат
        /// </summary>
        private void InitilizationWorldCoordinateMatrix()
        {
            double n1 = Math.Cos(1 * Math.PI / 180);           //[x2*-sin  y2*cos    z2*sin    0]   n2=-sin
            double n2 = -Math.Sin(1 * Math.PI / 180);          //[x3*sin   y3*-sin   z3*cos    0]   n3=sin
            double n3 = Math.Sin(1 * Math.PI / 180);           //[   0        0        0       1]   Матрица вращения относительно всех координат
            worldCoordinateMatrix[0, 0] = n1 * sizeX;
            worldCoordinateMatrix[1, 0] = n2;
            worldCoordinateMatrix[2, 0] = n3;
            worldCoordinateMatrix[3, 0] = k1;
            worldCoordinateMatrix[0, 1] = n3;
            worldCoordinateMatrix[1, 1] = n1 * sizeY;
            worldCoordinateMatrix[2, 1] = n2;
            worldCoordinateMatrix[3, 1] = l1;
            worldCoordinateMatrix[0, 2] = n2;
            worldCoordinateMatrix[1, 2] = n3;
            worldCoordinateMatrix[2, 2] = n1 * sizeZ;
            worldCoordinateMatrix[3, 2] = m1;
            worldCoordinateMatrix[0, 3] = 0;
            worldCoordinateMatrix[1, 3] = 0;
            worldCoordinateMatrix[2, 3] = 0;
            worldCoordinateMatrix[3, 3] = 1;
        }

        /// <summary>
        /// Рисовка осей
        /// </summary>
        private void AxesDraw()
        {
            InitializationAxes();
            InitializationConvertMatrix(pictureBox1.Width / 2, pictureBox1.Height / 2, 1, 5, false);
            double[,] newAxes = MatrixMultiply(axes, convertMatrix);
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height); ;
            Graphics cg = Graphics.FromImage(bmp);
            Pen myPen = new Pen(Color.Red, 2);   
            cg.DrawLine(myPen, Convert.ToInt32(newAxes[0, 0]), Convert.ToInt32(newAxes[0, 1]), 
                Convert.ToInt32(newAxes[1, 0]), Convert.ToInt32(newAxes[1, 1]));  //Ось ОХ
            cg.DrawLine(myPen, Convert.ToInt32(newAxes[2, 0]), Convert.ToInt32(newAxes[2, 1]), 
                Convert.ToInt32(newAxes[3, 0]), Convert.ToInt32(newAxes[3, 1]));  //Ось ОY
            cg.DrawLine(myPen, Convert.ToInt32(newAxes[4, 0]), Convert.ToInt32(newAxes[4, 1]), 
                Convert.ToInt32(newAxes[5, 0]), Convert.ToInt32(newAxes[5, 1]));  //Ось ОZ
            myPen = new Pen(Color.Blue, 2);
            if (textBox1.Text!="")
            {
                cg.DrawLine(myPen, Convert.ToInt32(newAxes[7, 0]), Convert.ToInt32(newAxes[7, 1]),
                    Convert.ToInt32(newAxes[6, 0]), Convert.ToInt32(newAxes[6, 1])); //Ось вращения
            }
            pictureBox1.BackgroundImage = bmp;
            cg.Dispose();
            myPen.Dispose();
        }  

        /// <summary>
        /// Получение и вывод мировых координат
        /// </summary>
        /// <param name="matrix">Матрица, из которой берутся мировые координаты</param>
        private void WorldCoordinates(double[,] matrix)
        {
            double Xmax = -1000;
            double Xmin = 1000;
            double Ymax = -1000;
            double Ymin = 1000;
            double Zmax = -1000;
            double Zmin = 1000;
            for (int i = 0; i < 5; i++)
            {
                if (matrix[i, 0] > Xmax)
                {
                    Xmax = (int)matrix[i, 0];
                }
                if (matrix[i, 0] < Xmin)
                {
                    Xmin = (int)matrix[i, 0];
                }
                if (matrix[i, 1] > Ymax)
                {
                    Ymax = (int)matrix[i, 1];
                }
                if (matrix[i, 1] < Ymin)
                {
                    Ymin = (int)matrix[i, 1];
                }
                if (matrix[i, 2] > Zmax)
                {
                    Zmax = (int)matrix[i, 2];
                }
                if (matrix[i, 2] < Zmin)
                {
                    Zmin = (int)matrix[i, 2];
                }
            }
            label4.Text = String.Format("Xmin: {0}", Xmin);
            label5.Text = String.Format("Xmax: {0}", Xmax);
            label6.Text = String.Format("Ymin: {0}", Ymax*-1);
            label7.Text = String.Format("Ymax: {0}", Ymin*-1);
            label8.Text = String.Format("Zmin: {0}", Zmin);
            label9.Text = String.Format("Zmax: {0}", Zmax);
        }

        /// <summary>
        /// Рисовка бимирамиды
        /// </summary>
        private void BipyramidDraw()
        {
            double[,] newBipyramid;
            if (rotationCheck)
            {
                InitializationRotationMatrix(k, l, m, alpha);
                newBipyramid = MatrixMultiply(tetrahedron, rotationMatrix);
            }
            else if (rotationCheckArbitaryAxis)
            {
                InitializationRotationMatrix(k, l, m, alpha);
                newBipyramid = MatrixMultiply(tetrahedron, rotationMatrix);
            }
            else
            {
                newBipyramid = MatrixMultiply(tetrahedron, convertMatrix);
            }
            if (radioButton5.Checked)
            {
                myPen = new Pen(color, 2);
            }            
            if (radioButton6.Checked)
            {
                myPen = new Pen(color, 1);
            }            
            if (radioButton7.Checked)
            {
                myPen = new Pen(color, 3);
            }
            if (checkBox1.Checked)
            {
                myPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            }
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics cg = Graphics.FromImage(bmp);
            Pen myPen1 = new Pen(Color.Green, 2);
            cg.DrawLine(myPen, (int)newBipyramid[0, 0], (int)newBipyramid[0, 1],
                (int)newBipyramid[1, 0], (int)newBipyramid[1, 1]);
            cg.DrawLine(myPen, (int)newBipyramid[0, 0], (int)newBipyramid[0, 1],
                (int)newBipyramid[2, 0], (int)newBipyramid[2, 1]);
            cg.DrawLine(myPen, (int)newBipyramid[0, 0], (int)newBipyramid[0, 1],
                (int)newBipyramid[3, 0], (int)newBipyramid[3, 1]);
            cg.DrawLine(myPen1, (int)newBipyramid[1, 0], (int)newBipyramid[1, 1],
                (int)newBipyramid[2, 0], (int)newBipyramid[2, 1]);
            cg.DrawLine(myPen1, (int)newBipyramid[2, 0], (int)newBipyramid[2, 1],
                (int)newBipyramid[3, 0], (int)newBipyramid[3, 1]);
            cg.DrawLine(myPen1, (int)newBipyramid[3, 0], (int)newBipyramid[3, 1],
                (int)newBipyramid[1, 0], (int)newBipyramid[1, 1]);
            pictureBox1.Image = bmp;
            cg.Dispose();
            myPen.Dispose();
            InitilizationWorldCoordinateMatrix();
            newBipyramid = MatrixMultiply(tetrahedron, worldCoordinateMatrix);
            WorldCoordinates(newBipyramid);
        }

        /// <summary>
        /// Инициализация матрицы вращения
        /// </summary>
        /// <param name="x1">Сдвиг по X</param>
        /// <param name="y1">Сдвиг по Y</param>
        /// <param name="z1">Сдвиг по Z</param>
        /// <param name="alpha1">Угол поворота фигуры</param>
        private void InitializationRotationMatrix(int x1, int y1, int z1, double alpha1)
        {
            int sign = 1;
            alpha1 = alpha1 * Math.PI / 180;
            if(radioButton3.Checked)
            {
                sign = 1;
            }
            if (radioButton4.Checked)
            {
                sign = -1;
            }
            if (radioButton8.Checked == true)   //Вращение относительно OX
            {
                rotationMatrix[0, 0] = 1; 
                rotationMatrix[0, 1] = 0; 
                rotationMatrix[0, 2] = 0; 
                rotationMatrix[0, 3] = 0;
                rotationMatrix[1, 0] = 0; 
                rotationMatrix[1, 1] = sign*Math.Cos(alpha1); 
                rotationMatrix[1, 2] = sign * Math.Sin(alpha1); 
                rotationMatrix[1, 3] = 0;
                rotationMatrix[2, 0] = 0; 
                rotationMatrix[2, 1] = sign * -Math.Sin(alpha1); 
                rotationMatrix[2, 2] = sign * Math.Cos(alpha1);
                rotationMatrix[2, 3] = 0;
                rotationMatrix[3, 0] = 0; 
                rotationMatrix[3, 1] = 0; 
                rotationMatrix[3, 2] = 0;
                rotationMatrix[3, 3] = 1;
            }
            if (radioButton9.Checked == true) //Вращение относительно OY
            {
                rotationMatrix[0, 0] = sign * Math.Cos(alpha1); 
                rotationMatrix[0, 1] = 0; 
                rotationMatrix[0, 2] = sign * -Math.Sin(alpha1); 
                rotationMatrix[0, 3] = 0;
                rotationMatrix[1, 0] = 0; 
                rotationMatrix[1, 1] = 1; 
                rotationMatrix[1, 2] = 0; 
                rotationMatrix[1, 3] = 0;
                rotationMatrix[2, 0] = sign * Math.Sin(alpha1); 
                rotationMatrix[2, 1] = 0; 
                rotationMatrix[2, 2] = sign * Math.Cos(alpha1); 
                rotationMatrix[2, 3] = 0;
                rotationMatrix[3, 0] = 0; 
                rotationMatrix[3, 1] = 0; 
                rotationMatrix[3, 2] = 0; 
                rotationMatrix[3, 3] = 1;
            }
            if (radioButton10.Checked == true) //Вращение относительно OZ
            {
                rotationMatrix[0, 0] = sign * Math.Cos(alpha1);
                rotationMatrix[0, 1] = sign * Math.Sin(alpha1);
                rotationMatrix[0, 2] = 0;
                rotationMatrix[0, 3] = 0;
                rotationMatrix[1, 0] = sign * -Math.Sin(alpha1);
                rotationMatrix[1, 1] = sign * Math.Cos(alpha1);
                rotationMatrix[1, 2] = 0; 
                rotationMatrix[1, 3] = 0;
                rotationMatrix[2, 0] = 0;
                rotationMatrix[2, 1] = 0;
                rotationMatrix[2, 2] = 1;
                rotationMatrix[2, 3] = 0;
                rotationMatrix[3, 0] = 0; 
                rotationMatrix[3, 1] = 0;
                rotationMatrix[3, 2] = 0;
                rotationMatrix[3, 3] = 1;
            }
            if (rotationCheckArbitaryAxis)
            {
                int shift = -Convert.ToInt32(textBox1.Text);
                rotationMatrix[0, 0] = 1;
                rotationMatrix[0, 1] = 0;
                rotationMatrix[0, 2] = 0;
                rotationMatrix[0, 3] = 0;
                rotationMatrix[1, 0] = 0;
                rotationMatrix[1, 1] = sign * Math.Cos(alpha1);
                rotationMatrix[1, 2] = sign * Math.Sin(alpha1);
                rotationMatrix[1, 3] = 0;
                rotationMatrix[2, 0] = 0;
                rotationMatrix[2, 1] = sign * -Math.Sin(alpha1);
                rotationMatrix[2, 2] = sign * Math.Cos(alpha1);
                rotationMatrix[2, 3] = 0;
                rotationMatrix[3, 0] = 0;
                rotationMatrix[3, 1] = shift;
                rotationMatrix[3, 2] = 0;
                rotationMatrix[3, 3] = 1;
            }
            InitializationShiftMatrix();
            rotationMatrix = MatrixMultiply(rotationMatrix, newShiftMatrix);
        }

        /// <summary>
        /// Инициализация матрицы преобразования
        /// </summary>
        /// <param name="x1">Смещение по X</param>
        /// <param name="y1">Смещение по Y</param>
        /// <param name="z1">Смещение по Z</param>
        /// <param name="alpha1">Угол, под которым сейчас фигура</param>
        /// <param name="otobr">Условие, когда необходимо делать отображение</param>
        private void InitializationConvertMatrix(int x1, int y1, int z1, int alpha1, bool otobr)
        {                                                           //[x1*cos   y1*sin   z1*-sin    0]   n1=cos
            double n1 = Math.Cos(alpha1 * Math.PI / 180);           //[x2*-sin  y2*cos    z2*sin    0]   n2=-sin
            double n2 = -Math.Sin(alpha1 * Math.PI / 180);          //[x3*sin   y3*-sin   z3*cos    0]   n3=sin
            double n3 = Math.Sin(alpha1 * Math.PI / 180);           //[   0        0        0       1]   Матрица вращения относительно всех координат
            if (otobr && radioButton1.Checked)                      // Отображение относительно YOZ
            {                                                       //[-1   0   0   0]
                convertMatrix[0, 0] = -n1 * sizeX;                    //[ 0   1   0   0]
                convertMatrix[1, 0] = -n2 ;                           //[ 0   0   1   0]
                convertMatrix[2, 0] = -n3 ;                           //[ 0   0   0   1]
            }
            else
            {
                convertMatrix[0, 0] = n1 * sizeX;
                convertMatrix[1, 0] = n2 ;
                convertMatrix[2, 0] = n3 ;
            }
            convertMatrix[3, 0] = x1;
            if (otobr && radioButton2.Checked)                      // Отображение относительно XOZ
            {                                                       //[1    0    0    0]
                convertMatrix[0, 1] = -n3 ;                           //[0   -1    0    0]
                convertMatrix[1, 1] = -n1 * sizeY;                    //[0    0    1    0]
                convertMatrix[2, 1] = -n2 ;                           //[0    0    0    1]
            }
            else
            {
                convertMatrix[0, 1] = n3 ;
                convertMatrix[1, 1] = n1 * sizeY;
                convertMatrix[2, 1] = n2 ;
            }
            convertMatrix[3, 1] = y1;
            if (otobr && radioButton18.Checked)                     // Отображение относительно XOY
            {                                                       //[1    0    0    0]
                convertMatrix[0, 2] = -n2 ;                           //[0    1    0    0]
                convertMatrix[1, 2] = -n3 ;                           //[0    0   -1    0]
                convertMatrix[2, 2] = -n1 * sizeZ;                    //[0    0    0    1]
            }
            else
            {
                convertMatrix[0, 2] = n2 ;
                convertMatrix[1, 2] = n3 ;
                convertMatrix[2, 2] = n1 * sizeZ;
            }
            convertMatrix[3, 2] = z1;
            convertMatrix[0, 3] = 0;
            convertMatrix[1, 3] = 0;
            convertMatrix[2, 3] = 0;
            convertMatrix[3, 3] = 1;
        }

        /// <summary>
        /// Умножение матриц
        /// </summary>
        /// <param name="matrix1">Исходная матрица</param>
        /// <param name="matrix2">Матрица преобразования</param>
        /// <returns>Преобразованная матрица</returns>
        private double[,] MatrixMultiply(double[,] matrix1, double[,] matrix2)
        {
            int n = matrix1.GetLength(0);
            int m = matrix1.GetLength(1);
            double[,] result = new double[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    result[i, j] = 0;
                    for (int ii = 0; ii < m; ii++)
                    {
                        result[i, j] += matrix1[i, ii] * matrix2[ii, j];
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Нарисовать Оси
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            AxesDraw();
        }

        /// <summary>
        /// Нарисовать фигуру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if(figureExist)
            {
                MessageBox.Show("Фигура уже создана");
                return;
            }
            k = pictureBox1.Width / 2;
            l = pictureBox1.Height / 2;
            m = 0;
            InitializationConvertMatrix(k, l, m, alpha, false);
            InitializationBipyramid();
            BipyramidDraw();
            figureExist = true;
        }

        /// <summary>
        /// Повернуть фигуру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (!figureExist)
            {
                MessageBox.Show("Фигура еще не создана");
                return;
            }
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button9.Enabled = false;
            alpha += 10 + rotationSpeed;
            rotationCheck = true;
            BipyramidDraw();
            rotationCheck = false;
        }

        /// <summary>
        /// Одновременный сдвиг по всем осям
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (!figureExist)
            {
                MessageBox.Show("Фигура еще не создана");
                return;
            }
            k += 5;
            l += 5;
            m += 5;
            InitializationConvertMatrix(k, l, m, alpha, false);
            BipyramidDraw();
        }

        /// <summary>
        /// Отображение фигуры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (!figureExist)
            {
                MessageBox.Show("Фигура еще не создана");
                return;
            }
            InitializationConvertMatrix(k, l, m, alpha, true);
            BipyramidDraw();
        }

        /// <summary>
        /// Изменение масштаба
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (!figureExist)
            {
                MessageBox.Show("Фигура еще не создана");
                return;
            }
            if (checkBox2.Checked)
            {
                sizeX *= 1.1;
            }
            else if (checkBox5.Checked)
            {
                sizeX /= 1.1;
            }
            if (checkBox3.Checked)
            {
                sizeY *= 1.1;
            }
            else if (checkBox6.Checked)
            {
                sizeY /= 1.1;
            }
            if (checkBox4.Checked)
            {
                sizeZ *= 1.1;
            }
            else if (checkBox7.Checked)
            {
                sizeZ /= 1.1;
            }
            InitializationConvertMatrix(k, l, m, alpha, false);
            BipyramidDraw();
        }

        /// <summary>
        /// Очистка экрана
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            alpha = 10;
            pictureBox1.Image=null;
            pictureBox1.BackgroundImage = null;
            figureExist = false;
            rotationSpeed = 0;
            rotationCheckArbitaryAxis = false;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button9.Enabled = true;
            sizeX = 1;
            sizeY = 1;
            sizeZ = 1;
        }

        /// <summary>
        /// Выбор цвета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult dialog = colorDialog1.ShowDialog();
            if (dialog == System.Windows.Forms.DialogResult.OK)
            {
                color = colorDialog1.Color;
                pictureBox2.BackColor = color;
            }
        }

        /// <summary>
        /// Начать непрервыное движение фигуры по экрану
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            if (!figureExist)
            {
                MessageBox.Show("Фигура еще не создана");
                return;
            }
            timer1.Interval = 100;
            button9.Text = "Стоп";
            if (keepMoving == true)
            {
                timer1.Start();
            }
            else
            {
                timer1.Stop();
                button9.Text = "Старт";
            }
            keepMoving = !keepMoving;
        }

        /// <summary>
        /// Вращение вокруг произвольной оси
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            if (!figureExist)
            {
                MessageBox.Show("Фигура еще не создана");
                return;
            }
            alpha += 10 + rotationSpeed;
            rotationCheckArbitaryAxis = true;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button9.Enabled = false;
            BipyramidDraw();
        }

        /// <summary>
        /// Увеличить скорость вращения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            if (!figureExist)
            {
                MessageBox.Show("Фигура еще не создана");
                return;
            }
            rotationSpeed += 5;
        }

        /// <summary>
        /// Уменьшить скорость вращения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            if (!figureExist)
            {
                MessageBox.Show("Фигура еще не создана");
                return;
            }
            rotationSpeed -= 5;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox5.Enabled = false;
            }
            else if(!checkBox2.Checked)
            {
                checkBox5.Enabled = true;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                checkBox6.Enabled = false;
            }
            else if (!checkBox3.Checked)
            {
                checkBox6.Enabled = true;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                checkBox7.Enabled = false;
            }
            else if (!checkBox4.Checked)
            {
                checkBox7.Enabled = true;
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                checkBox2.Enabled = false;
            }
            else if (!checkBox5.Checked)
            {
                checkBox2.Enabled = true;
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                checkBox3.Enabled = false;
            }
            else if (!checkBox6.Checked)
            {
                checkBox3.Enabled = true;
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                checkBox4.Enabled = false;
            }
            else if (!checkBox7.Checked)
            {
                checkBox4.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (radioButton12.Checked == true)
            {
                k -= 5;
                m -= 5;
                k1 -= 5;
                InitializationConvertMatrix(k, l, m, alpha, false);
                BipyramidDraw();
            }
            if (radioButton13.Checked == true)
            {
                k += 5;
                m += 5;
                k1 += 5;
                InitializationConvertMatrix(k, l, m, alpha, false);
                BipyramidDraw();
            }
            if (radioButton14.Checked == true)
            {
                l -= 5;
                m -= 5;
                l1 -= 5;
                InitializationConvertMatrix(k, l, m, alpha, false);
                BipyramidDraw();
            }
            if (radioButton15.Checked == true)
            {
                l += 5;
                m += 5;
                l1 += 5;
                InitializationConvertMatrix(k, l, m, alpha, false);
                BipyramidDraw();
            }
            if (radioButton16.Checked == true)
            {
                k -= 5;
                l += 5;
                m1 += 5;
                InitializationConvertMatrix(k, l, m, alpha, false);
                BipyramidDraw();
            }
            if (radioButton17.Checked == true)
            {
                k += 5;
                l -= 5;
                m1 -= 5;
                InitializationConvertMatrix(k, l, m, alpha, false);
                BipyramidDraw();
            }
            Thread.Sleep(100);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton18.Checked = true;
            radioButton3.Checked = true;
            radioButton5.Checked = true;
            radioButton9.Checked = true;
            radioButton12.Checked = true;
            pictureBox2.BackColor = Color.Black;
        }
    }
}
/*
1. Что такое векторно-полигональная модель объекта?
    Для описания пространственных объектов в данной модели используются такие элементы: вершины; отрезки прямых (векторы); полилинии, полигоны; полигональные поверхности
2. Опишите способы представления векторной полигональной модели 
    Первый способ. Сохраняем все грани в отдельности
    Второй способ описания. Для такого варианта координаты восьми вершин сохраняются без повторов. Вершины пронумерованы, а каждая грань дается в виде списка индексов вершин.
    Третий способ описания. Этот способ (в литературе его иногда называют линейно-узловой моделью) основывается на иерархии: вершины-ребра-грани.
3. Опишите матрицу преобразований в пространстве общего вида. Раскройте назначение каждого элемента.
    [a	b	c	p]
    [d	e	f	q]
    [h	i	j	r]
    [l	m	n	s]
    Матрица [a, b, c; d, e, f; h, i, j] -  осуществляет покоординатное изменение масштаба, сдвиг и вращение
    Вектор [l, m, n] - осуществляет перенос. Элементы [p, q, r] -  осуществляют преобразование в перспективе.
    Элемент [s] – общее изменение масштаба.
4. Опишите матрицы, используемые для 3D-преобразования.
    Покоординатное изменение масштаба:
    [a	0	0	0]
    [0	e	0	0]
    [0	0	j	0]
    [0	0	0	1]
    Общее изменение масштаба:
    [1	0	0	0]
    [0	1	0	0]
    [0	0	1	0]
    [0	0	0	s]
    Сдвиг и смещение:
    [1	b	c	0]
    [d	1	f	0]
    [h	i	1	0]
    [0	0	0	1]
    Вращение вокруг координатных осей:
        Вокруг ОХ
        [1	0	0	0]
        [0	cos	sin	0]
        [0	-sin	cos	0]
        [0	0	0	1]
        Вокруг ОY
        [cos	0	-sin	0]
        [0	1	0	0]
        [sin	0	cos	0]
        [0	0	0	1]
        Вокруг OZ
        [cos	sin	0	0]
        [-sin	cos	0	0]
        [0	0	1	0]
        [0	0	0	1]
    Отображение относительно плоскости XOY
    [1	0	0	0]
    [0	1	0	0]
    [0	0	-1	0]
    [0	0	0	1]
    Отображение относительно плоскости YOZ
    [-1	0	0	0]
    [0	1	0	0]
    [0	0	1	0]
    [0	0	0	1]
    Отображение относительно плоскости XOZ
    [1	0	0	0]
    [0	-1	0	0]
    [0	0	1	0]
    [0	0	0	1]
    Пространственный перенос:
    [1	0	0	0]
    [0	1	0	0]
    [0	0	1	0]
    [l	m	n	1]
5. Опишите процесс получения 3D-преобразований.
    Преобразование в пространстве однородных координат описываются 
    соотношением: [x* y* z* 1] = [x y z 1] * T, Т – матрица преобразования
6. Что такое «аналитическая модель»?
    Аналитической моделью будем называть описание поверхности математическими формулами. 
    В КГ можно использовать много разновидностей такого описания. Например:  в виде функции двух аргументов z = f(x, у) в неявном виде; уравнение F (х, у, z) = 0.
7. Опишите процедуру отображения графика трехмерной функции на экране.
    Для описания сложных поверхностей часто используют сплайны. Сплайн - это специальная функция, 
    более всего пригодная для аппроксимации отдельных фрагментов поверхности. Несколько сплайнов образовывают модель сложной поверхности. 
    Другими словами, сплайн — эта тоже поверхность, но такая, для которой можно достаточно просто вычислять координаты ее точек. 
    Обычно используют кубические сплайны. Сплайны часто задают параметрически.
8. Опишите процедуры получения вращения, масштабирования и перемещения полученной поверхности.
    Для получения вращения, масштабирования или перемещения используется соответствующая матрица преобразования для 3D пространства.
 */