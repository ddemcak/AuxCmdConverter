using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Usb;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AuxCmdConverter
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {

        //public string CustomTitle = "Test";

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        private string _customTitle;
        public string CustomTitle
        {
            get => _customTitle;
            set { _customTitle = value; NotifyPropertyChanged(nameof(CustomTitle)); }
        }

        public MainWindow()
        {
            this.InitializeComponent();
            MainGrid.DataContext = this;


            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);

            this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(150, 150, 660, 650));
            /*
            this.Title = string.Format("AUX Cmd Converter v{0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);
            
            CustomTitle = string.Format("AUX Cmd Converter v{0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);
            */

            CustomTitle = string.Format("AUX Cmd Converter v{0}", GetAssemblyVersion());

            lvCommands.Items.Add("#80");
            lvCommands.Items.Add("#22,200,100");
        }


        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? String.Empty;
        }


        private string BuildUpCommand(string device, string cmd)
        {
            return string.Format("echo -e '\\x1B{0}\\x0D' > {1}", ConvertCommand(cmd), device);
        }

        private string ConvertCommand(string cmd)
        {
            char[] array = cmd.ToCharArray();
            string final = "";
            foreach (var i in array)
            {
                string hex = String.Format("{0:X}", Convert.ToInt32(i));
                final += hex.Insert(0, "\\x");
            }
            final = final.TrimEnd();

            return final;
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            tbRawCommand.Text = BuildUpCommand(tbDevice.Text, tbCommand.Text);

            var package = new DataPackage();
            package.SetText(tbRawCommand.Text);
            Clipboard.SetContent(package);

            lvCommands.Items.Insert(0, tbCommand.Text);
            //lvCommands.ScrollIntoView(lvCommands.Items[lvCommands.Items.Count - 1]); 

        }

        private void lvCommands_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            
            if (lvCommands.Items.Count > 0)
            {
                tbCommand.Text = lvCommands.SelectedItem.ToString();
                tbRawCommand.Text = BuildUpCommand(tbDevice.Text, tbCommand.Text);
            
                var package = new DataPackage();
                package.SetText(tbRawCommand.Text);
                Clipboard.SetContent(package);
            }

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            lvCommands.Items.Clear();
        }
    }
}
