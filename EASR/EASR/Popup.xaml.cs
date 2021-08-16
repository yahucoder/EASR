using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;
using System.IO;
using SQLite;
using System.Collections.Specialized;

namespace Easr
{
   // [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Popup //: ContentPage
    {
        public Popup()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        string EncryptPass;
        public string Username { get; set; }
        public string Password { get; set; }
        

        private async void SetUpLogin_Clicked(object sender, EventArgs e)
        {

            try
            {
                

                if (!string.IsNullOrWhiteSpace(Uname.Text) && !string.IsNullOrWhiteSpace(PWord.Text))
                {
                    byte[] hashBytes = Encoding.Unicode.GetBytes(PWord.Text);
                    using (System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
                    {
                        hashBytes = md5.ComputeHash(hashBytes);
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (byte b in hashBytes)
                        stringBuilder.Append(b.ToString("X2"));
                        EncryptPass = stringBuilder.ToString();
                    }

                    WebClient Loginclient = new WebClient();
                    Uri uri = new Uri("https://domain.com/eas/eas-r/login.php");
                    NameValueCollection PValue = new NameValueCollection();
                    PValue.Add("Username", Uname.Text);
                    PValue.Add("Password", EncryptPass);
                    Loginclient.UploadValuesCompleted += Loginclient_UploadValuesCompleted; ;
                    Loginclient.UploadValuesAsync(uri, PValue);


                   // await DisplayAlert("ERROR", EncryptPass +" "+ Uname.Text, "OK");

                }
                else
                {

                    await DisplayAlert("ERROR", "Username and password cannot be empty.", "OK");
                }
            }
            catch (Exception)
            {

            }
           
            
        }

        private async void Loginclient_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            string resStatus = Encoding.UTF8.GetString(e.Result);
            string res = "Successful";
            if (string.Compare(res, resStatus) == 0)
            {
                Console.WriteLine(res);
                await DisplayAlert("TEst", resStatus, "OK");
                await Navigation.PushAsync(new EasrAdmin());
                await CloseAllPopup();
            }
            
                //PopupNavigation.Instance.PopAsync(new AdminPage());
            

        }

        private async Task CloseAllPopup()
        {
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}