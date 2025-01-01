using System;
using System.Drawing;
using System.Windows.Forms;

namespace pendulum_s
{
    public partial class Form2 : Form
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
        int currentPointIndex;

        public Form2()
        {
            InitializeComponent();
            InitializeAnimation();
        }

        private void InitializeAnimation()
        {
            penPendulum = new Pen(Color.Black, 2);
            brushMass = new SolidBrush(Color.Blue);
            animationTimer = new Timer();
            animationTimer.Interval = 16; // ~60 FPS
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double mass = Convert.ToDouble(textBox1.Text);
            double length = Convert.ToDouble(textBox2.Text);
            double stiffness = Convert.ToDouble(textBox3.Text);
            totalTime = Convert.ToDouble(textBox4.Text);
            double L1 = Convert.ToDouble(textBox5.Text);
            double p = 9.81;

            alpha10Degrees = Convert.ToDouble(textBox6.Text);
            alpha20Degrees = Convert.ToDouble(textBox7.Text);

            alpha10 = alpha10Degrees * Math.PI / 180;
            alpha20 = alpha20Degrees * Math.PI / 180;

            omega1 = Math.Round(Math.Sqrt(p / length), 3);
            omega2 = Math.Round(Math.Sqrt(p / length + (2 * stiffness * L1 * L1) / (mass * length * length)), 3);

            double dAlpha1_dt = -(omega1 * omega1) * alpha10;
            double dAlpha2_dt = -(omega2 * omega2) * alpha20;

            A1 = Math.Sqrt(Math.Pow((alpha10 + alpha20) / 2, 2)); //+ Math.Pow((dAlpha1_dt + dAlpha2_dt) / (2 * omega1), 2));
            A2 = Math.Sqrt(Math.Pow((alpha10 - alpha20) / 2, 2)); // Math.Pow((dAlpha1_dt - dAlpha2_dt) / (2 * omega2), 2));

            phi1 = Math.Atan2((alpha10 + alpha20), (dAlpha1_dt + dAlpha2_dt) * omega1);
            phi2 = Math.Atan2((alpha10 - alpha20), (dAlpha1_dt - dAlpha2_dt) * omega2);

            label11.Text = "w1 = " + omega1;
            label12.Text = "w2 = " + omega2;

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
            Graphics g3 = pictureBox3.CreateGraphics();
            g1.Clear(Color.White);
            g3.Clear(Color.White);

            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            UpdateGraph();
            UpdatePendulums();
            time += totalTime / points;
            if (currentPointIndex >= points)
            {
                if (currentPointIndex == points)
                {
                    UpdateGraph();
                }
                animationTimer.Stop();
            }
        }

        private void UpdateGraph()
        {
            int pointsPerTick = 2; // Кількість точок, які малюються за один кадр

            for (int i = 0; i < pointsPerTick && currentPointIndex < tValues.Length; i++)
            {
                if (currentPointIndex >= points) return;

                Graphics g = pictureBox1.CreateGraphics();
                Graphics g3 = pictureBox3.CreateGraphics();
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
                        g3.DrawLine(penGrid, x1, 0, x1, pictureBox3.Height);

                       
                        string timeLabel = t1.ToString() + " с";
                        Font font = new Font("Arial", 8);
                        Brush brush = Brushes.Black;
                        g.DrawString(timeLabel, font, brush, new PointF(x1 - 10, pictureBox1.Height - 20));
                        g3.DrawString(timeLabel, font, brush, new PointF(x1 - 10, pictureBox3.Height - 20));
                    }
                    g.DrawLine(penAxes, 0, pictureBox1.Height / 2, pictureBox1.Width, pictureBox1.Height / 2);
                    g3.DrawLine(penAxes, 0, pictureBox3.Height / 2, pictureBox3.Width, pictureBox3.Height / 2); 
                    g.DrawLine(penAxes, 0, 0, 0, pictureBox1.Height); 
                    g3.DrawLine(penAxes, 0, 0, 0, pictureBox3.Height);
                }

                double t = tValues[currentPointIndex];
                double alpha1 = A1 * Math.Sin(omega1 * t + phi1) + A2 * Math.Sin(omega2 * t + phi2);
                double alpha2 = A1 * Math.Sin(omega1 * t + phi1) - A2 * Math.Sin(omega2 * t + phi2);

                int x = (int)(t * scaleX); 

                
                int ySignal1 = pictureBox1.Height / 2 - (int)(alpha1 * scaleY);

                
                int ySignal2 = pictureBox3.Height / 2 - (int)(alpha2 * scaleY);

                
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




        private void UpdatePendulums()
        {
            Graphics g = pictureBox2.CreateGraphics();
            g.Clear(Color.White);

            // Рисуем стол
            g.FillRectangle(Brushes.Brown, 0, 470, pictureBox2.Width, 30);
            g.FillRectangle(Brushes.Black, 50, pictureBox2.Height / 6 - 6, 440, 30);
            g.FillRectangle(Brushes.Black, 50, pictureBox2.Height / 6 - 6, 30, 400);
            g.FillRectangle(Brushes.Black, 460, pictureBox2.Height / 6 - 6, 30, 400);

            int length = 250;
            int ballRadius = 30;

            // Первый маятник
            int pivotX1 = (pictureBox2.Width - 300) / 2 + 300 / 3;
            double angle1 = A1 * Math.Sin(omega1 * time + phi1) + A2 * Math.Sin(omega2 * time + phi2);
            int x1 = pivotX1 + (int)(length * Math.Sin(angle1));
            int y1 = 100 + (int)(length * Math.Cos(angle1));
            g.DrawLine(new Pen(Color.Black, 6), pivotX1, 100, x1, y1);
            g.FillEllipse(Brushes.Red, x1 - ballRadius, y1 - ballRadius, ballRadius * 2, ballRadius * 2);

            // Второй маятник
            int pivotX2 = (pictureBox2.Width - 300) / 2 + 2 * 300 / 3;
            double angle2 = A1 * Math.Sin(omega1 * time + phi1) - A2 * Math.Sin(omega2 * time + phi2);
            int x2 = pivotX2 + (int)(length * Math.Sin(angle2));
            int y2 = 100 + (int)(length * Math.Cos(angle2));
            g.DrawLine(new Pen(Color.Black, 6), pivotX2, 100, x2, y2);
            g.FillEllipse(Brushes.Blue, x2 - ballRadius, y2 - ballRadius, ballRadius * 2, ballRadius * 2);

            // Пружина между маятниками
            int midX1 = (pivotX1 + x1) / 2;
            int midY1 = (100 + y1) / 2;
            int midX2 = (pivotX2 + x2) / 2;
            int midY2 = (100 + y2) / 2;
            Pen springPen = new Pen(Color.Gray, 2);
            int springSegments = 10;
            PointF[] springPoints = new PointF[springSegments + 1];
            for (int i = 0; i <= springSegments; i++)
            {
                float t = (float)i / springSegments;
                float x = midX1 + t * (midX2 - midX1);
                float y = midY1 + t * (midY2 - midY1);
                y += i % 2 == 0 ? -10 : 10;
                springPoints[i] = new PointF(x, y);
            }
            g.DrawLines(springPen, springPoints);
        }

    }
}
