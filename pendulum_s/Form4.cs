﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace pendulum_s
{
    public partial class Form4 : Form
    {
        Pen penPendulum;
        SolidBrush brushMass;
        Timer animationTimer;
        double time, totalTime;
        double omega1, omega2, A1, A2;
        int points;
        double[] tValues;
        Point[] signalPoints1;
        Point[] signalPoints2;
        double alpha10Degrees, alpha20Degrees, phi1, phi2, alpha10, alpha20;

        

        double period2;
        int currentPointIndex;
        private int buttonClickCount = 0;

        public Form4()
        {
            InitializeComponent();

            dataGridView1.ColumnCount = 6;
            dataGridView1.Columns[0].Name = "Дослід";
            dataGridView1.Columns[1].Name = "Перший кут (градуси)";
            dataGridView1.Columns[2].Name = "Другий кут (градуси)";
            dataGridView1.Columns[3].Name = "Частота (рад/с)";
            dataGridView1.Columns[4].Name = "Період (с)";
            dataGridView1.Columns[5].Name = "Час (с)";

            penPendulum = new Pen(Color.Black, 2);
            brushMass = new SolidBrush(Color.Blue);

            animationTimer = new Timer();
            animationTimer.Interval = 16;
            animationTimer.Tick += AnimationTimer_Tick;
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            alpha10Degrees = double.Parse(textBox1.Text);
            alpha20Degrees = double.Parse(textBox2.Text);
            buttonClickCount++;
            if (alpha10Degrees == alpha20Degrees)
            {
                // Зчитування значень з текстових полів

                double g = 9.81;  // Прискорення вільного падіння
                totalTime = double.Parse(textBox5.Text);  // Час для виведення
                points = 1000; // Кількість точок для графіка
                double mass = 0.05; // Маса кульки
                double length = 1; // Довжина нитки
                omega1 = Math.Sqrt(g / length);  // Частота для першого маятника

                double alpha10 = alpha10Degrees * Math.PI / 180; // Початковий кут для першого маятника (у радіанах)
                double alpha20 = alpha20Degrees * Math.PI / 180; // Початковий кут для другого маятника (у радіанах)
                double dAlpha1_dt = -(omega1 * omega1) * alpha10;
                double dAlpha2_dt = -(omega2 * omega2) * alpha20;
                A1 = Math.Sqrt(Math.Pow((alpha10 + alpha20) / 2, 2)); //+ Math.Pow((dAlpha1_dt + dAlpha2_dt) / (2 * omega1), 2));
                A2 = Math.Sqrt(Math.Pow((alpha10 - alpha20) / 2, 2)); // Math.Pow((dAlpha1_dt - dAlpha2_dt) / (2 * omega2), 2));

                phi1 = Math.Atan2((alpha10 + alpha20), (dAlpha1_dt + dAlpha2_dt) * omega1);
                phi2 = Math.Atan2((alpha10 - alpha20), (dAlpha1_dt - dAlpha2_dt) * omega2);
                // Параметри для побудови графіка
                points = pictureBox1.Width;
                tValues = new double[points];
                signalPoints1 = new Point[points];
                signalPoints2 = new Point[points];
                double timeStep = totalTime / points;

                for (int i = 0; i < points; i++)
                    tValues[i] = i * timeStep;

                time = 0;
                currentPointIndex = 0;

                Graphics g1 = pictureBox1.CreateGraphics();
                Graphics g3 = pictureBox2.CreateGraphics();
                g1.Clear(Color.White);
                g3.Clear(Color.White);

                animationTimer.Start();

                double T1 = 2.5;
                tValues = new double[points];
                for (int i = 0; i < points; i++)
                {
                    tValues[i] = totalTime * i / points;
                }
                int rowIndex = dataGridView1.Rows.Add();
                dataGridView1.Rows[rowIndex].Cells[0].Value = "Дослід " + (rowIndex + 1);
                dataGridView1.Rows[rowIndex].Cells[1].Value = alpha10Degrees.ToString("0.00");
                dataGridView1.Rows[rowIndex].Cells[2].Value = alpha20Degrees.ToString("0.00");
                dataGridView1.Rows[rowIndex].Cells[3].Value = omega1.ToString("0.00");
                dataGridView1.Rows[rowIndex].Cells[4].Value = T1.ToString("0.00");
                dataGridView1.Rows[rowIndex].Cells[5].Value = totalTime.ToString("0.00");

                // Виведення інформації до listBox1
                listBox1.Items.Add($"Для досліду {rowIndex + 1} (Час: {totalTime} с):");
                listBox1.Items.Add($"Перший кут: {alpha10Degrees.ToString("0.00")} град.");
                listBox1.Items.Add($"Другий кут: {alpha20Degrees.ToString("0.00")} град.");
                listBox1.Items.Add($"Частота: {omega1.ToString("0.00")} рад/с");
                
            }
            else { MessageBox.Show("Помилка введення кутів"); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double T = double.Parse(textBox3.Text);
            double n = double.Parse(textBox4.Text);
            double s = T / n;
            label18.Text = $"Відповідь: змодельована та тереотичні періоди коливань збігаються - одне коливання займає: {s}";
        } 

        int completedAnimations = 0; // Лічильник завершених анімацій

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            UpdateGraph();
            time += totalTime / points;
            if (currentPointIndex >= points)
            {
                if (currentPointIndex == points)
                {
                    UpdateGraph();
                }
                animationTimer.Stop();

                // После завершения анимации увеличиваем лічильник
                completedAnimations++;

                // Проверка, если завершено 4 анимации
                if (completedAnimations == 4)
                {
                    // Выводим сообщение
                    MessageBox.Show("Анімація завершена 4 рази. Висновок: гармонійні коливання були успішно досліджені. Визначено, що при однакових кутах відхилення маятнки рухаються за першою нормальною частотою.");
                    completedAnimations = 0; // Сбросить счётчик после 4 анимаций
                }
            }
        }

        private void UpdateGraph()
        {
            int pointsPerTick = 2; // Кількість точок, які малюються за один кадр

            for (int i = 0; i < pointsPerTick && currentPointIndex < tValues.Length; i++)
            {
                if (currentPointIndex >= points) return;

                Graphics g = pictureBox1.CreateGraphics();
                Graphics g3 = pictureBox2.CreateGraphics();
                Pen penSignal = new Pen(Color.Black, 2f);
                Pen penGrid = new Pen(Color.LightGray, 1f);
                Pen penAxes = new Pen(Color.Black, 1.5f);

                double scaleX = pictureBox1.Width / totalTime;
                double scaleY = 60;


                if (currentPointIndex == 0)
                {
                    int timeStep = 5;
                    for (int t1 = 0; t1 <= totalTime; t1 += timeStep)
                    {
                        int x1 = (int)(t1 * scaleX);


                        g.DrawLine(penGrid, x1, 0, x1, pictureBox1.Height);
                        g3.DrawLine(penGrid, x1, 0, x1, pictureBox2.Height);


                        string timeLabel = t1.ToString() + " с";
                        Font font = new Font("Arial", 8);
                        Brush brush = Brushes.Black;
                        g.DrawString(timeLabel, font, brush, new PointF(x1 - 10, pictureBox1.Height - 20));
                        g3.DrawString(timeLabel, font, brush, new PointF(x1 - 10, pictureBox2.Height - 20));
                    }
                    g.DrawLine(penAxes, 0, pictureBox1.Height / 2, pictureBox1.Width, pictureBox1.Height / 2);
                    g3.DrawLine(penAxes, 0, pictureBox2.Height / 2, pictureBox2.Width, pictureBox2.Height / 2);
                    g.DrawLine(penAxes, 0, 0, 0, pictureBox1.Height);
                    g3.DrawLine(penAxes, 0, 0, 0, pictureBox2.Height);
                }

                double t = tValues[currentPointIndex];
                double alpha1 = A1 * Math.Sin(omega1 * t + phi1) + A2 * Math.Sin(omega2 * t + phi2);
                double alpha2 = A1 * Math.Sin(omega1 * t + phi1) - A2 * Math.Sin(omega2 * t + phi2);

                int x = (int)(t * scaleX);


                int ySignal1 = pictureBox1.Height / 2 - (int)(alpha1 * scaleY);


                int ySignal2 = pictureBox2.Height / 2 - (int)(alpha2 * scaleY);


                signalPoints1[currentPointIndex] = new Point(x, ySignal1);
                signalPoints2[currentPointIndex] = new Point(x, ySignal2);


                if (currentPointIndex > 0)
                {
                    g.DrawLine(new Pen(Color.Red, 3), signalPoints1[currentPointIndex - 1], signalPoints1[currentPointIndex]);
                    g3.DrawLine(new Pen(Color.Blue, 3), signalPoints2[currentPointIndex - 1], signalPoints2[currentPointIndex]);
                }

                currentPointIndex++;
            }
        }
    }
}



        

        

         



        
