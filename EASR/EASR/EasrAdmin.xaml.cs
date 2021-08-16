using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Easr
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EasrAdmin : ContentPage
    {
        public EasrAdmin()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            GroupPicker2.IsVisible = false;
            AltGroup.IsVisible = false;
            LoadSerial();            
            AccStatus();
        }
        string GroupSelected, ResGroupSelect, Group2Selected, vError;
      

        public void AccStatus()
        {
            if(easrSerial.Text != "")
            {
                RegisterBtn.IsVisible = false;
                UpdateBtn.IsVisible = true;
            }
            else if (easrSerial.Text == "")
            {
                RegisterBtn.IsVisible = true;
                UpdateBtn.IsVisible = false;
            }
            else
            {
                RegisterBtn.IsVisible = true;
                UpdateBtn.IsVisible = false;
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

        private async void Loadclient_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e) // This returns the value from loaduser function
        {
            try
            {
                string content = Encoding.UTF8.GetString(e.Result);
                var ContactLoad = JsonConvert.DeserializeObject<userdata>(content);
                Fname.Text = ContactLoad.Firstname;
                SSpecialty.Text = ContactLoad.Specialty;
                ResGroupSelect = ContactLoad.Mode;
                GroupSelected = ContactLoad.Group;
                Group2Selected = ContactLoad.AltGroup;
                AccountStatus.Text = ContactLoad.Status;
                if (ResGroupSelect == "Single Group")
                {
                    GroupMode.SelectedIndex = 0;
                }
                else if (ResGroupSelect == "Dual Group")
                {
                    GroupMode.SelectedIndex = 1;
                }

                if (GroupSelected == "Trauma")
                {
                    GroupPicker.SelectedIndex = 0;
                }
                else if (GroupSelected == "Paediatrics-Trauma")
                {
                    GroupPicker.SelectedIndex = 1;
                }
                else if (GroupSelected == "Anesthetic")
                {
                    GroupPicker.SelectedIndex = 2;
                }
                else if (GroupSelected == "Paediatrics-Anesthetic")
                {
                    GroupPicker.SelectedIndex = 3;
                }
                else if (GroupSelected == "Supports")
                {
                    GroupPicker.SelectedIndex = 4;
                }
                else if (GroupSelected == "Security")
                {
                    GroupPicker.SelectedIndex = 5;
                }
                else if (GroupSelected == "None")
                {
                    GroupPicker.SelectedIndex = 6;
                }


                if (Group2Selected == "Single Group")
                {
                    GroupPicker2.SelectedIndex = 0;
                }
                else if (Group2Selected == "Double Group")
                {
                    GroupPicker2.SelectedIndex = 1;
                }

                if (Group2Selected == "Trauma")
                {
                    GroupPicker2.SelectedIndex = 0;
                }
                else if (Group2Selected == "Paediatrics-Trauma")
                {
                    GroupPicker.SelectedIndex = 1;
                }
                else if (GroupSelected == "Anesthetic")
                {
                    GroupPicker2.SelectedIndex = 2;
                }
                else if (Group2Selected == "Paediatrics-Anesthetic")
                {
                    GroupPicker2.SelectedIndex = 3;
                }
                else if (Group2Selected == "Supports")
                {
                    GroupPicker2.SelectedIndex = 4;
                }
                else if (Group2Selected == "Security")
                {
                    GroupPicker2.SelectedIndex = 5;
                }
                else if (Group2Selected == "None")
                {
                    GroupPicker2.SelectedIndex = 6;
                }

                Console.WriteLine(ContactLoad.Firstname);
            }
            catch(Exception ex)
            {
                vError = ex.Message;
                await DisplayAlert("DATABASE ERROR", vError, "OK");
            }
        }

        private void GroupPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex == 0)
            {
                GroupSelected = "Trauma";
            }
            else if (selectedIndex == 1)
            {
                GroupSelected = "Paediatrics-Trauma";
            }
            else if (selectedIndex == 2)
            {
                GroupSelected = "Anesthetic";
            }
            else if (selectedIndex == 3)
            {
                GroupSelected = "Paediatrics-Anesthetic";
            }
            else if (selectedIndex == 4)
            {
                GroupSelected = "Supports";
            }
            else if (selectedIndex == 5)
            {
                GroupSelected = "Security";
            }
            else if (selectedIndex == 6)
            {
                GroupSelected = "None";
            }

        }

        private async void RegisterBtn_Clicked(object sender, EventArgs e) // Registration for the first time, this saves to both the local DB and the Cloud DB
        {

            try
            {
                var unixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                easrSerial.Text = unixTimestamp.ToString();
                WebClient client = new WebClient();
                Uri uri = new Uri("https://domain.com/eas/eas-r/create.php");

                string dpPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Person.db3"); //SQlite
                var db = new SQLiteConnection(dpPath);
                db.CreateTable<Persontable>();
                Persontable tbl = new Persontable();
                string ConvertSerial = tbl.AppSerialCode.ToString();
                if (ResGroupSelect == "Single Group")
                {
                    NameValueCollection parameters = new NameValueCollection();
                    parameters.Add("Fullname", Fname.Text);
                    parameters.Add("StaffSpecialty", SSpecialty.Text);
                    parameters.Add("StaffGroupSelect", ResGroupSelect);
                    parameters.Add("eGroupSelected", GroupSelected);
                    parameters.Add("eGroup2Selected", "None");
                    parameters.Add("AppSerial", easrSerial.Text);
                    client.UploadValuesCompleted += Client_UploadValuesCompleted;
                    client.UploadValuesAsync(uri, parameters);
                    var UserDetails = new Persontable()
                    {
                        AppSerialCode = Int32.Parse(easrSerial.Text)
                    };
                    db.Insert(UserDetails);
                    await DisplayAlert("SUCCESS", Fname.Text + " Account created successfully, please activate account", "OK");
                }
                else
                {
                    NameValueCollection parameters = new NameValueCollection();
                    parameters.Add("Fullname", Fname.Text);
                    parameters.Add("StaffSpecialty", SSpecialty.Text);
                    parameters.Add("StaffGroupSelect", ResGroupSelect);
                    parameters.Add("eGroupSelected", GroupSelected);
                    parameters.Add("eGroup2Selected", Group2Selected);
                    parameters.Add("AppSerial", easrSerial.Text);
                    client.UploadValuesCompleted += Client_UploadValuesCompleted;
                    client.UploadValuesAsync(uri, parameters);
                    var UserDetails = new Persontable()
                    {
                        AppSerialCode = Int32.Parse(easrSerial.Text)
                    };
                    db.Insert(UserDetails);
                    await DisplayAlert("SUCCESS", Fname.Text + " Account created successfully, please activate account.", "OK");
                }
                }
                catch (Exception ex)
                {
                    vError = ex.Message;
                    await DisplayAlert("ERROR", "Unable to create " + Fname.Text + " account. Please make sure you are connected to the internet, and try again. "+ vError, "OK");
                }
         
        }

        private void Client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e) //This is the return value from registration
        {
            string resStatus = Encoding.UTF8.GetString(e.Result);
             DisplayAlert("NOTIFICATION", resStatus, "OK");
            
        }

        private void UpdateBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                WebClient Updateclient = new WebClient();
                Uri uri = new Uri("https://domain.com/eas/eas-r/update.php");
                NameValueCollection parameters = new NameValueCollection();
                parameters.Add("AppSerial", easrSerial.Text);
                parameters.Add("Fullname", Fname.Text);
                parameters.Add("StaffSpecialty", SSpecialty.Text);
                parameters.Add("StaffGroupSelect", ResGroupSelect);
                parameters.Add("eGroupSelected", GroupSelected);
                parameters.Add("eGroup2Selected", Group2Selected);
                Updateclient.UploadValuesCompleted += Updateclient_UploadValuesCompleted;
                Updateclient.UploadValuesAsync(uri, parameters);

            }
            catch (Exception)
            {

            }
            
        }

        private void Updateclient_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e) //This is the return value from updating profile
        {
            string resStatus = Encoding.UTF8.GetString(e.Result);
            DisplayAlert("Success", resStatus, "OK");
        }

        private async void GroupPicker2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex == 0)
            {
                Group2Selected = "Trauma";
                if(GroupSelected == Group2Selected)
                {
                    await DisplayAlert("ERROR", "Sorry, you cannot belong to two same group at the same time", "OK");
                }
                
            }
            else if (selectedIndex == 1)
            {
                Group2Selected = "Paediatrics-Trauma";
                if (GroupSelected == Group2Selected)
                {
                    await DisplayAlert("ERROR", "Sorry, you cannot belong to two same group at the same time", "OK");
                }
            }
            else if (selectedIndex == 2)
            {
                Group2Selected = "Anesthetic";
                if (GroupSelected == Group2Selected)
                {
                    await DisplayAlert("ERROR", "Sorry, you cannot belong to two same group at the same time", "OK");
                }
            }
            else if (selectedIndex == 3)
            {
                Group2Selected = "Paediatrics-Anesthetic";
                if (GroupSelected == Group2Selected)
                {
                    await DisplayAlert("ERROR", "Sorry, you cannot belong to two same group at the same time", "OK");
                }
            }
            else if (selectedIndex == 4)
            {
                Group2Selected = "Supports";
                if (GroupSelected == Group2Selected)
                {
                    await DisplayAlert("ERROR", "Sorry, you cannot belong to two same group at the same time", "OK");
                }
            }
            else if (selectedIndex == 5)
            {
                Group2Selected = "Security";
                if (GroupSelected == Group2Selected)
                {
                    await DisplayAlert("ERROR", "Sorry, you cannot belong to two same group at the same time", "OK");
                }
            }
            else if (selectedIndex == 6)
            {
                Group2Selected = "None";
            }

        }

        private void GroupMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;
            if (selectedIndex == 0)
            {
                ResGroupSelect = "Single Group";
                GroupPicker2.IsVisible = false;
                AltGroup.IsVisible = false;
                GroupPicker.IsVisible = true;
            }
            else if (selectedIndex == 1)
            {
                ResGroupSelect = "Dual Group";
                AltGroup.IsVisible = true;
                GroupPicker2.IsVisible = true;
                GroupPicker.IsVisible = true;
            }
        }
    }
}