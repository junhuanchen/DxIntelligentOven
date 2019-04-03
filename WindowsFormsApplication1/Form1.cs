using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private DateTime InputBak = DateTime.Now;
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            DateTime tmp = DateTime.Now; // 检测两个输入之间的间隔，确保为非人工输入。
            if (tmp.Subtract(InputBak).Milliseconds > 30)
            {
                textBox1.Text = "";
            }
            InputBak = tmp;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text.Length > 10)
            {
                MessageBox.Show(textBox1.Text);
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
