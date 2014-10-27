using System.Windows;


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
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}
