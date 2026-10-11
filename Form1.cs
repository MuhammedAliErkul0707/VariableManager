using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using System.Drawing;

namespace VariableManagerNet
{
    public partial class Form1 : Form
    {
        private string dbFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
        private Dictionary<string, string> variables = new Dictionary<string, string>();

        private TextBox txtKey = null!;
        private TextBox txtValue = null!;
        private TextBox txtSearch = null!;
        private Label lblResult = null!;
        private ListBox listVariables = null!;

        public Form1()
        {
            InitializeComponent();
            LoadData();
            SetupUI();
            RefreshList();
        }

        private void LoadData()
        {
            if (File.Exists(dbFile))
            {
                try
                {
                    string json = File.ReadAllText(dbFile);
                    variables = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
                catch
                {
                    variables = new Dictionary<string, string>();
                }
            }
            else
            {
                variables = new Dictionary<string, string>();
            }
        }

        private void SaveData()
        {
            string json = JsonSerializer.Serialize(variables, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(dbFile, json);
        }

        private void SetupUI()
        {
            this.Text = "Permanent Variable Manager (.NET)";
            this.Size = new Size(600, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 23, 42); // Dark background

            // Title
            Label lblTitle = new Label()
            {
                Text = "Variable Manager",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(56, 189, 248),
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Add Section Group
            GroupBox grpAdd = CreateGroupBox("Save / Update Variable", 20, 60, 540, 130);
            
            txtKey = CreateTextBox(15, 30, 240, "Variable name (e.g. apple)");
            txtValue = CreateTextBox(270, 30, 250, "Variable value (e.g. 20 tl)");
            Button btnSave = CreateButton("Save Variable", 15, 75, 505, 35, Color.FromArgb(5, 150, 105), BtnSave_Click);

            grpAdd.Controls.Add(txtKey);
            grpAdd.Controls.Add(txtValue);
            grpAdd.Controls.Add(btnSave);
            this.Controls.Add(grpAdd);

            // Lookup Section Group
            GroupBox grpSearch = CreateGroupBox("Lookup Variable", 20, 205, 540, 110);
            
            txtSearch = CreateTextBox(15, 30, 360, "Enter variable name to search");
            Button btnSearch = CreateButton("Get Value", 385, 29, 135, 30, Color.FromArgb(2, 132, 199), BtnSearch_Click);
            lblResult = new Label()
            {
                Text = "Result will appear here...",
                ForeColor = Color.FromArgb(56, 189, 248),
                Location = new Point(15, 70),
                Size = new Size(505, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Italic)
            };

            grpSearch.Controls.Add(txtSearch);
            grpSearch.Controls.Add(btnSearch);
            grpSearch.Controls.Add(lblResult);
            this.Controls.Add(grpSearch);

            // List Section Group
            GroupBox grpList = CreateGroupBox("All Stored Variables", 20, 330, 540, 135);
            
            listVariables = new ListBox()
            {
                Location = new Point(15, 25),
                Size = new Size(505, 95),
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };

            grpList.Controls.Add(listVariables);
            this.Controls.Add(grpList);
        }

        private GroupBox CreateGroupBox(string text, int x, int y, int w, int h)
        {
            return new GroupBox()
            {
                Text = text,
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(x, y),
                Size = new Size(w, h),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
        }

        private TextBox CreateTextBox(int x, int y, int w, string placeholder)
        {
            return new TextBox()
            {
                Location = new Point(x, y),
                Size = new Size(w, 25),
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
        }

        private Button CreateButton(string text, int x, int y, int w, int h, Color bgColor, EventHandler onClick)
        {
            Button btn = new Button()
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = bgColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += onClick;
            return btn;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            string key = txtKey.Text.Trim();
            string val = txtValue.Text.Trim();

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(val))
            {
                MessageBox.Show("Please fill in both fields!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            variables[key] = val;
            SaveData();
            RefreshList();

            MessageBox.Show($"Variable '{key}' successfully saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtKey.Clear();
            txtValue.Clear();
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            string key = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(key))
            {
                lblResult.Text = "Please enter a variable name.";
                return;
            }

            if (variables.ContainsKey(key))
            {
                lblResult.Text = $"{key} = {variables[key]}";
            }
            else
            {
                lblResult.Text = $"Variable '{key}' not found.";
            }
        }

        private void RefreshList()
        {
            listVariables.Items.Clear();
            foreach (var kvp in variables)
            {
                listVariables.Items.Add($"{kvp.Key} : {kvp.Value}");
            }
        }
    }
}