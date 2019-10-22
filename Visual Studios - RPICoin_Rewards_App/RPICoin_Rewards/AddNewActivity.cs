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
    [Activity(Label = "Activity1")]
    public class AddNewActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SimpleStorage.SetContext(ApplicationContext);
            SetContentView(Resource.Layout.activity_Add);

            LinearLayout ll = (LinearLayout)FindViewById(Resource.Id.linearLayout1);

            Button backButton = (Button)FindViewById(Resource.Id.buttonBack);
            backButton.Click += addPageBack;

            ImageButton QRButton = (ImageButton)FindViewById(Resource.Id.imageButtonQR);
            QRButton.Click += ScanQR;

            MobileBarcodeScanner.Initialize(Application);

            Button saveButton = (Button)FindViewById(Resource.Id.buttonSave);
            saveButton.Click += SaveData;
        }

        public void addPageBack(Object sender, EventArgs e)
        {
            Finish();
        }

        public async void ScanQR(Object sender, EventArgs e)
        {
            var scanner = new MobileBarcodeScanner();
            var result = await scanner.Scan();

            if (result != null)
            {
                EditText etPub = (EditText)FindViewById(Resource.Id.editTextPub);
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