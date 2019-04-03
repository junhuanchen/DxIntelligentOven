using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Keyboard;

namespace Oven
{
    public partial class Verification : DevComponents.DotNetBar.Metro.MetroForm
    {
        public Verification()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeComponent();

            keyboardControl1.Keyboard = CreateNumericKeyboard();

            keyboardControl1.Invalidate();
        }

        private Keyboard CreateNumericKeyboard()
        {
            Keyboard keyboard = new Keyboard();

            LinearKeyboardLayout Default = new LinearKeyboardLayout();

            Default.AddKey("7");
            Default.AddKey("8");
            Default.AddKey("9");
            Default.AddKey("Del", "{BACKSPACE}", height: 21);
            Default.AddLine();

            Default.AddKey("4");
            Default.AddKey("5");
            Default.AddKey("6");
            Default.AddLine();

            Default.AddKey("1");
            Default.AddKey("2");
            Default.AddKey("3");
            Default.AddKey("0");
            Default.AddLine();

            keyboard.Layouts.Add(Default);

            return keyboard;
        }

        private void textBoxX1_TextChanged(object sender, EventArgs e)
        {
            if(textBoxX1.Text == "20170507")
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
        }
    }
}