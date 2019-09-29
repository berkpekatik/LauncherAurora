using AuroraLauncher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuroraServerClient
{
    public partial class Client : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public Client()
        {
            InitializeComponent();
        }

        private void Client_Load(object sender, EventArgs e)
        {
            label1.BackColor = System.Drawing.Color.Transparent;
            label2.BackColor = System.Drawing.Color.Transparent;
            if (Main.model.Version != Main.version)
            {
                label2.Text = "Aurora Launcher " + Main.version + " old";
                notif.Text = "Aurora Launcher " + Main.version + " old";
            }
            else
            {
                label2.Text = "Aurora Launcher " + Main.model.Version;
                notif.Text = "Aurora Launcher " + Main.model.Version;
            }
            playerCounter.Start();
            players.Value = 0;
            players.MaxValue = Main.model.MaxClient;
            marquee.Start();
            label4.Text = Main.model.Adversiment[0].ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Main.players == null)
            {
                return;
            }

            players.Value += 1;
            if (players.Value == Main.players.Count)
            {
                playerCounter.Stop();
            }
        }

        private void gameControl_Tick(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("FiveM").Length == 0)
            {
                this.Show();
                this.Focus();
                var playerList = Main.getPlayerList();
                players.Value = playerList.Count();
                gameControl.Stop();
            }
        }

        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void min_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }


        private void question_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by vNoisy", "Launcher");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private bool controlFivem()
        {
            if (Process.GetProcessesByName("FiveM").Length != 0)
            {
                if (MessageBox.Show(
        "FiveM Açık Görünüyor, Sunucuya Giriş yapmak için FiveM'i kapatlamalısınız. FiveM'i sonlandırmak istiyor musunuz ?", "Launcher", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk
    ) == DialogResult.Yes)
                {
                    Process[] processes = Process.GetProcessesByName("FiveM");
                    for (int i = 0; i < processes.Length; i++)
                    {
                        processes[i].Kill();
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }
            return true;
        }

        private void connect_Click(object sender, EventArgs e)
        {
            if (!PingHost(Main.model.Ip, int.Parse(Main.model.Port)))
            {
                notif.BalloonTipText = "Sunucu Çevrimdışı";
                notif.ShowBalloonTip(1500);
                return;
            }

            if (controlFivem())
            {
                Process.Start("fivem://connect/" + Main.model.Ip + ":" + Main.model.Port);
                this.Hide();
                gameControl.Start();
            }
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void marquee_Tick(object sender, EventArgs e)
        {
            if (label4.Left < 0 && (Math.Abs(label4.Left) > label4.Width))
                label4.Left = this.Width;

            label4.Left -= 1;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start(Main.model.DiscordUrl);
        }

        private void yenidenBaşlatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void kapatToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.TaskManagerClosing || e.CloseReason == CloseReason.MdiFormClosing || e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.FormOwnerClosing || e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.None || e.CloseReason == CloseReason.ApplicationExitCall)
            {
                if (Process.GetProcessesByName("FiveM").Length != 0)
                {
                    notif.BalloonTipText = "Lütfen Önce FiveM'i Kapatın.";
                    notif.ShowBalloonTip(1500);
                    e.Cancel = true;
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        public static bool PingHost(string nameOrAddress, int Port)
        {
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(nameOrAddress, Port);
                if (reply != null)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}
