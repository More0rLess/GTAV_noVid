using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace GTAV_noVid
{
    public partial class frmMain : Form
    {

        public frmMain()
        {
            InitializeComponent();
        }

        //Yes, I am not the best coder.
        //What it does: It just searches for 706C6174666F726D3A2F6D6F766965732F726F636B737461725F6C6F676F730062696B
        //in the GTA5.exe and overwrites it with zeros.
        //Please give credit if you want to modify the tool.

        string filePath;

        private void Form1_Load(object sender, EventArgs e)
        {
            Random r = new Random();
            int random = r.Next(1, 4);
            switch (random)
            {
                case 1:
                    pictureBox1.Image = global::GTAV_noVid.Properties.Resources._1;
                    break;
                case 2:
                    pictureBox1.Image = global::GTAV_noVid.Properties.Resources._2;
                    break;
                case 3:
                    pictureBox1.Image = global::GTAV_noVid.Properties.Resources._3;
                    break;
                case 4:
                    pictureBox1.Image = global::GTAV_noVid.Properties.Resources._4;
                    break;
            }
        }

        private void btnPatch_Click(object sender, EventArgs e)
        {
            string installPath = string.Empty;
            try
            {
                RegistryKey regKey = Registry.LocalMachine;
                regKey = regKey.OpenSubKey("Software\\Wow6432Node\\rockstar games\\GTAV");

                if (regKey != null)
                {
                    installPath = regKey.GetValue("InstallFolderSteam").ToString();
                    installPath = installPath.Remove(installPath.Length - 4);
                }
                else
                {
                    RegistryKey regKey2 = Registry.LocalMachine;
                    regKey2 = regKey2.OpenSubKey("Software\\Wow6432Node\\rockstar games\\Grand Theft Auto V");

                    if (regKey2 != null)
                        installPath = regKey2.GetValue("InstallFolder").ToString();
                    else
                    {
                        // Cannot find path in registry, let's check if the tool is in GTA V folder
                        if (File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\GTA5.exe"))
                            installPath = Path.GetDirectoryName(Application.ExecutablePath);
                        else
                        {
                            MessageBox.Show("Unable to find GTA V install path! Please move me in GTA V folder and launch me again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }
                    }
                }
            }
            catch { }
            //HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\rockstar games
            lblDebug.Text += "Initial path: " + installPath + "\r\n";

            if (!File.Exists(installPath + "\\GTA5.exe"))
            {
                OpenFileDialog fileOpenDialog = new OpenFileDialog();
                fileOpenDialog.Title = "Open GTA5.exe";
                fileOpenDialog.InitialDirectory = Path.GetFullPath(installPath);
                fileOpenDialog.RestoreDirectory = true;
                fileOpenDialog.Filter = "GTA5.exe|GTA5.exe|All files (*.*)|*.*";
                fileOpenDialog.FilterIndex = 1;

                if (fileOpenDialog.ShowDialog() == DialogResult.OK)
                    filePath = fileOpenDialog.FileName;
            }
            else
                filePath = installPath + "\\GTA5.exe";

            lblDebug.Text += "File path: " + filePath + "\r\n";

            string stringDebug = string.Empty;

            try
            {
                File.Copy(filePath, filePath + ".bak");
                stringDebug += "Backup: " + filePath + ".bak" + "\r\n";
            }
            catch
            {
                stringDebug += "File COULDN'T be backed up (already exists?). \r\n";
            }

            byte[] pattern = { 112, 108, 97, 116, 102, 111, 114, 109, 58, 47, 109, 111, 118, 105, 101, 115, 47, 114, 111, 99, 107, 115, 116, 97, 114, 95, 108, 111, 103, 111, 115, 0, 98, 105, 107 };
            byte[] data = { 0 };

            try
            {
                data = File.ReadAllBytes(@filePath);
            }
            catch { }

            bool vidFound = false;
            int firstByte = -1, lastByte = -1, lenghtByte;

            for (int i = 0; i < data.Length - pattern.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    //stringDebug = (i + pattern.Length) + "";
                    firstByte = i;
                    lastByte = i + pattern.Length;
                    lenghtByte = pattern.Length;
                    stringDebug += "First byte: " + firstByte.ToString("X") + " \r\nLast byte: " + lastByte.ToString("X") + "\r\n";
                    stringDebug += "Debug: " + getBytesDec(firstByte) + "\r\n";
                    //getBytesDec(firstByte, lenghtByte);
                    stringDebug += "Intro video was successful cut." + "\r\n";
                    stringDebug += "Project page: https://github.com/Xeramon/GTAV_noVid \r\n";
                    vidFound = true;
                    break;
                }
            }

            if (vidFound)
                lblDebug.Text += "Intro video found! \r\n" + stringDebug + "\r\n";
            else
                lblDebug.Text += "Intro video NOT found! (already patched? if not, then conntact me on Github) \r\nProject page: https://github.com/Xeramon/GTAV_noVid \r\n" + stringDebug + "\r\n";
        }

        private int getBytesDec(int readStart)
        {
            byte[] bClean = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var f = new FileStream(filePath, FileMode.Open, FileAccess.Write);
            f.Seek(readStart, SeekOrigin.Begin);
            f.Write(bClean, 0, bClean.Length);
            f.Close();
            return 0;
        }
    }
}













//made by xeramon from reddit.com/u/xeramon