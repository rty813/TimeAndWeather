using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;

namespace TimeAndWeather
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private DispatcherTimer ShowTimer;
        public MainWindow()
        {
            InitializeComponent();
            ShowTimer = new System.Windows.Threading.DispatcherTimer();
            ShowTimer.Tick += new EventHandler(ShowCurTimer);
            ShowTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            ShowTimer.Start();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://wthrcdn.etouch.cn/WeatherApi?citykey=" + "101110102");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();

            GZipStream gzip = new GZipStream(myResponseStream, CompressionMode.Decompress);
            string result;
            using (StreamReader reader = new StreamReader(gzip, Encoding.GetEncoding("utf-8")))
            {
                result = reader.ReadToEnd();
            }
            myResponseStream.Close();
            gzip.Close();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            XmlElement resp = (XmlElement)doc.SelectSingleNode("resp");
            XmlElement city = (XmlElement)resp.SelectSingleNode("city");
            XmlElement updatetime = (XmlElement)resp.SelectSingleNode("updatetime");
            XmlElement wendu = (XmlElement)resp.SelectSingleNode("wendu");
            XmlElement fengli= (XmlElement)resp.SelectSingleNode("fengli");
            XmlElement shidu= (XmlElement)resp.SelectSingleNode("shidu");
            XmlElement fengxiang= (XmlElement)resp.SelectSingleNode("fengxiang");
            XmlElement sunrise = (XmlElement)resp.SelectSingleNode("sunrise_1");
            XmlElement sunset = (XmlElement)resp.SelectSingleNode("sunset_1");

            StringBuilder builder = new StringBuilder();
            builder.Append("城市：" + city.InnerText + "\r\n");
            builder.Append("更新时间：" + updatetime.InnerText + "\r\n");
            builder.Append("温度：" + wendu.InnerText + "\r\n");
            builder.Append("风力：" + fengli.InnerText + "\r\n");
            builder.Append("湿度：" + shidu.InnerText + "\r\n");
            builder.Append("风向：" + fengxiang.InnerText + "\r\n");
            builder.Append("日出时间：" + sunrise.InnerText + "\r\n");
            builder.Append("日落时间：" + sunset.InnerText + "\r\n");
            
            label_weather.Content = builder.ToString();

            List<StringBuilder> list = new List<StringBuilder>();

            XmlNode forecast = resp.SelectSingleNode("forecast");
            XmlNodeList weathers = forecast.SelectNodes("weather");
            foreach(XmlNode weather in weathers)
            {
                builder = new StringBuilder();
                XmlElement date = (XmlElement)weather.SelectSingleNode("date");
                XmlElement high = (XmlElement)weather.SelectSingleNode("high");
                XmlElement low = (XmlElement)weather.SelectSingleNode("low");
                XmlNode day = weather.SelectSingleNode("day");
                XmlNode night = weather.SelectSingleNode("night");
                XmlElement type1 = (XmlElement)day.SelectSingleNode("type");
                XmlElement fengxiang1 = (XmlElement)day.SelectSingleNode("fengxiang");
                XmlElement fengli1 = (XmlElement)day.SelectSingleNode("fengli");
                XmlElement type2 = (XmlElement)night.SelectSingleNode("type");
                XmlElement fengxiang2 = (XmlElement)night.SelectSingleNode("fengxiang");
                XmlElement fengli2 = (XmlElement)night.SelectSingleNode("fengli");

                builder.Append(date.InnerText + "\r\n");
                builder.Append(high.InnerText + "\r\n");
                builder.Append(low.InnerText + "\r\n");
                builder.Append("白天：\r\n" + type1.InnerText + "\r\n");
                builder.Append(fengxiang1.InnerText + "\r\n");
                builder.Append(fengli1.InnerText + "\r\n");
                builder.Append("夜晚：\r\n" + type2.InnerText + "\r\n");
                builder.Append(fengxiang2.InnerText + "\r\n");
                builder.Append(fengli2.InnerText + "\r\n");
                list.Add(builder);
            }
            label_first.Content = list[0].ToString();
            label_second.Content = list[1].ToString();
            label_third.Content = list[2].ToString();
            label_forth.Content = list[3].ToString();
            label_fifth.Content = list[4].ToString();
        }

        private void ShowCurTimer(object sender, EventArgs e)
        {
            label_time.Content = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
            label_time.Content += " ";
            label_time.Content += DateTime.Now.ToString("yyyy年MM月dd日"); 
            label_time.Content += " ";
            label_time.Content += DateTime.Now.ToString("HH:mm:ss");
        }

       
    }
}
