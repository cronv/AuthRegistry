using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Microsoft.Win32;

namespace AuthRegedit
{
    /// <summary>
    /// Regular Expression (рус. класс "Регулярные Выражения")
    /// Класс для простой работы с регулярными выражениями и реестром. Вы не ошиблись с выбором!
    /// </summary>
    class RE
    {
        [ThreadStatic]
        private int a, b;
        const int min = 1; // Начало БД; MAX - конец БД
        private RegistryKey LM = Registry.CurrentUser;

        /// <summary>
        /// Процедура для проверки информации по шаблону регулярных выражений.
        /// </summary>
        /// <param name="InputText"></param>
        /// <param name="RegularPatterns"></param>
        /// <param name="Message"></param>
        /// <param name="c"></param>
        public void CheckSample(String InputText, String[] RegularPatterns, out String Message, out Color c, out bool Token)
        {
            c = Color.Empty; // Без него будет ругаться - НЕ ТРОГАТЬ!
            Token = false; // НЕ ТРОГАТЬ!
            if (InputText.Length >= 6)
            {
                for (int i = 0; i <= RegularPatterns.Length - 1; i++)
                {
                    Regex reg = new Regex(RegularPatterns[i]);

                    if (reg.Matches(InputText).Count > 0 && InputText.Length >= 8)
                        b++;

                    if (reg.Matches(InputText).Count > 0 && (InputText.Length >= 6 && InputText.Length <= 8))
                        a++;
                }

                if (b == RegularPatterns.Length)
                {
                    c = Color.Green;
                    Message = "Пароль надёжный";
                    Token = true;
                }
                else if (a == RegularPatterns.Length)
                {
                    c = Color.Orange;
                    Message = "Хороший пароль";
                    Token = true;
                }
                else
                {
                    Message = "Плохой пароль";
                    Token = false;
                }
            }
            else
            {
                c = Color.Red;
                Message = "Пароль должен быть из 6 символов";
            }

            b = a = 0;
            GC.Collect(); // Собираем ненужный мусор
        }

        /// <summary>
        /// Ищет логин по все разделам базы данных и сравнивает, существует ли он. Работает для регистрации.
        /// </summary>
        /// <param name="login">Логин</param>
        /// <returns></returns>
        public bool FindLogin(String login)
        {
            bool a = true;

            if (RegistryLength() == 1) // Если бд пустая, то даём разрешение на первую запись
            {
                a = true;
            }

            for (int i = min; i <= RegistryLength(); i++)
            {
                if (LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString(), false) != null) // Если существует
                {
                    if (login.Equals(LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString()).GetValue("login")))
                    {
                        a = false;
                    }
                }
            }

            GC.Collect(); // Собираем ненужный мусор

            return a;
        }

        /// <summary>
        /// Функция для проверки данных по ключу.
        /// </summary>
        /// <param name="Parameter">Параметр</param>
        /// <param name="Key">Ключ</param>
        /// <returns></returns>
        public bool FindGetValue(String Parameter, String Key)
        {
            bool result = false;

            for (int i = min; i <= RegistryLength(); i++)
            {
                if (LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString(), false) != null) // Если существует
                {
                    if (Parameter.Equals(LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString()).GetValue(Key)))
                    {
                        result = true;
                    }
                }
            }

            GC.Collect(); // Собираем ненужный мусор

            return result;
        }

        /// <summary>
        /// Позволяет изменить параметр ключа "SavePass" на true, false
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        public void SavePassword(bool key)
        {
            RegistryKey savetf = LM.OpenSubKey("SOFTWARE\\LauncherAuth", true);
            savetf.SetValue("SavePass", key);
            savetf.Close();

            GC.Collect(); // Собираем ненужный мусор
        }

        /// <summary>
        /// Позволяет получить значение ключа "SavePass"
        /// </summary>
        /// <returns></returns>
        public bool GetSavePassword()
        {
            bool result = false;
            string tmp;

            RegistryKey savetf = LM.OpenSubKey("SOFTWARE\\LauncherAuth", false);
            tmp = ((string)savetf.GetValue("SavePass"));
            savetf.Close();

            // C# не дружит с bool реестра
            if (tmp == "False")
                result = false;
            else
                result = true;

            GC.Collect(); // Собираем ненужный мусор

            return result;
        }

        /// <summary>
        /// Позволяет получить ключ привелегии из реестра по логину.
        /// </summary>
        /// <returns></returns>
        public int GetAccessValue(String Login)
        {
            int result = 0;

            for (int i = min; i <= RegistryLength(); i++)
            {
                if (LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString(), false) != null) // Если существует
                {
                    if (Login.Equals(LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString()).GetValue("login")))
                    {
                        result = ((int)LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString()).GetValue("access"));
                    }
                }
            }

            GC.Collect(); // Собираем ненужный мусор

            return result;
        }

        /// <summary>
        /// Позволяет получать значения типа String из реестра
        /// </summary>
        /// <param name="Login">Логин</param>
        /// <param name="Key">Ключ</param>
        /// <returns></returns>
        public string GetStringValue(String Login, String Key)
        {
            string result = "";

            for (int i = min; i <= RegistryLength(); i++)
            {
                if (LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString(), false) != null) // Если существует
                {
                    if (Login.Equals(LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString(), false).GetValue("login")))
                    {
                        result = (string)LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString(), false).GetValue(Key);
                    }
                }
            }

            GC.Collect(); // Собираем ненужный мусор

            return result;
        }

        /// <summary>
        /// Позволяет получать значения типа String из реестра вместе с ID
        /// </summary>
        /// <param name="Key">Ключ</param>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public string GetIDStringValue(int id, String Key)
        {
            string result = "";

            for (int i = min; i <= RegistryLength(); i++)
            {
                if (LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString(), false) != null) // Если существует
                {
                    if (id == i)
                    {
                        result = LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString(), false).GetValue(Key).ToString();
                    }
                }
            }
            GC.Collect(); // Собираем ненужный мусор

            return result;
        }

        public int RegistryLength()
        {
            int result = ((int)LM.OpenSubKey("SOFTWARE\\LauncherAuth", false).GetValue("id"));

            if (result == 1)
                result++;

            GC.Collect(); // Собираем ненужный мусор

            return result;
        }

        /// <summary>
        /// Позволяет удалять пользователя из реестра по id типа string.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteUser(string id)
        {
            bool result = false;
            for (int i = min; i <= RegistryLength(); i++)
            {
                if (LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString(), false) != null) // Если существует
                {
                    if (id.Equals(i.ToString()))
                    {
                        LM.DeleteSubKey("SOFTWARE\\LauncherAuth\\User\\" + i.ToString());
                        result = true;
                    }
                }
            }

            GC.Collect(); // Собираем ненужный мусор

            return result;
        }

        /// <summary>
        /// Процедура позволяет изменять пользователя.
        /// </summary>
        /// <param name="ID">id</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="family">Фамилия</param>
        /// <param name="name">Имя</param>
        /// <param name="patronymic">Отчество</param>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public bool EditUser(string ID, string login, string password, string family, string name, string patronymic, int group)
        {
            bool result = false;

            if (!FindLogin(login))
            {
                // Создание пользователя
                RegistryKey StreamData = LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + ID, true);
                StreamData.SetValue("access", group); // Права пользователя всегда = 0, Админ = 1
                StreamData.SetValue("login", login);
                StreamData.SetValue("password", password);
                StreamData.SetValue("Family", family);
                StreamData.SetValue("Name", name);
                StreamData.SetValue("Patronymic", patronymic);
                StreamData.Close();

                result = true;
            }
            else
            {
                result = false;
            }
            GC.Collect();

            return result;
        }

        /// <summary>
        /// Процедура позволяет создавать пользователя.
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="family">Фамилия</param>
        /// <param name="name">Имя</param>
        /// <param name="patronymic">Отчество</param>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public bool AddUser(string login, string password, string family, string name, string patronymic, int group)
        {
            bool result = false;
            int tmp = RegistryLength();

            if (FindLogin(login))
            {
                RegistryKey Add = LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User", true).CreateSubKey(tmp.ToString()); // Создаем ссылку пользователя (папка)
                Add.Close();

                // Создание пользователя
                RegistryKey StreamData = LM.OpenSubKey("SOFTWARE\\LauncherAuth\\User\\" + tmp.ToString(), true);
                StreamData.SetValue("access", group); // Права пользователя всегда = 0, Админ = 1
                StreamData.SetValue("login", login);
                StreamData.SetValue("password", password);
                StreamData.SetValue("Family", family);
                StreamData.SetValue("Name", name);
                StreamData.SetValue("Patronymic", patronymic);
                StreamData.Close();

                RegistryKey bdCount = LM.OpenSubKey("SOFTWARE\\LauncherAuth", true);
                tmp++;
                bdCount.SetValue("id", tmp);
                bdCount.Close(); // Закрываем запись счётчика для индексации псевдо-БД

                result = true;
            }
            else
            {
                result = false;
            }
            GC.Collect();

            return result;
        }

        /// <summary>
        /// Определяет группу по числу 0 = Пользователи; 1 - Администраторы.
        /// </summary>
        /// <param name="i">Значение группы</param>
        /// <returns></returns>
        public string GroupNameID(int i)
        {
            string result = "null";
            switch(i)
            {
                case 0:
                {
                    result = "Пользователи";
                } break;

                case 1:
                {
                    result = "Администраторы";
                } break;
            }
            GC.Collect(); // Собираем ненужный мусор

            return result;
        }

        /// <summary>
        /// Определяет индекс группы, возвращает int.
        /// </summary>
        /// <param name="GroupName">Название группы</param>
        /// <returns></returns>
        public int GroupNameIDInt(string GroupName)
        {
            int result = 0;

            if (GroupName == "Пользователи")
            {
                result = 0;
            }

            if (GroupName == "Администраторы")
            {
                result = 1;
            }

            GC.Collect(); // Собираем ненужный мусор

            return result;
        }
    }
}