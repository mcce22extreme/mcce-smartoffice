namespace Mcce.SmartOffice.AdminApp
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
