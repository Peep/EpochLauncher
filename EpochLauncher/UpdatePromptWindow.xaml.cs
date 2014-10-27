using System.Windows;
using System.Windows.Threading;


namespace EpochLauncher
{
    public partial class UpdatePromptWindow : Window
    {
        public UpdatePromptWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Updater.DoUpdate();
            Close();
            Application.Current.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}
