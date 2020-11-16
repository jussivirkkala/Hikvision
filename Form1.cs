﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

/**
 * @jussivirkkala
 * 2020-11-13 Remove ..\bin from CHCNetSDK
 * 2020-11-12 1.0.3 Application already running
 * 2020-11-10 1.0.2 opacity
 * 2020-11-10 1.0.1 git
 * 2020-11-08 1.0.0 .NET 4.5.2, allow unsafe code, Platform x64
 * 
 */


namespace HIK_Set
{

    public partial class Form1 : Form
    {
        // On top
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private Int32 m_lUserID = -1;
        private bool m_bInitSDK = false;
        public int PreSetNo = 1;

        CHCNetSDK.REALDATACALLBACK RealData = null;
        public CHCNetSDK.NET_DVR_PTZPOS m_struPtzCfg;

        public Form1()
        {
            InitializeComponent();
             m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (m_bInitSDK == false)
            {
                label1.Text = "SDK error";
            }
        }


        [STAThread]
        static void Main()
        {
            Application.Run(new Form1());
        }


//        https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-5-0

        private void Form1_Load(object sender, EventArgs e)
        {
            // Always on top
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            string appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            this.Text = appName;
            this.Opacity = .95;

            // On closing event
            this.FormClosing += new FormClosingEventHandler(Form1_Closing);


            // Load file
            if (!File.Exists(this.Text + ".ini"))
            {
                MessageBox.Show("Missing "+appName+".ini", appName);
                }
                else
                {
                    foreach (string line in File.ReadLines(this.Text + ".ini"))
                    {
                        if (!line.StartsWith("#"))
                        {
                            MessageBox.Show(line);
                        }
                    }

                }
            

        }


        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
  //          PreSetNo = 46;
//            SendPreset();
            radioButton3.Checked = true;
            //MessageBox.Show("a");
            //e.Cancel = true;
        }

        
            private void SendPreset()
        {
            label1.Text = "";
            label1.Text += "Cam1:";
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;
            string DVRIPAddress;
            DVRIPAddress = "192.168.106.5";
            Preset(DVRIPAddress);
            label1.Text += " Cam2:";
            DVRIPAddress = "192.168.106.6";
            Preset(DVRIPAddress);
            radioButton1.Enabled = true;
            radioButton2.Enabled = true;
            radioButton3.Enabled = true;

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
            {      PreSetNo = 39;
            SendPreset();
    //            PreSetNo = 46;
      //          SendPreset();
            //    radioButton3.Checked = true;

            }
        }


        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
        { 
                PreSetNo = 40;
                SendPreset();
//                PreSetNo = 46;
  //              SendPreset();
              //  radioButton3.Checked = true;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
                if (rb.Checked)
            {
                PreSetNo = 46;
                SendPreset();
            }
        }

        void Log(string s)
        {
            try { 
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd-THH:mm:ss.fff") + DateTime.Now.ToString("zzz") + "\t" + Environment.MachineName+"\t"+s);
            }
            }
            catch
            { }

        }
        private void Preset(string DVRIPAddress)
        { 
            Int16 DVRPortNumber = Int16.Parse("8000");
            string DVRUserName = "admin";
            string DVRPassword = "!kv14ST2$";

            CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

            //µÇÂ¼Éè±¸ Login the device
            m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
            if (m_lUserID < 0)
            {
                label1.Text += " login err";
                Log("login err");
            }
            else
            {
                if (!CHCNetSDK.NET_DVR_PTZPreset_Other(m_lUserID, 1, CHCNetSDK.GOTO_PRESET, (UInt32)(PreSetNo)))
                    label1.Text += " set err";
                else
                {
                    label1.Text += " ok";
               //     PreSetNo = 46;
                 //   if (!CHCNetSDK.NET_DVR_PTZPreset_Other(m_lUserID, 1, CHCNetSDK.GOTO_PRESET, (UInt32)(PreSetNo)))
                   //     label1.Text += ", 46";


                }

                if (!CHCNetSDK.NET_DVR_Logout(m_lUserID))
                {
                    label1.Text += ", logout err";
                    m_lUserID = -1;
                }
            }
        }

    }
}
