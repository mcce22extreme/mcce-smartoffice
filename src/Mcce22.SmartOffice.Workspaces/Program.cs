namespace Mcce22.SmartOffice.Workspaces
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await new Bootstrap().Run(args);
        }
    }
}
