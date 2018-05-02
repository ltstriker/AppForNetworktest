using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace AppForNetworktest
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string res;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Res.Text = await myConnection.getConnection().getConnectToGetWeatherAsync(place.Text);
        }

        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            Res.Text = await myConnection.getConnection().getConnectToTranslateAsync(place.Text);
        }

        private void switch_fun_Click(object sender, RoutedEventArgs e)
        {
            if ((String)get.Content == "getWeather")
            {
                get.Content = "translate";
                get.Click -= Button_Click;
                get.Click += Button2_Click;
            }
            else
            {
                get.Content = "getWeather";
                get.Click -= Button2_Click;
                get.Click += Button_Click;
            }
            
        }
    }
}
