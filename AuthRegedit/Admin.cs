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
    public partial class Admin : Form
    {
        private RE proverka = new RE();
        private RegistryKey LM = Registry.CurrentUser;
        private int tmp_id, tmpAccess = 0;
        private string tmpID, tmpLogin;

        private string m;
        private Color c;
        private bool authgo; // Разрешение
        public Admin()
        {
            InitializeComponent();

            // Анализатор функций
            checkedListBox1.ItemCheck += checkedList;
            checkedListBox1.SelectedIndexChanged += checkeds;

            // Проверка пароля
            textBox2.TextChanged += passp;
            textBox9.TextChanged += passp;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            updateLV();
        }

        private void updateLV()
        {
            ListViewItem lvi = new ListViewItem();
            if (listView1.Items != null)
                listView1.Items.Clear();

            for (int i = 1; i <= proverka.RegistryLength(); i++)
            {
                if (LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString(), false) != null)
                {
                    lvi = new ListViewItem(new string[] { i.ToString(), proverka.GroupNameID(proverka.GetAccessValue(proverka.GetIDStringValue(i, "login"))), proverka.GetIDStringValue(i, "login"), proverka.GetIDStringValue(i, "password"), proverka.GetIDStringValue(i, "Family"), proverka.GetIDStringValue(i, "Name"), proverka.GetIDStringValue(i, "Patronymic") }, i);
                    listView1.Items.Add(lvi);
                }
            }
        }

        // Определяем вектор выбора в checkedListBox
        private void checkedList(object sender, ItemCheckEventArgs e)
        {
            // Изменение свойств, в зависимости от выбранной нами функции
            if (e.NewValue == CheckState.Checked)
            {
                foreach (int index in (sender as CheckedListBox).CheckedIndices)
                {
                    if (index != e.Index)
                    {
                        (sender as CheckedListBox).SetItemChecked(index, false);
                    }
                }
            }
        }

        // Описываем грамотность проверки checkedListBOx
        private void checkeds(object sender, EventArgs e)
        {
            if (checkedListBox1.GetItemChecked(0) == true)
            {
                EditPanel(true, false, false);
            }
            else if (checkedListBox1.GetItemChecked(1) == true)
            {
                EditPanel(false, true, false);
            }
            else if (checkedListBox1.GetItemChecked(2) == true)
            {
                EditPanel(false, false, true);
            }
            else
                EditPanel(false, false, false);
        }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }

            Application.OpenForms.OfType<Admin>().Single().Visible = false;
            Application.OpenForms.OfType<Main>().Single().Visible = true;

            GC.Collect(); // Очищаем мусор
        }

        public void profile(String login)
        {
            tmpLogin = login;
            this.Text = "Инженер администратор: <" + login + "> <" + proverka.GetStringValue(login, "Family") + " " + proverka.GetStringValue(login, "Name") + " " + proverka.GetStringValue(login, "Patronymic") + ">";
        }

        public void passp(object sender, EventArgs e)
        {
            if (textBox2.Enabled.Equals(true))
            {
                if (textBox2.Text != "")
                {
                    proverka.CheckSample(textBox2.Text, Application.OpenForms.OfType<Main>().Single().pattern, out m, out c, out authgo);
                    label14.Text = m + "\n";
                    label14.ForeColor = c;
                }
            }

            if (textBox9.Enabled.Equals(true))
            {
                if (textBox9.Text != "")
                {
                    proverka.CheckSample(textBox9.Text, Application.OpenForms.OfType<Main>().Single().pattern, out m, out c, out authgo);
                    label14.Text = m + "\n";
                    label14.ForeColor = c;
                }
            }
        }

        /// <summary>
        /// Функция для изменения свойства Enabled функциональных панелей.
        /// </summary>
        /// <param name="add"></param>
        /// <param name="delete"></param>
        /// <param name="edit"></param>
        private void EditPanel(bool add, bool delete, bool edit)
        {
            // Добавление
            textBox1.Enabled = add;
            textBox2.Enabled = add;
            textBox3.Enabled = add;
            textBox4.Enabled = add;
            textBox5.Enabled = add;
            comboBox1.Enabled = add;
            button2.Enabled = add;

            // Удаление
            textBox11.Enabled = delete;
            button4.Enabled = delete;

            // Изменение
            textBox6.Enabled = edit;
            textBox7.Enabled = edit;
            textBox8.Enabled = edit;
            textBox9.Enabled = edit;
            textBox10.Enabled = edit;
            comboBox2.Enabled = edit;
            button3.Enabled = edit;

            GC.Collect(); // Очищаем мусор
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateLV();
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.GetItemChecked(1) == true)
            {
                textBox11.Text = listView1.Items[listView1.FocusedItem.Index].SubItems[0].Text;
            }
            else if (checkedListBox1.GetItemChecked(2) == true)
            {
                // Изменение
                tmp_id = Convert.ToInt32(listView1.Items[listView1.FocusedItem.Index].SubItems[0].Text);
                textBox10.Text = listView1.Items[listView1.FocusedItem.Index].SubItems[2].Text;
                textBox9.Text = listView1.Items[listView1.FocusedItem.Index].SubItems[3].Text;
                textBox6.Text = listView1.Items[listView1.FocusedItem.Index].SubItems[4].Text;
                textBox8.Text = listView1.Items[listView1.FocusedItem.Index].SubItems[5].Text;
                textBox7.Text = listView1.Items[listView1.FocusedItem.Index].SubItems[6].Text;
                tmpAccess = proverka.GroupNameIDInt(listView1.Items[listView1.FocusedItem.Index].SubItems[1].Text);

                if (listView1.Items[listView1.FocusedItem.Index].SubItems[2].Text == tmpLogin)
                {
                    comboBox2.SelectedIndex = tmpAccess;
                    comboBox2.Enabled = false;
                }
                else
                    comboBox2.SelectedIndex = tmpAccess;
            }

            if (listView1.SelectedItems.Count != 0)
            {
                tmpID = listView1.Items[listView1.FocusedItem.Index].SubItems[0].Text;
            }
        }

        // Функция удаления
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox11.Equals(""))
            {
                MessageBox.Show("Поле ID пустое!");
            }
            {
                if (proverka.DeleteUser(textBox11.Text))
                {
                    MessageBox.Show("Пользовател с ID:" + textBox11.Text + " успешно удалён!");
                }
                else
                {
                    MessageBox.Show("Пользователя с ID:" + textBox11.Text + " не существует!");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Проверка
            if (textBox1.Text.Equals(""))
            {
                MessageBox.Show("Поле <Логин> пустое\n");
            }
            else if (textBox2.Text.Equals(""))
            {
                MessageBox.Show("Поле <Пароль> пустое\n");
            }
            else if (textBox5.Text.Equals(""))
            {
                MessageBox.Show("Поле <Фамилия> пустое\n");
            }
            else if (textBox3.Text.Equals(""))
            {
                MessageBox.Show("Поле <Имя> пустое\n");
            }
            else if (textBox4.Text.Equals(""))
            {
                MessageBox.Show("Поле <Отчество> пустое\n");
            }
            else if (comboBox1.Text.Equals(""))
            {
                MessageBox.Show("<Права> пользователя не выбраны!\n");
            }
            else
            {
                // Add
                if (authgo)
                {
                    if (proverka.FindLogin(textBox1.Text))
                    {
                        bool aa = proverka.AddUser(textBox1.Text, textBox2.Text, textBox5.Text, textBox3.Text, textBox4.Text, comboBox1.SelectedIndex);
                        MessageBox.Show("Пользователь <" + textBox1.Text + "> успешно создан!" + aa.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Логин <" + textBox1.Text + "> уже существует в базе данных!");
                    }
                }
            }
            updateLV();
        }

        private void button3_Click(object sender, EventArgs e) // В стадии разработки >>>
        {
            // Изменение
            if (textBox10.Text.Equals(""))
            {
                MessageBox.Show("Поле <Логин> пустое.");
            }
            else if (textBox9.Text.Equals(""))
            {
                MessageBox.Show("Поле <Пароль> пустое.");
            }
            else if (textBox6.Text.Equals(""))
            {
                MessageBox.Show("Поле <Фамилия> пустое.");
            }
            else if (textBox8.Text.Equals(""))
            {
                MessageBox.Show("Поле <Имя> пустое.");
            }
            else if (textBox7.Text.Equals(""))
            {
                MessageBox.Show("Поле <Отчество> пустое.");
            }
            else
            {
                if (authgo)
                {
                    if (tmpID != null) // Если выбран элемент, то true
                    {
                        if (!proverka.FindLogin(listView1.Items[listView1.FocusedItem.Index].SubItems[2].Text))
                        {
                            proverka.EditUser(tmpID, textBox10.Text, textBox9.Text, textBox6.Text, textBox8.Text, textBox7.Text, tmpAccess);
                            MessageBox.Show("Пользователь с ID:" + tmpID + " <" + textBox10.Text + "> успешно измёнен!");
                        }
                        else
                        {
                            MessageBox.Show("Логин <" + listView1.Items[listView1.FocusedItem.Index].SubItems[2].Text + "> не существует в базе данных!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пользователь из списка не выбран!");
                    }

                    if (!comboBox2.Enabled)
                        comboBox2.Enabled = true;
                    updateLV(); // Обновляем listView1
                    tmpID = null;
                }
            }
        }
    }
}