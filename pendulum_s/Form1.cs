using System;
using System.Drawing;
using System.Windows.Forms;

namespace pendulum_s
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form2 graphsForm = new Form2();
            graphsForm.FormClosed += (s, args) => this.Show();
            graphsForm.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 labWorkForm = new Form3();
            labWorkForm.FormClosed += (s, args) => this.Show(); 
            labWorkForm.Show();
            this.Hide();
        }
    }
}



