using SampSharp.GameMode;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestServer.World
{
    [PooledType]
    public class Player : BasePlayer
    {
        private int _pauseTick;

        public const int Rate = 30;

        #region Properties

        /// <summary>
        ///     Gets whether this <see cref="Player"/> has an <see cref="Account"/>.
        /// </summary>
        public bool HasAccount =>
            Account.Exists(Name);

        /// <summary>
        ///     Gets whether this <see cref="Player"/> is logged into any <see cref="World.Account"/>.
        /// </summary>
        public bool IsLoggedIn =>
            Account != null;

        /// <summary>
        ///     Gets whether this <see cref="Player"/> has an <see cref="World.Account"/> and it logged in.
        /// </summary>
        public bool IsHasAccountAndLoggedIn =>
           HasAccount && IsLoggedIn;

        /// <summary>
        ///     Gets the <see cref="Account"/> this <see cref="Player"/> is currently in.
        /// </summary>
        public Account Account { get; private set; }

        /// <summary>
        ///     Gets whether this <see cref="Player"/>'s name matches the roleplay format.
        /// </summary>
        public bool IsRolePlayName =>
            Regex.IsMatch(Name, @"^([A-Z][a-z]+_[A-Z][a-z]+)$");

        /// <summary>
        ///     Gets whether this <see cref="Player"/> is paused.
        /// </summary>
        public bool IsPaused =>
            (PausedTime / 1000) > 0;

        /// <summary>
        ///     Gets the amount of time (in milliseconds) that a player has been paused.
        /// </summary>
        public int PausedTime =>
            _pauseTick * Rate;

        #endregion

        #region Static Properties

        /// <summary>
        /// <inheritdoc cref="IdentifiedPool{TInstance}.All"/>
        /// </summary>
        public static new IEnumerable<Player> All =>
            BasePlayer.All.Select(player => (Player)player);

        #endregion

        #region Methods

        /// <summary>
        ///     Kicks this <see cref="Player"/> from the server when the timer expires.
        /// </summary>
        /// <param name="timer">The timer of the kick.</param>
        public void Kick(int timer) =>
            Timer.RunOnce(timer, () => Kick());

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
            
            var spawn = account.Spawn;

            SetSpawnInfo(Team, account.Skin, spawn.Position, spawn.Angle);

            Money = account.Money;
            Score = account.Score;
            Health = account.Health;
            Armour = account.Armour;
            Color = account.Color;
            Account = account;

            return true;
        }

        /// <summary>
        ///     Logs out this <see cref="Player"/> from the current account.
        /// </summary>
        public void LogOut()
        {
            SetSpawnInfo(Team, 0, new Vector3(), 0);

            Money = 0;
            Score = 0;
            Health = 100;
            Armour = 0;
            Color = 0;
            Account = null;
        }

        /// <summary>
        ///     Shows the login dialog for this <see cref="Player"/>.
        /// </summary>
        public void ShowLoginDialog()
        {
            var dialog = new InputDialog("Login", "Enter the password", true, "Ok");

            dialog.Response += (sender, e) =>
            {
                if (!LogIn(Name, e.InputText))
                {
                    dialog.Show(this);
                    return;
                }

                Spawn();
            };

            dialog.Show(this);
        }

        /// <summary>
        ///     Shows the register dialog for this <see cref="Player"/>.
        /// </summary>
        public void ShowRegisterDialog()
        {
            var dialog = new InputDialog("Register", "Enter a password", false, "Ok");

            dialog.Response += (sender, e) =>
            {
                string password = e.InputText;

                if (string.IsNullOrWhiteSpace(password))
                {
                    dialog.Show(this);
                    return;
                }

                Account.Create(Name, password);

                LogIn(Name, password);
                Spawn();
            };

            dialog.Show(this);
        }

        #endregion

        #region Callback Methods

        public override void OnText(TextEventArgs e)
        {
            base.OnText(e);
            
            // TODO: Make your own chat.
            if (!IsHasAccountAndLoggedIn)
                e.SendToPlayers = false;

            string errorMessage = null;

            if (!HasAccount)
                errorMessage = "You don't have an account.";
            else if (!IsLoggedIn)
                errorMessage = "You aren't logged in.";

            if (errorMessage != null)
                SendClientMessage(errorMessage);
        }

        public override void OnUpdate(PlayerUpdateEventArgs e)
        {
            base.OnUpdate(e);

            _pauseTick = 0;
        }

        public override void OnConnected(EventArgs e)
        {
            base.OnConnected(e);

            // Starts a timer that runs in the background (even when this player has minimized the game).
            Timer.Run(Rate, () => _pauseTick++);

            Timer.RunOnce(Ping, () =>
            {
                if (!IsRolePlayName)
                {
                    SendClientMessage("Your name doesn't match the roleplay format.");
                    Kick(Ping);
                    return;
                }

                if (HasAccount)
                    ShowLoginDialog();
                else
                    ShowRegisterDialog();
            });
        }

        public override void OnDisconnected(DisconnectEventArgs e)
        {
            base.OnDisconnected(e);

            LogOut();
        }

        public override void OnRequestSpawn(RequestSpawnEventArgs e)
        {
            base.OnRequestSpawn(e);

            if (!IsHasAccountAndLoggedIn)
                e.PreventSpawning = true;
        }

        #endregion
    }
}