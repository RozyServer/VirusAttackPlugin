using CommandSystem;
using Exiled.API.Features;
using System;
using Exiled.Permissions.Extensions;
using Exiled.API.Enums;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using Exiled.API.Features.Items;

namespace VirusAttackPlugin.Commands
{
    /// <summary>
    /// Command to remove virus from the system.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DAttackCommand : ICommand
    {
        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command { get; } = "dattack";

        /// <summary>
        /// Gets the command description.
        /// </summary>
        public string Description { get; } = "Removes a virus from the system.";

        /*/// <summary>
        /// Gets the command usage.
        /// </summary>
        public string Usage { get; } = "dattack";*/

        public string[] Aliases => new string[] { "deleteattack" };

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="sender">Command sender.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <returns>Command result.</returns>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);

            if (player == null)
            {
                response = "You must be a player to use this command.";
                return false;
            }
            if (VirusAttackPlugin.PlayerData.ContainsKey(player))
            {
                if (VirusAttackPlugin.PlayerData[player].IsUploading)
                {
                    float distance = Vector3.Distance(player.Position, VirusAttackPlugin.PlayerData[player].StartPosition);
                    if (distance >= 1f)
                        VirusAttackPlugin.ResetUpload(player);
                }
            }
            if (player.CurrentRoom.Type != RoomType.HczServers)
            {
                response = "You must be using the servers to perform this action.";
                return false;
            }
            if (player.CurrentItem.Type != ItemType.KeycardMTFCaptain)
            {
                response = "You do not have the proper knowledge or your position does not allow you to do this";
                return false;
            }
            if (VirusAttackPlugin.PlayerData[player].IsUploading)
            {
                response = "You are already removing a virus.";
                return false;
            }

            // Start removal process
            VirusAttackPlugin.PlayerData[player].IsUploading = true;
            VirusAttackPlugin.PlayerData[player].UploadPercentage = 0;
            VirusAttackPlugin.PlayerData[player].StartPosition = player.Position;
            player.Broadcast(10, GenerateProgressBar(0));

            Timing.RunCoroutine(RemoveCoroutine(player));
            response = "Virus removal initiated.";
            return true;
        }

        /// <summary>
        /// Coroutine to handle virus removal.
        /// </summary>
        /// <param name="player">The player removing the virus.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        private IEnumerator<float> RemoveCoroutine(Player player)
        {
            float duration = VirusAttackPlugin.Instance.Config.UploadDuration;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (!VirusAttackPlugin.PlayerData[player].IsUploading)
                    yield break;

                VirusAttackPlugin.PlayerData[player].UploadPercentage = (elapsed / duration) * 100f;
                player.ClearBroadcasts();
                player.Broadcast(1, GenerateProgressBar(VirusAttackPlugin.PlayerData[player].UploadPercentage));
                elapsed += 1f;
                yield return Timing.WaitForSeconds(1f);
            }

            // Removal complete
            VirusAttackPlugin.PlayerData[player].IsUploading = false;
            player.ClearBroadcasts();
            player.Broadcast(5, "<color=green>Virus successfully removed!</color>");
            Cassie.Message(VirusAttackPlugin.Instance.Config.CassieVirusDelete, false, true, true);

            // Trigger lights off and on coroutine
            Timing.RunCoroutine(LightsOffOnCoroutine());
        }

        /// <summary>
        /// Coroutine to turn lights off and on.
        /// </summary>
        /// <returns>IEnumerator for coroutine.</returns>
        private IEnumerator<float> LightsOffOnCoroutine()
        {
            Map.TurnOffAllLights(0.4f);
            yield return Timing.WaitForSeconds(0.5f);
            Map.TurnOffAllLights(0.4f);
        }

        /// <summary>
        /// Generates a progress bar for the broadcast.
        /// </summary>
        /// <param name="percentage">Upload percentage.</param>
        /// <returns>Progress bar string.</returns>
        private string GenerateProgressBar(float percentage)
        {
            int totalBars = 10;
            int filledBars = Mathf.FloorToInt((percentage / 100f) * totalBars);
            int emptyBars = totalBars - filledBars;

            string progressBar = "<color=green>" + new string('█', filledBars) + "</color>" +
                                 "<color=white>" + new string('█', emptyBars) + "</color>";

            return progressBar;
        }
    }
}
