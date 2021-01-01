using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace WV2Example
{
    public partial class Form1 : Form
    {
        private readonly int modeSSL = 1;
        public Form1()
        {
            InitializeComponent();
            Resize += new EventHandler(Form_Resize);
            // webView.NavigationStarting += EnsureHttps;
            InitializeAsync();
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            webView.Size = this.ClientSize - new Size(webView.Location);
            btnGo.Left = this.ClientSize.Width - btnGo.Width;
            tbUrl.Width = btnGo.Left - tbUrl.Left;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tbUrl.Text = "localhost.loc";
            SetProtocolInURL(modeSSL);
            webView.Source = new Uri(tbUrl.Text);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SetProtocolInURL(modeSSL);
            if (webView != null && webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.Navigate(tbUrl.Text);
            }
        }

        string SetProtocolInURL(int enableSSL = 0)
        {
            var protonorm = @"http://";
            var protossl = @"https://";

            tbUrl.Text = tbUrl.Text.Replace(protossl, "");
            tbUrl.Text = tbUrl.Text.Replace(protonorm, "");

            if (enableSSL == 1)
            {
                if (!tbUrl.Text.StartsWith(protossl))
                {
                    tbUrl.Text = protossl + tbUrl.Text;
                }
            }
            else
            {
                if (!tbUrl.Text.StartsWith(protonorm))
                {
                    tbUrl.Text = protonorm + tbUrl.Text;
                }
            }

            return tbUrl.Text;
        }

        async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.WebMessageReceived += UpdateAddressBar;

            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.addEventListener(\'message\', event => console.log(event.data));");
        }

        void UpdateAddressBar(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String uri = args.TryGetWebMessageAsString();
            tbUrl.Text = uri;
            webView.CoreWebView2.PostWebMessageAsString(uri);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                string projectDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\.."));
                string file = projectDir + @"/assets/pages/mytemplate.html";

                if (webView != null && webView.CoreWebView2 != null)
                {
                    tbUrl.Text = "about:blank";
                    webView.CoreWebView2.NavigateToString(File.ReadAllText(file));
                }
            }
            else
            {
                if (webView != null && webView.CoreWebView2 != null)
                {
                    tbUrl.Text = "localhost.loc";
                    SetProtocolInURL(modeSSL);
                    webView.CoreWebView2.Navigate(tbUrl.Text);
                }
            }
        }

        private void tbUrl_TextChanged(object sender, EventArgs e)
        {
            return;
        }

        //void EnsureHttps(object sender, CoreWebView2NavigationStartingEventArgs args)
        //{
        //    String uri = args.Uri;
        //    if (!uri.StartsWith("https://"))
        //    {
        //        webView.CoreWebView2.ExecuteScriptAsync($"alert('{uri} is not safe, try an https link')");
        //        args.Cancel = true;
        //    }
        //}
    }
}
