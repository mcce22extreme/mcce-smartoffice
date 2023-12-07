namespace Mcce.SmartOffice.MobileApp
{
    public partial class App : Application
    {
        public App(Shell shell)
        {
            InitializeComponent();

            MainPage = shell;
        }
    }
}
