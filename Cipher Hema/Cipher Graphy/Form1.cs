using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;





namespace Cipher_Graphy


//=======================================//
//              AES 256 Bit              //
//=======================================//
//   "  QnkgTWFkZSBIYW1hdGNoaQ==   "     //



{
    public partial class Cipher : Form
    {

        BackgroundWorker background1 = new BackgroundWorker();
        BackgroundWorker background2 = new BackgroundWorker();

        public Cipher()
        {

            InitializeComponent();

            background1.WorkerReportsProgress = true;
            background1.WorkerSupportsCancellation = true;

            background1.DoWork += Background1_DoWork;
            background1.ProgressChanged += Background1_ProgressChanged;
            background1.RunWorkerCompleted += Background1_RunWorkerCompleted;


            background2.WorkerReportsProgress = true;
            background2.WorkerSupportsCancellation = true;

            background2.DoWork += Background2_DoWork;
            background2.ProgressChanged += Background2_ProgressChanged;
            background2.RunWorkerCompleted += Background2_RunWorkerCompleted;
        }

        private void Background2_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("The File Has Been Successfully Decrypted");
            progressBar2.Value = 0;
            label6.Text = "0%";

            button3.Enabled = false;
            button4.Enabled = false;

        }

        private void Background2_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
            label6.Text = progressBar2.Value + "%";
            label5.Text = "Decrypted File";
        }

        private void Background2_DoWork(object? sender, DoWorkEventArgs e)
        {
            getDecryptionFille(pass, salt, fileName, finallFolderTaget);
        }

        private void Background1_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("The File Has Been Successfully Encrypted");
            progressBar1.Value = 0;
            label6.Text = "0%";
            label5.Text = ". . . . . . . . . ";
            button3.Enabled = true;
            button4.Enabled = true;
        }

        private void Background1_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label6.Text = progressBar1.Value + "%";
            label5.Text = "Encryptet File";

        }

        // ==================================================================
        //                             Vars
        // ================================================================== 

        string fileName = string.Empty;
        string folderTarget = string.Empty;
        string finallFolderTaget = string.Empty;
        string pass = string.Empty;
        string salt = "choice any key";
        





        // ==================================================================
        //                          Backgrounds
        // ==================================================================

        private void Background1_DoWork(object? sender, DoWorkEventArgs e)
        {
            getEncryptionFile(pass, salt, fileName, finallFolderTaget);
        }

        // ==================================================================
        //                          The Form
        // ==================================================================

        private void button4_Click(object sender, EventArgs e)
        {
            pass = textBox3.Text;
            button3.Enabled = false;
            button4.Enabled = false;
            progressBar1.Hide();
            progressBar2.Show();
            background2.RunWorkerAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
                textBox1.Text = openFileDialog.FileName;

            }



        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                folderTarget = folderBrowserDialog.SelectedPath;
                //textBox2.Text = folderBrowserDialog.SelectedPath;

            }

            folderTarget += @"\";
            folderTarget += Path.GetFileName(folderTarget);
            
            for (int i = 0; i < folderTarget.Length; i++)
            {
                if (i == 3 && folderTarget[i] == '\\')
                {
                    finallFolderTaget = folderBrowserDialog.SelectedPath;
                    finallFolderTaget += Path.GetFileName(fileName);
                }
                {
                    if (i == 3 && folderTarget[i] != '\\') 
                    {
                        finallFolderTaget = folderBrowserDialog.SelectedPath;
                        finallFolderTaget += @"\";
                        finallFolderTaget += Path.GetFileName(fileName);
                    }
                }
            }

            textBox2.Text = finallFolderTaget;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button4.Enabled = false;
            pass = textBox3.Text;
            progressBar1.Show();
            progressBar2.Hide();
           background1.RunWorkerAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // ==================================================================
        //                          The Actions
        // ==================================================================


        private void getEncryptionFile(string strKey, string strByte, string inputFile, string outFolderTarget)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(strKey);
            byte[] saltByte = Encoding.UTF8.GetBytes(strByte);
            try
            {
                using(var aes = new RijndaelManaged())
                {
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Mode = CipherMode.CBC;
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(keyByte, saltByte, 50000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);


                    using (var stream1 = new FileStream(outFolderTarget, FileMode.Create))
                    {
                        stream1.Write(saltByte, 0, saltByte.Length);
                        using (var ICryptoStream = new CryptoStream(stream1, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            using (var Stream2 = new FileStream(inputFile, FileMode.Open))
                            {
                                byte[] buffer = new byte[1048576];
                                int readAllBytes;
                                while ((readAllBytes = Stream2.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    ICryptoStream.Write(buffer, 0, buffer.Length);
                                    background1.ReportProgress((int)(stream1.Position * 100 / Stream2.Length));
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {


                MessageBox.Show(ex.Message);
            }



        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void getDecryptionFille(string strKey, string strByte, string inputFile, string outFolderTarget)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(strKey);
            byte[] saltByte = Encoding.UTF8.GetBytes(strByte);
            try
            {
              
                using(var aes = new RijndaelManaged())
                {
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Mode = CipherMode.CBC;
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(keyByte, saltByte, 50000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    using (var stream1 = new FileStream(inputFile, FileMode.Open))
                    {
                        stream1.Read(saltByte, 0, saltByte.Length);
                        using(var iCryptoStream = new CryptoStream(stream1, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using(var stream2 = new FileStream(outFolderTarget, FileMode.Create))
                            {
                                byte[] buffer = new byte[1048576];
                                int readAllBytes;
                                while ((readAllBytes = iCryptoStream.Read(buffer, 0, buffer.Length)) > 0 )
                                {
                                    stream2.Write(buffer, 0, buffer.Length);
                                    background2.ReportProgress((int)(stream1.Position * 100 / stream1.Length));
                                }
                            }    
                        }    
                    }
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }



    }
}
