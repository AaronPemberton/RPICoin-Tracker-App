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
using System.Net.Http;
using System.Threading.Tasks;
using Microcharts;
using SkiaSharp;
using Microcharts.Droid;
using System.Threading;
using PerpetualEngine.Storage;

namespace RPICoin_Rewards
{
    [Activity(Label = "StatsActivity")]
    public class StatsActivity : Activity
    {
        public static bool bMN = false;
        public static bool bST = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_stats);

            List<Dictionary<string, int>> mnL = new List<Dictionary<string, int>>();
            List<Dictionary<string, int>> stL = new List<Dictionary<string, int>>();
            List<string> days1 = new List<string>();
            List<int> amounts = new List<int>();

            string address = Intent.GetStringExtra("address") ?? string.Empty;

            SimpleStorage.SetContext(ApplicationContext);
            var storage = SimpleStorage.EditGroup(address);

            string isMN = storage.Get("IsMN");
            string IsStaking = storage.Get("IsStaking");
            string Nickname = storage.Get("Nickname");
            bMN = false;
            bST = false;
            if (isMN == "true")
            {
                bMN = true;
            }
            if (IsStaking == "true")
            {
                bST = true;
            }

            TextView label = (TextView)FindViewById(Resource.Id.textView1);
            label.Text = Nickname + " Stats";

            string response = prepToGetTxs(address);
            if (response == "Successful")
            {
                foreach (string s in days)
                {
                    int mnCount = 0;
                    int stCount = 0;
                    List<string> parseDay = dataPerDay[s];
                    foreach (string s1 in parseDay)
                    {
                        if (s1 == "MN")
                        {
                            mnCount++;
                        }
                        else if (s1 == "ST")
                        {
                            stCount++;
                        }
                    }
                    Dictionary<string, int> tempMN = new Dictionary<string, int>();
                    Dictionary<string, int> tempST = new Dictionary<string, int>();
                    tempMN.Add(s,mnCount);
                    mnL.Add(tempMN);
                    tempST.Add(s, stCount);
                    stL.Add(tempST);
                }

                int least = 0;
                int most = 0;

                days1.Clear();
                amounts.Clear();
                var chartView = FindViewById<ChartView>(Resource.Id.chartViewMN);
                TextView mnText = FindViewById<TextView>(Resource.Id.textViewMNH);
                if (bMN)
                {
                    foreach (Dictionary<string, int> ls in mnL)
                    {
                            string day = ls.Keys.First();
                            string[] parse = day.Split(' ');
                            day = parse[1] + " " + parse[0];
                            days1.Add(day);
                            amounts.Add(ls.Values.First());
                    }

                    least = 0;
                    most = 0;
                    List<int> nums = new List<int>();
                    List<string> colorSeq = new List<string>();
                    foreach (int num in amounts)
                    {
                            if (num < least)
                            {
                                least = num;
                            }
                            if (num > most)
                            {
                                most = num;
                            }
                    }

                    for (int i = 0; i < 8; i++)
                    {
                        if (amounts[i] < amounts[i + 1])
                        {
                            //color red
                            colorSeq.Add("#fc5a65");
                        }
                        else if (amounts[i] > amounts[i + 1])
                        {
                            //color green
                            colorSeq.Add("#49a43d");
                        }
                        else
                        {
                            //color yellow
                            colorSeq.Add("#ebca14");
                        }
                    }


                    var entries = new[]
                    {
                    new Entry((float)amounts[7])
                    {
                        Label = days1[7],
                        ValueLabel = (amounts[7]*137.5).ToString(),
                        Color = SKColor.Parse(colorSeq[7])
                    },
                    new Entry((float)amounts[6])
                    {
                        Label = days1[6],
                        ValueLabel = (amounts[6]*137.5).ToString(),
                        Color = SKColor.Parse(colorSeq[6])
                    },
                    new Entry((float)amounts[5])
                    {
                        Label = days1[5],
                        ValueLabel = (amounts[5]*137.5).ToString(),
                        Color = SKColor.Parse(colorSeq[5])
                    },
                    new Entry((float)amounts[4])
                    {
                        Label = days1[4],
                        ValueLabel = (amounts[4]*137.5).ToString(),
                        Color = SKColor.Parse(colorSeq[4])
                    },
                    new Entry((float)amounts[3])
                    {
                        Label = days1[3],
                        ValueLabel = (amounts[3]*137.5).ToString(),
                        Color = SKColor.Parse(colorSeq[3])
                    },
                    new Entry((float)amounts[2])
                    {
                        Label = days1[2],
                        ValueLabel = (amounts[2]*137.5).ToString(),
                        Color = SKColor.Parse(colorSeq[2])
                    },
                    new Entry((float)amounts[1])
                    {
                        Label = days1[1],
                        ValueLabel = (amounts[1]*137.5).ToString(),
                        Color = SKColor.Parse(colorSeq[1])
                    },
                    new Entry((float)amounts[0])
                    {
                        Label = days1[0],
                        ValueLabel = (amounts[0]*137.5).ToString(),
                        Color = SKColor.Parse(colorSeq[0])
                    }
                };

                    var chart = new LineChart() { Entries = entries };
                    chart.LabelTextSize = 35f;
                    chart.LineMode = LineMode.Straight;
                    chart.LineSize = 5f;
                    chart.MinValue = least;
                    chart.MaxValue = most;
                    chartView.Chart = chart;

                    string status = "Masternode Not Found";
                    string lastSeen = "Masternode Not Found";
                    string lastPaid = "Masternode Not Found";
                    string uptime = "Masternode Not Found";
                    string rank = "Masternode Not Found";

                    string URL = @"https://explorer.rpicoin.com/ext/getmasternodes";
                    var requestTask = client.GetStringAsync(URL);
                    var reply = Task.Run(() => requestTask);
                    string content = reply.Result.ToString();
                    string mnInfo = "";
                    if (content.Contains(address))
                    {
                        string[] parse = content.Split(new string[] { "},{" }, StringSplitOptions.None);
                        foreach(string s in parse)
                        {
                            if (s.Contains(address))
                            {
                                mnInfo = s;
                                break;
                            }
                        }
                        string[] parseInfo = mnInfo.Split(',');
                        foreach(string s1 in parseInfo)
                        {
                            if (s1.Contains("status"))
                            {
                                string temp = s1.Replace("\"", "");
                                string[] tempArray = temp.Split(':');
                                status = tempArray[1];
                            }
                            else if (s1.Contains("lastseen"))
                            {
                                string temp = s1.Replace("\"", "");
                                string[] tempArray = temp.Split(':');
                                lastSeen = tempArray[1];
                            }
                            else if (s1.Contains("lastpaid"))
                            {
                                string temp = s1.Replace("\"", "");
                                string[] tempArray = temp.Split(':');
                                lastPaid = tempArray[1];
                            }
                            else if (s1.Contains("activetime"))
                            {
                                string temp = s1.Replace("\"", "");
                                string[] tempArray = temp.Split(':');
                                uptime = tempArray[1];
                            }
                            else if (s1.Contains("rank"))
                            {
                                string temp = s1.Replace("\"", "");
                                string[] tempArray = temp.Split(':');
                                rank = tempArray[1];
                            }
                        }
                    }
                    DateTime currentTime = DateTime.Now;
                    DateTime lastSeenDate = FromUnixTime(Convert.ToInt64(lastSeen));
                    DateTime lastPaidDate = FromUnixTime(Convert.ToInt64(lastPaid));
                    double seconds = Convert.ToDouble(uptime);
                    seconds = seconds * -1;
                    DateTime uptimeDate = currentTime.AddSeconds(seconds);

                    string format = "dddd, dd MMMM yyyy HH:mm:ss";
                    lastSeen = lastSeenDate.ToString(format);
                    if (lastSeen.Contains("1970"))
                    {
                        lastSeen = "Unknown";
                    }
                    lastPaid = lastPaidDate.ToString(format);
                    if (lastPaid.Contains("1970"))
                    {
                        lastPaid = "Unknown";
                    }

                    //string format1 = "dd'd' hh'h' mm'm' ss's'";
                    TimeSpan uptimeSpan = currentTime.Subtract(uptimeDate);

                    string upDays = uptimeSpan.Days.ToString();
                    string upHours = uptimeSpan.Hours.ToString();
                    string upMinutes = uptimeSpan.Minutes.ToString();
                    string upSeconds = uptimeSpan.Seconds.ToString();
                    uptime = upDays + "days " + upHours + "hours " + upMinutes + "minutes " + upSeconds + "seconds";


                    LinearLayout ll = (LinearLayout)FindViewById(Resource.Id.linearLayout2);

                    TextView statusLabel = new TextView(this) { Id = View.GenerateViewId() };
                    statusLabel.Text = "Status: " + status;
                    statusLabel.TextSize = 15;
                    ll.AddView(statusLabel);

                    TextView rankLabel = new TextView(this) { Id = View.GenerateViewId() };
                    rankLabel.Text = "Current Rank: " + rank;
                    rankLabel.TextSize = 15;
                    ll.AddView(rankLabel);


                    TextView seenLabel = new TextView(this) { Id = View.GenerateViewId() };
                    seenLabel.Text = "Last Seen: " + lastSeen;
                    seenLabel.TextSize = 15;
                    ll.AddView(seenLabel);

                    TextView paidLabel = new TextView(this) { Id = View.GenerateViewId() };
                    paidLabel.Text = "Last Paid: " + lastPaid;
                    paidLabel.TextSize = 15;
                    ll.AddView(paidLabel);

                    TextView uptimeLabel = new TextView(this) { Id = View.GenerateViewId() };
                    uptimeLabel.Text = "Active Time: " + uptime;
                    uptimeLabel.TextSize = 15;
                    ll.AddView(uptimeLabel);

                    //FromUnixTime(long)
                    //add info to layout
                    //status
                    //last seen
                    //last paid
                    //uptime


                    //Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                    //Android.App.AlertDialog alert = dialog.Create();
                    //alert.SetTitle("Info");
                    //alert.SetMessage(amounts.Count().ToString() + least + " " + most);
                    //alert.SetButton("OK", (c, ev) =>
                    //{
                    //    //ok button click task
                    //});
                    //alert.Show();

                }
                else
                {
                    chartView.Visibility = ViewStates.Gone;
                    mnText.Visibility = ViewStates.Gone;
                }

                days1.Clear();
                amounts.Clear();

                var chartView1 = FindViewById<ChartView>(Resource.Id.chartViewST);
                TextView stText = FindViewById<TextView>(Resource.Id.textViewSTH);
                if (bST)
                {
                    foreach (Dictionary<string, int> ls1 in stL)
                    {
                            string day = ls1.Keys.First();
                            string[] parse = day.Split(' ');
                            day = parse[1] + " " + parse[0];
                            days1.Add(day);
                            amounts.Add(ls1.Values.First());
                    }

                    least = 0;
                    most = 0;
                    List<int> nums = new List<int>();
                    List<string> colorSeq = new List<string>();
                    foreach(int num in amounts)
                    {
                            if (num < least)
                            {
                                least = num;
                            }
                            if (num > most)
                            {
                                most = num;
                            }
                    }

                    for(int i = 0; i<8; i++)
                    {
                        if(amounts[i] < amounts[i+1])
                        {
                            //color red
                            colorSeq.Add("#fc5a65");
                        }
                        else if (amounts[i] > amounts[i + 1])
                        {
                            //color green
                            colorSeq.Add("#49a43d");
                        }
                        else
                        {
                            //color yellow
                            colorSeq.Add("#ebca14");
                        }
                    }

                    var entries1 = new[]
                    {
                    new Entry((float)amounts[7])
                    {
                        Label = days1[7],
                        ValueLabel = (amounts[7]*112.5).ToString(),
                        Color = SKColor.Parse(colorSeq[7])
                    },
                    new Entry((float)amounts[6])
                    {
                        Label = days1[6],
                        ValueLabel = (amounts[6]*112.5).ToString(),
                        Color = SKColor.Parse(colorSeq[6])
                    },
                    new Entry((float)amounts[5])
                    {
                        Label = days1[5],
                        ValueLabel = (amounts[5]*112.5).ToString(),
                        Color = SKColor.Parse(colorSeq[5])
                    },
                    new Entry((float)amounts[4])
                    {
                        Label = days1[4],
                        ValueLabel = (amounts[4]*112.5).ToString(),
                        Color = SKColor.Parse(colorSeq[4])
                    },
                    new Entry((float)amounts[3])
                    {
                        Label = days1[3],
                        ValueLabel = (amounts[3]*112.5).ToString(),
                        Color = SKColor.Parse(colorSeq[3])
                    },
                    new Entry((float)amounts[2])
                    {
                        Label = days1[2],
                        ValueLabel = (amounts[2]*112.5).ToString(),
                        Color = SKColor.Parse(colorSeq[2])
                    },
                    new Entry((float)amounts[1])
                    {
                        Label = days1[1],
                        ValueLabel = (amounts[1]*112.5).ToString(),
                        Color = SKColor.Parse(colorSeq[1])
                    },
                    new Entry((float)amounts[0])
                    {
                        Label = days1[0],
                        ValueLabel = (amounts[0]*112.5).ToString(),
                        Color = SKColor.Parse(colorSeq[0])
                    }
                };

                    var chart1 = new LineChart() { Entries = entries1 };
                    chart1.LabelTextSize = 35f;
                    chart1.LineMode = LineMode.Straight;
                    chart1.LineSize = 5f;
                    chart1.MinValue = least;
                    chart1.MaxValue = most;
                    //chart1.BackgroundColor = SKColor.Parse("#FDA2A2");
                    chartView1.Chart = chart1;
                }
                else
                {
                    chartView1.Visibility = ViewStates.Gone;
                    stText.Visibility = ViewStates.Gone;
                }

                decimal currentAmount = -1;
                string currentBalance = getBalance(address).Result;
                try
                {
                    currentAmount = Convert.ToDecimal(currentBalance);
                }
                catch
                {
                    Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog alert = dialog.Create();
                    alert.SetTitle("Info");
                    alert.SetMessage(currentBalance);
                    alert.SetButton("OK", (c, ev) =>
                    {
                        //ok button click task
                    });
                    alert.Show();
                }

                if (currentAmount > -1)
                {
                    List<decimal> balanceTrend= new List<decimal>();
                    balanceTrend.Add(Math.Floor(currentAmount));
                    foreach (string s in days)
                    {
                        if (balancePerDay.ContainsKey(s))
                        {
                            List<string> parseBalance = balancePerDay[s];
                            decimal total = 0;
                            decimal conv = Convert.ToDecimal(0.00000001);
                            string lastTXID = "";
                            foreach(string s1 in parseBalance)
                            {
                                if (s1.Contains("]],"))
                                {
                                    string[] splitEnd = s1.Split(new string[] { "]]" }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] parString = splitEnd[0].Split(',');
                                    string vIn = parString[parString.Length - 2];
                                    string vOut = parString[parString.Length - 1];
                                    string TXID = parString[parString.Length - 3];
                                    if (TXID != lastTXID)
                                    {
                                        lastTXID = TXID;
                                        try
                                        {
                                            decimal d = Convert.ToDecimal(vIn);
                                            d = d * conv;
                                            d = Math.Round(d, 2, MidpointRounding.AwayFromZero);
                                            total = total + d;
                                        }
                                        catch { }
                                        try
                                        {
                                            decimal d = Convert.ToDecimal(vOut);
                                            d = d * conv;
                                            d = Math.Round(d, 2, MidpointRounding.AwayFromZero);
                                            total = total - d;
                                        }
                                        catch { }
                                    }
                                }
                                else
                                {
                                    string[] parString = s1.Split(',');
                                    string vIn = parString[parString.Length - 2];
                                    string vOut = parString[parString.Length - 1];
                                    string TXID = parString[parString.Length - 3];
                                    if (TXID != lastTXID)
                                    {
                                        lastTXID = TXID;
                                        try
                                        {
                                            decimal d = Convert.ToDecimal(vIn);
                                            d = d * conv;
                                            d = Math.Round(d, 2, MidpointRounding.AwayFromZero);
                                            total = total + d;
                                        }
                                        catch { }
                                        try
                                        {
                                            decimal d = Convert.ToDecimal(vOut);
                                            d = d * conv;
                                            d = Math.Round(d, 2, MidpointRounding.AwayFromZero);
                                            total = total - d;
                                        }
                                        catch { }
                                    }
                                }
                            }
                            balanceTrend.Add(total);
                        }
                    }

                    //string rr = "";
                    //foreach (decimal gggg in balanceTrend)
                    //{
                    //    rr += "_" + gggg.ToString();
                    //}

                    //Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                    //Android.App.AlertDialog alert = dialog.Create();
                    //alert.SetTitle("Info");
                    //alert.SetMessage(rr);
                    //alert.SetButton("OK", (c, ev) =>
                    //{
                    //    //ok button click task
                    //});
                    //alert.Show();


                    var chartView2 = FindViewById<ChartView>(Resource.Id.chartViewBL);
                    days1.Clear();
                    foreach (string ls2 in days)
                    {
                        string day = ls2;
                        string[] parse = day.Split(' ');
                        day = parse[1] + " " + parse[0];
                        days1.Add(day);
                    }

                    decimal start = 0;
                    List<decimal> totals = new List<decimal>();
                    int pos = 0;
                    foreach(decimal dd in balanceTrend)
                    {
                        pos++;
                        if(pos == 1)
                        {
                            totals.Add(dd);
                            start = dd;
                        }
                        else
                        {
                            start = start - dd;
                            totals.Add(start);
                        }
                    }
                    totals.Add(0);

                    decimal min = 0;
                    foreach (decimal num in totals)
                    {
                        if (num < min)
                        {
                            min = num;
                        }
                    }

                    List<int> nums = new List<int>();
                    List<string> colorSeq = new List<string>();

                    for (int i = 0; i < 8; i++)
                    {
                        if (totals[i] < totals[i + 1])
                        {
                            //color red
                            colorSeq.Add("#fc5a65");
                        }
                        else if (totals[i] > totals[i + 1])
                        {
                            //color green
                            colorSeq.Add("#49a43d");
                        }
                        else
                        {
                            //color yellow
                            colorSeq.Add("#ebca14");
                        }
                    }

                    var entries2 = new[]
                    {
                    new Entry((float)totals[7])
                    {
                        Label = days1[7],
                        ValueLabel = (totals[7]).ToString(),
                        Color = SKColor.Parse(colorSeq[7])
                    },
                    new Entry((float)totals[6])
                    {
                        Label = days1[6],
                        ValueLabel = (totals[6]).ToString(),
                        Color = SKColor.Parse(colorSeq[6])
                    },
                    new Entry((float)totals[5])
                    {
                        Label = days1[5],
                        ValueLabel = (totals[5]).ToString(),
                        Color = SKColor.Parse(colorSeq[5])
                    },
                    new Entry((float)totals[4])
                    {
                        Label = days1[4],
                        ValueLabel = (totals[4]).ToString(),
                        Color = SKColor.Parse(colorSeq[4])
                    },
                    new Entry((float)totals[3])
                    {
                        Label = days1[3],
                        ValueLabel = (totals[3]).ToString(),
                        Color = SKColor.Parse(colorSeq[3])
                    },
                    new Entry((float)totals[2])
                    {
                        Label = days1[2],
                        ValueLabel = (totals[2]).ToString(),
                        Color = SKColor.Parse(colorSeq[2])
                    },
                    new Entry((float)totals[1])
                    {
                        Label = days1[1],
                        ValueLabel = (totals[1]).ToString(),
                        Color = SKColor.Parse(colorSeq[1])
                    },
                    new Entry((float)totals[0])
                    {
                        Label = days1[0],
                        ValueLabel = (totals[0]).ToString(),
                        Color = SKColor.Parse(colorSeq[0])
                    }
                };

                    var chart2 = new LineChart() { Entries = entries2 };
                    chart2.LabelTextSize = 35f;
                    chart2.LineMode = LineMode.Straight;
                    chart2.LineSize = 5f;
                    chartView2.Chart = chart2;

                }
            }
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        public async Task<string> getBalance(string address)
        {
            string URL = @"https://explorer.rpicoin.com/ext/getaddress/" + address;

            var requestTask = client.GetStringAsync(URL);
            var response = Task.Run(() => requestTask);
            string content = response.Result.ToString();
            string result = "";
            if (content.Contains("address not found"))
            {
                result = "Address Not Found";
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
                                    result = amount;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = "Error getting address info!";
                        }
                        break;
                    }
                }

                return result;
            }
            else
            {
                result = "Error getting explorer data!";
                return result;
            }
        }


        private static readonly HttpClient client = new HttpClient();
        public static string trxs = "";
        public static Dictionary<string, List<string>> dataPerDay = new Dictionary<string, List<string>>();
        public static Dictionary<string, List<string>> balancePerDay = new Dictionary<string, List<string>>();
        List<string> days = new List<string>();

        List<string> todayList = new List<string>();
        List<string> day2List = new List<string>();
        List<string> day3List = new List<string>();
        List<string> day4List = new List<string>();
        List<string> day5List = new List<string>();
        List<string> day6List = new List<string>();
        List<string> day7List = new List<string>();
        List<string> day8List = new List<string>();
        List<string> day9List = new List<string>();
        List<string> todayBalance = new List<string>();
        List<string> day2Balance = new List<string>();
        List<string> day3Balance = new List<string>();
        List<string> day4Balance = new List<string>();
        List<string> day5Balance = new List<string>();
        List<string> day6Balance = new List<string>();
        List<string> day7Balance = new List<string>();
        List<string> day8Balance = new List<string>();

        public string prepToGetTxs(string address)
        {
            string URL = @"https://explorer.rpicoin.com/ext/getaddresstxsajax/?address=" + address;

            var requestTask = client.GetStringAsync(URL);
            var response = Task.Run(() => requestTask);
            string content = response.Result.ToString();
            string result = "";
            int totalTxs = 0;

            if (content.Contains("\"recordsTotal\":0,"))
            {
                result = "No transactions found";
            }
            else
            {
                string[] parse = content.Split(',');
                foreach (string s in parse.Reverse())
                {
                    if (s.Contains("recordsTotal"))
                    {
                        string[] parse1 = s.Split(':');
                        try
                        {
                            int total = Convert.ToInt32(parse1[1]);
                            totalTxs = total;
                        }
                        catch
                        {
                            totalTxs = 0;
                            result = "Error gathering transactions";
                        }
                        break;
                    }
                }
            }

            if (totalTxs > 0)
            {
                days.Clear();
                //List<string> defaultList = new List<string>();
                DateTime currentDate = DateTime.Now;
                string format = "dd MMM yyyy";
                int neg = 0;
                string lastdate = "";
                for (int i = 0; i < 9; i++)
                {
                    DateTime calcDate = currentDate;
                    neg = -i;
                    calcDate = calcDate.AddDays(neg);
                    lastdate = calcDate.ToString(format);
                    days.Add(lastdate);
                    //dataPerDay.Add(lastdate, defaultList);
                }

                List<string> pageInfo = new List<string>();
                string[] parse1 = content.Split(new string[] { "],[" }, StringSplitOptions.None);
                foreach (string s in parse1)
                {
                    pageInfo.Add(s);
                }

                //List<string> pageInfo = getTxs(address, totalTxs);
                string day1 = days[0];
                string day2 = days[1];
                string day3 = days[2];
                string day4 = days[3];
                string day5 = days[4];
                string day6 = days[5];
                string day7 = days[6];
                string day8 = days[7];
                string day9 = days[8];
                todayList.Clear();
                day2List.Clear();
                day3List.Clear();
                day4List.Clear();
                day5List.Clear();
                day6List.Clear();
                day7List.Clear();
                day8List.Clear();
                day9List.Clear();
                todayBalance.Clear();
                day2Balance.Clear();
                day3Balance.Clear();
                day4Balance.Clear();
                day5Balance.Clear();
                day6Balance.Clear();
                day7Balance.Clear();
                day8Balance.Clear();


                foreach (string s in pageInfo)
                {
                    if (s.Contains(day1))
                    {
                        todayBalance.Add(s);
                        {
                            if (s.Contains("13750000000,0"))
                            {
                                todayList.Add("MN");
                            }
                            else if (s.Contains("11250000000,0"))
                            {
                                todayList.Add("ST");
                            }
                        }
                    }
                    else if (s.Contains(day2))
                    {
                        day2Balance.Add(s);
                        if (s.Contains("13750000000,0"))
                        {
                            day2List.Add("MN");
                        }
                        else if (s.Contains("11250000000,0"))
                        {
                            day2List.Add("ST");
                        }
                    }
                    else if (s.Contains(day3))
                    {
                        day3Balance.Add(s);
                        if (s.Contains("13750000000,0"))
                        {
                            day3List.Add("MN");
                        }
                        else if (s.Contains("11250000000,0"))
                        {
                            day3List.Add("ST");
                        }
                    }
                    else if (s.Contains(day4))
                    {
                        day4Balance.Add(s);
                        if (s.Contains("13750000000,0"))
                        {
                            day4List.Add("MN");
                        }
                        else if (s.Contains("11250000000,0"))
                        {
                            day4List.Add("ST");
                        }
                    }
                    else if (s.Contains(day5))
                    {
                        day5Balance.Add(s);
                        if (s.Contains("13750000000,0"))
                        {
                            day5List.Add("MN");
                        }
                        else if (s.Contains("11250000000,0"))
                        {
                            day5List.Add("ST");
                        }
                    }
                    else if (s.Contains(day6))
                    {
                        day6Balance.Add(s);
                        if (s.Contains("13750000000,0"))
                        {
                            day6List.Add("MN");
                        }
                        else if (s.Contains("11250000000,0"))
                        {
                            day6List.Add("ST");
                        }
                    }
                    else if (s.Contains(day7))
                    {
                        day7Balance.Add(s);
                        if (s.Contains("13750000000,0"))
                        {
                            day7List.Add("MN");
                        }
                        else if (s.Contains("11250000000,0"))
                        {
                            day7List.Add("ST");
                        }
                    }
                    else if (s.Contains(day8))
                    {
                        day8Balance.Add(s);
                        if (s.Contains("13750000000,0"))
                        {
                            day8List.Add("MN");
                        }
                        else if (s.Contains("11250000000,0"))
                        {
                            day8List.Add("ST");
                        }
                    }
                    else
                    {
                        day9List.Add("End");
                    }
                }

                if (!day9List.Any())
                {
                    pageInfo.Clear();
                    string results = "";
                    results = getTxs(address, totalTxs, days);
                    string[] parse = results.Split(new string[] { "],[" }, StringSplitOptions.None);
                    foreach (string s in parse)
                    {
                        pageInfo.Add(s);
                    }

                    foreach (string s in pageInfo)
                    {
                        if (s.Contains(day1))
                        {
                            todayBalance.Add(s);
                            {
                                if (s.Contains("13750000000,0"))
                                {
                                    todayList.Add("MN");
                                }
                                else if (s.Contains("11250000000,0"))
                                {
                                    todayList.Add("ST");
                                }
                            }
                        }
                        else if (s.Contains(day2))
                        {
                            day2Balance.Add(s);
                            if (s.Contains("13750000000,0"))
                            {
                                day2List.Add("MN");
                            }
                            else if (s.Contains("11250000000,0"))
                            {
                                day2List.Add("ST");
                            }
                        }
                        else if (s.Contains(day3))
                        {
                            day3Balance.Add(s);
                            if (s.Contains("13750000000,0"))
                            {
                                day3List.Add("MN");
                            }
                            else if (s.Contains("11250000000,0"))
                            {
                                day3List.Add("ST");
                            }
                        }
                        else if (s.Contains(day4))
                        {
                            day4Balance.Add(s);
                            if (s.Contains("13750000000,0"))
                            {
                                day4List.Add("MN");
                            }
                            else if (s.Contains("11250000000,0"))
                            {
                                day4List.Add("ST");
                            }
                        }
                        else if (s.Contains(day5))
                        {
                            day5Balance.Add(s);
                            if (s.Contains("13750000000,0"))
                            {
                                day5List.Add("MN");
                            }
                            else if (s.Contains("11250000000,0"))
                            {
                                day5List.Add("ST");
                            }
                        }
                        else if (s.Contains(day6))
                        {
                            day6Balance.Add(s);
                            if (s.Contains("13750000000,0"))
                            {
                                day6List.Add("MN");
                            }
                            else if (s.Contains("11250000000,0"))
                            {
                                day6List.Add("ST");
                            }
                        }
                        else if (s.Contains(day7))
                        {
                            day7Balance.Add(s);
                            if (s.Contains("13750000000,0"))
                            {
                                day7List.Add("MN");
                            }
                            else if (s.Contains("11250000000,0"))
                            {
                                day7List.Add("ST");
                            }
                        }
                        else if (s.Contains(day8))
                        {
                            day8Balance.Add(s);
                            if (s.Contains("13750000000,0"))
                            {
                                day8List.Add("MN");
                            }
                            else if (s.Contains("11250000000,0"))
                            {
                                day8List.Add("ST");
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(s))
                        {
                                day9List.Add("End");
                        }
                    }
                }

                //string rr = "";
                //rr += "_" + todayList.Count.ToString();
                //rr += "_" + day2List.Count.ToString();
                //rr += "_" + day3List.Count.ToString();
                //rr += "_" + day4List.Count.ToString();
                //rr += "_" + day5List.Count.ToString();
                //rr += "_" + day6List.Count.ToString();
                //rr += "_" + day7List.Count.ToString();
                //rr += "_" + day8List.Count.ToString();
                //rr += "_" + day9List.Count.ToString();

                //Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                //Android.App.AlertDialog alert = dialog.Create();
                //alert.SetTitle("Info");
                //alert.SetMessage(rr);
                //alert.SetButton("OK", (c, ev) =>
                //{
                //    //ok button click task
                //});
                //alert.Show();

                dataPerDay.Clear();
                balancePerDay.Clear();
                dataPerDay.Add(day1, todayList);
                dataPerDay.Add(day2, day2List);
                dataPerDay.Add(day3, day3List);
                dataPerDay.Add(day4, day4List);
                dataPerDay.Add(day5, day5List);
                dataPerDay.Add(day6, day6List);
                dataPerDay.Add(day7, day7List);
                dataPerDay.Add(day8, day8List);
                dataPerDay.Add(day9, day9List);
                balancePerDay.Add(day1, todayBalance);
                balancePerDay.Add(day2, day2Balance);
                balancePerDay.Add(day3, day3Balance);
                balancePerDay.Add(day4, day4Balance);
                balancePerDay.Add(day5, day5Balance);
                balancePerDay.Add(day6, day6Balance);
                balancePerDay.Add(day7, day7Balance);
                balancePerDay.Add(day8, day8Balance);


                return "Successful";
            }
            else
            {
                return result;
            }
        }

        int page = 0;

        public string getTxs(string address, int totalTxs, List<string> days)
        {
            bool finish = false;
            decimal numOfPages = Convert.ToDecimal(totalTxs);
            numOfPages = Math.Ceiling(numOfPages / 100);
            int pages = Convert.ToInt32(numOfPages);
            string results = "";

            while (!finish)
            {
                page++;
                string URL = "";
                if (page <= pages)
                {
                    int pageRange = page * 100;
                    URL = @"https://explorer.rpicoin.com/ext/getaddresstxsajax/?address=" + address + "&start=" + pageRange.ToString();
                    var requestTask = client.GetStringAsync(URL);
                    var response = Task.Run(() => requestTask);
                    string content = response.Result.ToString();

                    if (content.Contains("\"recordsTotal\":0,"))
                    {
                        finish = true;
                    }
                    else
                    {
                        bool found = false;
                        foreach (string da in days)
                        {
                            if (content.Contains(da))
                            {
                                results += "],[" + content;
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            finish = true;
                        }
                    }
                }
            }
            return results;
        }
    }
}