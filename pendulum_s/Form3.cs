using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pendulum_s
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 graphsForm = new Form4();
            graphsForm.FormClosed += (s, args) => this.Show();
            graphsForm.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form6 graphsForm = new Form6();
            graphsForm.FormClosed += (s, args) => this.Show();
            graphsForm.Show();
            this.Hide();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Form5 graphsForm = new Form5();
            graphsForm.FormClosed += (s, args) => this.Show();
            graphsForm.Show();
            this.Hide();
        }
    }
}
