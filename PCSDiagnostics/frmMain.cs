using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Pipes;
using System.ComponentModel;
using NetProcGame.game;

namespace PCSDiagnostics
{
    public partial class frmMain : Form
    {

        public frmMain()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(frmMain_FormClosing);
        }

        void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
        }

    }
}
