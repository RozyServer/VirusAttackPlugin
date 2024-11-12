// Plugin.cs
using Exiled.API.Features;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using Player = Exiled.API.Features.Player;
using PlayerInfo = VirusAttackPlugin.PlayerInfo;
/// <summary>
/// Main plugin class.
/// </summary>
namespace VirusAttackPlugin
{
    /// <summary>
    /// Main plugin class inheriting from Plugin base class.
    /// </summary>
    public class VirusAttackPlugin : Plugin<Config>
    {
        /// <summary>
        /// Gets the plugin instance.
        /// </summary>
        public static VirusAttackPlugin Instance { get; private set; }

        /// <summary>
        /// Gets the plugin name.
        /// </summary>
        public override string Name => "VirusAttackPlugin";

        /// <summary>
        /// Gets the plugin prefix.
        /// </summary>
        public override string Prefix => "VAP";

        /// <summary>
        /// Gets the plugin version.
        /// </summary>
        public override Version Version => new Version(1, 0, 0);

        /// <summary>
        /// Gets the required Exiled version.
        /// </summary>
        public override Version RequiredExiledVersion => new Version(5, 2, 0);

        /// <summary>
        /// Plugin loaded event.
        /// </summary>
        public override void OnEnabled()
        {
            Instance = this;
            RegisterEvents();
            base.OnEnabled();
        }

        /// <summary>
        /// Plugin disabled event.
        /// </summary>
        public override void OnDisabled()
        {
            UnregisterEvents();
            Instance = null;
            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            Exiled.Events.Handlers.Player.Handcuffing += OnPlayerHandcuffing;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
        }

        private void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Verified -= OnPlayerVerified;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            Exiled.Events.Handlers.Player.Handcuffing -= OnPlayerHandcuffing;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
        }

        private void OnPlayerVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            // Initialize player data.
            if (!PlayerData.ContainsKey(ev.Player))
                PlayerData.Add(ev.Player, new PlayerInfo());
        }

        private void OnPlayerLeft(Exiled.Events.EventArgs.Player.LeftEventArgs ev)
        {
            // Clean up player data.
            if (PlayerData.ContainsKey(ev.Player))
                PlayerData.Remove(ev.Player);
        }

        private void OnPlayerDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            // Reset if player dies.
            if (PlayerData.ContainsKey(ev.Player))
            {
                if (PlayerData[ev.Player].IsUploading)
                    ResetUpload(ev.Player);
            }
        }

        private void OnPlayerHandcuffing(Exiled.Events.EventArgs.Player.HandcuffingEventArgs ev)
        {
            // Reset if player is cuffed.
            if (PlayerData.ContainsKey(ev.Target))
            {
                if (PlayerData[ev.Target].IsUploading)
                    ResetUpload(ev.Target);
            }
        }
        /// <summary>
        /// Dictionary to hold player data.
        /// </summary>
        public static Dictionary<Player, PlayerInfo> PlayerData = new Dictionary<Player, PlayerInfo>();

        /// <summary>
        /// Resets the upload process for a player.
        /// </summary>
        /// <param name="player">The player to reset.</param>
        public static void ResetUpload(Exiled.API.Features.Player player)
        {
            PlayerData[player].IsUploading = false;
            PlayerData[player].UploadPercentage = 0;
            player.ClearBroadcasts();
            player.Broadcast(5, $"<color=red>Upload interrupted!</color>");
        }
    }
}
