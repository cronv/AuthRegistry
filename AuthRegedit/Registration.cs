using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Win32;

namespace AuthRegedit
{
    public partial class Registration : Form
    {
        private RE proverka = new RE();
        private RegistryKey LM = Registry.CurrentUser;

        private string m;
        private Color c;
        private bool authgo; // Разрешение

        private int token_id; // Существующий ID

        public Registration()
        {
            InitializeComponent();
            textBox2.TextChanged += pass; // Проверка пароля
            textBox3.TextChanged += pass; // Проверка пароля
        }

        private void pass(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                proverka.CheckSample(textBox2.Text, Application.OpenForms.OfType<Main>().Single().pattern, out m, out c, out authgo);
                label7.Text = m + "\n";
                label7.ForeColor = c;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Проверка
            if (textBox1.Text.Equals(""))
            {
                label7.Text = "Поле <Логин> пустое\n";
                label7.ForeColor = Color.Red;
            }
            else if (textBox2.Text.Equals(""))
            {
                label7.Text = "Поле <Пароль> пустое\n";
                label7.ForeColor = Color.Red;
            }
            else if (!textBox2.Text.Equals(textBox3.Text))
            {
                label7.Text = "Пароли не совпадают!\n";
                label7.ForeColor = Color.Red;
            }
            else if (textBox4.Text.Equals(""))
            {
                label7.Text = "Поле <Фамилия> пустое\n";
                label7.ForeColor = Color.Red;
            }
            else if (textBox5.Text.Equals(""))
            {
                label7.Text = "Поле <Имя> пустое\n";
                label7.ForeColor = Color.Red;
            }
            else if (textBox6.Text.Equals(""))
            {
                label7.Text = "Поле <Отчество> пустое\n";
                label7.ForeColor = Color.Red;
            }
            else
            {
                if (authgo) // Разрешение от шаблона Regulars (Проверки пароля)
                {
                    // Логин
                    if (LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User") != null) // Если файл пути существует
                    {
                        token_id = ((int)LM.OpenSubKey("SOFTWARE\\LauncherAuth").GetValue("id")); // Извлекаем из записи значение, переводим его в int

                        if (proverka.FindLogin(textBox1.Text))
                        {
                            RegistryKey Add = LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User", true).CreateSubKey(token_id.ToString()); // Создаем ссылку пользователя (папка)
                            Add.Close();
                            if (token_id > 1)
                            {
                                // Создание пользователя
                                RegistryKey StreamData = LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + token_id.ToString(), true);
                                StreamData.SetValue("access", 0); // Права пользователя всегда = 0, Админ = 1
                                StreamData.SetValue("login", textBox1.Text);
                                StreamData.SetValue("password", textBox3.Text);
                                StreamData.SetValue("Family", textBox4.Text);
                                StreamData.SetValue("Name", textBox5.Text);
                                StreamData.SetValue("Patronymic", textBox6.Text);
                                StreamData.Close();
                            }
                            else
                            {
                                // Первый всегда админ
                                RegistryKey StreamData = LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + token_id.ToString(), true);
                                StreamData.SetValue("access", 1); // Права пользователя всегда = 0, Админ = 1
                                StreamData.SetValue("login", textBox1.Text);
                                StreamData.SetValue("password", textBox3.Text);
                                StreamData.SetValue("Family", textBox4.Text);
                                StreamData.SetValue("Name", textBox5.Text);
                                StreamData.SetValue("Patronymic", textBox6.Text);
                                StreamData.Close();                              
                            }

                            MessageBox.Show("Учётная запись <" + textBox1.Text + "> успешно создана!\nID: " + token_id.ToString());
                            RegistryKey bdCount = LM.OpenSubKey("SOFTWARE\\LauncherAuth", true);
                            token_id++;
                            bdCount.SetValue("id", token_id);
                            bdCount.Close(); // Закрываем запись счётчика для индексации псевдо-БД
                        }
                        else
                        {
                            label7.Text = "Логин <" + textBox1.Text + "> уже существует!\n";
                            label7.ForeColor = Color.Gold;
                        }
                    }
                    else // В противном случае создаём БД
                    {
                        label7.Text = Color.Red + "БД не существовало!\nМы создали её за вас!\nДля продолжения нажмите ещё раз.";
                        label7.ForeColor = Color.Orange;
                        RegistryKey create = LM.CreateSubKey("SOFTWARE\\LauncherAuth").CreateSubKey("User");
                        RegistryKey bdid = LM.OpenSubKey("SOFTWARE\\LauncherAuth", true);
                        bdid.SetValue("id", 1);
                        bdid.SetValue("SavePass", false);
                        create.Close();
                        bdid.Close();
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GC.Collect(); // Освобождаем мусор
            this.Close();
        }
    }
}
