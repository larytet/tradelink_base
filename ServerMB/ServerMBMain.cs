﻿using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;
using TradeLink.Common;
using TradeLink.AppKit;

namespace ServerMB
{
    public partial class ServerMBMain : AppTracker
    {
        
        DebugWindow _dw = new DebugWindow();
        public const string PROGRAM = "ServerMB BETA";
        Log _log = new Log(PROGRAM);

        ServerMB tl;
        
        public ServerMBMain()
        {
            TradeLink.API.TLServer tls;
            if (Properties.Settings.Default.TLClientAddress == string.Empty)
                tls = new TradeLink.Common.TLServer_WM();
            else
                tls = new TradeLink.Common.TLServer_IP(Properties.Settings.Default.TLClientAddress, Properties.Settings.Default.TLClientPort);
            tl = new ServerMB(tls);
            tl.VerboseDebugging = Properties.Settings.Default.VerboseDebugging;
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            tl.SendDebugEvent += new DebugDelegate(debug);
            tl.LoginEvent += new VoidDelegate(tl_LoginEvent);
            FormClosing += new FormClosingEventHandler(ServerMBMain_FormClosing);
				_verbon.Text = "Verbose " + (tl.VerboseDebugging ? "ON" : "OFF");  //pmh 9/15/12 - let the button tell the story.
		  }

        void tl_LoginEvent()
        {
            BackColor = tl.isValid ? Color.Green : Color.Red;
        }
        
        void rightmessage(object sender, EventArgs e)
        {
            _dw.Toggle();
        }

        void ServerMBMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            try
            {
                _log.Stop();
                tl.Stop();
            }
            catch (Exception) { }
        }

        void debug(string msg)
        {
        	//TODO: add a check for verbose debugging
            _log.GotDebug(msg);
            _dw.GotDebug(msg);
        }

        private void _loginbut_Click(object sender, EventArgs e)
        {
            bool ok = tl.Start((int)_id.Value, _user.Text, _pass.Text);
            BackColor = ok ? Color.Green : Color.Red;
				_loginbut.Enabled = ok ? false : true;                   //pmh - 10/4/12 - disable if already logged in.
        }

        private void _togmsg_Click(object sender, EventArgs e)
        {
            _dw.Toggle();
        }

        private void _report_Click(object sender, EventArgs e)
        {
            HelpReportCommunity.Help(PROGRAM);
        }

        private void _verbon_CheckedChanged(object sender, EventArgs e)
        {
            tl.VerboseDebugging = _verbon.Checked;
			  _verbon.Text = "Verbose " + (tl.VerboseDebugging ? "ON" : "OFF");
            debug("verbose mode: " + (tl.VerboseDebugging ? "ON" : "OFF"));
        }
    }
}
