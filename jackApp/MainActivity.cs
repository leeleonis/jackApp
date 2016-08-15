using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;

namespace jackApp
{
    [Activity(Label = "勻的算錢", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        string vspinnerYear, vspinnerMon;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            //年選單
            DateTime dt = DateTime.Today;
            var yearlist = new List<string>();
            for (int i = 0; i <= 2; i++)
            {
                var ldt = dt.AddYears(i);
                yearlist.Add(ldt.Year.ToString());
            }
            Spinner spinneryeay = FindViewById<Spinner>(Resource.Id.spinnerYear);
            spinneryeay.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapterYear = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, yearlist);
            adapterYear.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinneryeay.Adapter = adapterYear;
            //月選單
            var monlist = new List<string>();
            for (int i = 1; i <= 12; i++)
            {
                monlist.Add(i.ToString());
            }
            Spinner spinnerMon = FindViewById<Spinner>(Resource.Id.spinnerMon);
            spinnerMon.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapterMon = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, monlist);
            adapterMon.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerMon.Adapter = adapterMon;
            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += setButton_Click;
        }
        private void setButton_Click(object sender, EventArgs e)

        {
            var seditTextNO = FindViewById<TextView>(Resource.Id.editTextNO).Text;
            var seditTextTot = FindViewById<TextView>(Resource.Id.editTextTot).Text;
            var seditTextBook = FindViewById<TextView>(Resource.Id.editTextBook).Text;
            var seditTextFood = FindViewById<TextView>(Resource.Id.editTextFood).Text;
            if (string.IsNullOrWhiteSpace(vspinnerYear))
            {
                Toast.MakeText(this, "必須選年份", ToastLength.Long).Show();
            }
            else if (string.IsNullOrWhiteSpace(vspinnerMon))
            {
                Toast.MakeText(this, "必須選月份", ToastLength.Long).Show();
            }
            else if (string.IsNullOrWhiteSpace(seditTextNO))
            {
                Toast.MakeText(this, "請假天數沒有填", ToastLength.Long).Show();
            }
            else if (string.IsNullOrWhiteSpace(seditTextTot))
            {
                Toast.MakeText(this, "總金額沒有填", ToastLength.Long).Show();
            }
            else if (string.IsNullOrWhiteSpace(seditTextBook))
            {
                Toast.MakeText(this, "教材費沒有填", ToastLength.Long).Show();
            }
            else if (string.IsNullOrWhiteSpace(seditTextFood))
            {
                Toast.MakeText(this, "餐費沒有填", ToastLength.Long).Show();
            }
            else
            {
                var vtextViewSdt = FindViewById<TextView>(Resource.Id.textViewSdt);
                var vtextViewEdt = FindViewById<TextView>(Resource.Id.textViewEdt);
                var vtextViewWorkday = FindViewById<TextView>(Resource.Id.textViewWorkday);
                var vtextViewMtot = FindViewById<TextView>(Resource.Id.textViewMtot);
                var vtextViewNtot = FindViewById<TextView>(Resource.Id.textViewNtot);
                var vtextView4Mtot = FindViewById<TextView>(Resource.Id.textView4Mtot);
                var vtextView4Ntot = FindViewById<TextView>(Resource.Id.textView4Ntot);
                //var vspinnerYear = FindViewById<Spinner>(Resource.Id.spinnerYear);
                //var vspinnerMon = FindViewById<Spinner>(Resource.Id.spinnerMon);
                DateTime sdt = DateTime.Parse(vspinnerYear + "/" + vspinnerMon + "/01");
                DateTime edt = sdt.AddMonths(1).AddDays(-1);
                vtextViewSdt.Text = sdt.ToString("yyyy/MM/dd");
                vtextViewEdt.Text = edt.ToString("yyyy/MM/dd");
                var weekday = GetWorkaday(sdt, edt);
                vtextViewWorkday.Text = "共" + weekday.ToString() + "天";
                var DoubleNO = Double.Parse(seditTextNO);
                var DoubleTot = Double.Parse(seditTextTot);
                var DoubleBook = Double.Parse(seditTextBook);
                var DoubleFood = Double.Parse(seditTextFood);
                var ntot = (DoubleTot - DoubleBook) * (DoubleNO / weekday);//退費
                vtextViewNtot.Text = ntot.ToString("#.###");
                vtextView4Ntot.Text = ntot.ToString("#");
                vtextViewMtot.Text = (DoubleTot - ntot).ToString("#.###");
                vtextView4Mtot.Text = (DoubleTot - ntot).ToString("#");
            }
        }
        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            if (spinner.Id == Resource.Id.spinnerYear)
            {
                var val = spinner.GetItemAtPosition(e.Position);
                string toast = string.Format("選取{0}年", val);
                vspinnerYear= string.Format("{0}", val);
                Toast.MakeText(this, toast, ToastLength.Long).Show();
            }
            else if (spinner.Id == Resource.Id.spinnerMon)
            {
                var val = spinner.GetItemAtPosition(e.Position);
                string toast = string.Format("選取{0}月", val);
                vspinnerMon = string.Format("{0}", val);
                Toast.MakeText(this, toast, ToastLength.Long).Show();
            }                 
        }
        /// <summary>  
        /// 獲取日期段裡的工作日【除去 週六、日】   
        /// </summary>  
        /// <returns></returns>  
        public static int GetWorkaday(DateTime sdt ,DateTime edt)
        {
            //TimeSpan得到fromTime和toTime的時間間隔    
            var ts = edt.Subtract(sdt);
            //獲取兩個日期間的總天數    
            long countday = ts.Days;
            //工作日  
            int weekday = 0;
            //循環用來扣除總天數中的雙休日    
            for (var i = 0; i < countday; i++)
            {
                var tempdt = sdt.Date.AddDays(i + 1);
                if (tempdt.DayOfWeek != DayOfWeek.Saturday && tempdt.DayOfWeek != DayOfWeek.Sunday)
                {
                    weekday++;
                }
            }
            return weekday;
        }
    }
}

