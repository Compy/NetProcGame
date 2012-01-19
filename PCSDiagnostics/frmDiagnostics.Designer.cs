namespace PCSDiagnostics
{
    partial class frmDiagnostics
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtLogger = new System.Windows.Forms.TextBox();
            this.mainTabs = new System.Windows.Forms.TabControl();
            this.switchesPage = new System.Windows.Forms.TabPage();
            this.lvSwitches = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.troughPage = new System.Windows.Forms.TabPage();
            this.btnLaunchAll = new System.Windows.Forms.Button();
            this.btnLaunchTest = new System.Windows.Forms.Button();
            this.total_balls = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.balls_in_play = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.balls_in_trough = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ball_status = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.clawPage = new System.Windows.Forms.TabPage();
            this.btnCloseDivertor = new System.Windows.Forms.Button();
            this.btnOpenDivertor = new System.Windows.Forms.Button();
            this.displayPage = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDisplayEnabled = new System.Windows.Forms.Label();
            this.mainTabs.SuspendLayout();
            this.switchesPage.SuspendLayout();
            this.troughPage.SuspendLayout();
            this.clawPage.SuspendLayout();
            this.displayPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLogger
            // 
            this.txtLogger.BackColor = System.Drawing.Color.White;
            this.txtLogger.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLogger.Location = new System.Drawing.Point(12, 594);
            this.txtLogger.Multiline = true;
            this.txtLogger.Name = "txtLogger";
            this.txtLogger.ReadOnly = true;
            this.txtLogger.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogger.Size = new System.Drawing.Size(984, 126);
            this.txtLogger.TabIndex = 2;
            // 
            // mainTabs
            // 
            this.mainTabs.Controls.Add(this.switchesPage);
            this.mainTabs.Controls.Add(this.troughPage);
            this.mainTabs.Controls.Add(this.clawPage);
            this.mainTabs.Controls.Add(this.displayPage);
            this.mainTabs.Location = new System.Drawing.Point(12, 76);
            this.mainTabs.Name = "mainTabs";
            this.mainTabs.SelectedIndex = 0;
            this.mainTabs.Size = new System.Drawing.Size(984, 512);
            this.mainTabs.TabIndex = 3;
            // 
            // switchesPage
            // 
            this.switchesPage.Controls.Add(this.lvSwitches);
            this.switchesPage.Location = new System.Drawing.Point(4, 22);
            this.switchesPage.Name = "switchesPage";
            this.switchesPage.Padding = new System.Windows.Forms.Padding(3);
            this.switchesPage.Size = new System.Drawing.Size(976, 486);
            this.switchesPage.TabIndex = 0;
            this.switchesPage.Text = "Switches";
            this.switchesPage.UseVisualStyleBackColor = true;
            // 
            // lvSwitches
            // 
            this.lvSwitches.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvSwitches.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvSwitches.FullRowSelect = true;
            this.lvSwitches.Location = new System.Drawing.Point(6, 6);
            this.lvSwitches.Name = "lvSwitches";
            this.lvSwitches.Size = new System.Drawing.Size(964, 474);
            this.lvSwitches.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvSwitches.TabIndex = 0;
            this.lvSwitches.UseCompatibleStateImageBehavior = false;
            this.lvSwitches.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Switch Name";
            this.columnHeader1.Width = 353;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Switch No.";
            this.columnHeader2.Width = 223;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Status";
            this.columnHeader3.Width = 120;
            // 
            // troughPage
            // 
            this.troughPage.Controls.Add(this.btnLaunchAll);
            this.troughPage.Controls.Add(this.btnLaunchTest);
            this.troughPage.Controls.Add(this.total_balls);
            this.troughPage.Controls.Add(this.label8);
            this.troughPage.Controls.Add(this.balls_in_play);
            this.troughPage.Controls.Add(this.label5);
            this.troughPage.Controls.Add(this.balls_in_trough);
            this.troughPage.Controls.Add(this.label3);
            this.troughPage.Controls.Add(this.ball_status);
            this.troughPage.Controls.Add(this.label1);
            this.troughPage.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.troughPage.Location = new System.Drawing.Point(4, 22);
            this.troughPage.Name = "troughPage";
            this.troughPage.Padding = new System.Windows.Forms.Padding(3);
            this.troughPage.Size = new System.Drawing.Size(976, 486);
            this.troughPage.TabIndex = 1;
            this.troughPage.Text = "Trough";
            this.troughPage.UseVisualStyleBackColor = true;
            // 
            // btnLaunchAll
            // 
            this.btnLaunchAll.Location = new System.Drawing.Point(19, 218);
            this.btnLaunchAll.Name = "btnLaunchAll";
            this.btnLaunchAll.Size = new System.Drawing.Size(179, 50);
            this.btnLaunchAll.TabIndex = 9;
            this.btnLaunchAll.Text = "Launch All";
            this.btnLaunchAll.UseVisualStyleBackColor = true;
            this.btnLaunchAll.Click += new System.EventHandler(this.btnLaunchAll_Click);
            // 
            // btnLaunchTest
            // 
            this.btnLaunchTest.Location = new System.Drawing.Point(19, 162);
            this.btnLaunchTest.Name = "btnLaunchTest";
            this.btnLaunchTest.Size = new System.Drawing.Size(179, 50);
            this.btnLaunchTest.TabIndex = 8;
            this.btnLaunchTest.Text = "Launch Test";
            this.btnLaunchTest.UseVisualStyleBackColor = true;
            this.btnLaunchTest.Click += new System.EventHandler(this.btnLaunchTest_Click);
            // 
            // total_balls
            // 
            this.total_balls.AutoSize = true;
            this.total_balls.Location = new System.Drawing.Point(179, 122);
            this.total_balls.Name = "total_balls";
            this.total_balls.Size = new System.Drawing.Size(19, 18);
            this.total_balls.TabIndex = 7;
            this.total_balls.Text = "5";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 122);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(104, 18);
            this.label8.TabIndex = 6;
            this.label8.Text = "Total Balls:";
            // 
            // balls_in_play
            // 
            this.balls_in_play.AutoSize = true;
            this.balls_in_play.Location = new System.Drawing.Point(179, 88);
            this.balls_in_play.Name = "balls_in_play";
            this.balls_in_play.Size = new System.Drawing.Size(19, 18);
            this.balls_in_play.TabIndex = 5;
            this.balls_in_play.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 18);
            this.label5.TabIndex = 4;
            this.label5.Text = "Balls in Play:";
            // 
            // balls_in_trough
            // 
            this.balls_in_trough.AutoSize = true;
            this.balls_in_trough.Location = new System.Drawing.Point(179, 50);
            this.balls_in_trough.Name = "balls_in_trough";
            this.balls_in_trough.Size = new System.Drawing.Size(19, 18);
            this.balls_in_trough.TabIndex = 3;
            this.balls_in_trough.Text = "5";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "Balls in Trough:";
            // 
            // ball_status
            // 
            this.ball_status.AutoSize = true;
            this.ball_status.Location = new System.Drawing.Point(179, 13);
            this.ball_status.Name = "ball_status";
            this.ball_status.Size = new System.Drawing.Size(91, 18);
            this.ball_status.TabIndex = 1;
            this.ball_status.Text = "DRAINED";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ball Status:";
            // 
            // clawPage
            // 
            this.clawPage.Controls.Add(this.btnCloseDivertor);
            this.clawPage.Controls.Add(this.btnOpenDivertor);
            this.clawPage.Location = new System.Drawing.Point(4, 22);
            this.clawPage.Name = "clawPage";
            this.clawPage.Padding = new System.Windows.Forms.Padding(3);
            this.clawPage.Size = new System.Drawing.Size(976, 486);
            this.clawPage.TabIndex = 2;
            this.clawPage.Text = "Claw & Ramp";
            this.clawPage.UseVisualStyleBackColor = true;
            // 
            // btnCloseDivertor
            // 
            this.btnCloseDivertor.Location = new System.Drawing.Point(6, 35);
            this.btnCloseDivertor.Name = "btnCloseDivertor";
            this.btnCloseDivertor.Size = new System.Drawing.Size(196, 23);
            this.btnCloseDivertor.TabIndex = 3;
            this.btnCloseDivertor.Text = "Close Divertor";
            this.btnCloseDivertor.UseVisualStyleBackColor = true;
            this.btnCloseDivertor.Click += new System.EventHandler(this.btnCloseDivertor_Click);
            // 
            // btnOpenDivertor
            // 
            this.btnOpenDivertor.Location = new System.Drawing.Point(6, 6);
            this.btnOpenDivertor.Name = "btnOpenDivertor";
            this.btnOpenDivertor.Size = new System.Drawing.Size(196, 23);
            this.btnOpenDivertor.TabIndex = 2;
            this.btnOpenDivertor.Text = "Open Divertor";
            this.btnOpenDivertor.UseVisualStyleBackColor = true;
            this.btnOpenDivertor.Click += new System.EventHandler(this.btnOpenDivertor_Click);
            // 
            // displayPage
            // 
            this.displayPage.Controls.Add(this.lblDisplayEnabled);
            this.displayPage.Controls.Add(this.label2);
            this.displayPage.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.displayPage.Location = new System.Drawing.Point(4, 22);
            this.displayPage.Name = "displayPage";
            this.displayPage.Padding = new System.Windows.Forms.Padding(3);
            this.displayPage.Size = new System.Drawing.Size(976, 486);
            this.displayPage.TabIndex = 3;
            this.displayPage.Text = "Display";
            this.displayPage.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "Display Monitor:";
            // 
            // lblDisplayEnabled
            // 
            this.lblDisplayEnabled.AutoSize = true;
            this.lblDisplayEnabled.Location = new System.Drawing.Point(160, 13);
            this.lblDisplayEnabled.Name = "lblDisplayEnabled";
            this.lblDisplayEnabled.Size = new System.Drawing.Size(98, 18);
            this.lblDisplayEnabled.TabIndex = 2;
            this.lblDisplayEnabled.Text = "DISABLED";
            // 
            // frmDiagnostics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::PCSDiagnostics.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1008, 732);
            this.Controls.Add(this.mainTabs);
            this.Controls.Add(this.txtLogger);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.Name = "frmDiagnostics";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pinball Control System - Diagnostics";
            this.mainTabs.ResumeLayout(false);
            this.switchesPage.ResumeLayout(false);
            this.troughPage.ResumeLayout(false);
            this.troughPage.PerformLayout();
            this.clawPage.ResumeLayout(false);
            this.displayPage.ResumeLayout(false);
            this.displayPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLogger;
        private System.Windows.Forms.TabControl mainTabs;
        private System.Windows.Forms.TabPage switchesPage;
        private System.Windows.Forms.ListView lvSwitches;
        private System.Windows.Forms.TabPage troughPage;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label total_balls;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label balls_in_play;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label balls_in_trough;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label ball_status;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLaunchTest;
        private System.Windows.Forms.Button btnLaunchAll;
        private System.Windows.Forms.TabPage clawPage;
        private System.Windows.Forms.Button btnOpenDivertor;
        private System.Windows.Forms.Button btnCloseDivertor;
        private System.Windows.Forms.TabPage displayPage;
        private System.Windows.Forms.Label lblDisplayEnabled;
        private System.Windows.Forms.Label label2;
    }
}

