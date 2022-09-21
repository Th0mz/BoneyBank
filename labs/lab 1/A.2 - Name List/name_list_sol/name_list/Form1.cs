using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace name_list
{
    public partial class Form1 : Form
    {
        NameList names = new NameList();
        public Form1()
        {
            InitializeComponent();
        }

        private void submit_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            textBox1.Text = "";

            if (text == "")
            {
                MessageBox.Show("Invalid name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // add element to name list
            names.addName(text);

        }

        private void update_Click(object sender, EventArgs e)
        {
            string[] list = names.getNames();
            listBox1.Items.Clear();

            foreach (string name in list)
            {
                listBox1.Items.Add(name);
            }

        }

        private void delete_Click(object sender, EventArgs e)
        {
            // erase method
            names.eraseNames();
        }
    }
}
