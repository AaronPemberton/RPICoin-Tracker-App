using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PerpetualEngine.Storage;
using ZXing;
using ZXing.Mobile;

namespace RPICoin_Rewards
{
    [Activity(Label = "SettingsActivity")]
    public class SettingsActivity : Activity
    {
        public static string currentAddy = "";
        public static Dictionary<string, string> addyAndNicks = new Dictionary<string, string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SimpleStorage.SetContext(ApplicationContext);
            SetContentView(Resource.Layout.activity_settings);

            Button clearButton = (Button)FindViewById(Resource.Id.buttonClear);
            clearButton.Click += ClearData;

            Button editButton = (Button)FindViewById(Resource.Id.buttonEdit);
            editButton.Click += EditData;

            addyAndNicks.Clear();

            Spinner nicknameSpinner = (Spinner)FindViewById(Resource.Id.spinnerSelectNickname);
            List<string> nicknames = new List<string>();
            nicknames.Clear();

            var storage = SimpleStorage.EditGroup("Addresses");
            var addressList = storage.Get("AddressList", "").Split(',').ToList();
            List<string> addresses = new List<string>();
            foreach (string s in addressList)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    addresses.Add(s);
                }
            }
            if (addresses.Any())
            {
                nicknames.Add("Select an address to edit");
                foreach(string s in addresses)
                {
                    var addressData = SimpleStorage.EditGroup(s);
                    string nickname = addressData.Get("Nickname");
                    nicknames.Add(nickname);
                    addyAndNicks.Add(nickname, s);
                }
            }
            else
            {
                nicknames.Add("No stored addresses");
            }


            nicknameSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, nicknames);
            nicknameSpinner.Adapter = adapter;
        }

        public void EditData(Object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(currentAddy))
            {
                var activity = new Intent(this, typeof(EditActivity));
                activity.PutExtra("address", currentAddy);
                StartActivityForResult(activity, 1);
            }
        }

        public void ClearData(Object sender, EventArgs e)
        {
            var storage = SimpleStorage.EditGroup("Addresses");
            var addressList = storage.Get("AddressList", "").Split(',').ToList();
            foreach (string s in addressList)
            {
                var addressData = SimpleStorage.EditGroup(s);
                addressData.Delete("Nickname");
                addressData.Delete("IsMN");
                addressData.Delete("IsStaking");
            }
            storage.Delete("AddressList");
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Android.App.Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Android.App.Result.Ok)
            {
                var intent = new Intent();
                SetResult(Android.App.Result.Ok, intent);
                Finish();

            }
        }



                    private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            Button button = (Button)FindViewById(Resource.Id.buttonEdit);
            if(e.Position != 0)
            {
                button.Enabled = true;
                string key = spinner.GetItemAtPosition(e.Position).ToString();
                currentAddy = addyAndNicks[key];
                //string toast = string.Format("The address is {0}", spinner.GetItemAtPosition(e.Position));
                //Toast.MakeText(this, toast, ToastLength.Long).Show();
            }
            else
            {
                button.Enabled = false;
                currentAddy = "";
            }
        }

        public void addPageBack(Object sender, EventArgs e)
        {
            Finish();
        }


        public void SaveData(Object sender, EventArgs e)
        {
            EditText etPub = (EditText)FindViewById(Resource.Id.editTextPub);
            EditText etNick = (EditText)FindViewById(Resource.Id.editTextNick);
            CheckBox ckMN = (CheckBox)FindViewById(Resource.Id.checkBoxMN);
            CheckBox ckST = (CheckBox)FindViewById(Resource.Id.checkBoxStake);

            var storage = SimpleStorage.EditGroup("Addresses");
            var addressList = storage.Get("AddressList", "").Split(',').ToList();
            bool notEmpty = false;
            bool existing = false;
            foreach(string s in addressList)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    notEmpty = true;
                    break;
                }
            }
            if(notEmpty)
            {
                if (!addressList.Contains(etPub.Text))
                {
                    string value = "";
                    foreach(string s in addressList)
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            value += s + ",";
                        }
                    }
                    value += etPub.Text;
                    storage.Put("AddressList", value);
                }
                else
                {
                    existing = true;
                }
            }
            else
            {
                string value = etPub.Text;
                storage.Put("AddressList", value);
            }

            if (!existing)
            {
                var AddressData = SimpleStorage.EditGroup(etPub.Text);
                AddressData.Put("Nickname", etNick.Text);
                string ismn = "";
                if (ckMN.Checked)
                {
                    ismn = "true";
                }
                else
                {
                    ismn = "false";
                }
                string isst = "";
                if (ckST.Checked)
                {
                    isst = "true";
                }
                else
                {
                    isst = "false";
                }
                AddressData.Put("IsMN", ismn);
                AddressData.Put("IsStaking", isst);

                var intent = new Intent();
                SetResult(Android.App.Result.Ok, intent);
                Finish();
            }
            else
            {
                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = dialog.Create();
                alert.SetTitle("Error");
                alert.SetMessage("This Address is already stored in this app. Delete the current instance before adding the address.");
                alert.SetButton("OK", (c, ev) =>
                {
                    //ok button click task
                });
                alert.Show();
            }

        }
    }
}