using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuthRegedit
{
    public partial class Profile : Form
    {
        private RE proverka = new RE();
        
        public Profile()
        {
            InitializeComponent();
            this.MinimizeBox = false;
            this.MaximizeBox = false;
        }

        public void profile(String login)
        {
            this.label1.Text = "Фамилия: " + proverka.GetStringValue(login, "Family");
            this.label2.Text = "Имя: " + proverka.GetStringValue(login, "Name");
            this.label3.Text = "Отчество: " + proverka.GetStringValue(login, "Patronymic");
            this.label4.Text = "Логин: " + login;
            this.label5.Text = "Группа: " + proverka.GroupNameID(proverka.GetAccessValue("login"));
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }

            Application.OpenForms.OfType<Profile>().Single().Visible = false;
            Application.OpenForms.OfType<Main>().Single().Visible = true;

            GC.Collect(); // Очищаем мусор
        }
    }
}
