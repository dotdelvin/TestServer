using SampSharp.GameMode;
using SampSharp.GameMode.SAMP;
using System;

namespace TestServer
{
    public class GameMode : BaseMode
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // TODO: Delete.
            Server.SetWorldTime(0);
        }
    }
}