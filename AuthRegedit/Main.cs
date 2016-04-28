using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;

namespace AuthRegedit
{
    public partial class Main : Form
    {
        private RE proverka = new RE();
        private RegistryKey LM = Registry.CurrentUser;

        Form form2 = new Registration(); // Форма регистрации
        Form form3 = new Profile(); // Форма авторизации
        Form form4 = new Admin(); // Форма администратора
        public String[] pattern = { "[a-z]{1}", "[A-Z]{1}", "[?#@$]{1}" };
        private String[] json_type = { "login", "password" };
        private String unloading = "unloading.json";

        private string tmp;
        private int token_id;

        public Dictionary<string, string> WriteUn = new Dictionary<string, string>();

        public Main()
        {
            InitializeComponent();
            this.MaximizeBox = false; // Запретить сворачивать/разворачивать окно
            this.Visible = true;

            // Добавляем коллекцию ключей, при успешной авторизации значение
            for (int i = 0; i < json_type.Length; i++)
            {
                WriteUn.Add(json_type[i], null);
            }
        }

        // Регистрация
        private void button1_Click(object sender, EventArgs e)
        {
            form2.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Проверка

            if (textBox1.Text.Equals(""))
            {
                label2.Text = "Поле <Логин> пустое\n";
                label2.ForeColor = Color.Red;
            }
            else if (textBox2.Equals(""))
            {
                label2.Text = "Поле <Пароль> пустое\n";
                label2.ForeColor = Color.Red;
            }
            else
            {
                if (LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User", false) != null) // Если путь существует
                {
                    token_id = Convert.ToInt32(LM.OpenSubKey("SOFTWARE\\LauncherAuth").GetValue("id")); // Извлекаем значение бд
                    if (proverka.FindGetValue(textBox1.Text, "login")) // Если логин существует, true
                    {
                        if (proverka.FindGetValue(textBox2.Text, "password")) // Если пароль действителен, true
                        {
                            if (proverka.GetAccessValue(textBox1.Text) == 0) // Если пустое значение, true
                            {
                                MessageBox.Show("Поздравляем с успешной авторизацией " + textBox1.Text + "!");
                                form3.Text = "Вы авторизированы: " + textBox1.Text;
                                form3.Show();
                                Application.OpenForms.OfType<Profile>().Single().profile(textBox1.Text);
                            }
                            else
                            {
                                // Заходим в админ панель
                                MessageBox.Show("Добро пожаловать панель инженеров " + textBox1.Text + "!");
                                form3.Text = "Инженер администратор: " + textBox1.Text;
                                form4.Show();
                                Application.OpenForms.OfType<Admin>().Single().profile(textBox1.Text);
                            }

                            this.Visible = false; // Скрываем первую форму авторизации

                            // Добавляем в коллекцию ключей значения
                            WriteUn[json_type[0]] = textBox1.Text;
                            WriteUn[json_type[1]] = textBox2.Text;


                            if (File.Exists(unloading)) // Если файл существует
                            {
                                File.Delete(unloading); // Удаляем файл, чтобы избежать конфликта
                                File.WriteAllText(unloading, JsonConvert.SerializeObject(WriteUn)); // Создаём json файл
                            }
                            else
                            {
                                File.WriteAllText(unloading, JsonConvert.SerializeObject(WriteUn)); // Создаём json файл
                            }
                        }
                        else // Пароль неправильный, false
                        {
                            label2.Text = "Не правильный пароль!\n";
                            label2.ForeColor = Color.Red;
                        }
                    }
                    else
                    {
                        label2.Text = "Пользователя <" + textBox1.Text + "> не существует!\n";
                        label2.ForeColor = Color.Red;
                    }
                }
                else // Если логин не существует, false
                {
                    label2.Text = Color.Red + "БД не существовало!\nМы создали её за вас!\nДля продолжения нажмите ещё раз.";
                    label2.ForeColor = Color.Orange;
                    RegistryKey create = LM.CreateSubKey("SOFTWARE\\LauncherAuth").CreateSubKey("User");
                    RegistryKey bdid = LM.OpenSubKey("SOFTWARE\\LauncherAuth", true);
                    bdid.SetValue("id", 1);
                    bdid.SetValue("SavePass", false);
                    create.Close();
                    bdid.Close();
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            proverka.SavePassword(checkBox1.Checked);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User", false) != null) // Если путь существует
            {
                // Если существует параметр "SavePass", то выгружаем его
                if (proverka.GetSavePassword())
                    checkBox1.Checked = proverka.GetSavePassword();

                if (proverka.GetSavePassword()) // Если значение true SavePass реестра
                {
                    if (File.Exists(unloading))
                    {
                        tmp = File.ReadAllText(unloading);
                        var jObj = JObject.Parse(tmp);
                        textBox1.Text = (string)jObj["login"];
                        textBox2.Text = (string)jObj["password"];
                    }
                }
            }
        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            MessageBox.Show(proverka.RegistryLength().ToString());
        }
    }
}