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

namespace PCSDiagnostics
{
    public partial class frmMain : Form
    {
        private BackgroundWorker workerThread;
        private NamedPipeClientStream clientPipe;

        public frmMain()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(frmMain_FormClosing);
        }

        void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            workerThread.CancelAsync();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            workerThread = new BackgroundWorker();
            workerThread.WorkerSupportsCancellation = true;
            workerThread.DoWork += new System.ComponentModel.DoWorkEventHandler(workerThread_DoWork);
            workerThread.RunWorkerAsync();
        }

        void workerThread_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            clientPipe = new NamedPipeClientStream(".", "PCSServer", PipeDirection.InOut, PipeOptions.None);
            clientPipe.Connect(5000);
            int numBytes = -1;
            byte[] buffer = new byte[1024];
            string dataRead = "";
            while (numBytes != 0 && workerThread.CancellationPending)
            {
                numBytes = clientPipe.Read(buffer, 0, buffer.Length);
                dataRead = Encoding.UTF8.GetString(buffer, 0, numBytes);
                processData(dataRead);
            }
        }

        public void send(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            clientPipe.Write(buffer, 0, buffer.Length);
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
