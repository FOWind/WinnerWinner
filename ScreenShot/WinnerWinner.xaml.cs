using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ScreenShot
{
    /// <summary>
    /// OpenShot.xaml 的交互逻辑
    /// </summary>
    public partial class OpenShot : Window
    {
        private Config config = new Config();
        public OpenShot()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window screenshotInstance = new MainWindow();
            this.Visibility = Visibility.Hidden;
            screenshotInstance.ShowDialog();
            this.Visibility = Visibility.Visible;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(@"Config.json"))
            {
                TextWriter tw = new StreamWriter(@"Config.json");
                tw.WriteLine("{}");
                tw.Close();
            }
            else
            {

                StreamReader sR = File.OpenText(@"Config.json");
                string configStr = sR.ReadToEnd();
                config = JsonConvert.DeserializeObject<Config>(configStr);
                
                sR.Close();
            }
            Console.Write(config.APIKey);
            if (!string.IsNullOrEmpty(config.APIKey) && !string.IsNullOrEmpty(config.SecretKey) )
            {
                apikey.Text = config.APIKey;
                secretkey.Text = config.SecretKey;
                apikey.IsEnabled = false;
                secretkey.IsEnabled = false;
                apiLock.Content = "编辑Key";
            }
        }
        public string HandleOcr(string originString)
        {
            for (int i = 0; i < originString.Length; i++)
            {
                if (i == 3)
                {
                    continue;
                }
                if (originString[i].Equals('.'))
                {
                    return originString.Substring(i + 1);
                }
            }
            return originString;
        }
        private void Ocr()
        {
            var API_KEY = apikey.Text;
            var SECRET_KEY = secretkey.Text;
            var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            //注释掉的代码会导致文件释放不及时，导致不能连续截屏
            //FileStream fs = new FileStream(@"test.jpg", FileMode.Open,FileAccess.Read);
            //BinaryReader binReader = new BinaryReader(fs);
            //var image = new Byte[fs.Length];
            //binReader.Read(image, 0, (int)fs.Length);
            //binReader.Close();
            //fs.Close();
            var image = File.ReadAllBytes("test.jpg");
            string str = "";
            try
            {
                // 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
                // var result = client.AccurateBasic(image);
                // Console.WriteLine(result);
                // 如果有可选参数
                var options = new Dictionary<string, object>{
                    {"language_type", "CHN_ENG"},
                    {"detect_direction", "true"},
                    {"detect_language", "true"},
                    {"probability", "true"}
                };
                // 带参数调用通用文字识别, 图片参数为本地图片
                var result = client.GeneralBasic(image, options);
                Console.WriteLine(result);
                int wordN = Int32.Parse(result["words_result_num"].ToString());
                for (int i = 0; i < wordN; i++)
                    str += result["words_result"][i]["words"].ToString();
                str = HandleOcr(str);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                str = "error";
            }
            simpleBrowser.Navigate("https://www.baidu.com/#ie={inputEncoding}&wd=" + str);
            var browser = simpleBrowser as WebBrowser;
            WebBrowserExtensions.SuppressScriptErrors(simpleBrowser, true);
        }
        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg); // 坑点：格式选Bmp时，不带透明度
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
        private void ShowNowImg()
        {
            var imgByte = File.ReadAllBytes("test.jpg");
            var stream = new MemoryStream(imgByte);
            Bitmap bit = new Bitmap((System.Drawing.Image)new Bitmap(stream));
            BitmapImage bi = BitmapToBitmapImage(bit);
            nowImg.Source = bi;
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            int w = (int)App.Cr.Width;
            int h = (int)App.Cr.Height;
            int startX = (int)App.Cr.StartX;
            int startY = (int)App.Cr.StartY;
            Bitmap image = new Bitmap(w, h);
            Graphics imgGraphics = Graphics.FromImage(image);
            //设置截屏区域
            imgGraphics.CopyFromScreen(startX, startY, 0, 0, new System.Drawing.Size(w, h));
            image.Save("test.jpg", ImageFormat.Jpeg);
            ShowNowImg();
            Ocr();
        }
        private void SaveConfigToFile(string configJson)
        {
            FileStream fs = new FileStream(@"Config.json", FileMode.Create, FileAccess.Write);
            fs.SetLength(0);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(configJson);
            sw.Close();
        }
        private void API_Lock_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(apikey.Text))
            {
                config.APIKey = apikey.Text;
                config.SecretKey = secretkey.Text;
                string currentConfigJson = JsonConvert.SerializeObject(config);
                SaveConfigToFile(currentConfigJson);
                secretkey.IsEnabled = !secretkey.IsEnabled;
                apikey.IsEnabled = !apikey.IsEnabled;
            }
        }
        private void Enable_Change(object sender, DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine("enable changed");
            if (apikey.IsEnabled)
            {
                apiLock.Content = "锁定及保存Key";
            }
            else
            {
                apiLock.Content = "编辑Key";
            }
        }
    }
}
