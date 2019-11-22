using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace webm_to_mp4
{
    public partial class Form1 : Form
    {
        string path;
        int count;
        int total;
        bool error;

        public Form1()
        {            
            InitializeComponent();
            label2.Text = String.Empty;
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            error = false;
            count = 0;
            total = openFileDialog1.FileNames.Length;
            string name;

            foreach (String file in openFileDialog1.FileNames)
            {
                if (file.Split('.').Length - 1 != 1 || file.Split('.')[1] != "webm")
                {
                    error = true;
                    break;
                }
                name = file.Split('\\')[file.Split('\\').Length - 1].Split('.')[0];
                FFMpegConverter ffMpeg = new FFMpegConverter();
                ffMpeg.ConvertMedia(file, path + name + ".mp4", Format.mp4);
                backgroundWorker1.ReportProgress(count * 100 / total);
                ++count;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            path = string.Empty;

            openFileDialog1.ShowDialog();
            openFileDialog1.InitialDirectory = @"C:\";                       
            if (textBox1.Text == String.Empty && openFileDialog1.FileNames.Length != 0) {
                var tab = openFileDialog1.FileNames[0].Split('\\');
                for (int i = 0; i < tab.Length - 1; ++i) path += tab[i] + '\\';
            }
            textBox1.Text = path;
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label2.Text = count + "/" + total;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != String.Empty)
            {
                label2.Text = "loading ...";
                progressBar1.Value = 0;
                backgroundWorker1.RunWorkerAsync();
            }
            else MessageBox.Show("No file selected.");
        }

        private string Reverse(string s)
        {
            string res = String.Empty;
            for (int i = s.Length - 1; i >= 0; --i) res += s[i];
            return res;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (error)
            {
                MessageBox.Show("something went wrong ... input files must be webms and there must be only 1 point in the filename");
                progressBar1.Value = 0;
                label2.Text = 0 + "/" + 0;
                return;
            }

            progressBar1.Value = 100;
            label2.Text = total + "/" + total;

            DialogResult dialogResult = MessageBox.Show(count + " mp4 file created in this folder : " + path + ". do you want to go there ?", "it worked :)", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Process.Start(path);
            }
        }
    }
}
