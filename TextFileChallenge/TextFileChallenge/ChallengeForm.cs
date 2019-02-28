using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TextFileChallenge
{
    public partial class ChallengeForm : Form
    {
        private readonly string path =
            @"F:\GitHub\WeeklyChallange\TextFileChallenge\TextFileChallenge\AdvancedDataSet.csv";

        private readonly BindingList<UserModel> users = new BindingList<UserModel>();

        private string[] propertyNamesOrder;

        public ChallengeForm()
        {
            InitializeComponent();
            InitializeUsers();
            WireUpDropDown();
        }

        private int GetPropertyIndex(string propertyName)
        {
            return Array.IndexOf(propertyNamesOrder, propertyName);
        }

        private void InitializeUsers()
        {
            var textLines = File.ReadLines(path).ToArray();

            propertyNamesOrder = textLines[0].Split(',');

            var usersProperties = textLines.Skip(1).Select(l => l.Split(',')).ToArray();

            foreach (var userProperties in usersProperties)
            {
                var userModel = new UserModel
                {
                    FirstName = userProperties[GetPropertyIndex(nameof(UserModel.FirstName))],
                    LastName = userProperties[GetPropertyIndex(nameof(UserModel.LastName))],
                    Age = int.Parse(userProperties[GetPropertyIndex(nameof(UserModel.Age))]),
                    IsAlive = userProperties[GetPropertyIndex(nameof(UserModel.IsAlive))] != "0"
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
                arr[GetPropertyIndex(nameof(UserModel.FirstName))] = u.FirstName;
                arr[GetPropertyIndex(nameof(UserModel.LastName))] = u.LastName;
                arr[GetPropertyIndex(nameof(UserModel.Age))] = u.Age.ToString();
                arr[GetPropertyIndex(nameof(UserModel.IsAlive))] = u.IsAlive ? "1" : "0";
                return arr;
            });

            using (var fs = File.OpenWrite(path))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(string.Join(",", propertyNamesOrder));
                sw.Write(Environment.NewLine);
                foreach (var userProperty in userProperties)
                {
                    sw.Write(string.Join(",", userProperty));
                    sw.Write(Environment.NewLine);
                }
            }

            MessageBox.Show("Saved!");
        }

        private void addUserButton_Click(object sender, EventArgs e)
        {
            users.Add(new UserModel
            {
                Age = (int) agePicker.Value,
                LastName = lastNameText.Text,
                FirstName = firstNameText.Text,
                IsAlive = isAliveCheckbox.Checked
            });
        }
    }
}