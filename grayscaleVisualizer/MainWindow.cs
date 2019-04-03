using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace grayscaleVisualizer
{
    public partial class main_window : Form
    {
        string imageFile = "";
        string settingFile="./setting.txt";
        string outputDirector = "./out/";
        Dictionary<int, string> colorPallet;
        
        public main_window()
        {
            InitializeComponent();
            loadSetting();
            pictureBox.AllowDrop = true;
            if (!Directory.Exists(outputDirector))
                Directory.CreateDirectory(outputDirector);
            
        }

        private void makeColorBitmap() {
            if (imageFile.Equals("")) return;
            Bitmap srcImage = new Bitmap(imageFile);
            if (colorPallet != null)
            {                
                int width = srcImage.Width;
                int height = srcImage.Height;
                Bitmap dstImage = new Bitmap(width, height);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color grayColor= srcImage.GetPixel(x, y);//グレースケールの場合R,G,B全て同じ値
                        string convertColorCode;
                        bool hasValue = colorPallet.TryGetValue(grayColor.R, out convertColorCode);
                        if (hasValue)
                        {
                            Color convertColor = System.Drawing.ColorTranslator.FromHtml(convertColorCode);
                            dstImage.SetPixel(x,y,convertColor);
                            //Console.WriteLine(convertColor);

                        }
                        else
                        {
                            dstImage.SetPixel(x, y, grayColor);
                        }                        

                    }
                }

                pictureBox.Image = dstImage;
            }
            else {
                pictureBox.Image = srcImage;
                Console.WriteLine("setting not yet!");
            }
        }
        private void loadSetting()
        {
            if (!File.Exists(settingFile))
            {
                Console.WriteLine("setting file not found");
                return;
            }
            else {
                if (colorPallet != null) colorPallet.Clear();
                colorPallet = new Dictionary<int, string>();

                string[] lines = File.ReadAllLines(settingFile);
                Console.WriteLine("setting file loaded:");
                foreach (string line in lines)
                {
                    Console.WriteLine("\t" + line);
                    var splitLine = line.Split(',');
                    colorPallet.Add(int.Parse(splitLine[0]),splitLine[1]);

                }
            }

        }
        private void clickOpen(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "image file|*.bmp;*.gif;*.jpg;*.png;*.jpeg;*.ico|all|*.*";
            if (ofd.ShowDialog() == DialogResult.OK) imageFile = ofd.FileName;
            this.Text = ofd.FileName;
            makeColorBitmap();
        }


        private void clickSetting(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "setting file|setting.txt;";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                settingFile=ofd.FileName;
                loadSetting();
            }
        }

        private void dragDropPictureBox(object sender, DragEventArgs e)
        {
            // ファイルが渡されていなければ、何もしない
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            // 渡されたファイルに対して処理を行う
            foreach (var filePath in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                try
                {                    
                    imageFile = filePath;                    
                    makeColorBitmap();
                    this.Text = filePath;
                }
                catch (Exception)
                {
                    MessageBox.Show("please put image file", "error");
                }
                
                break;
            }
        }

        private void dragEnterPictureBox(object sender, DragEventArgs e)
        {
            // ドラッグドロップ時にカーソルの形状を変更
            e.Effect = DragDropEffects.All;
        }

        private void clickSave(object sender, EventArgs e)
        {
            string filename = Path.GetFileNameWithoutExtension(imageFile);
            if(pictureBox.Image!=null)
                pictureBox.Image.Save(outputDirector + filename+".png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
