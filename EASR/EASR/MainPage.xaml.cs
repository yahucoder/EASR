using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using SQLite;
using Xamarin.Forms;

namespace Easr
{
    public partial class MainPage : ContentPage
    {
        int count;
        string vError;
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            MessagingCenter.Subscribe<object, string>(this, "UpdateLabel", (s, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    string BackgroundServiceLabel = e;
                });
            });

            LoadSerial();
            
           

        }
        string RCount;

        public void NotificationFirstCall()
        {
            if (AccountStatus.Text == "Activated")
            {
                var minutes = TimeSpan.FromSeconds(10);
                NotificationCenter.Current.NotificationTapped += Current_NotificationTapped;
                Device.StartTimer(minutes, () =>
                {
                    // call your method to check for notifications here
                    try
                    {
                        ReadDatas();
                        if (RCount == "PENDING")
                        {
                            var notification = new NotificationRequest
                            {

                                BadgeNumber = 1,
                                Description = "Emergency Notification",
                                Title = "EAS-R",
                                ReturningData = "Emergency Notification",
                                NotificationId = 1001,

                                Android = new AndroidOptions
                                {
                                    IconSmallName = new AndroidIcon("my_icon"),
                                    IconLargeName = new AndroidIcon("easr"),
                                },
                            };
                            NotificationCenter.Current.Show(notification);

                        }

                    }
                    catch (Exception ex)
                    {

                        vError = ex.Message;
                        DisplayAlert("Alert", vError, "OK");
                    }

                    // Returning true means you want to repeat this timer
                    return true;
                });
            }
            else
            {
                DisplayAlert("ACCOUNT DISABLED", "Your account is disabled, please contact Cedarcrest ICT Unit for activation.", "OK");
            }

        }


        private void Current_NotificationTapped(NotificationEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool answer = await DisplayAlert("ALERT!!!", "Your attention is needed at the ER Unit", "ACCEPT", "DECLINED");
                Debug.WriteLine("Answer: " + answer);
                //string ConAns = answer.ToString();
                if (answer)
                {
                    try
                    {
                        WebClient client = new WebClient();
                        Uri uri = new Uri("https://domain.com/eas/eas-r/EasrResponse.php");
                        NameValueCollection parameters = new NameValueCollection();
                        parameters.Add("FirstGroup", EasrGroup.Text);
                        parameters.Add("SecondGroup", AltEasrGroup.Text);
                        client.UploadValuesCompleted += Client_UploadValuesCompleted;
                        client.UploadValuesAsync(uri, parameters);
                        
                    }
                    catch (Exception ex)
                    {
                        vError = ex.Message;
                        await DisplayAlert("ERROR", vError, "OK");
                    }
                }
                else
                {
                    await DisplayAlert("DECLINED", "Emergency response, DECLINED.", "CLOSE");
                }
                //DisplayAlert("Notification Tabbed", e.ToString(), "OK");
            });
        }

        private void Client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            try
            {
                string resStatus = Encoding.UTF8.GetString(e.Result);
                DisplayAlert("Success", resStatus, "OK");
            }
            catch (Exception)
            {

            }
        }

        public void ReadDatas() //Reades notification from the cloud database
        {
            WebClient ReadNotify = new WebClient();
            Uri uri = new Uri("https://domain.com/eas/eas-r/EasrRead.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("FirstGroup", EasrGroup.Text); 
            parameters.Add("SecondGroup", AltEasrGroup.Text);
            ReadNotify.UploadValuesCompleted += ReadNotify_UploadValuesCompleted;
            ReadNotify.UploadValuesAsync(uri, parameters);
        }

        private void ReadNotify_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            try
            {
                string content = Encoding.UTF8.GetString(e.Result);
                var ContactLoad = JsonConvert.DeserializeObject<ReadNotificationClass>(content);
                RCount = ContactLoad.Status;
            }
            catch (Exception)
            {

            }
        }

        public void LoadSerial() //Load serial from SQLite
        {
            try
            {

                string dpPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Person.db3"); //SQlite
                var db = new SQLiteConnection(dpPath);
                db.CreateTable<Persontable>();
                Persontable persontable = db.FindWithQuery<Persontable>("select AppSerialCode from Persontable where id = 1");
                var Userdetails = persontable;
                easrSerial.Text = Userdetails.AppSerialCode.ToString();
                LoadUser(); //This is a call to load user from the cloud database
                

            }
            catch (Exception ex)
            {
                vError = ex.Message;
                DisplayAlert("NEW ACCOUNT", "This is a new device, kindly setup for usage", "OK");
            }

        }

        public async void LoadUser() //Load user from the cloud database
        {
            try
            {
                if (easrSerial.Text != "")
                {

                    WebClient Loadclient = new WebClient();
                    Uri uri = new Uri("https://domain.com/eas/eas-r/read.php");
                    NameValueCollection parameters = new NameValueCollection();
                    parameters.Add("AppSerial", easrSerial.Text);
                    Loadclient.UploadValuesCompleted += Loadclient_UploadValuesCompleted;
                    Loadclient.UploadValuesAsync(uri, parameters);

                }
                else
                {

                }
            }
            catch (Exception)
            {

                await DisplayAlert("NEW ACCOUNT", "This is a new device, kindly setup for usage", "OK");
            }
        }

        private async void Loadclient_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            try
            {
                string content = Encoding.UTF8.GetString(e.Result);
                var ContactLoad = JsonConvert.DeserializeObject<userdata>(content);
                Fname.Text = ContactLoad.Firstname;
                Specialty.Text = ContactLoad.Specialty;
                EasrMode.Text = ContactLoad.Mode;
                EasrGroup.Text = ContactLoad.Group;
                AltEasrGroup.Text = ContactLoad.AltGroup;
                AccountStatus.Text = ContactLoad.Status;
                NotificationFirstCall();
            }
            catch (Exception ex)
            {
                vError = ex.Message;
                await DisplayAlert("DATABASE ERROR", vError, "OK");
            }
        }

        private async void SetUpBtn_Clicked(object sender, EventArgs e)
        {
             await Navigation.PushAsync(new EasrLogin());


            //PopupNavigation.Instance.PushAsync(new Popup());
        }
    }

    }
