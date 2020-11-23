using SampSharp.GameMode.Pools;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestServer.World
{
    [PooledType]
    public class Player : BasePlayer
    {
        #region Properties

        /// <summary>
        ///     Gets whether this <see cref="Player"/> is logged into any <see cref="World.Account"/>.
        /// </summary>
        public bool IsLoggedIn =>
            Account != null;

        /// <summary>
        ///     Gets the <see cref="Account"/> this <see cref="Player"/> in currently in.
        /// </summary>
        public Account Account { get; private set; }

        #endregion

        #region Static Properties

        public static new IEnumerable<Player> All =>
            BasePlayer.All.Select(player => (Player)player);

        #endregion

        #region Methods

        /// <summary>
        ///     Logs this <see cref="Player"/> in a specified account.
        /// </summary>
        /// <param name="name">The name of the <see cref="Account"/>.</param>
        /// <param name="password">The password of the <see cref="Account"/>.</param>
        /// <returns>True if successfully logged in; otherwise false.</returns>
        public bool LogIn(string name, string password)
        {
            var account = Account.Find(name);

            if (account == null)
                return false;

            if (!password.Equals(account.Password))
                return false;

            Account = account;

            return true;
        }

        /// <summary>
        ///     Logs out this <see cref="Player"/> from the current account.
        /// </summary>
        public void LogOut()
        {
            Account = null;
        }

        #endregion

        #region Callback Methods

        public override void OnConnected(EventArgs e)
        {
            base.OnConnected(e);
            
            LogIn(Name, "Qwerty");
        }

        #endregion
    }
}