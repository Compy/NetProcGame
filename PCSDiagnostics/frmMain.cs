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
        private UIProcessClient uiClient;

        public frmMain()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(frmMain_FormClosing);
        }

        void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            uiClient.Disconnect();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            uiClient = new UIProcessClient();
            uiClient.MessageReceived += new UIProcessClient.MessageReceivedHandler(uiClient_MessageReceived);
            uiClient.Connect(@"\\.\pipe\uiserver");
        }

        void uiClient_MessageReceived(byte[] message)
        {
            string received = Encoding.UTF8.GetString(message);
            processData(received);
        }

        public void send(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            uiClient.SendMessage(buffer);
            
        }

        private void processData(string data)
        {
            System.Diagnostics.Trace.WriteLine("Got data: " + data);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            send("PING");
        }
    }
}
