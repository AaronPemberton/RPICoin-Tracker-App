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
    [Activity(Label = "EditActivity")]
    public class EditActivity : Activity
    {
        public static string addy = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string address = Intent.GetStringExtra("address") ?? string.Empty;
            addy = address;

            SimpleStorage.SetContext(ApplicationContext);
            SetContentView(Resource.Layout.activity_edit);

            Button backButton = (Button)FindViewById(Resource.Id.buttonEditBack);
            backButton.Click += editPageBack;

            ImageButton QRButton = (ImageButton)FindViewById(Resource.Id.imageButtonQREdit);
            QRButton.Click += EditScanQR;

            MobileBarcodeScanner.Initialize(Application);

            Button saveButton = (Button)FindViewById(Resource.Id.buttonEditSave);
            saveButton.Click += EditSaveData;

            Button deleteButton = (Button)FindViewById(Resource.Id.buttonDeleteAddy);
            deleteButton.Click += deleteAddy;

            SimpleStorage.SetContext(ApplicationContext);
            var storage = SimpleStorage.EditGroup(address);

            string isMN = storage.Get("IsMN");
            string IsStaking = storage.Get("IsStaking");
            string Nickname = storage.Get("Nickname");

            EditText pubAddy = (EditText)FindViewById(Resource.Id.editTextPubEdit);
            pubAddy.Text = address;
            EditText pubNick = (EditText)FindViewById(Resource.Id.editTextNickEdit);
            pubNick.Text = Nickname;
            CheckBox mnChk = (CheckBox)FindViewById(Resource.Id.checkBoxMNEdit);
            CheckBox stChk = (CheckBox)FindViewById(Resource.Id.checkBoxStakeEdit);

            if (isMN == "true")
            {
                mnChk.Checked = true;
            }
            else
            {
                mnChk.Checked = false;
            }
            if (IsStaking == "true")
            {
                stChk.Checked = true;
            }
            else
            {
                stChk.Checked = false;
            }

        }

        public void deleteAddy(Object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(addy))
            {
                var addressData = SimpleStorage.EditGroup(addy);
                addressData.Delete("Nickname");
                addressData.Delete("IsMN");
                addressData.Delete("IsStaking");
                List<string> currentAddList = new List<string>();
                List<string> newAddList = new List<string>();
                var storage = SimpleStorage.EditGroup("Addresses");
                currentAddList = storage.Get("AddressList", "").Split(',').ToList();
                foreach (string s in currentAddList)
                {
                    if (s != addy)
                    {
                        newAddList.Add(s);
                    }
                }

                string newList = "";
                foreach (string s1 in newAddList)
                {
                    newList += s1 + ",";
                }
                newList = newList.TrimEnd(',');
                storage.Delete("AddressList");
                storage.Put("AddressList", newList);
            }
        }


        public void editPageBack(Object sender, EventArgs e)
        {
            Finish();
        }

        public async void EditScanQR(Object sender, EventArgs e)
        {
            var scanner = new MobileBarcodeScanner();
            var result = await scanner.Scan();

            if (result != null)
            {
                EditText etPub = (EditText)FindViewById(Resource.Id.editTextPubEdit);
                string text = result.ToString();
                if (text.Contains("rpicoin:"))
                {
                    text = text.Replace("rpicoin:", "");
                }
                if (text.Contains("?"))
                {
                    string[] parse = text.Split('?');
                    text = parse[0];
                }
                etPub.Text = text;
            }
        }

        public void EditSaveData(Object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(addy))
            {
                var addressData = SimpleStorage.EditGroup(addy);
                addressData.Delete("Nickname");
                addressData.Delete("IsMN");
                addressData.Delete("IsStaking");
                List<string> currentAddList = new List<string>();
                List<string> newAddList = new List<string>();
                var storage1 = SimpleStorage.EditGroup("Addresses");
                currentAddList = storage1.Get("AddressList", "").Split(',').ToList();
                foreach (string s in currentAddList)
                {
                    if (s != addy)
                    {
                        newAddList.Add(s);
                    }
                }

                string newList = "";
                foreach (string s1 in newAddList)
                {
                    newList += s1 + ",";
                }
                newList = newList.TrimEnd(',');
                storage1.Delete("AddressList");
                storage1.Put("AddressList", newList);

                var intent = new Intent();
                SetResult(Android.App.Result.Ok, intent);
                Finish();

            }

            EditText etPub = (EditText)FindViewById(Resource.Id.editTextPubEdit);
            EditText etNick = (EditText)FindViewById(Resource.Id.editTextNickEdit);
            CheckBox ckMN = (CheckBox)FindViewById(Resource.Id.checkBoxMNEdit);
            CheckBox ckST = (CheckBox)FindViewById(Resource.Id.checkBoxStakeEdit);

            var storage = SimpleStorage.EditGroup("Addresses");
            var addressList = storage.Get("AddressList", "").Split(',').ToList();
            bool notEmpty = false;
            bool existing = false;
            foreach (string s in addressList)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    notEmpty = true;
                    break;
                }
            }
            if (notEmpty)
            {
                if (!addressList.Contains(etPub.Text))
                {
                    string value = "";
                    foreach (string s in addressList)
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
