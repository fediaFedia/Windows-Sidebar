using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class VariantControl : UserControl
{
    List<string> variants;
    int index = 0;
    string basePath;

    PictureBox picture;
    Label label;

    public string SelectedVariant => variants[index];

    public VariantControl(List<string> vars, string path)
    {
        variants = vars;
        basePath = path;

        Height = 140;
        Width = 220;

        picture = new PictureBox
        {
            Height = 100,
            Width = 160,
            Top = 0,
            Left = 30,
            SizeMode = PictureBoxSizeMode.Zoom
        };

        label = new Label
        {
            Top = 105,
            Width = 220,
            Height = 20,
            TextAlign = ContentAlignment.MiddleCenter
        };

        Button prev = new Button { Text = "◀", Left = 0, Top = 40, Width = 25 };
        Button next = new Button { Text = "▶", Left = 195, Top = 40, Width = 25 };

        prev.Click += (s, e) => Change(-1);
        next.Click += (s, e) => Change(1);

        Controls.Add(picture);
        Controls.Add(label);
        Controls.Add(prev);
        Controls.Add(next);

        UpdateDisplay();
    }

    void Change(int delta)
    {
        index += delta;
        if (index < 0) index = variants.Count - 1;
        if (index >= variants.Count) index = 0;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        string name = variants[index];
        label.Text = name;

        string img = Path.Combine(basePath, "Variants", name + ".png");

        if (File.Exists(img))
            picture.Image = Image.FromFile(img);
        else
            picture.Image = null;
    }

    public void SetValue(string val)
    {
        int i = variants.IndexOf(val);
        if (i >= 0)
        {
            index = i;
            UpdateDisplay();
        }
    }
}