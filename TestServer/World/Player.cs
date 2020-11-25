using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TestServer.Events;

namespace TestServer.World
{
    [PooledType]
    public class Player : BasePlayer
    {
        #region Fields

        private int _tick;
        private Timer _updateTimer;

        #endregion

        #region Events
        
        public event EventHandler<EventArgs> Joined;
        public event EventHandler<EventArgs> Paused;
        public event EventHandler<ResumeEventArgs> Resumed;
        public event EventHandler<AccountEventArgs> LoggedIn;
        public event EventHandler<EventArgs> LoggedOut;
        public event EventHandler<AccountEventArgs> Registered;
        public event EventHandler<EventArgs> Unregistered;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets whether this <see cref="Player"/> has any <see cref="Account"/>.
        /// </summary>
        public bool HasAccount =>
            Account != null;

        /// <summary>
        ///     Gets whether this <see cref="Player"/> is logged into his <see cref="Account"/>.
        /// </summary>
        public bool IsLoggedIn { get; private set; }

        /// <summary>
        ///     Gets whether this <see cref="Player"/> has any account and is logged in.
        /// </summary>
        public bool IsHasAccountAndLoggedIn =>
            HasAccount && IsLoggedIn;

        /// <summary>
        ///     Gets the <see cref="World.Account"/> of this <see cref="Player"/>.
        /// </summary>
        public Account Account =>
            Account.All.SingleOrDefault(account => account.Name == Name);

        /// <summary>
        ///     Gets whether this <see cref="Player"/>'s name matches the roleplay format.
        /// </summary>
        public bool IsRolePlayName =>
            Regex.IsMatch(Name, @"^([A-Z][a-z]+_[A-Z][a-z]+)$");

        /// <summary>
        ///     Gets whether this <see cref="Player"/> is paused (minimized the game).
        /// </summary>
        public bool IsPaused =>
            (_tick > 0) && (PausedTime / 1000) > 0;

        /// <summary>
        ///     Gets the amount of time (in milliseconds) that a player has been paused.
        /// </summary>
        public int PausedTime =>
            Server.GetTickCount() - _tick;

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
        ///     Logs the <see cref="Player"/> into this <see cref="Account"/>.
        /// </summary>
        /// <param name="password">The password of this <see cref="Account"/>.</param>
        /// <returns></returns>
        public bool Login(string password)
        {
            if (IsLoggedIn)
                return false;

            if (!password.Equals(Account.Password))
                return false;

            var spawn = Account.Spawn;

            SetSpawnInfo(Account.Team, Account.Skin, spawn.Position, spawn.Angle);

            Team = Account.Team;
            Skin = Account.Skin;
            Money = Account.Money;
            Score = Account.Score;
            Health = Account.Health;
            Armour = Account.Armour;
            Color = Account.Color;

            IsLoggedIn = true;

            OnLogin(new AccountEventArgs(Account));

            return true;
        }

        /// <summary>
        ///     Logs out the <see cref="Player"/> from this <see cref="Account"/>.
        /// </summary>
        public void Logout()
        {
            if (!IsLoggedIn)
                return;

            SetSpawnInfo(0, 0, new Vector3(), 0);

            Team = 0;
            Skin = 0;
            Money = 0;
            Score = 0;
            Health = 100;
            Armour = 0;
            Color = 0;

            IsLoggedIn = false;

            OnLogout(new EventArgs());
        }

        /// <summary>
        ///     Registers this <see cref="Player"/> on this server.
        /// </summary>
        /// <param name="password"></param>
        public void Register(string password)
        {
            if (HasAccount)
                return;

            Account.Create(Name, password);

            OnRegistered(new AccountEventArgs(Account));
        }

        /// <summary>
        ///     Unregisters this <see cref="Player"/> on this server.
        /// </summary>
        public void Unregister()
        {
            if (!HasAccount)
                return;

            Account.Dispose();

            OnUnregistered(new EventArgs());
        }

        /// <summary>
        ///     Shows the login dialog for this <see cref="Player"/>.
        /// </summary>
        public void ShowLoginDialog()
        {
            string[] lines =
            {
                $"{Color.White}Name: {Color.IndianRed}{Name}\n",
                $"{Color.White}Enter the password of this account:"
            };

            var dialog = new InputDialog("Login", string.Join('\n', lines), true, "Ok", "Leave");

            dialog.Response += (sender, e) =>
            {
                if (e.DialogButton == DialogButton.Right)
                {
                    Kick();
                    return;
                }

                if (!Login(e.InputText))
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
            string[] lines =
            {
                $"{Color.White}Name: {Color.IndianRed}{Name}\n",
                $"{Color.White}Enter the password for this account:"
            };
            
            var dialog = new InputDialog("Register", string.Join('\n', lines), false, "Ok", "Leave");

            dialog.Response += (sender, e) =>
            {
                string password = e.InputText;

                if (e.DialogButton == DialogButton.Right)
                {
                    Kick();
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    dialog.Show(this);
                    return;
                }

                Register(password);
                ShowLoginDialog();
            };

            dialog.Show(this);
        }

        /// <summary>
        ///     Sends a message to the global chat.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendChatMessage(string message) =>
            SendChatMessage(Color.White, message);

        /// <summary>
        ///     Sends a message to the global chat.
        /// </summary>
        /// <param name="color">The color for the message.</param>
        /// <param name="message">The message to send.</param>
        public void SendChatMessage(Color color, string message)
        {
            if (!IsHasAccountAndLoggedIn)
                return;

            float radius = 20;

            foreach (var player in All)
            {
                if (!player.IsHasAccountAndLoggedIn)
                    continue;

                if (!player.IsInRangeOfPoint(radius, Position))
                    continue;

                player.SendClientMessage(color, message);
            }
        }

        #endregion

        #region Callback Methods

        /// <summary>
        ///     Raises the <see cref="Joined"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        public void OnJoined(EventArgs e)
        {
            Joined?.Invoke(this, e);

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
        }

        /// <summary>
        ///     Raises the <see cref="Paused"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        public void OnPaused(EventArgs e)
        {
            Paused?.Invoke(this, e);

            SetChatBubble($"Pause {PausedTime / 1000}s", Color.White, 20, 1000);
        }

        /// <summary>
        ///     Raises the <see cref="Resumed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ResumeEventArgs"/> that contains the event data.</param>
        public void OnResumed(ResumeEventArgs e)
        {
            Resumed?.Invoke(this, e);

            SendClientMessage($"You've been paused for {e.Time / 1000} seconds.");
        }

        /// <summary>
        ///     Raises the <see cref="LoggedIn"/> event.
        /// </summary>
        /// <param name="e">An <see cref="AccountEventArgs"/> that contains the event data.</param>
        public void OnLogin(AccountEventArgs e)
        {
            LoggedIn?.Invoke(this, e);

            SendClientMessage("You've successfully logged in.");
        }

        /// <summary>
        ///     Raises the <see cref="LoggedOut"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        public void OnLogout(EventArgs e)
        {
            LoggedOut?.Invoke(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="Registered"/> event.
        /// </summary>
        /// <param name="e">An <see cref="AccountEventArgs"/> that contains the event data.</param>
        public void OnRegistered(AccountEventArgs e)
        {
            Registered?.Invoke(this, e);

            SendClientMessage("You've successfully registered.");
        }

        /// <summary>
        ///     Raises the <see cref="Unregistered"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        public void OnUnregistered(EventArgs e)
        {
            Unregistered?.Invoke(this, e);
        }

        #endregion

        #region Override Methods

        protected override void Initialize()
        {
            base.Initialize();

            _updateTimer = Timer.Run(30, () =>
            {
                if (IsPaused)
                    OnPaused(new EventArgs());
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _updateTimer.Dispose();
        }

        #endregion

        #region Override Callback Methods

        public override void OnText(TextEventArgs e)
        {
            base.OnText(e);

            // Disables standard chat.
            e.SendToPlayers = false;

            string errorMessage = null;

            if (!HasAccount)
                errorMessage = "You don't have an account.";
            else if (!IsLoggedIn)
                errorMessage = "You aren't logged in.";

            if (errorMessage != null)
            {
                SendClientMessage(errorMessage);
                return;
            }

            SendChatMessage($"{Account.Color}{Account.Name}{Color.White} says: {e.Text}");
        }

        public override void OnUpdate(PlayerUpdateEventArgs e)
        {
            base.OnUpdate(e);

            if (IsPaused)
                OnResumed(new ResumeEventArgs(PausedTime));

            _tick = Server.GetTickCount();
        }

        public override void OnConnected(EventArgs e)
        {
            base.OnConnected(e);

            Timer.RunOnce(Ping, () => OnJoined(e));
        }

        public override void OnDisconnected(DisconnectEventArgs e)
        {
            base.OnDisconnected(e);

            Logout();
        }

        public override void OnRequestSpawn(RequestSpawnEventArgs e)
        {
            base.OnRequestSpawn(e);

            if (!(HasAccount && IsLoggedIn))
                e.PreventSpawning = true;
        }

        #endregion
    }
}