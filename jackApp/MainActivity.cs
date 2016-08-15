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
            var seditTextNDay = FindViewById<TextView>(Resource.Id.editTextNDay).Text;
            var seditTextADay = FindViewById<TextView>(Resource.Id.editTextADay).Text;
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
            //else if (string.IsNullOrWhiteSpace(seditTextFood))
            //{
            //    Toast.MakeText(this, "餐費沒有填", ToastLength.Long).Show();
            //}
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
                var weekday = GetWorkaday(sdt, edt, seditTextNDay, seditTextADay);
                vtextViewWorkday.Text = "共" + weekday.ToString() + "天";
                var DoubleNO = Double.Parse(seditTextNO);
                var DoubleTot = Double.Parse(seditTextTot);
                var DoubleBook = Double.Parse(seditTextBook);
                //var DoubleFood = Double.Parse(seditTextFood);
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
                vspinnerYear = string.Format("{0}", val);
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
        public static int GetWorkaday(DateTime sdt, DateTime edt,string seditTextNDay, string seditTextADay)
        {
            int iaday = 0;
            int inday = 0;
            var noWeek = new List<Tuple<string, string, int, string>>();
            #region 年上班日
            noWeek.Add(new Tuple<string, string, int, string>("20160101", "五", 2, "開國紀念日"));
            noWeek.Add(new Tuple<string, string, int, string>("20160102", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160103", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160104", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160105", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160106", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160107", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160108", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160109", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160110", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160111", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160112", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160113", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160114", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160115", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160116", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160117", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160118", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160119", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160120", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160121", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160122", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160123", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160124", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160125", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160126", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160127", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160128", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160129", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160130", "六", 0, "補行上班"));
            noWeek.Add(new Tuple<string, string, int, string>("20160131", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160201", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160202", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160203", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160204", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160205", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160206", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160207", "日", 2, "農曆除夕"));
            noWeek.Add(new Tuple<string, string, int, string>("20160208", "一", 2, "春節"));
            noWeek.Add(new Tuple<string, string, int, string>("20160209", "二", 2, "春節"));
            noWeek.Add(new Tuple<string, string, int, string>("20160210", "三", 2, "春節"));
            noWeek.Add(new Tuple<string, string, int, string>("20160211", "四", 2, "補假"));
            noWeek.Add(new Tuple<string, string, int, string>("20160212", "五", 2, "調整放假"));
            noWeek.Add(new Tuple<string, string, int, string>("20160213", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160214", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160215", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160216", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160217", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160218", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160219", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160220", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160221", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160222", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160223", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160224", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160225", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160226", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160227", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160228", "日", 2, "二二八紀念日"));
            noWeek.Add(new Tuple<string, string, int, string>("20160229", "一", 2, "補假"));
            noWeek.Add(new Tuple<string, string, int, string>("20160301", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160302", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160303", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160304", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160305", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160306", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160307", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160308", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160309", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160310", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160311", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160312", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160313", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160314", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160315", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160316", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160317", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160318", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160319", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160320", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160321", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160322", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160323", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160324", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160325", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160326", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160327", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160328", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160329", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160330", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160331", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160401", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160402", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160403", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160404", "一", 2, "兒童節清明"));
            noWeek.Add(new Tuple<string, string, int, string>("20160405", "二", 2, "補假"));
            noWeek.Add(new Tuple<string, string, int, string>("20160406", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160407", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160408", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160409", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160410", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160411", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160412", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160413", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160414", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160415", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160416", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160417", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160418", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160419", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160420", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160421", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160422", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160423", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160424", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160425", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160426", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160427", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160428", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160429", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160430", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160501", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160502", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160503", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160504", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160505", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160506", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160507", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160508", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160509", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160510", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160511", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160512", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160513", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160514", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160515", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160516", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160517", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160518", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160519", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160520", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160521", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160522", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160523", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160524", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160525", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160526", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160527", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160528", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160529", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160530", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160531", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160601", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160602", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160603", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160604", "六", 0, "補行上班"));
            noWeek.Add(new Tuple<string, string, int, string>("20160605", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160606", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160607", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160608", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160609", "四", 2, "端午節"));
            noWeek.Add(new Tuple<string, string, int, string>("20160610", "五", 2, "調整放假"));
            noWeek.Add(new Tuple<string, string, int, string>("20160611", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160612", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160613", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160614", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160615", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160616", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160617", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160618", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160619", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160620", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160621", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160622", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160623", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160624", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160625", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160626", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160627", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160628", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160629", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160630", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160701", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160702", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160703", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160704", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160705", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160706", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160707", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160708", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160709", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160710", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160711", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160712", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160713", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160714", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160715", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160716", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160717", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160718", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160719", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160720", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160721", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160722", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160723", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160724", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160725", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160726", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160727", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160728", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160729", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160730", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160731", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160801", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160802", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160803", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160804", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160805", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160806", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160807", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160808", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160809", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160810", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160811", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160812", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160813", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160814", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160815", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160816", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160817", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160818", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160819", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160820", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160821", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160822", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160823", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160824", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160825", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160826", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160827", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160828", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160829", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160830", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160831", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160901", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160902", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160903", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160904", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160905", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160906", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160907", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160908", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160909", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160910", "六", 0, "補行上班"));
            noWeek.Add(new Tuple<string, string, int, string>("20160911", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160912", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160913", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160914", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160915", "四", 2, "中秋節"));
            noWeek.Add(new Tuple<string, string, int, string>("20160916", "五", 2, "調整放假"));
            noWeek.Add(new Tuple<string, string, int, string>("20160917", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160918", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160919", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160920", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160921", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160922", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160923", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160924", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160925", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160926", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160927", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160928", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160929", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20160930", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161001", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161002", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161003", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161004", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161005", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161006", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161007", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161008", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161009", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161010", "一", 2, "國慶日"));
            noWeek.Add(new Tuple<string, string, int, string>("20161011", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161012", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161013", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161014", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161015", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161016", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161017", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161018", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161019", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161020", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161021", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161022", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161023", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161024", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161025", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161026", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161027", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161028", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161029", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161030", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161031", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161101", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161102", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161103", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161104", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161105", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161106", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161107", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161108", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161109", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161110", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161111", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161112", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161113", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161114", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161115", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161116", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161117", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161118", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161119", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161120", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161121", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161122", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161123", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161124", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161125", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161126", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161127", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161128", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161129", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161130", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161201", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161202", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161203", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161204", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161205", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161206", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161207", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161208", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161209", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161210", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161211", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161212", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161213", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161214", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161215", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161216", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161217", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161218", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161219", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161220", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161221", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161222", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161223", "五", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161224", "六", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161225", "日", 2, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161226", "一", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161227", "二", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161228", "三", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161229", "四", 0, ""));
            noWeek.Add(new Tuple<string, string, int, string>("20161230", "五", 0, ""));
            #endregion
            //TimeSpan得到fromTime和toTime的時間間隔    
            var ts = edt.Subtract(sdt);
            //獲取兩個日期間的總天數    
            long countday = ts.Days;
            //工作日  
            int weekday = 0;
            //循環用來扣除總天數中的雙休日    
            for (var i = 0; i <= countday; i++)
            {
                var tempdt = sdt.Date.AddDays(i);
                if (tempdt.DayOfWeek != DayOfWeek.Saturday && tempdt.DayOfWeek != DayOfWeek.Sunday)
                {
                    weekday++;
                }
            }
            int.TryParse(seditTextNDay, out inday);
            int.TryParse(seditTextADay, out iaday);
            weekday += iaday;
            weekday -= inday;
            return weekday;
        }
    }
}

