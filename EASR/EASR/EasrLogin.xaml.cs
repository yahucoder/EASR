using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Easr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EasrLogin : ContentPage
	{
		public EasrLogin ()
		{
			InitializeComponent ();
			NavigationPage.SetHasNavigationBar(this, false);
		}

        string EncryptPass, LogStat;
        private async void Login_Clicked(object sender, EventArgs e)
        {
            try
            {
                LogStat = "";

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
                    Uri uri = new Uri("https://domian.com/eas/eas-r/login.php");
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
            try
            {
                string resStatus = Encoding.UTF8.GetString(e.Result);
                var ContactLoad = JsonConvert.DeserializeObject<LoginClass>(resStatus);
                LogStat = ContactLoad.Status.ToString();
                if (LogStat == "ACTIVATED")
                {
                    await Navigation.PushAsync(new EasrAdmin());
                }
                else
                {
                    await DisplayAlert("ERROR", "Unable to Login", "OK");
                }
            }
            catch(Exception)
            {
                await DisplayAlert("LOGIN ERROR", "Invalid Login Details", "OK");
            }
           
           
        }
    }
}