namespace Mcce.SmartOffice.DigitalFrameApp
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
