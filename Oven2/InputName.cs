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
    public partial class InputName : DevComponents.DotNetBar.Metro.MetroForm
    {
        TextBox result = null;
        public InputName()
        {
            InitializeComponent();
        }
        public InputName(TextBox data, string tip)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeComponent();

            keyboardControl1.Keyboard = CreateNumericKeyboard();

            keyboardControl1.Invalidate();

            textBoxX2.Text = tip;

            result = data;
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

        private void buttonX1_Click(object sender, EventArgs e)
        {
            result.Text = textBoxX1.Text != "" ? textBoxX1.Text : "0";
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}