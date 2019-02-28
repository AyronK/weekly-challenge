using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TextFileChallenge
{
    public partial class ChallengeForm : Form
    {
        private readonly BindingList<UserModel> users = new BindingList<UserModel>();
        private Dictionary<string, int> propertiesOrderInFile = new Dictionary<string, int>();
        private string path = @"F:\GitHub\WeeklyChallange\TextFileChallenge\TextFileChallenge\AdvancedDataSet.csv";

        public ChallengeForm()
        {
            InitializeComponent();
            InitializeUsers();
            WireUpDropDown();
        }

        private void InitializeUsers()
        {
            string[] textLines = File.ReadLines(path).ToArray();

            string[] propertyNames = textLines[0].Split(',');

            foreach (var propertyName in propertyNames)
            {
                propertiesOrderInFile.Add(propertyName, Array.IndexOf(propertyNames, propertyName));
            }

            string[][] usersProperties = textLines.Skip(1).Select(l => l.Split(',')).ToArray();

            foreach (var userProperties in usersProperties)
            {
                var userModel = new UserModel
                {
                    FirstName = userProperties[propertiesOrderInFile[nameof(UserModel.FirstName)]],
                    LastName = userProperties[propertiesOrderInFile[nameof(UserModel.LastName)]],
                    Age = int.Parse(userProperties[propertiesOrderInFile[nameof(UserModel.Age)]]),
                    IsAlive = userProperties[propertiesOrderInFile[nameof(UserModel.IsAlive)]] != "0",
                };

                users.Add(userModel);
            }
        }

        private void WireUpDropDown()
        {
            usersListBox.DataSource = users;
            usersListBox.DisplayMember = nameof(UserModel.DisplayText);
        }

        private void saveListButton_Click(object sender, EventArgs e)
        {
            var userProperties = users.Select(u =>
            {
                var arr = new string[4];
                arr[propertiesOrderInFile[nameof(UserModel.FirstName)]] = u.FirstName;
                arr[propertiesOrderInFile[nameof(UserModel.LastName)]] = u.LastName;
                arr[propertiesOrderInFile[nameof(UserModel.Age)]] = u.Age.ToString();
                arr[propertiesOrderInFile[nameof(UserModel.IsAlive)]] = u.IsAlive ? "1" : "0";
                return arr;
            });

            using (var fs = File.OpenWrite(path))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(string.Join(",", propertiesOrderInFile.OrderBy(p => p.Value).Select(p => p.Key)));
                sw.Write(Environment.NewLine);
                foreach (var userProperty in userProperties)
                {
                    sw.Write(string.Join(",", userProperty));
                    sw.Write(Environment.NewLine);
                }
            }

            MessageBox.Show("yay");
        }

        private void addUserButton_Click(object sender, EventArgs e)
        {
            users.Add(new UserModel
            {
                Age = (int)agePicker.Value,
                LastName = lastNameText.Text,
                FirstName = firstNameText.Text,
                IsAlive = isAliveCheckbox.Checked
            });
        }
    }
}