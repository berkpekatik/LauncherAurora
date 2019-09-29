using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace AuroraServerClient
{
    public partial class Main : Form
    {

        double fade = 0.03;
        bool anim = false, stopAnim = false;
        private string programJsonFile = "http://fivemsunucum.com/aurora/program.json";
        public static ProgramModel model;
        public static List<PlayersModel> players;
        public static string version = "0.0.1";
        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            controlFivem();
            this.TransparencyKey = Color.WhiteSmoke;
            this.BackColor = Color.WhiteSmoke;
            animation.Start();
            try
            {
                var result = string.Empty;
                using (var webClient = new System.Net.WebClient())
                {
                    result = webClient.DownloadString(programJsonFile);
                }

                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                model = jsonSerializer.Deserialize<ProgramModel>(result);

                if (model == null)
                {
                    MessageBox.Show("Sunucuyla Bağlantı Kurulamadı.");
                }
                getPlayerList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (version != model.Version)
            {
                if (MessageBox.Show(
        "Güncellemeyi Yapmak İstermisiniz ?", "Launcher", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk
    ) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(model.DownloadLink);
                }
            }

            if (checkPrograms(model.ProgramList))
            {
                stopAnimation.Start();
                animation.Stop();
            }
            programFinder.Start();
        }

        public static List<PlayersModel> getPlayerList()
        {
            try
            {
                var resultPlayer = string.Empty;
                using (var webClient = new System.Net.WebClient())
                {
                    resultPlayer = webClient.DownloadString("http://" + model.Ip + ":" + model.Port + "/players.json");
                }

                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                players = jsonSerializer.Deserialize<List<PlayersModel>>(resultPlayer);
                return players;
            }
            catch (Exception)
            {
                MessageBox.Show("Oyuncu Listesi Alınamadı");
                return players;
            }
        }
        private void animation_Tick(object sender, EventArgs e)
        {
            if (this.Opacity > 0.1 && !anim)
            {
                this.Opacity -= fade;
                return;
            }
            anim = true;
            if (this.Opacity < 0.9 && anim)
            {
                this.Opacity += fade;
                return;
            }
            anim = false;
        }

        private void stopAnimation_Tick(object sender, EventArgs e)
        {
            if (stopAnim)
            {
                Client client = new Client();
                client.Show();
                stopAnimation.Stop();
            }
            this.Opacity -= fade;
            if (this.Opacity == 0)
            {
                stopAnim = true;
            }
        }

        private void backgroundController_DoWork(object sender, DoWorkEventArgs e)
        {
            checkPrograms(model.ProgramList);
        }

        private void programFinder_Tick(object sender, EventArgs e)
        {
            backgroundController.RunWorkerAsync();
        }


        private bool checkPrograms(List<string> programList)
        {
            try
            {
                foreach (var item in programList)
                {
                    Process[] processes = Process.GetProcessesByName(item);
                    for (int i = 0; i < processes.Length; i++)
                    {
                        processes[i].Kill();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.TaskManagerClosing || e.CloseReason == CloseReason.MdiFormClosing || e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.FormOwnerClosing || e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.None)
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

        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            MessageBox.Show("hi");
        }
        static void controlFivem()
        {
            if (Process.GetProcessesByName("FiveM").Length != 0)
            {
                if (MessageBox.Show(
        "FiveM Açık Görünüyor, Sunucuya Giriş yapıp ve Launcher'i kullanmak için FiveM'i kapatlamalısınız. FiveM'i sonlandırmak istiyor musunuz ?", "Launcher", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk
    ) == DialogResult.Yes)
                {
                    Process[] processes = Process.GetProcessesByName("FiveM");
                    for (int i = 0; i < processes.Length; i++)
                    {
                        processes[i].Kill();
                    }
                }
                else
                {
                    Environment.FailFast("Fivem Açıkken Launcher'i kullanamazsınız.");
                }
            }
        }
    }
}
