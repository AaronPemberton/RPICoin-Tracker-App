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

namespace RPICoin_Rewards
{
    [Activity(Label = "StatsActivity")]
    public class StatsActivity_old : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_stats);
            List<List<string>> mnL = new List<List<string>>();
            List<List<string>> stL = new List<List<string>>();
            List<string> days1 = new List<string>();
            List<string> amounts = new List<string>();

            string address = Intent.GetStringExtra("address") ?? string.Empty;
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
                    List<string> tempMN = new List<string>();
                    List<string> tempST = new List<string>();
                    tempMN.Add(s);
                    tempMN.Add(mnCount.ToString());
                    mnL.Add(tempMN);
                    tempST.Add(s);
                    tempST.Add(stCount.ToString());
                    stL.Add(tempST);

                }

                //string bbb = "";
                //foreach(List<string> bb in mnL)
                //{
                //    foreach(string b in bb)
                //    {
                //        bbb += "___" + b;
                //    }
                //}
                //Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                //Android.App.AlertDialog alert = dialog.Create();
                //alert.SetTitle("Info");
                //alert.SetMessage(bbb);
                //alert.SetButton("OK", (c, ev) =>
                //{
                //    //ok button click task
                //});
                //alert.Show();

                days1.Clear();
                amounts.Clear();

                foreach (List<string> ls in mnL)
                {
                    string day = ls[0];
                    string[] parse = day.Split(' ');
                    day = parse[1] + " " + parse[0];
                    days1.Add(day);
                    amounts.Add(ls[1]);
                }

                var entries = new[]
                {
                    new Entry((float)Convert.ToInt32(amounts[0]))
                    {
                        Label = days1[0],
                        ValueLabel = (Convert.ToInt32(amounts[0])*137.5).ToString(),
                        Color = SKColor.Parse("#266489")
                    },
                    new Entry((float)Convert.ToInt32(amounts[1]))
                    {
                        Label = days1[1],
                        ValueLabel = (Convert.ToInt32(amounts[1])*137.5).ToString(),
                        Color = SKColor.Parse("#68B9C0")
                    },
                    new Entry((float)Convert.ToInt32(amounts[2]))
                    {
                        Label = days1[2],
                        ValueLabel = (Convert.ToInt32(amounts[2])*137.5).ToString(),
                        Color = SKColor.Parse("#90D585")
                    },
                    new Entry((float)Convert.ToInt32(amounts[3]))
                    {
                        Label = days1[3],
                        ValueLabel = (Convert.ToInt32(amounts[3])*137.5).ToString(),
                        Color = SKColor.Parse("#266489")
                    },
                    new Entry((float)Convert.ToInt32(amounts[4]))
                    {
                        Label = days1[4],
                        ValueLabel = (Convert.ToInt32(amounts[4])*137.5).ToString(),
                        Color = SKColor.Parse("#68B9C0")
                    },
                    new Entry((float)Convert.ToInt32(amounts[5]))
                    {
                        Label = days1[5],
                        ValueLabel = (Convert.ToInt32(amounts[5])*137.5).ToString(),
                        Color = SKColor.Parse("#90D585")
                    },
                    new Entry((float)Convert.ToInt32(amounts[6]))
                    {
                        Label = days1[6],
                        ValueLabel = (Convert.ToInt32(amounts[6])*137.5).ToString(),
                        Color = SKColor.Parse("#266489")
                    },
                    new Entry((float)Convert.ToInt32(amounts[7]))
                    {
                        Label = days1[7],
                        ValueLabel = (Convert.ToInt32(amounts[7])*137.5).ToString(),
                        Color = SKColor.Parse("#68B9C0")
                    }
                };

                var chart = new LineChart() { Entries = entries };
                chart.LabelTextSize = 35f;
                chart.LineMode = LineMode.Straight;
                chart.LineSize = 5f;
                chart.MinValue = 80;
                chart.MaxValue = 120;
                var chartView = FindViewById<ChartView>(Resource.Id.chartViewMN);
                chartView.Chart = chart;

                days1.Clear();
                amounts.Clear();
                foreach (List<string> ls1 in stL)
                {
                    string day = ls1[0];
                    string[] parse = day.Split(' ');
                    day = parse[1] + " " + parse[0];
                    days1.Add(day);
                    amounts.Add(ls1[1]);
                }

                var entries1 = new[]
                {
                    new Entry((float)Convert.ToInt32(amounts[0]))
                    {
                        Label = days1[0],
                        ValueLabel = (Convert.ToInt32(amounts[0])*112.5).ToString(),
                        Color = SKColor.Parse("#266489")
                    },
                    new Entry((float)Convert.ToInt32(amounts[1]))
                    {
                        Label = days1[1],
                        ValueLabel = (Convert.ToInt32(amounts[1])*112.5).ToString(),
                        Color = SKColor.Parse("#68B9C0")
                    },
                    new Entry((float)Convert.ToInt32(amounts[2]))
                    {
                        Label = days1[2],
                        ValueLabel = (Convert.ToInt32(amounts[2])*112.5).ToString(),
                        Color = SKColor.Parse("#90D585")
                    },
                    new Entry((float)Convert.ToInt32(amounts[3]))
                    {
                        Label = days1[3],
                        ValueLabel = (Convert.ToInt32(amounts[3])*112.5).ToString(),
                        Color = SKColor.Parse("#266489")
                    },
                    new Entry((float)Convert.ToInt32(amounts[4]))
                    {
                        Label = days1[4],
                        ValueLabel = (Convert.ToInt32(amounts[4])*112.5).ToString(),
                        Color = SKColor.Parse("#68B9C0")
                    },
                    new Entry((float)Convert.ToInt32(amounts[5]))
                    {
                        Label = days1[5],
                        ValueLabel = (Convert.ToInt32(amounts[5])*112.5).ToString(),
                        Color = SKColor.Parse("#90D585")
                    },
                    new Entry((float)Convert.ToInt32(amounts[6]))
                    {
                        Label = days1[6],
                        ValueLabel = (Convert.ToInt32(amounts[6])*112.5).ToString(),
                        Color = SKColor.Parse("#266489")
                    },
                    new Entry((float)Convert.ToInt32(amounts[7]))
                    {
                        Label = days1[7],
                        ValueLabel = (Convert.ToInt32(amounts[7])*112.5).ToString(),
                        Color = SKColor.Parse("#68B9C0")
                    }
                };

                var chart1 = new LineChart() { Entries = entries1 };
                chart1.LabelTextSize = 35f;
                chart1.LineMode = LineMode.Straight;
                chart1.LineSize = 5f;
                chart1.MinValue = 0;
                chart1.MaxValue = 10;
                //chart1.BackgroundColor = SKColor.Parse("#FDA2A2");
                var chartView1 = FindViewById<ChartView>(Resource.Id.chartViewST);
                chartView1.Chart = chart1;

            }


            //var entries = new[]
            //{
            //    new Entry(200)
            //    {
            //    Label = "January",
            //    ValueLabel = "200",
            //    Color = SKColor.Parse("#266489")
            //    },
            //    new Entry(400)
            //    {
            //    Label = "February",
            //    ValueLabel = "400",
            //    Color = SKColor.Parse("#68B9C0")
            //    },
            //    new Entry(-100)
            //    {
            //    Label = "March",
            //    ValueLabel = "-100",
            //    Color = SKColor.Parse("#90D585")
            //    }
            //};

            //var chart = new LineChart() { Entries = entries };
            //chart.LabelTextSize = 50f;
            //chart.LineMode = LineMode.Straight;
            //chart.LineSize = 5f;
            //var chartView = FindViewById<ChartView>(Resource.Id.chartViewMN);
            //chartView.Chart = chart;


            //Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            //Android.App.AlertDialog alert = dialog.Create();
            //alert.SetTitle("Info");
            //alert.SetMessage(temp);
            //alert.SetButton("OK", (c, ev) =>
            //{
            //    //ok button click task
            //});
            //alert.Show();
        }

        private static readonly HttpClient client = new HttpClient();
        public static string trxs = "";
        public static Dictionary<string, List<string>> dataPerDay = new Dictionary<string, List<string>>();
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
                List<string> defaultList = new List<string>();
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
                    dataPerDay.Add(lastdate, defaultList);
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

                foreach (string s in pageInfo)
                {
                    if (s.Contains(day1) && s.Contains("13750000000,0"))
                    {
                        todayList.Add("MN");
                    }
                    else if (s.Contains(day2) && s.Contains("13750000000,0"))
                    {
                        day2List.Add("MN");
                    }
                    else if (s.Contains(day3) && s.Contains("13750000000,0"))
                    {
                        day3List.Add("MN");
                    }
                    else if (s.Contains(day4) && s.Contains("13750000000,0"))
                    {
                        day4List.Add("MN");
                    }
                    else if (s.Contains(day5) && s.Contains("13750000000,0"))
                    {
                        day5List.Add("MN");
                    }
                    else if (s.Contains(day6) && s.Contains("13750000000,0"))
                    {
                        day6List.Add("MN");
                    }
                    else if (s.Contains(day7) && s.Contains("13750000000,0"))
                    {
                        day7List.Add("MN");
                    }
                    else if (s.Contains(day8) && s.Contains("13750000000,0"))
                    {
                        day8List.Add("MN");
                    }
                    else if (s.Contains(day1) && s.Contains("11250000000,0"))
                    {
                        todayList.Add("ST");
                    }
                    else if (s.Contains(day2) && s.Contains("11250000000,0"))
                    {
                        day2List.Add("ST");
                    }
                    else if (s.Contains(day3) && s.Contains("11250000000,0"))
                    {
                        day3List.Add("ST");
                    }
                    else if (s.Contains(day4) && s.Contains("11250000000,0"))
                    {
                        day4List.Add("ST");
                    }
                    else if (s.Contains(day5) && s.Contains("11250000000,0"))
                    {
                        day5List.Add("ST");
                    }
                    else if (s.Contains(day6) && s.Contains("11250000000,0"))
                    {
                        day6List.Add("ST");
                    }
                    else if (s.Contains(day7) && s.Contains("11250000000,0"))
                    {
                        day7List.Add("ST");
                    }
                    else if (s.Contains(day8) && s.Contains("11250000000,0"))
                    {
                        day8List.Add("ST");
                    }
                    else
                    {
                        bool found = false;
                        foreach (string da in days)
                        {
                            if (s.Contains(da))
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            day9List.Add("End");
                        }
                    }
                }

                Android.App.AlertDialog.Builder dialog3 = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert3 = dialog3.Create();
                alert3.SetTitle("Info");
                alert3.SetMessage(day9List.Count.ToString());
                alert3.SetButton("OK", (c, ev) =>
                {
                    //ok button click task
                });
                alert3.Show();


                //if (!day9List.Any())
                //{
                //page++;
                //pageInfo.Clear();
                //List<string> results = new List<string>();
                //decimal numOfPages = Convert.ToDecimal(totalTxs);
                //numOfPages = Math.Ceiling(numOfPages / 100);
                //int pages = Convert.ToInt32(numOfPages);
                //if (page <= pages)
                //{
                //    int pageRange = page * 100;
                //    URL = @"https://explorer.rpicoin.com/ext/getaddresstxsajax/?address=" + address + "&start=" + pageRange.ToString();

                //    //Android.App.AlertDialog.Builder dialog1 = new Android.App.AlertDialog.Builder(this);
                //    //Android.App.AlertDialog alert1 = dialog1.Create();
                //    //alert1.SetTitle("Info");
                //    //alert1.SetMessage(URL);
                //    //alert1.SetButton("OK", (c, ev) =>
                //    //{
                //    //    //ok button click task
                //    //});
                //    //alert1.Show();

                //    var requestTask1 = client.GetStringAsync(URL);
                //    var response1 = Task.Run(() => requestTask1);
                //    string content1 = response1.Result.ToString();

                //    if (content1.Contains("\"recordsTotal\":0,"))
                //    {

                //        results.Add("No transactions found");
                //    }
                //    else
                //    {
                //        string[] parse2 = content1.Split(new string[] { "],[" }, StringSplitOptions.None);
                //        foreach (string s in parse2)
                //        {
                //            results.Add(s);
                //        }
                //    }
                //    page++;
                //}

                //pageInfo = results;
                while (!day9List.Any())
                {
                    pageInfo.Clear();
                    pageInfo = getTxs(address, totalTxs);


                    foreach (string s in pageInfo)
                    {
                        if (s.Contains(day1) && s.Contains("13750000000,0"))
                        {
                            todayList.Add("MN");
                        }
                        else if (s.Contains(day2) && s.Contains("13750000000,0"))
                        {
                            day2List.Add("MN");
                        }
                        else if (s.Contains(day3) && s.Contains("13750000000,0"))
                        {
                            day3List.Add("MN");
                        }
                        else if (s.Contains(day4) && s.Contains("13750000000,0"))
                        {
                            day4List.Add("MN");
                        }
                        else if (s.Contains(day5) && s.Contains("13750000000,0"))
                        {
                            day5List.Add("MN");
                        }
                        else if (s.Contains(day6) && s.Contains("13750000000,0"))
                        {
                            day6List.Add("MN");
                        }
                        else if (s.Contains(day7) && s.Contains("13750000000,0"))
                        {
                            day7List.Add("MN");
                        }
                        else if (s.Contains(day8) && s.Contains("13750000000,0"))
                        {
                            day8List.Add("MN");
                        }
                        else if (s.Contains(day1) && s.Contains("11250000000,0"))
                        {
                            todayList.Add("ST");
                        }
                        else if (s.Contains(day2) && s.Contains("11250000000,0"))
                        {
                            day2List.Add("ST");
                        }
                        else if (s.Contains(day3) && s.Contains("11250000000,0"))
                        {
                            day3List.Add("ST");
                        }
                        else if (s.Contains(day4) && s.Contains("11250000000,0"))
                        {
                            day4List.Add("ST");
                        }
                        else if (s.Contains(day5) && s.Contains("11250000000,0"))
                        {
                            day5List.Add("ST");
                        }
                        else if (s.Contains(day6) && s.Contains("11250000000,0"))
                        {
                            day6List.Add("ST");
                        }
                        else if (s.Contains(day7) && s.Contains("11250000000,0"))
                        {
                            day7List.Add("ST");
                        }
                        else if (s.Contains(day8) && s.Contains("11250000000,0"))
                        {
                            day8List.Add("ST");
                        }
                        else
                        {
                            bool found = false;
                            foreach (string da in days)
                            {
                                if (s.Contains(da))
                                {
                                    found = true;
                                }
                            }
                            if (!found)
                            {
                                day9List.Add("End");
                            }
                        }
                    }
                }

                string rr = "";
                rr += "_" + todayList.Count.ToString();
                rr += "_" + day2List.Count.ToString();
                rr += "_" + day3List.Count.ToString();
                rr += "_" + day4List.Count.ToString();
                rr += "_" + day5List.Count.ToString();
                rr += "_" + day6List.Count.ToString();
                rr += "_" + day7List.Count.ToString();
                rr += "_" + day8List.Count.ToString();
                rr += "_" + day9List.Count.ToString();

                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = dialog.Create();
                alert.SetTitle("Info");
                alert.SetMessage(rr);
                alert.SetButton("OK", (c, ev) =>
                {
                    //ok button click task
                });
                alert.Show();


                return "Successful";
            }
            else
            {
                return result;
            }
        }

        int page = 0;

        public List<string> getTxs(string address, int totalTxs)
        {
            page++;

            List<string> results = new List<string>();
            string URL = "";
            decimal numOfPages = Convert.ToDecimal(totalTxs);
            numOfPages = Math.Ceiling(numOfPages / 100);
            int pages = Convert.ToInt32(numOfPages);
            if (page <= pages)
            {
                int pageRange = page * 100;
                URL = @"https://explorer.rpicoin.com/ext/getaddresstxsajax/?address=" + address + "&start=" + pageRange.ToString();
                var requestTask = client.GetStringAsync(URL);
                var response = Task.Run(() => requestTask);
                string content = response.Result.ToString();

                if (content.Contains("\"recordsTotal\":0,"))
                {

                    results.Add("No transactions found");
                }
                else
                {
                    string[] parse = content.Split(new string[] { "],[" }, StringSplitOptions.None);
                    foreach (string s in parse)
                    {
                        results.Add(s);
                    }
                }
            }
            return results;
        }
    }
}