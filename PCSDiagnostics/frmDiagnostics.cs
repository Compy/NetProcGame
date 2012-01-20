using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetProcGame.tools;
using NetProcGame.game;
using System.Yaml;
using System.Yaml.Serialization;

namespace PCSDiagnostics
{
    public partial class frmDiagnostics : Form, ILogger
    {
        private List<Button> menuOptions;
        private int _currentMenuSelection = 0;

        private Timer _timer;
        private int sortColumn = -1;

        private bool _setupCompleted = false;

        public frmDiagnostics()
        {
            InitializeComponent();
            this.Load += new EventHandler(frmDiagnostics_Load);
            _timer = new Timer();
        }

        void frmDiagnostics_Load(object sender, EventArgs e)
        {
            lvSwitches.ColumnClick += new ColumnClickEventHandler(lvSwitches_ColumnClick);
            Program.StartGameThread(this);
            _timer.Interval = 500;
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }

        void lvSwitches_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewSorter Sorter = new ListViewSorter();
            lvSwitches.ListViewItemSorter = Sorter;
            if (!(lvSwitches.ListViewItemSorter is ListViewSorter))
                return;
            Sorter = (ListViewSorter)lvSwitches.ListViewItemSorter;

            if (Sorter.LastSort == e.Column)
            {
                if (lvSwitches.Sorting == SortOrder.Ascending)
                    lvSwitches.Sorting = SortOrder.Descending;
                else
                    lvSwitches.Sorting = SortOrder.Ascending;
            }
            else
            {
                lvSwitches.Sorting = SortOrder.Descending;
            }
            Sorter.ByColumn = e.Column;

            lvSwitches.Sort();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (Program.Game == null) return;

            if (!_setupCompleted)
            {
                _setupCompleted = true;
                //Program.Game.trough.onBallDrained += new NetProcGame.modes.DrainCallbackHandler(trough_onBallDrained);
                //Program.Game.trough.onBallLaunched += new NetProcGame.modes.LaunchCallbackHandler(trough_onBallLaunched);
                lblDisplayEnabled.Invoke(new MethodInvoker(delegate()
                {
                    lblDisplayEnabled.Text = Program.Game.Config.PRGame.displayMonitor.ToString();
                }));
            }

            foreach (Switch s in Program.Game.Switches.Values)
            {
                bool isActive = s.IsActive();
                if (lvSwitches.Items.ContainsKey(s.Name) && !isActive)
                {
                    lvSwitches.Invoke(new MethodInvoker(delegate() {
                        lvSwitches.Items.RemoveByKey(s.Name);
                    }));
                }
                else if (!lvSwitches.Items.ContainsKey(s.Name) && isActive)
                {
                    lvSwitches.Items.Add(s.Name, s.Name, 0);
                    lvSwitches.Items[s.Name].SubItems.Add(s.Number.ToString());
                    lvSwitches.Items[s.Name].SubItems.Add("ACTIVE");
                }
            }

            balls_in_trough.Text = Program.Game.trough.num_balls().ToString();
            balls_in_play.Text = Program.Game.trough.num_balls_in_play.ToString();
            ball_status.Text = (Program.Game.trough.is_full() ? "DRAINED" : "IN PLAY");
        }

        void trough_onBallLaunched()
        {
        }

        void trough_onBallDrained()
        {
            Log("-- ON DRAINED CALLED -- ");
        }

        public void Log(string text)
        {
            // Add text to log
            txtLogger.BeginInvoke(new MethodInvoker(delegate()
                {
                    txtLogger.Text += text + "\r\n";
                    txtLogger.SelectionStart = txtLogger.Text.Length;
                    txtLogger.ScrollToCaret();
                }));
        }

        private void btnLaunchTest_Click(object sender, EventArgs e)
        {
            Program.Game.auto_launch_next_ball = true;
            Program.Game.trough.launch_balls(1, null, false);
            Program.Game.ball_save.start();
        }

        private void btnLaunchAll_Click(object sender, EventArgs e)
        {
            Program.Game.trough.launch_balls(5, null, false);
        }

        private void btnOpenDivertor_Click(object sender, EventArgs e)
        {
            Program.Game.open_divertor();
        }

        private void btnCloseDivertor_Click(object sender, EventArgs e)
        {
            Program.Game.close_divertor();
        }

        private void btnClearJams_Click(object sender, EventArgs e)
        {
            Program.Game.ClearJams();
        }


    }
}
