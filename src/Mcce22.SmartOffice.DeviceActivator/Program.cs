namespace Mcce22.SmartOffice.DeviceActivator
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await new Bootstrap().Run(args);
        }
    }
}
