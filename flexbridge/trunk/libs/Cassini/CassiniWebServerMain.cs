/*=======================================================================
  Copyright (C) Microsoft Corporation.  All rights reserved.
 
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
=======================================================================*/

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Cassini.WebServerSample {
    //
    // Simple WinForms application to start a Cassini server
    //
    // Command line:  <physical-path> <port> <virtual-path>
    // Example:       c:\foo 80 /
    //
    public class CassiniWebServer : Form {
        [STAThread]
        public static int Main(String[] args) {
            Application.Run(new CassiniForm(args));
            return 0;
        }
    }

    public class CassiniForm : Form {
        // web server settings
        private static String _appPath;
        private static String _portString;
        private static String _virtRoot;

        // the web server
        private Cassini.Server _server;

        // controls on the Windows Form
		private Panel       logoPanel       = new Panel();
		private Label       logoLabel       = new Label();
		private Label       appDirLabel     = new Label();
		private TextBox     appDirTextBox   = new TextBox();
        private Label       portLabel       = new Label();
		private TextBox     portTextBox     = new TextBox();
		private Label       vrootLabel      = new Label();
		private TextBox     vrootTextBox    = new TextBox();
		private Label       browseLabel     = new Label();
		private LinkLabel   browseLink      = new LinkLabel();
		private Button      startButton     = new Button();
		private Button      stopButton      = new Button();


        public CassiniForm(String[] args) {
            ParseArgs(args);

            InitializeForm();

            if (_appPath.Length > 0) {
                Start();
            }
            else {
                appDirTextBox.Focus();
            }
        }

        private void ParseArgs(String[] args) {
            try {
                if (args.Length >= 1)
                    _appPath = args[0];

                if (args.Length >= 2)
                    _portString = args[1];

                if (args.Length >= 3)
                    _virtRoot = args[2];
            }
            catch {
            }

            if (_portString == null)
                _portString = "80";

            if (_virtRoot == null)
                _virtRoot = "/";

            if (_appPath == null)
                _appPath = String.Empty;
        }

        private void InitializeForm() {
			logoPanel.SuspendLayout();
			SuspendLayout();

			logoLabel.BackColor = Color.White;
			logoLabel.Font = new Font("Arial", 18F, (FontStyle.Bold | FontStyle.Italic), GraphicsUnit.Point, (byte)(0));
			logoLabel.ForeColor = Color.RoyalBlue;
			logoLabel.Location = new Point(24, 24);
			logoLabel.Name = "logoLabel";
			logoLabel.Size = new Size(515, 46);
			logoLabel.TabIndex = 0;
			logoLabel.Text = "Cassini Personal Web Server v2.0";

			logoPanel.BackColor = Color.White;
			logoPanel.BorderStyle = BorderStyle.FixedSingle;
            logoPanel.Controls.AddRange(new Control[] { logoLabel});
			logoPanel.Name = "logoPanel";
			logoPanel.Size = new Size(560, 80);
			logoPanel.TabIndex = 0;

			appDirLabel.BackColor = Color.Transparent;
			appDirLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, (byte)(0));
			appDirLabel.Location = new Point(24, 104);
			appDirLabel.Name = "appDirLabel";
			appDirLabel.Size = new Size(152, 20);
			appDirLabel.TabIndex = 1;
			appDirLabel.Text = "Application &Directory:";
			appDirLabel.TextAlign = ContentAlignment.TopRight;

			appDirTextBox.BackColor = SystemColors.ActiveCaptionText;
			appDirTextBox.Location = new Point(184, 104);
			appDirTextBox.Name = "appDirTextBox";
			appDirTextBox.Size = new Size(344, 22);
			appDirTextBox.TabIndex = 2;
			appDirTextBox.Text = _appPath;

			portLabel.BackColor = Color.Transparent;
			portLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, (byte)(0));
			portLabel.Location = new Point(24, 144);
			portLabel.Name = "portLabel";
			portLabel.Size = new Size(152, 19);
			portLabel.TabIndex = 3;
			portLabel.Text = "Server &Port:";
			portLabel.TextAlign = ContentAlignment.TopRight;

			portTextBox.BackColor = SystemColors.ActiveCaptionText;
			portTextBox.Location = new Point(184, 144);
			portTextBox.Name = "portTextBox";
			portTextBox.Size = new Size(72, 22);
			portTextBox.TabIndex = 4;
			portTextBox.Text = _portString;

			vrootLabel.BackColor = Color.Transparent;
			vrootLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, (byte)(0));
			vrootLabel.Location = new Point(24, 184);
			vrootLabel.Name = "vrootLabel";
			vrootLabel.Size = new Size(152, 20);
			vrootLabel.TabIndex = 5;
			vrootLabel.Text = "Virtual &Root:";
			vrootLabel.TextAlign = ContentAlignment.TopRight;

			vrootTextBox.BackColor = SystemColors.ActiveCaptionText;
			vrootTextBox.Location = new Point(184, 184);
			vrootTextBox.Name = "vrootTextBox";
			vrootTextBox.Size = new Size(120, 22);
			vrootTextBox.TabIndex = 6;
			vrootTextBox.Text = _virtRoot;

            browseLabel.BackColor = Color.Transparent;
			browseLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, (byte)(0));
			browseLabel.Location = new Point(24, 224);
			browseLabel.Name = "browseLabel";
			browseLabel.Size = new Size(152, 19);
			browseLabel.TabIndex = 7;
			browseLabel.Text = "Click To Browse:";
			browseLabel.TextAlign = ContentAlignment.TopRight;
            browseLabel.Visible = false;

            browseLink.Location = new Point(184, 224);
			browseLink.Name = "browseLink";
			browseLink.Size = new Size(308, 30);
			browseLink.TabIndex = 8;
            browseLink.Text = "";
            browseLink.LinkClicked += new LinkLabelLinkClickedEventHandler(OnLinkClick);
            browseLabel.Visible = false;

			startButton.BackColor = SystemColors.Control;
			startButton.FlatStyle = FlatStyle.Popup;
			startButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, (byte)(0));
			startButton.Location = new Point(328, 264);
			startButton.Name = "startButton";
			startButton.Size = new Size(96, 28);
			startButton.TabIndex = 9;
			startButton.Text = "Start";
			startButton.Click += new System.EventHandler(OnStartButtonClick);

            stopButton.BackColor = SystemColors.Control;
			stopButton.DialogResult = DialogResult.Cancel;
			stopButton.FlatStyle = FlatStyle.Popup;
			stopButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, (byte)(0));
			stopButton.Location = new Point(440, 264);
			stopButton.Name = "stopButton";
			stopButton.Size = new Size(96, 28);
			stopButton.TabIndex = 10;
			stopButton.Text = "Stop";
			stopButton.Click += new System.EventHandler(OnStopButtonClick);

			AcceptButton = startButton;
			AutoScaleBaseSize = new Size(6, 15);
			AutoScroll = true;
			CancelButton = stopButton;
			ClientSize = new Size(560, 312);
            Controls.AddRange(new Control[] { 
                logoPanel, 
                appDirLabel, appDirTextBox, 
                portLabel, portTextBox, 
                vrootLabel, vrootTextBox, 
                browseLabel, browseLink,
                startButton, stopButton
            });

            Text = "Cassini Personal Web Server v2.0";
            Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("CassiniWebServerIcon"));

			MaximizeBox = false;
			MinimizeBox = true;
			Name = "CassiniForm";
			ShowInTaskbar = true;
			StartPosition = FormStartPosition.CenterParent;

			logoPanel.ResumeLayout(false);
			ResumeLayout(false);
        }

        private String GetLinkText() {
            String s = "http://localhost";
            if (_portString != "80")
                s += ":" + _portString;
            s += _virtRoot;
            if (!s.EndsWith("/"))
                s += "/";
            return s;
        }

        private void OnLinkClick(Object sender, LinkLabelLinkClickedEventArgs e) {
            browseLink.Links[browseLink.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(browseLink.Text);
        }

        private void OnStartButtonClick(object sender, System.EventArgs e) {
            Start();
        }

        private void OnStopButtonClick(object sender, System.EventArgs e) {
            Stop();
        }

        private void ShowError(String err) {
            MessageBox.Show(err, "Cassini Personal Web Server v2.0", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Start() {
            _appPath = appDirTextBox.Text;
            if (_appPath.Length == 0 || !Directory.Exists(_appPath)) {
                ShowError("Invalid Application Directory");
                appDirTextBox.SelectAll();
                appDirTextBox.Focus();
                return;
            }

            _portString = portTextBox.Text;
            int portNumber = -1;
            try {
                portNumber = Int32.Parse(_portString);
            }
            catch {
            }
            if (portNumber <= 0) {
                ShowError("Invalid Port");
                portTextBox.SelectAll();
                portTextBox.Focus();
                return;
            }

            _virtRoot = vrootTextBox.Text;
            if (_virtRoot.Length == 0 || !_virtRoot.StartsWith("/")) {
                ShowError("Invalid Virtual Root");
                vrootTextBox.SelectAll();
                vrootTextBox.Focus();
                return;
            }

            try {
                _server = new Cassini.Server(portNumber, _virtRoot, _appPath);
                _server.Start();
            }
            catch {
                ShowError(
                    "Cassini Managed Web Server failed to start listening on port " + portNumber + ".\r\n" +
                    "Possible conflict with another Web Server on the same port.");
                portTextBox.SelectAll();
                portTextBox.Focus();
                return;
            }

            startButton.Enabled = false;
            appDirTextBox.Enabled = false;
            portTextBox.Enabled = false;
            vrootTextBox.Enabled = false;
            browseLabel.Visible = true;
            browseLink.Text = GetLinkText();
            browseLink.Visible = true;
            browseLink.Focus();
        }

        private void Stop() {
            try {
                if (_server != null) {
                    _server.Stop();
                    _server = null;
                }
            }
            catch {
            }

            Close();
        }
    }
}