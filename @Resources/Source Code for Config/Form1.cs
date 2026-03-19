using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RainConfigApp
{
    public class MainForm : Form
    {
        string CURRENTCONFIG;
        string CURRENTPATH;
        string CURRENTFILE;
        string PROGRAMPATH;
        int newX;
        int newY;

        FlowLayoutPanel container;
        Dictionary<string, Control> controlsMap = new();
        Dictionary<string, string> userValues = new();

        public MainForm(string config, string path, string file, string programPath, int? posX, int? posY)
        {
            CURRENTCONFIG = config;
            CURRENTPATH = path;
         //  CURRENTPATH = "C:\\Users\\User\\Documents\\Rainmeter\\Skins\\Vista Rainbar\\Clock\\Normal";
           
           
            // MessageBox.Show(CURRENTPATH);
            CURRENTFILE = file;
            PROGRAMPATH = programPath;

           //PROGRAMPATH = "C:\\Program Files\\Rainmeter\\";
         MinimizeBox = false;  
            MaximizeBox = false;
            ValidatePaths();
            InitializeUI(posX, posY);
            LoadUserVariables();
            LoadConfig();
        }

        void ValidatePaths()
        {
            if (!Directory.Exists(CURRENTPATH))
                throw new Exception("CURRENTPATH not found.");

            string rainExe = Path.Combine(PROGRAMPATH, "Rainmeter.exe");
            if (!File.Exists(rainExe))
                throw new Exception("Rainmeter.exe not found in PROGRAMPATH.");
        }

        void InitializeUI(int? posX, int? posY)
        {
            Size = new Size(300, 500);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.Manual;

            if (posX.HasValue && posY.HasValue) {
                newY = posY.Value;
                newX = posX.Value;
                // Location = new Point(posX.Value - Width, posY.Value - Height);
                if (Screen.PrimaryScreen.Bounds.Height < posY.Value + Height) {
                    newY = Screen.PrimaryScreen.Bounds.Height - Height - 20;
                }
                if (posX.Value < Width) {
                    newX = Width;
                }
                Location = new Point(newX - Width, newY);
            }
            else
                CenterToScreen();

            // Header
            Panel header = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(21,26,30)
            };

            string headerText = CURRENTCONFIG.TrimEnd('\\');
            if (headerText.Contains("\\"))
                headerText = headerText.Split('\\').Last();

            Label title = new Label
            {
                Text = headerText,
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            header.Controls.Add(title);
           // Controls.Add(header);

            container = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false,
                Height = 240,
                Padding = new Padding(20, 0, 0, 0)
                // Left, Top, Right
            };

            Controls.Add(container);

            // Bottom buttons
            Panel bottom = new Panel { Height = 50, Dock = DockStyle.Bottom };

            Button btnReset = new Button { Text = "Reset", Width = 80, Left = 10, Top = 10 };
            Button btnCancel = new Button { Text = "Cancel", Width = 80, Left = 100, Top = 10 };
            Button btnOK = new Button { Text = "OK", Width = 80, Left = 190, Top = 10 };

            btnReset.Click += BtnReset_Click;
            btnCancel.Click += (s, e) => Close();
            btnOK.Click += BtnOK_Click;

            bottom.Controls.Add(btnReset);
            bottom.Controls.Add(btnCancel);
            bottom.Controls.Add(btnOK);
            Controls.Add(bottom);
        }

        void LoadUserVariables()
        {
            string file = Path.Combine(CURRENTPATH, "UserVariables.inc");
            if (!File.Exists(file)) return;

            var lines = File.ReadAllLines(file);
            bool inVars = false;

            foreach (var line in lines)
            {
                if (line.Trim().Equals("[Variables]", StringComparison.OrdinalIgnoreCase))
                {
                    inVars = true;
                    continue;
                }

                if (!inVars || !line.Contains("=")) continue;

                var parts = line.Split(new[] { '=' }, 2);
                userValues[parts[0].Trim()] = parts[1].Trim();
            }
        }

        void LoadConfig()
        {
            try
            {
                string file = Path.Combine(CURRENTPATH, "RainConfigure.cfg");
                if (!File.Exists(file))
                {
                    MessageBox.Show("RainConfigure.cfg not found.");
                    return;
                }

                var lines = File.ReadAllLines(file);
                bool inVars = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Trim() == "[Variables]")
                    {
                        inVars = true;
                        continue;
                    }

                    if (!inVars || string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string name = lines[i++].Trim();
                    string desc = lines[i++].Trim();
                    string type = lines[i].Trim();

                    CreateControl(name, desc, type);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Config Load Error");
            }
        }

        void CreateControl(string name, string desc, string typeLine)
        {
            Panel panel = new Panel { Width = 240, AutoSize = true };

            Label lblName = new Label
            {
                Text = name.Replace('_', ' '),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true
            };

            Label lblDesc = new Label
            {
                Text = desc,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                Top = 15,
                AutoSize = true
            };

            panel.Controls.Add(lblName);
            panel.Controls.Add(lblDesc);

            Control ctrl = null;
            int top = 32;

            if (typeLine.StartsWith("Variant:"))
            {
                var variants = typeLine.Split(':')[1].Split('|').ToList();
                VariantControl vc = new VariantControl(variants, CURRENTPATH);
                vc.Top = top;

                if (userValues.ContainsKey(name))
                    vc.SetValue(userValues[name]);

                ctrl = vc;
            }
            else if (typeLine.StartsWith("ComboboxEdit:"))
            {
                var values = typeLine.Split(':')[1].Split('|');
                ComboBox cb = new ComboBox { Top = top, Width = 220 };
                cb.Items.AddRange(values);
                if (userValues.ContainsKey(name))
                    cb.SelectedItem = userValues[name];
                ctrl = cb;
            }
            else if (typeLine.StartsWith("Combobox:"))
            {
                var values = typeLine.Split(':')[1].Split('|');
                ComboBox cb = new ComboBox { Top = top, Width = 220, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
                cb.Items.AddRange(values);
                if (userValues.ContainsKey(name))
                    cb.SelectedItem = userValues[name];
                ctrl = cb;
            }
            else if (typeLine.StartsWith("Checkbox:"))
            {
                var parts = typeLine.Split(':');
                CheckBox chk = new CheckBox
                {
                    Left = 4,
                    Top = top,
                    Text = parts[3]
                };
                if (userValues.ContainsKey(name))
                    chk.Checked = userValues[name] == parts[2];
                chk.Tag = parts;
                ctrl = chk;
            }
            else if (typeLine.StartsWith("Text"))
            {
                TextBox tb = new TextBox { Top = top, Width = 220, MaxLength = 50 };
                if (userValues.ContainsKey(name))
                    tb.Text = userValues[name];
                ctrl = tb;
            }
else if (typeLine.StartsWith("Number:", StringComparison.OrdinalIgnoreCase))
{
    var parts = typeLine.Split(':');

    if (parts.Length < 3)
    {
        MessageBox.Show($"Invalid Number format for {name}");
        return;
    }

    if (!int.TryParse(parts[1], out int min))
        min = 0;

    if (!int.TryParse(parts[2], out int max))
        max = 100;

    NumericUpDown nud = new NumericUpDown
    {
        Top = top,
        Width = 220,
        Minimum = min,
        Maximum = max
    };

    // Populate from UserVariables
    if (userValues.ContainsKey(name) &&
        int.TryParse(userValues[name], out int val))
    {
        if (val < min) val = min;
        if (val > max) val = max;

        nud.Value = val;
    }
    else
    {
        nud.Value = min; // default
    }

    ctrl = nud;
}
            else if (typeLine.StartsWith("Slider:"))
            {
                var parts = typeLine.Split(':');
                TrackBar tr = new TrackBar
                {
                    Top = top,
                    Width = 220,
                    Minimum = int.Parse(parts[1]),
                    Maximum = int.Parse(parts[2])
                };
                if (userValues.ContainsKey(name) &&
                    int.TryParse(userValues[name], out int val))
                    tr.Value = Math.Max(tr.Minimum, Math.Min(tr.Maximum, val));
                ctrl = tr;
            }
            else if (typeLine.StartsWith("Browse:Folder"))
            {
                TextBox tb = new TextBox { Top = top, Width = 150 };
                Button btn = new Button { Text = "...", Left = 160, Top = top, Width = 60 };
                btn.Click += (s, e) =>
                {
                    using FolderBrowserDialog dlg = new();
                    if (dlg.ShowDialog() == DialogResult.OK)
                        tb.Text = dlg.SelectedPath;
                };
                if (userValues.ContainsKey(name))
                    tb.Text = userValues[name];
                panel.Controls.Add(btn);
                ctrl = tb;
            }
            else if (typeLine.StartsWith("Browse:File"))
            {
                TextBox tb = new TextBox { Top = top, Width = 150 };
                Button btn = new Button { Text = "...", Left = 160, Top = top, Width = 60 };
                btn.Click += (s, e) =>
                {
                    using OpenFileDialog dlg = new();
                    if (dlg.ShowDialog() == DialogResult.OK)
                        tb.Text = dlg.FileName;
                };
                if (userValues.ContainsKey(name))
                    tb.Text = userValues[name];
                panel.Controls.Add(btn);
                ctrl = tb;
            }
            else if (typeLine.StartsWith("Color"))
            {
                Button btn = new Button { Text = "Pick Color", Top = top, Width = 220 };
                if (userValues.ContainsKey(name))
                {
                    var rgb = userValues[name].Split(',');
                    if (rgb.Length == 3)
                    {
                        var c = Color.FromArgb(
                            int.Parse(rgb[0]),
                            int.Parse(rgb[1]),
                            int.Parse(rgb[2]));
                        btn.BackColor = c;
                        btn.Tag = c;
                    }
                }
                btn.Click += (s, e) =>
                {
                    using ColorDialog dlg = new();
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        btn.BackColor = dlg.Color;
                        btn.Tag = dlg.Color;
                    }
                };
                ctrl = btn;
            }

            if (ctrl != null)
            {
                panel.Controls.Add(ctrl);
                controlsMap[name] = ctrl;
            }

            container.Controls.Add(panel);
            
            Label separator = new Label
            {
AutoSize = false,
Height = 2,
            Top = 10,
                    Width = 220,
BorderStyle = BorderStyle.Fixed3D
            };
             container.Controls.Add(separator);
        }

        void BtnReset_Click(object sender, EventArgs e)
        {
            try
            {
                string bak = Path.Combine(CURRENTPATH, "UserVariables.bak");
                string inc = Path.Combine(CURRENTPATH, "UserVariables.inc");

                if (File.Exists(bak))
                    File.Copy(bak, inc, true);

                controlsMap.Clear();
                container.Controls.Clear();
                userValues.Clear();
                LoadUserVariables();
                LoadConfig();
                RefreshRainmeter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Reset Error");
            }
        }

        void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string inc = Path.Combine(CURRENTPATH, "UserVariables.inc");

                using StreamWriter sw = new(inc);
                sw.WriteLine("[Variables]");

                foreach (var kv in controlsMap)
                {
                    string value = "";

                    switch (kv.Value)
                    {
                        case TextBox tb:
                            value = tb.Text;
                            break;
                        case NumericUpDown nud:
                            value = nud.Value.ToString();
                            break;
                        case ComboBox cb:
                            value = cb.SelectedItem?.ToString();
                            break;
                        case CheckBox chk:
                            var parts = (string[])chk.Tag;
                            value = chk.Checked ? parts[2] : parts[1];
                            break;
                        case TrackBar tr:
                            value = tr.Value.ToString();
                            break;
                        case VariantControl vc:
                            value = vc.SelectedVariant;
                            break;
                        case Button btn when btn.Tag is Color c:
                            value = $"{c.R},{c.G},{c.B}";
                            break;
                    }

                    sw.WriteLine($"{kv.Key}={value}");
                }

                RefreshRainmeter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Error");
            }
        }

        void RefreshRainmeter()
        {
            try
            {
                string exe = Path.Combine(PROGRAMPATH, "Rainmeter.exe");

                ProcessStartInfo psi = new()
                {
                    FileName = exe,
                    Arguments = $"!Refresh \"{CURRENTCONFIG}\"",
                    UseShellExecute = false
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Refresh Error");
            }
        }
    }
}