using SampSharp.Core;

namespace TestServer
{
    public static class Program
    {
        public static void Main()
        {
            new GameModeBuilder()
                .Use<GameMode>()
                .Run();
        }
    }
}