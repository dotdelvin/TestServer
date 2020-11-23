using SampSharp.GameMode;
using System;
using TestServer.World;

namespace TestServer
{
    public class GameMode : BaseMode
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Account.Create("Delvis", "Qwerty");
        }
    }
}