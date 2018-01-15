using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScreenShot
{
    public static class WebBrowserExtensions
    {
        public static void SuppressScriptErrors(this WebBrowser webBrowser, bool hide)
        {
            FieldInfo fInfo = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if(fInfo == null)
            {
                return;
            }
            object objBrowser = fInfo.GetValue(webBrowser);
            if(objBrowser == null)
            {
                return;
            }
            objBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objBrowser, new object[] { hide });
        }
    }
}
