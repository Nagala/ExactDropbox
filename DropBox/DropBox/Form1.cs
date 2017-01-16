using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nemiro.OAuth;
using Nemiro.OAuth.LoginForms;
using System.IO;
using ExactOnline.Client.Models;
using ExactOnline.Client.Sdk;
using ExactOnline.Client.Sdk.Controllers;
using DotNetOpenAuth.OAuth2;
namespace DropBox
{
    public partial class Form1 : Form
    {
        
      
        private string CurrentPath = "/";
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.AccessToken))
            {
                this.GetAccessToken();
            }
        }
        
       
       

        private void GetAccessToken()
        {
            var login = new DropboxLogin("k10o7ajlmmtp8qj","3g69wza3v700xw0");
            login.Owner = this;
            login.ShowDialog();
            if (login.IsSuccessfully)
            {
                Properties.Settings.Default.AccessToken = login.AccessToken.Value;
                Properties.Settings.Default.Save();
            }
            else
            {
            MessageBox.Show("error..");
            }
        }
        private void GetFiles()
        {
            OAuthUtility.GetAsync
                (
                "https://api.dropbox.com/1/metadata/auto/",
                new HttpParameterCollection
                 {
                     {"path",this.CurrentPath },
                     {"access_token",Properties.Settings.Default.AccessToken }
                 },
                callback: GetFiles_Result
                );
        }
        private void GetFiles_Result(RequestResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<RequestResult>(GetFiles_Result), result);
                return;
            }
            if (result.StatusCode == 200)
            {

            }
            else 
            {
                MessageBox.Show("Error..");
            }
 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (String.IsNullOrEmpty(textBox1.Text))
            //{ return; }
            OAuthUtility.PutAsync
                (
                "https://content.dropboxapi.com/1/files_put/auto/",
                new HttpParameterCollection
                {
            {"access_token",Properties.Settings.Default.AccessToken},
            {"path",Path.Combine(this.CurrentPath,Path.GetFileName(ofd.FileName)).Replace("\\","/")},
            {"overwrite", "true"},
            {"autorename","true"},
            {ofd.OpenFile()}
                },
                callback: Upload_Result
                );
        }
       
          
       
          
        private void Upload_Result(RequestResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<RequestResult>(Upload_Result),result);
                return;
            }

            if (result.StatusCode == 200)
            {
                this.GetFiles();
                Application.Exit();
                
        }
            else 
            {
                if (result["error"].HasValue)
                {
                    MessageBox.Show(result["error"].ToString());
                }
                else
                {
                    MessageBox.Show(result.ToString());
                }
            }
        }
        OpenFileDialog ofd = new OpenFileDialog();
        private void button2_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
            }
        }
    }
}
