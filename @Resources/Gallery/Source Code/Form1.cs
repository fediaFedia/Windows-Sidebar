using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using WinBlur;
using static System.Windows.Forms.DataFormats;



namespace GadgetGallery
{
    public class MainForm : Form
    {

        private List<Gadget> allGadgets = new List<Gadget>();
        private List<Gadget> filteredGadgets = new List<Gadget>();
        private string getMoreGadgetsLink = "https://google.com/";

        private FlowLayoutPanel gridPanel;
        private Gadget selectedGadget;

        private bool isDragging = false;
        private Point dragStart;
        private DragPreviewForm dragPreview;

        private const int DragThreshold = 6;
        private readonly Color NormalColor = Color.Transparent;
        private readonly Color HoverColor = Color.FromArgb(50, 50, 50);
        private readonly Color SelectedColor = Color.FromArgb(70, 70, 70);

        private Panel selectedPanel;

        private Label btnPrev;
        private Label btnNext;
        private Label lblPage;
        private Label lblPage2;
        private Label btnPrev2;
        private Label btnNext2;

        private TextBox txtSearch;
        private Label txtSearch2;
        private Panel detailsPanel;
        private Label lblDetailsName;
        private Label lblDetailsDesc;
        private PictureBox microsoftLogo;
        private Label lblDetailsCopyright;
        private Label lblDetailsCopyright2;
        private Label lblDetailsCopyright3;
        private Label btnToggleDetails;
        private Label btnToggleDetails2;
        private Label divider;
        private Label getMoreLabel;
        private Label getMoreLabel2;
        private int currentPage = 1;
        private int itemsPerPage = 12;
        private int extraWidth = 0;


        private bool vistaBeta1 = false;
        private bool vistaBeta2 = false;

        private string[] startupArgs;
        private string gadgetsXml = "gadgets.xml";
        private bool detailsVisible = false;

        public MainForm(string[] args)
        {
            startupArgs = args;
            HandleStartupArguments();
            InitializeUI();
            MinimizeBox = false;  
            MaximizeBox = false;
           // ControlBox = false;
         //  FormBorderStyle = FormBorderStyle.None;

            this.ShowIcon = false;
            this.Text = "";
             this.Icon = new Icon("icon.ico");
            LoadGadgets();
            ApplyLayout();
            RefreshGrid();
            this.Load += new System.EventHandler(this.Form1_Load);
          

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ActiveControl = detailsPanel;    
            UI.SetBlurStyle(cntrl: this, blurType: UI.BlurType.Mica, designMode: UI.Mode.DarkMode);
        }
        // ---------------- UI ----------------
void SetPlaceholder(TextBox box, string text)
{
    box.Text = text;
    box.ForeColor = Color.Gray;

    box.GotFocus += (s, e) =>
    {
        if (box.ForeColor == Color.Gray)
        {
            box.Text = "";
            box.ForeColor = Color.White;
        }
    };

    box.LostFocus += (s, e) =>
    {
        if (string.IsNullOrWhiteSpace(box.Text))
        {
            box.Text = text;
            box.ForeColor = Color.Gray;
        }
    };
}
        private void InitializeUI()
        {
            Text = "Windows Sidebar Gadgets";
            Width = 700;
            Height = 550;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = System.Drawing.Color.Black;
            // Top bar
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Visible = !vistaBeta1,
            };

            // Top bar
            Panel rightPanel = new Panel
            {
                Dock = DockStyle.Right,
                AutoSize = true

            };


            btnPrev = new Label
            {
                Text = "",
                Width = 40,
                Left = 14,
                Top = 12,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe Fluent Icons", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                Visible = !vistaBeta2

            };

            btnNext = new Label
            {
                Text = "",
                Top = 12,
                Width = 40,
                Font = new Font("Segoe Fluent Icons", 10, FontStyle.Bold),
                Left = 124,
                Cursor = Cursors.Hand,
                ForeColor = System.Drawing.Color.White,
                Visible = !vistaBeta2
            };

            lblPage = new Label
            {
                Text = "Page 1 of 1",
                AutoSize = true,
                Left = 50,
                Top = 12,
                ForeColor = System.Drawing.Color.White,
                Visible = !vistaBeta2
            };

            txtSearch = new TextBox
            {
                Width = 200,
                Top = 8,
                Left = -20,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = System.Drawing.Color.Black,
                ForeColor = System.Drawing.Color.Gray,
               // PlaceholderText = "Search all gadgets...",
  Text = "Search all gadgets...",
                Cursor = Cursors.IBeam,
                BorderStyle = BorderStyle.None,
            };
            // SetPlaceholder(txtSearch, "Search all gadgets...");
            
            txtSearch2 = new Label
            {
                Text = "\uE721", // Search icon
                Font = new Font("Segoe Fluent Icons", 10f),
                AutoSize = false,
                Width = 32,
                Height = 12,
                Top = 9,
                Left = 155,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray,
                Cursor = Cursors.Hand
            };


            btnPrev.Click += (s, e) => ChangePage(-1);
            btnNext.Click += (s, e) => ChangePage(1);
            
            //btnNext.DoubleClick += (s, e) => ChangePage(1);

            txtSearch.TextChanged += (s, e) => ApplySearch();

            topPanel.Controls.Add(btnPrev);
            topPanel.Controls.Add(lblPage);
            topPanel.Controls.Add(btnNext);

            if (vistaBeta1 || vistaBeta2) { 
            btnPrev2 = new Label
            {
                Text = "",
                Width = 35, //MAke rightPanel wider here
                Left = 5,
                Top = 5,
              //  Anchor =  AnchorStyles.Right,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe Fluent Icons", 8, FontStyle.Bold),
                ForeColor = System.Drawing.Color.Gray
            };
                lblPage2 = new Label
                {
                    Text = "1 / 1 ",
                    AutoSize = true,
                    Left = -1,
                    Top = 15,
                    Font = new Font("Segoe UI", 8),
                    ForeColor = System.Drawing.Color.Gray,
                    Visible = vistaBeta2
                };
                btnNext2 = new Label
            {
                Text = "",
                Top = 30,
                Left = 5,
                Width = 20,
             //  Anchor = AnchorStyles.Right,
                Font = new Font("Segoe Fluent Icons", 8, FontStyle.Bold),
    
                Cursor = Cursors.Hand,
                ForeColor = System.Drawing.Color.Gray
            };


                btnPrev2.Click += (s, e) => ChangePage(-1);
                btnNext2.Click += (s, e) => ChangePage(1);
                rightPanel.Controls.Add(btnNext2);
                rightPanel.Controls.Add(lblPage2);
                rightPanel.Controls.Add(btnPrev2);
                Controls.Add(rightPanel);
            }

            topPanel.Controls.Add(txtSearch);
            topPanel.Controls.Add(txtSearch2);
            txtSearch2.BringToFront();
            Controls.Add(topPanel);

            // Grid

            if (vistaBeta1)
            {
                gridPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = false,
                    Padding = new Padding(25, 0, 0, 0),
                    WrapContents = true
                };
            } else if (vistaBeta2)
            {
                gridPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = false,
                    Padding = new Padding(10, 55, 0, 0),
                    WrapContents = true
                };
            } else
            {
                gridPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = false,
                    Padding = new Padding(25, 55, 0, 0),
                    BackColor = Color.FromArgb(20, 20, 20),
   
                    WrapContents = true
                };
            }


            Controls.Add(gridPanel);
       

            // Bottom bar
            Panel bottomPanel = new Panel
            {
                Visible = !vistaBeta1,
                Dock = DockStyle.Bottom,
                Height = 50
            };


            divider = new Label
            {
                Text = "s",
                AutoSize = false,
   Width=1600,
                Height = 1,
                BackColor = Color.FromArgb(30, 30, 30),
            };
      
            btnToggleDetails2 = new Label
            {
                Text = "",
                Left = 22,
                Width = 22,
                Cursor = Cursors.Hand,
                Top = 15,
                Font = new Font("Segoe Fluent Icons", 10, FontStyle.Bold),
                ForeColor = System.Drawing.Color.White
            };
            btnToggleDetails = new Label
            {
                Text = "Show details",
                Left = 45,
                Cursor = Cursors.Hand,
                Top = 15,
                ForeColor = System.Drawing.Color.White,
                Visible = !vistaBeta2
            };

            getMoreLabel2 = new Label
            {
                Text = "",
                Left = 18,
                Width = 22,
                Top = 15,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe Fluent Icons", 12, FontStyle.Bold),
                ForeColor = System.Drawing.Color.Teal
            };

            getMoreLabel = new Label
            {
                Text = "Get more gadgets online",
                Left = 40,
                Top = 15,
                AutoSize = true,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                ForeColor = System.Drawing.Color.Teal
            };

            btnToggleDetails.Click += ToggleDetails;
            btnToggleDetails2.Click += ToggleDetails;
            getMoreLabel.Tag = getMoreGadgetsLink;
            getMoreLabel.Click += OpenURL;

       
            bottomPanel.Controls.Add(btnToggleDetails2);

            bottomPanel.Controls.Add(btnToggleDetails);
            bottomPanel.Controls.Add(getMoreLabel2);
            bottomPanel.Controls.Add(getMoreLabel);


           

            // Details panel
            detailsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                Visible = false,
                // BorderStyle = BorderStyle.FixedSingle
                BorderStyle = BorderStyle.None
            };

            lblDetailsName = new Label
            {
                Font = new Font("Segoe UI", 12),
                Left = 15,
                Top = 15,
                AutoSize = true,
                ForeColor = System.Drawing.Color.White
            };

            lblDetailsDesc = new Label
            {
                Left = 15,
               
                Top = 37,
                Width = 300,
                Height = 90,
                ForeColor = System.Drawing.Color.White
            };

            microsoftLogo = new PictureBox
            {
                Width = 48,
                Height = 48,
                Top = 15,
                Visible = false,
                Left = -50,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            LoadLogoImage();

            lblDetailsCopyright = new Label
            {
                Text = "Microsoft Corporation",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe UI", 10),
                Left = 0,
                Top = 10,
                AutoSize = true,
                ForeColor = System.Drawing.Color.White
            };
            lblDetailsCopyright2 = new Label
            {
                Text = "© 2006",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,

                Left = 0,
                Top = 30,
                AutoSize = true,
                ForeColor = System.Drawing.Color.White
            };
            lblDetailsCopyright3 = new Label
            {
                Text = "www.gallery.microsoft.com",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Left = 0,
                Top = 45,
                AutoSize = true,
                Cursor = Cursors.Hand,
                ForeColor = System.Drawing.Color.Teal
            };

            lblDetailsCopyright3.Tag = "https://google.com";
            lblDetailsCopyright3.Click += OpenURL;
            detailsPanel.Controls.Add(divider);
            detailsPanel.Controls.Add(lblDetailsName);
            detailsPanel.Controls.Add(lblDetailsDesc);
            detailsPanel.Controls.Add(microsoftLogo);
            detailsPanel.Controls.Add(lblDetailsCopyright);
            detailsPanel.Controls.Add(lblDetailsCopyright2);
            detailsPanel.Controls.Add(lblDetailsCopyright3);
            
            Controls.Add(detailsPanel);
            Controls.Add(bottomPanel);
        }
        // Workaround for no PlaceholderText in 4.8
        private void txtSearch_Enter(object sender, EventArgs e){
            txtSearch.ForeColor = System.Drawing.Color.White;
txtSearch.Text = "";
}
        private void txtSearch_Leave(object sender, EventArgs e){
            txtSearch.ForeColor = System.Drawing.Color.Gray;
txtSearch.Text = "Search all gadgets...";
}
        // ---------------- XML Load ----------------

        private void GetOnMouseEnter(object sender, EventArgs e)
        {
            getMoreLabel.Font = new Font(getMoreLabel.Font.Name, getMoreLabel.Font.SizeInPoints, FontStyle.Underline);
        }

        private void GetOnMouseLeave(object sender, EventArgs e)
        {
            getMoreLabel.Font = new Font(getMoreLabel.Font.Name, getMoreLabel.Font.SizeInPoints, FontStyle.Regular);
        }
        private void LoadGadgets()
        {
            // getMoreGadgetsLink = "http://bing.com";
            XDocument doc = XDocument.Load(gadgetsXml);

//GetMoreGadgetsLink = doc.Root?.Value;


            allGadgets = doc.Descendants("Gadget")
                .Select(g => new Gadget
                {
                    Id = g.Element("Skin")?.Value,
                    Id2 = g.Element("Meter")?.Value,
                    Name = g.Element("Name")?.Value,
                    Icon = g.Element("Icon")?.Value,
                    Description = g.Element("Description")?.Value,
                    Version = g.Element("Version")?.Value,
                    Author = g.Element("Author")?.Value,
                    Copyright = g.Element("Copyright")?.Value,
                    Link = g.Element("Link")?.Value
                })
                .ToList();

            filteredGadgets = allGadgets;
        }

        // ---------------- Layout Rules ----------------
        private void LoadLogoImage()
        {
            try
            {
                string exeDir =
                    AppDomain.CurrentDomain.BaseDirectory;

                string logoPath =
                    System.IO.Path.Combine(exeDir, "logo.png");

                if (System.IO.File.Exists(logoPath))
                {
                    microsoftLogo.Image = Image.FromFile(logoPath);
                }
            }
            catch
            {
                // Ignore if missing/corrupt
            }
        }

        private void ApplyLayout()
        {
            int count = filteredGadgets.Count;

            if (vistaBeta1)
            {
                extraWidth = 30;
            }
            if (count <= 5)
            {
                itemsPerPage = 10; // 5x2
                Width = 490 + extraWidth;
                Height = 370;
            }

            else if (count >= 6 && count <= 10   || vistaBeta2)
            {
                
                itemsPerPage = 10; // 5x2
                Width = 590 + extraWidth; // was 590
                Height = 370;
            }
            else if (count >= 11 && count <= 12)
            {
                itemsPerPage = 12; // 7x3
                Width = 700 + extraWidth;
                Height = 370;
      
            }
            else if (count >= 13 && count <= 18)
            {
                itemsPerPage = 18; // 7x3
                Width = 700 + extraWidth;
                Height = 480;

            }


            else
            {
                itemsPerPage = 21; // 7x3
                Width = 800 + extraWidth;
                Height = 480;
            }
        }

        // ---------------- Grid ----------------
        private void OpenURL(object sender, EventArgs e)
        {
            if (sender is Control ctrl &&
                ctrl.Tag is string url &&
                !string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(url)
                    {
                        UseShellExecute = true
                    });
                }
                catch
                {
                    MessageBox.Show("Could not open link.");
                }
            }
        }

        private void RefreshGrid()
        {
            // Need to find a way to re-use ToggleDetails
            if (detailsPanel.Visible)
            {
                detailsVisible = !detailsVisible;

                detailsPanel.Visible = detailsVisible;
                ActiveControl = detailsPanel;
                if (detailsVisible)
                {
                    Height += detailsPanel.Height;
                    btnToggleDetails.Text = "Hide details";
                    btnToggleDetails2.Text = "";
                }
                else
                {
                    Height -= detailsPanel.Height;
                    btnToggleDetails.Text = "Show details";
                    btnToggleDetails2.Text = "";
                }
            }

            //ActiveControl = detailsPanel;
            gridPanel.Controls.Clear();

            //ApplyLayout();

            int start = (currentPage - 1) * itemsPerPage;

            var pageItems = filteredGadgets
                .Skip(start)
                .Take(itemsPerPage)
                .ToList();

            foreach (var gadget in pageItems)
            {
                gridPanel.Controls.Add(CreateGadgetCard(gadget));
            }

            UpdatePageLabel();
        }
        private void Gadget_MouseDown(object sender, MouseEventArgs e)
        {
            ActiveControl = detailsPanel;
            if (e.Button != MouseButtons.Left) return;

            Control ctrl = sender as Control;

            Panel panel = FindParentPanel(ctrl);

            if (panel == null) return;

            if (!(panel.Tag is Gadget gadget)) return;

            SelectOnly(panel, gadget);

            dragStart = Cursor.Position;
            isDragging = false;
        }


        private void Gadget_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == 0)
                return;

            if (selectedGadget == null) return;

            Point current = Cursor.Position;

            int dx = Math.Abs(current.X - dragStart.X);
            int dy = Math.Abs(current.Y - dragStart.Y);

            if (!isDragging && (dx > DragThreshold || dy > DragThreshold))
            {
                StartDragging();
            }

            if (isDragging && dragPreview != null)
            {
                dragPreview.Left =
                    current.X - dragPreview.Width / 2;

                dragPreview.Top =
                    current.Y - dragPreview.Height / 2;
            }
        }



        private void Gadget_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isDragging) return;

            FinishDragging(Cursor.Position);
        }


        private void StartDragging()
        {
            isDragging = true;

           // RunRainmeterActivate(selectedGadget.Id);

            CreateDragPreview(selectedGadget.Icon);
        }

        private void FinishDragging(Point screenPos)
        {
            isDragging = false;

            if (dragPreview != null)
            {
                dragPreview.Close();
                dragPreview.Dispose();
                dragPreview = null;
            }

            RunRainmeterMove(screenPos.X, screenPos.Y, selectedGadget.Id, selectedGadget.Id2);
        }


        private void CreateDragPreview(string iconPath)
        {
            if (!System.IO.File.Exists(iconPath))
                return;

            Image img = Image.FromFile(iconPath);

            dragPreview = new DragPreviewForm(img, 110);

            Point pos = Cursor.Position;

            dragPreview.Left = pos.X - dragPreview.Width / 2;
            dragPreview.Top = pos.Y - dragPreview.Height / 2;

            dragPreview.Show();
        }


        private void SelectOnly(Panel selected, Gadget gadget)
        {
            foreach (Control c in gridPanel.Controls)
            {
                if (c is Panel p)
                    p.BackColor = NormalColor;
            }

            selected.BackColor = SelectedColor;

            selectedPanel = selected;
            selectedGadget = gadget;

            ShowDetails(gadget);
        }


        private void RunRainmeterMove(int x, int y, string id, string id2)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "C:\\Program Files\\Rainmeter\\Rainmeter.exe",
                    Arguments = $"!ActivateConfig \"{id}\" \"{id2}\"\"",
                    UseShellExecute = true
                });
                Console.WriteLine(id);
                Process.Start(new ProcessStartInfo
                {
                    FileName = "C:\\Program Files\\Rainmeter\\Rainmeter.exe",
                    Arguments = $"!Move {x-100} {y-100} \"{id}\"",
                    UseShellExecute = true
                });
            }
            catch { }
        }



        private Control CreateGadgetCard(Gadget gadget)
        {
            Panel panel = new Panel
            {
                Width = 100,
                Height = 100,
                BorderStyle = BorderStyle.None,
                //BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(2),
                Cursor = Cursors.Hand,
                Tag = gadget,
                BackColor = Color.Transparent
            };

            PictureBox pic = new PictureBox
            {
                Width = 64,
                Height = 64,
                Top = 5,
                Left = 20,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            if (System.IO.File.Exists(gadget.Icon))
                pic.Image = Image.FromFile(gadget.Icon);

            Label lbl = new Label
            {
                Text = gadget.Name,
                Top = 70,
                Width = 100,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = System.Drawing.Color.White
            };

            panel.Controls.Add(pic);
            panel.Controls.Add(lbl);

            // Mouse handling

   
            panel.MouseDown += Gadget_MouseDown;
            panel.MouseMove += Gadget_MouseMove;
            panel.MouseUp += Gadget_MouseUp;


            pic.MouseDown += Gadget_MouseDown;
            pic.MouseMove += Gadget_MouseMove;
            pic.MouseUp += Gadget_MouseUp;
            pic.DoubleClick += AddGadgetClick;

            lbl.MouseDown += Gadget_MouseDown;
            lbl.MouseMove += Gadget_MouseMove;
            lbl.MouseUp += Gadget_MouseUp;

            panel.MouseEnter += Gadget_MouseEnter;
            panel.MouseLeave += Gadget_MouseLeave;

            pic.MouseEnter += Gadget_MouseEnter;
            pic.MouseLeave += Gadget_MouseLeave;

            lbl.MouseEnter += Gadget_MouseEnter;
            lbl.MouseLeave += Gadget_MouseLeave;

            getMoreLabel.MouseEnter += GetOnMouseEnter;
            getMoreLabel.MouseLeave += GetOnMouseLeave;

            txtSearch.MouseDown += txtSearch_Enter;
 // txtSearch.Leave += txtSearch_Leave;

            return panel;
        }
        private void Gadget_MouseEnter(object sender, EventArgs e)
        {
            Panel panel = FindParentPanel(sender as Control);

            if (panel == null) return;

            if (panel != selectedPanel)
                panel.BackColor = HoverColor;
        }

        private void Gadget_MouseLeave(object sender, EventArgs e)
        {
            Panel panel = FindParentPanel(sender as Control);

            if (panel == null) return;

            if (panel != selectedPanel)
                panel.BackColor = NormalColor;
        }


        // ---------------- Selection ----------------

        private void SelectGadget(Gadget gadget)
        {
            ShowDetails(gadget);
            RunRainmeter(gadget.Id);
        }
        private Panel FindParentPanel(Control ctrl)
        {
            while (ctrl != null && !(ctrl is Panel))
                ctrl = ctrl.Parent;

            return ctrl as Panel;
        }
        private void ShowDetails(Gadget gadget)
        {
            if (gadget == null) return;

            lblDetailsName.Text = gadget.Name + " " + gadget.Version  ?? "";
            lblDetailsDesc.Text = gadget.Description  ?? "";
            lblDetailsCopyright.Text = gadget.Author ?? "";
            lblDetailsCopyright2.Text = gadget.Copyright ?? "";
            lblDetailsCopyright3.Text = gadget.Link ?? "";
            lblDetailsCopyright3.Tag = gadget.Link ?? "";
            if (gadget.Author == "Microsoft Corporation")
            {
                microsoftLogo.Visible = true;
            } else
            {
                microsoftLogo.Visible = false;
            }

        }

        // ---------------- Rainmeter ----------------

        private void RunRainmeter(string id)
        {
            try
            {
                string args = $"!ActivateConfig \"{id}\" \"{id}.ini\"\"";

                Process.Start(new ProcessStartInfo
                {
                    FileName = "Rainmeter.exe",
                    Arguments = args,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show("Could not launch Rainmeter.");
            }
        }

        // ---------------- Pagination ----------------

        private void ChangePage(int delta)
        {
            int maxPages = (int)Math.Ceiling(
                (double)filteredGadgets.Count / itemsPerPage);

            currentPage += delta;

            if (currentPage < 1) currentPage = 1;
            if (currentPage > maxPages) currentPage = maxPages;

            RefreshGrid();
        }

        private void UpdatePageLabel()
        {
            lblPage.Text = $"Page {currentPage} of {(int)Math.Ceiling(
                (double)filteredGadgets.Count / itemsPerPage)}";
            if (vistaBeta1 || vistaBeta2)
            {
                lblPage2.Text = $"{currentPage} / {(int)Math.Ceiling(
        (double)filteredGadgets.Count / itemsPerPage)}";
            }
        }

        // ---------------- Search ----------------

        private void ApplySearch()
        {
            string q = txtSearch.Text.ToLower();

            filteredGadgets = allGadgets
                .Where(g =>
                    g.Name.ToLower().Contains(q) ||
                    g.Description.ToLower().Contains(q))
                .ToList();

            currentPage = 1;

            RefreshGrid();
        }

        private void AddGadgetClick(object sender, EventArgs e)
        {

            RunRainmeterMove(250, 250, selectedGadget.Id, selectedGadget.Id2);
        }
        // ---------------- Details Toggle ----------------

        private void ToggleDetails(object sender, EventArgs e)
        {

            detailsVisible = !detailsVisible;

            detailsPanel.Visible = detailsVisible;
            ActiveControl = detailsPanel;
            if (detailsVisible)
            {
                Height += detailsPanel.Height;
                btnToggleDetails.Text = "Hide details";
                btnToggleDetails2.Text = "";
            }
            else
            {
                Height -= detailsPanel.Height;
                btnToggleDetails.Text = "Show details";
                btnToggleDetails2.Text = "";
            }
        }

        private void HandleStartupArguments()
        {
            if (startupArgs == null || startupArgs.Length == 0)
                return;

            for (int i = 0; i < startupArgs.Length; i++)
            {
                string arg = startupArgs[i];

                if (arg.Equals("--gadgets", StringComparison.OrdinalIgnoreCase)
                    && i + 1 < startupArgs.Length)
                {
                    gadgetsXml = startupArgs[i + 1];
                }
                if (arg.Equals("--beta1", StringComparison.OrdinalIgnoreCase)
                    && i + 1 < startupArgs.Length)
                {
                    vistaBeta1 = true;
                }
                if (arg.Equals("--beta2", StringComparison.OrdinalIgnoreCase)
                    && i + 1 < startupArgs.Length)
                {
                    vistaBeta2 = true;

                }
                if (arg.Equals("--rainmeter", StringComparison.OrdinalIgnoreCase)
                    && i + 1 < startupArgs.Length)
                {
                    //txtSearch.Text = startupArgs[i + 1];
                }
            }
        }
    }
}
