using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using PerpetualEngine.Storage;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using Android.Webkit;

namespace RPICoin_Rewards
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static Dictionary<string, int> labelDictionary = new Dictionary<string, int>();
        public static Dictionary<string, int> TotalsDictionary = new Dictionary<string, int>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SimpleStorage.SetContext(ApplicationContext);

            SetContentView(Resource.Layout.activity_main);

            WebView webView = (WebView)FindViewById(Resource.Id.webView1);
            WebSettings webSettings = webView.Settings;
            webSettings.JavaScriptEnabled = true;

            string HTMLText = "<html>" + "<body><script src=\"https://widgets.coingecko.com/coingecko-coin-ticker-widget.js\"></script>" +
                "<coingecko-coin-ticker-widget currency=\"usd\" coin-id=\"rpicoin\" locale=\"en\"></coingecko-coin-ticker-widget></body>" +
                              "</html>";

            webView.LoadData(HTMLText, "text/html", null);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            string status = MNInfo();
            if (status != "Error")
            {
                status = status.Replace("}", "");
                status = status.Replace("{", "");

                string[] parse = status.Split(',');
                foreach (string s in parse)
                {
                    if (s.Contains("total"))
                    {
                        string[] temp = s.Split(':');
                        TextView totalMN = (TextView)FindViewById(Resource.Id.textViewMNTotal);
                        totalMN.Text = "Total Masternodes: " + temp[1];
                    }
                    else if (s.Contains("stable"))
                    {
                        string[] temp = s.Split(':');
                        TextView stableMN = (TextView)FindViewById(Resource.Id.textViewStable);
                        stableMN.Text = "Stable: " + temp[1];
                    }
                    else if (s.Contains("enabled"))
                    {
                        string[] temp = s.Split(':');
                        TextView enabledMN = (TextView)FindViewById(Resource.Id.textViewEnabled);
                        enabledMN.Text = "Enabled: " + temp[1];
                    }
                    else if (s.Contains("inqueue"))
                    {
                        string[] temp = s.Split(':');
                        TextView inQueueMN = (TextView)FindViewById(Resource.Id.textViewInQueue);
                        inQueueMN.Text = "In Queue: " + temp[1];
                    }
                    else if (s.Contains("ipv4"))
                    {
                        string[] temp = s.Split(':');
                        TextView ipv4MN = (TextView)FindViewById(Resource.Id.textViewIPv4);
                        ipv4MN.Text = "IPv4: " + temp[1];
                    }
                    else if (s.Contains("ipv6"))
                    {
                        string[] temp = s.Split(':');
                        TextView ipv6MN = (TextView)FindViewById(Resource.Id.textViewIPv6);
                        ipv6MN.Text = "IPv6: " + temp[1];
                    }
                    else if (s.Contains("onion"))
                    {
                        string[] temp = s.Split(':');
                        TextView onionMN = (TextView)FindViewById(Resource.Id.textViewOnion);
                        onionMN.Text = "Onion: " + temp[1];
                    }
                }
            }
            else
            {
                TextView totalMN = (TextView)FindViewById(Resource.Id.textViewMNTotal);
                totalMN.Text = "Total Masternodes: ERROR GATHERING DATA";
            }

            LinearLayout ll = (LinearLayout)FindViewById(Resource.Id.linearLayout1);

            List<Button> buttonList = new List<Button>();
            List<LinearLayout> rows = new List<LinearLayout>();

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
                labelDictionary.Clear();
                for (int j = 0; j < (addresses.Count + 3); j++)
                {
                    LinearLayout row = new LinearLayout(this);
                    row.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                    rows.Add(row);
                    row.Id = (j + 1);
                }

                for (int i = 1; i < (addresses.Count + 2); i++)
                {

                    RelativeLayout rl = new RelativeLayout(this);
                    rl.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                    rl.SetPadding(5, 0, 5, 0);
                    if (i % 2 == 0)
                    {
                        rl.SetBackgroundColor(Android.Graphics.Color.LightGray);
                    }
                    else
                    {
                        rl.SetBackgroundColor(Android.Graphics.Color.White);
                    }

                    if (i == 1)
                    {
                        TextView label1 = new TextView(this) { Id = View.GenerateViewId() };
                        label1.Text = "Address Nickname";
                        label1.TextSize = 12;
                        label1.SetTextColor(Android.Graphics.Color.Black);
                        RelativeLayout.LayoutParams lbp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        lbp.AddRule(LayoutRules.AlignParentLeft);
                        lbp.AddRule(LayoutRules.CenterVertical);
                        rl.AddView(label1, lbp);

                        TextView label3 = new TextView(this) { Id = View.GenerateViewId() };
                        label3.Text = "Stats";
                        label3.TextSize = 12;
                        label3.SetTextColor(Android.Graphics.Color.Black);
                        label3.SetPadding(0, 0, 30, 0);
                        RelativeLayout.LayoutParams lbp3 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        lbp3.AddRule(LayoutRules.AlignParentRight);
                        lbp3.AddRule(LayoutRules.CenterVertical);
                        rl.AddView(label3, lbp3);

                        TextView label2 = new TextView(this) { Id = View.GenerateViewId() };
                        label2.Text = "Balance";
                        label2.TextSize = 12;
                        label2.SetTextColor(Android.Graphics.Color.Black);
                        label2.SetPadding(0, 0, 100, 0);
                        label2.TextAlignment = TextAlignment.ViewStart;
                        RelativeLayout.LayoutParams lbp2 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        lbp2.AddRule(LayoutRules.LeftOf, label3.Id);
                        lbp2.AddRule(LayoutRules.CenterVertical);
                        rl.AddView(label2, lbp2);

                    }
                    else
                    {
                        var addressData = SimpleStorage.EditGroup(addresses[i - 2]);

                        string mnNick = addressData.Get("Nickname");
                        TextView mnLabel = new TextView(this) { Id = View.GenerateViewId() };
                        mnLabel.Tag =
                        mnLabel.Text = mnNick;
                        mnLabel.TextSize = 20;
                        mnLabel.SetTextColor(Android.Graphics.Color.Black);
                        RelativeLayout.LayoutParams mnp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        mnp.AddRule(LayoutRules.AlignParentLeft);
                        mnp.AddRule(LayoutRules.CenterVertical);
                        rl.AddView(mnLabel, mnp);

                        ImageButton iButton = new ImageButton(this) { Id = View.GenerateViewId() };
                        iButton.Tag = addresses[i - 2];
                        //iButton.Background = null;
                        iButton.SetImageResource(Resource.Drawable.stats);
                        RelativeLayout.LayoutParams bnp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        bnp.AddRule(LayoutRules.AlignParentRight);
                        bnp.AddRule(LayoutRules.CenterVertical);
                        iButton.Click += delegate (object sender, EventArgs e)
                        {
                            btnStatsClick(sender, e, iButton.Tag.ToString());
                        };
                        rl.AddView(iButton, bnp);

                        TextView blLabel = new TextView(this) { Id = View.GenerateViewId() };
                        labelDictionary.Add(addresses[i - 2], blLabel.Id);
                        blLabel.Text = "Contacting explorer...";
                        blLabel.TextSize = 15;
                        blLabel.SetTextColor(Android.Graphics.Color.Goldenrod);
                        blLabel.SetPadding(10, 0, 10, 0);
                        blLabel.TextAlignment = TextAlignment.ViewEnd;
                        RelativeLayout.LayoutParams blp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        blp.AddRule(LayoutRules.LeftOf, iButton.Id);
                        blp.AddRule(LayoutRules.CenterVertical);
                        rl.AddView(blLabel, blp);

                    }

                    rows[i - 1].AddView(rl);
                    ll.AddView(rows[i - 1]);
                }

                TotalsDictionary.Clear();
                RelativeLayout rl3 = new RelativeLayout(this);
                rl3.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                rl3.SetPadding(5, 0, 5, 0);
                rl3.SetBackgroundColor(Android.Graphics.Color.Black);
                TextView grandTotal = new TextView(this) { Id = View.GenerateViewId() };
                TotalsDictionary.Add("total", grandTotal.Id);
                grandTotal.Text = "Total RPI";
                grandTotal.SetPadding(10, 0, 10, 0);
                grandTotal.TextSize = 15;
                grandTotal.SetTextColor(Android.Graphics.Color.White);
                grandTotal.TextAlignment = TextAlignment.ViewStart;
                RelativeLayout.LayoutParams lbp4 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                lbp4.AddRule(LayoutRules.AlignParentStart);
                lbp4.AddRule(LayoutRules.CenterVertical);
                rl3.AddView(grandTotal, lbp4);
                TextView USDValue = new TextView(this) { Id = View.GenerateViewId() };
                TotalsDictionary.Add("value", USDValue.Id);
                USDValue.Text = "USD Value";
                USDValue.SetPadding(10, 0, 10, 0);
                USDValue.TextSize = 15;
                USDValue.SetTextColor(Android.Graphics.Color.White);
                USDValue.TextAlignment = TextAlignment.ViewStart;
                RelativeLayout.LayoutParams lbp5 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                lbp5.AddRule(LayoutRules.AlignParentEnd);
                lbp5.AddRule(LayoutRules.CenterVertical);
                rl3.AddView(USDValue, lbp5);
                rows[rows.Count - 2].AddView(rl3);
                ll.AddView(rows[rows.Count - 2]);


                RelativeLayout rl2 = new RelativeLayout(this);
                rl2.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                rl2.SetPadding(5, 0, 5, 0);
                Button button2 = new Button(this) { Id = View.GenerateViewId() };
                button2.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                button2.Text = "Add New Address";
                button2.TextAlignment = TextAlignment.Center;
                RelativeLayout.LayoutParams blp2 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                blp2.AddRule(LayoutRules.CenterHorizontal);
                blp2.AddRule(LayoutRules.CenterVertical);
                button2.Click += btnAddClick;

                rl2.AddView(button2, blp2);
                rows[rows.Count - 1].AddView(rl2);
                ll.AddView(rows[rows.Count - 1]);
            }
            else
            {
                labelDictionary.Clear();
                for (int j = 0; j < 3; j++)
                {
                    LinearLayout row = new LinearLayout(this);
                    row.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                    rows.Add(row);
                    row.Id = (j + 1);
                }

                for (int i = 1; i < 3; i++)
                {

                    RelativeLayout rl = new RelativeLayout(this);
                    rl.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                    rl.SetPadding(5, 0, 5, 0);
                    rl.SetBackgroundColor(Android.Graphics.Color.White);

                    if (i == 1)
                    {
                        TextView label1 = new TextView(this) { Id = View.GenerateViewId() };
                        label1.Text = "Address Nickname";
                        label1.TextSize = 12;
                        label1.SetTextColor(Android.Graphics.Color.Black);
                        RelativeLayout.LayoutParams lbp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        lbp.AddRule(LayoutRules.AlignParentLeft);
                        lbp.AddRule(LayoutRules.CenterVertical);
                        rl.AddView(label1, lbp);

                        TextView label3 = new TextView(this) { Id = View.GenerateViewId() };
                        label3.Text = "Stats";
                        label3.TextSize = 12;
                        label3.SetTextColor(Android.Graphics.Color.Black);
                        label3.SetPadding(0, 0, 30, 0);
                        RelativeLayout.LayoutParams lbp3 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        lbp3.AddRule(LayoutRules.AlignParentRight);
                        lbp3.AddRule(LayoutRules.CenterVertical);
                        rl.AddView(label3, lbp3);

                        TextView label2 = new TextView(this) { Id = View.GenerateViewId() };
                        label2.Text = "Balance";
                        label2.TextSize = 12;
                        label2.SetTextColor(Android.Graphics.Color.Black);
                        label2.SetPadding(0, 0, 100, 0);
                        label2.TextAlignment = TextAlignment.ViewStart;
                        RelativeLayout.LayoutParams lbp2 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        lbp2.AddRule(LayoutRules.LeftOf, label3.Id);
                        lbp2.AddRule(LayoutRules.CenterVertical);
                        rl.AddView(label2, lbp2);

                    }
                    else
                    {
                        TextView mnLabel = new TextView(this) { Id = View.GenerateViewId() };
                        mnLabel.Text = "No saved addresses." + System.Environment.NewLine + "Please select \"Add New Address\" below.";
                        mnLabel.TextSize = 20;
                        mnLabel.Gravity = GravityFlags.CenterHorizontal;
                        mnLabel.SetTextColor(Android.Graphics.Color.Maroon);
                        RelativeLayout.LayoutParams mnp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        mnp.AddRule(LayoutRules.CenterHorizontal);
                        mnp.AddRule(LayoutRules.CenterVertical);
                        rl.AddView(mnLabel, mnp);
                    }


                    rows[i - 1].AddView(rl);
                    ll.AddView(rows[i - 1]);
                }

                RelativeLayout rl2 = new RelativeLayout(this);
                rl2.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                rl2.SetPadding(5, 0, 5, 0);
                Button button2 = new Button(this) { Id = View.GenerateViewId() };
                button2.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                button2.Text = "Add New Address";
                button2.TextAlignment = TextAlignment.Center;
                RelativeLayout.LayoutParams blp2 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                blp2.AddRule(LayoutRules.CenterHorizontal);
                blp2.AddRule(LayoutRules.CenterVertical);
                button2.Click += btnAddClick;

                rl2.AddView(button2, blp2);
                rows[rows.Count - 1].AddView(rl2);
                ll.AddView(rows[rows.Count - 1]);
            }
        }

        private static readonly HttpClient client = new HttpClient();

        public string MNInfo()
        {
            string URL = @"https://explorer.rpicoin.com/api/getmasternodecount";

            var requestTask = client.GetStringAsync(URL);
            var response = Task.Run(() => requestTask);
            string content = response.Result.ToString();
            if (content.Contains("total"))
            {
                return content;
            }
            else
            {
                return "Error";
            }
        }

        public async Task<string> getBalance(string address)
        {
            string URL = @"https://explorer.rpicoin.com/ext/getaddress/" + address;

            var requestTask = client.GetStringAsync(URL);
            var response = Task.Run(() => requestTask);
            string content = response.Result.ToString();
            string result = "";
            if (content.Contains("address not found"))
            {
                result = address + ",Address Not Found";
                return result;
            }
            else if (content.Contains("balance"))
            {
                string[] parse = content.Split(',');
                foreach (string s in parse)
                {
                    if (s.Contains("address"))
                    {
                        string[] parse2 = s.Split(':');
                        string addy = parse2[1];
                        addy = addy.Replace("\"", "");
                        if (addy == address)
                        {
                            foreach (string s1 in parse)
                            {
                                if (s1.Contains("balance"))
                                {
                                    string[] parse3 = s1.Split(':');
                                    string amount = parse3[1];
                                    amount = amount.Replace("\"", "");
                                    result = address + "," + amount;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = address + ",Error getting address info!";
                        }
                        break;
                    }
                }

                return result;
            }
            else
            {
                result = address + ",Error getting explorer data!";
                return result;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            List<string> keys = new List<string>();
            keys = labelDictionary.Keys.ToList();
            decimal total = 0;
            foreach (string key in keys)
            {
                string labelBalance = getBalance(key).Result;
                string[] result = labelBalance.Split(',');
                string address = result[0];
                string balance = labelBalance.Replace(address + ",", "");
                TextView label = (TextView)FindViewById(labelDictionary[address]);
                if (!balance.Contains("Not Found") || !balance.Contains("Error"))
                {
                    label.SetTextColor(Android.Graphics.Color.ForestGreen);
                    try
                    {
                        total = total + Convert.ToDecimal(balance);
                    }
                    catch { }
                }
                label.Text = balance;
            }
            try
            {
                TextView totText = (TextView)FindViewById(TotalsDictionary["total"]);
                totText.Text = "Total RPI: " + total.ToString();
            }
            catch
            {

            }
            if (total > 0)
            {
                TextView valText = (TextView)FindViewById(TotalsDictionary["value"]);

                string URL = @"https://api.coingecko.com/api/v3/simple/price?ids=rpicoin&vs_currencies=usd";

                var requestTask = client.GetStringAsync(URL);
                var response = Task.Run(() => requestTask);
                string content = response.Result.ToString();
                if (content.Contains("usd\":"))
                {
                    string[] parse = content.Split(':');
                    string value = parse[parse.Length - 1];
                    value = value.Replace("}", "");
                    if (value.Contains("e-"))
                    {
                        string[] parse1 = value.Split('-');
                        string value2 = parse1[0];
                        value2 = value2.Replace("e", "");
                        decimal value3 = Convert.ToDecimal(value2);
                        int exp = Convert.ToInt32(parse1[parse1.Length - 1]);
                        for (int i = 0; i < exp; i++)
                        {
                            value3 = value3 * (decimal)0.1;
                        }
                        value = value3.ToString();
                    }
                    decimal calc = total * Convert.ToDecimal(value);
                    calc = Math.Round(calc, 2);
                    value = calc.ToString();
                    valText.Text = "USD Value: $" + value;
                }
                else
                {
                    valText.Text = "USD Value: Error obtaining price";
                }

            }
        }

        public void btnStatsClick(Object sender, EventArgs e, string address)
        {
            var activity = new Intent(this, typeof(StatsActivity));
            activity.PutExtra("address", address);
            StartActivity(activity);
            //int labelID = labelDictionary[address];
            //TextView label = (TextView)FindViewById(labelID);
            //label.Text = "Image button clicked.";
        }

        public void btnAddClick(Object sender, EventArgs e)
        {
            Intent myIntent = new Intent(this, typeof(AddNewActivity));
            this.StartActivityForResult(myIntent, 1);

            //Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            //Android.App.AlertDialog alert = dialog.Create();
            //alert.SetTitle("Info");
            //alert.SetMessage("The button was clicked");
            //alert.SetButton("OK", (c,ev) =>
            //{
            //    //ok button click task
            //});
            //alert.Show();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 1 || resultCode == Result.Ok)
            {
                if (resultCode == Result.Ok)
                {
                    labelDictionary.Clear();

                    SimpleStorage.SetContext(ApplicationContext);

                    SetContentView(Resource.Layout.activity_main);

                    WebView webView = (WebView)FindViewById(Resource.Id.webView1);
                    WebSettings webSettings = webView.Settings;
                    webSettings.JavaScriptEnabled = true;

                    string HTMLText = "<html>" + "<body><script src=\"https://widgets.coingecko.com/coingecko-coin-ticker-widget.js\"></script>" +
                        "<coingecko-coin-ticker-widget currency=\"usd\" coin-id=\"rpicoin\" locale=\"en\"></coingecko-coin-ticker-widget></body>" +
                                      "</html>";

                    webView.LoadData(HTMLText, "text/html", null);

                    Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                    SetSupportActionBar(toolbar);

                    string status = MNInfo();
                    if (status != "Error")
                    {
                        status = status.Replace("}", "");
                        status = status.Replace("{", "");

                        string[] parse = status.Split(',');
                        foreach (string s in parse)
                        {
                            if (s.Contains("total"))
                            {
                                string[] temp = s.Split(':');
                                TextView totalMN = (TextView)FindViewById(Resource.Id.textViewMNTotal);
                                totalMN.Text = "Total Masternodes: " + temp[1];
                            }
                            else if (s.Contains("stable"))
                            {
                                string[] temp = s.Split(':');
                                TextView stableMN = (TextView)FindViewById(Resource.Id.textViewStable);
                                stableMN.Text = "Stable: " + temp[1];
                            }
                            else if (s.Contains("enabled"))
                            {
                                string[] temp = s.Split(':');
                                TextView enabledMN = (TextView)FindViewById(Resource.Id.textViewEnabled);
                                enabledMN.Text = "Enabled: " + temp[1];
                            }
                            else if (s.Contains("inqueue"))
                            {
                                string[] temp = s.Split(':');
                                TextView inQueueMN = (TextView)FindViewById(Resource.Id.textViewInQueue);
                                inQueueMN.Text = "In Queue: " + temp[1];
                            }
                            else if (s.Contains("ipv4"))
                            {
                                string[] temp = s.Split(':');
                                TextView ipv4MN = (TextView)FindViewById(Resource.Id.textViewIPv4);
                                ipv4MN.Text = "IPv4: " + temp[1];
                            }
                            else if (s.Contains("ipv6"))
                            {
                                string[] temp = s.Split(':');
                                TextView ipv6MN = (TextView)FindViewById(Resource.Id.textViewIPv6);
                                ipv6MN.Text = "IPv6: " + temp[1];
                            }
                            else if (s.Contains("onion"))
                            {
                                string[] temp = s.Split(':');
                                TextView onionMN = (TextView)FindViewById(Resource.Id.textViewOnion);
                                onionMN.Text = "Onion: " + temp[1];
                            }
                        }
                    }
                    else
                    {
                        TextView totalMN = (TextView)FindViewById(Resource.Id.textViewMNTotal);
                        totalMN.Text = "Total Masternodes: ERROR GATHERING DATA";
                    }

                    LinearLayout ll = (LinearLayout)FindViewById(Resource.Id.linearLayout1);

                    List<Button> buttonList = new List<Button>();
                    List<LinearLayout> rows = new List<LinearLayout>();

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

                    //Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                    //Android.App.AlertDialog alert = dialog.Create();
                    //alert.SetTitle("Info");
                    //alert.SetMessage(addressList.Count.ToString());
                    //alert.SetButton("OK", (c, ev) =>
                    //{
                    //    //ok button click task
                    //});
                    //alert.Show();

                    if (addresses.Any())
                    {
                        labelDictionary.Clear();
                        for (int j = 0; j < (addresses.Count + 3); j++)
                        {
                            LinearLayout row = new LinearLayout(this);
                            row.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                            rows.Add(row);
                            row.Id = (j + 1);
                        }

                        for (int i = 1; i < (addresses.Count + 2); i++)
                        {

                            RelativeLayout rl = new RelativeLayout(this);
                            rl.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                            rl.SetPadding(5, 0, 5, 0);
                            if (i % 2 == 0)
                            {
                                rl.SetBackgroundColor(Android.Graphics.Color.LightGray);
                            }
                            else
                            {
                                rl.SetBackgroundColor(Android.Graphics.Color.White);
                            }

                            if (i == 1)
                            {
                                TextView label1 = new TextView(this) { Id = View.GenerateViewId() };
                                label1.Text = "Address Nickname";
                                label1.TextSize = 12;
                                label1.SetTextColor(Android.Graphics.Color.Black);
                                RelativeLayout.LayoutParams lbp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                                lbp.AddRule(LayoutRules.AlignParentLeft);
                                lbp.AddRule(LayoutRules.CenterVertical);
                                rl.AddView(label1, lbp);

                                TextView label3 = new TextView(this) { Id = View.GenerateViewId() };
                                label3.Text = "Stats";
                                label3.TextSize = 12;
                                label3.SetTextColor(Android.Graphics.Color.Black);
                                label3.SetPadding(0, 0, 30, 0);
                                RelativeLayout.LayoutParams lbp3 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                                lbp3.AddRule(LayoutRules.AlignParentRight);
                                lbp3.AddRule(LayoutRules.CenterVertical);
                                rl.AddView(label3, lbp3);

                                TextView label2 = new TextView(this) { Id = View.GenerateViewId() };
                                label2.Text = "Balance";
                                label2.TextSize = 12;
                                label2.SetTextColor(Android.Graphics.Color.Black);
                                label2.SetPadding(0, 0, 100, 0);
                                label2.TextAlignment = TextAlignment.ViewStart;
                                RelativeLayout.LayoutParams lbp2 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                                lbp2.AddRule(LayoutRules.LeftOf, label3.Id);
                                lbp2.AddRule(LayoutRules.CenterVertical);
                                rl.AddView(label2, lbp2);

                            }
                            else
                            {
                                var addressData = SimpleStorage.EditGroup(addresses[i - 2]);

                                string mnNick = addressData.Get("Nickname");
                                TextView mnLabel = new TextView(this) { Id = View.GenerateViewId() };
                                mnLabel.Text = mnNick;
                                mnLabel.TextSize = 20;
                                mnLabel.SetTextColor(Android.Graphics.Color.Black);
                                RelativeLayout.LayoutParams mnp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                                mnp.AddRule(LayoutRules.AlignParentLeft);
                                mnp.AddRule(LayoutRules.CenterVertical);
                                rl.AddView(mnLabel, mnp);

                                ImageButton iButton = new ImageButton(this) { Id = View.GenerateViewId() };
                                iButton.SetImageResource(Resource.Drawable.stats);
                                //iButton.Background = null;
                                iButton.Tag = addresses[i - 2];
                                RelativeLayout.LayoutParams bnp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                                bnp.AddRule(LayoutRules.AlignParentRight);
                                bnp.AddRule(LayoutRules.CenterVertical);
                                iButton.Click += delegate (object sender, EventArgs e)
                                {
                                    btnStatsClick(sender, e, iButton.Tag.ToString());
                                };
                                rl.AddView(iButton, bnp);

                                TextView blLabel = new TextView(this) { Id = View.GenerateViewId() };
                                labelDictionary.Add(addresses[i - 2], blLabel.Id);
                                blLabel.Text = "Contacting explorer...";
                                blLabel.TextSize = 15;
                                blLabel.SetTextColor(Android.Graphics.Color.Goldenrod);
                                blLabel.SetPadding(10, 0, 10, 0);
                                blLabel.TextAlignment = TextAlignment.ViewEnd;
                                RelativeLayout.LayoutParams blp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                                blp.AddRule(LayoutRules.LeftOf, iButton.Id);
                                blp.AddRule(LayoutRules.CenterVertical);
                                rl.AddView(blLabel, blp);
                            }


                            rows[i - 1].AddView(rl);
                            ll.AddView(rows[i - 1]);
                        }

                        TotalsDictionary.Clear();
                        RelativeLayout rl3 = new RelativeLayout(this);
                        rl3.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                        rl3.SetPadding(5, 0, 5, 0);
                        rl3.SetBackgroundColor(Android.Graphics.Color.Black);
                        TextView grandTotal = new TextView(this) { Id = View.GenerateViewId() };
                        TotalsDictionary.Add("total", grandTotal.Id);
                        grandTotal.Text = "Total RPI";
                        grandTotal.SetPadding(10, 0, 10, 0);
                        grandTotal.TextSize = 15;
                        grandTotal.SetTextColor(Android.Graphics.Color.White);
                        grandTotal.TextAlignment = TextAlignment.ViewStart;
                        RelativeLayout.LayoutParams lbp4 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        lbp4.AddRule(LayoutRules.AlignParentStart);
                        lbp4.AddRule(LayoutRules.CenterVertical);
                        rl3.AddView(grandTotal, lbp4);
                        TextView USDValue = new TextView(this) { Id = View.GenerateViewId() };
                        TotalsDictionary.Add("value", USDValue.Id);
                        USDValue.Text = "USD Value";
                        USDValue.SetPadding(10, 0, 10, 0);
                        USDValue.TextSize = 15;
                        USDValue.SetTextColor(Android.Graphics.Color.White);
                        USDValue.TextAlignment = TextAlignment.ViewStart;
                        RelativeLayout.LayoutParams lbp5 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        lbp5.AddRule(LayoutRules.AlignParentEnd);
                        lbp5.AddRule(LayoutRules.CenterVertical);
                        rl3.AddView(USDValue, lbp5);
                        rows[rows.Count - 2].AddView(rl3);
                        ll.AddView(rows[rows.Count - 2]);



                        RelativeLayout rl2 = new RelativeLayout(this);
                        rl2.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                        rl2.SetPadding(5, 0, 5, 0);
                        Button button2 = new Button(this) { Id = View.GenerateViewId() };
                        button2.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                        button2.Text = "Add New Address";
                        button2.TextAlignment = TextAlignment.Center;
                        RelativeLayout.LayoutParams blp2 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        blp2.AddRule(LayoutRules.CenterHorizontal);
                        blp2.AddRule(LayoutRules.CenterVertical);
                        button2.Click += btnAddClick;

                        rl2.AddView(button2, blp2);
                        rows[rows.Count - 1].AddView(rl2);
                        ll.AddView(rows[rows.Count - 1]);
                    }
                    else
                    {
                        labelDictionary.Clear();
                        for (int j = 0; j < 3; j++)
                        {
                            LinearLayout row = new LinearLayout(this);
                            row.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                            rows.Add(row);
                            row.Id = (j + 1);
                        }

                        for (int i = 1; i < 3; i++)
                        {

                            RelativeLayout rl = new RelativeLayout(this);
                            rl.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                            rl.SetPadding(5, 0, 5, 0);
                            rl.SetBackgroundColor(Android.Graphics.Color.White);

                            if (i == 1)
                            {
                                TextView label1 = new TextView(this) { Id = View.GenerateViewId() };
                                label1.Text = "Address Nickname";
                                label1.TextSize = 12;
                                label1.SetTextColor(Android.Graphics.Color.Black);
                                RelativeLayout.LayoutParams lbp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                                lbp.AddRule(LayoutRules.AlignParentLeft);
                                lbp.AddRule(LayoutRules.CenterVertical);
                                rl.AddView(label1, lbp);

                                TextView label3 = new TextView(this) { Id = View.GenerateViewId() };
                                label3.Text = "Stats";
                                label3.TextSize = 12;
                                label3.SetTextColor(Android.Graphics.Color.Black);
                                label3.SetPadding(0, 0, 30, 0);
                                RelativeLayout.LayoutParams lbp3 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                                lbp3.AddRule(LayoutRules.AlignParentRight);
                                lbp3.AddRule(LayoutRules.CenterVertical);
                                rl.AddView(label3, lbp3);

                                TextView label2 = new TextView(this) { Id = View.GenerateViewId() };
                                label2.Text = "Balance";
                                label2.TextSize = 12;
                                label2.SetTextColor(Android.Graphics.Color.Black);
                                label2.SetPadding(0, 0, 100, 0);
                                label2.TextAlignment = TextAlignment.ViewStart;
                                RelativeLayout.LayoutParams lbp2 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                                lbp2.AddRule(LayoutRules.LeftOf, label3.Id);
                                lbp2.AddRule(LayoutRules.CenterVertical);
                                rl.AddView(label2, lbp2);

                            }
                            else
                            {
                                TextView mnLabel = new TextView(this) { Id = View.GenerateViewId() };
                                mnLabel.Text = "No saved addresses." + System.Environment.NewLine + "Please select \"Add New Address\" below.";
                                mnLabel.TextSize = 20;
                                mnLabel.Gravity = GravityFlags.CenterHorizontal;
                                mnLabel.SetTextColor(Android.Graphics.Color.Maroon);
                                RelativeLayout.LayoutParams mnp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                                mnp.AddRule(LayoutRules.CenterHorizontal);
                                mnp.AddRule(LayoutRules.CenterVertical);
                                rl.AddView(mnLabel, mnp);
                            }


                            rows[i - 1].AddView(rl);
                            ll.AddView(rows[i - 1]);
                        }

                        RelativeLayout rl2 = new RelativeLayout(this);
                        rl2.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                        rl2.SetPadding(5, 0, 5, 0);
                        Button button2 = new Button(this) { Id = View.GenerateViewId() };
                        button2.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                        button2.Text = "Add New Address";
                        button2.TextAlignment = TextAlignment.Center;
                        RelativeLayout.LayoutParams blp2 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                        blp2.AddRule(LayoutRules.CenterHorizontal);
                        blp2.AddRule(LayoutRules.CenterVertical);
                        button2.Click += btnAddClick;

                        rl2.AddView(button2, blp2);
                        rows[rows.Count - 1].AddView(rl2);
                        ll.AddView(rows[rows.Count - 1]);

                    }
                }
            }
            else
            {
                Intent myIntent = new Intent(this, typeof(Refresh));
                this.StartActivityForResult(myIntent, 1);
            }
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                Intent myIntent = new Intent(this, typeof(SettingsActivity));
                this.StartActivityForResult(myIntent, 1);

            }
            if (id == Resource.Id.action_refresh)
            {
                Intent myIntent = new Intent(this, typeof(Refresh));
                this.StartActivityForResult(myIntent, 1);
            }
            if (id == Resource.Id.action_explorer)
            {
                var uri = Android.Net.Uri.Parse("https://explorer.rpicoin.com");
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}
