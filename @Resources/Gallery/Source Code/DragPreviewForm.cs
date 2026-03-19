using System.Drawing;
using System.Windows.Forms;

namespace GadgetGallery
{
    public class DragPreviewForm : Form
    {
        public PictureBox PreviewImage { get; private set; }

        public DragPreviewForm(Image img, int size = 96)
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;

            BackColor = Color.Black;
            TransparencyKey = Color.Black;

            Width = size;
            Height = size;

            PreviewImage = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = img
            };

            Controls.Add(PreviewImage);
        }
    }
}
