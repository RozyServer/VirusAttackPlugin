// Commands/VAttackCommand.cs
using CommandSystem;
using Exiled.API.Features;
using System;
using Exiled.Permissions.Extensions;
using Exiled.API.Enums;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace VirusAttackPlugin.Commands
{
    /// <summary>
    /// Command to initiate virus upload.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public class VAttackCommand : ICommand
    {
        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command { get; } = "vattack";

        /// <summary>
        /// Gets the command description.
        /// </summary>
        public string Description { get; } = "Uploads a virus to the system.";
        public string[] Aliases => new string[] { "virusattack" };

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="sender">The command sender.</param>
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
            if (player.CurrentItem.Type != ItemType.KeycardChaosInsurgency)
            {
                response = "You do not have the proper knowledge or your position does not allow you to do this";
                return false;
            }
            if (VirusAttackPlugin.PlayerData[player].IsUploading)
            {
                response = "You are already uploading a virus.";
                return false;
            }

            // Start upload process

            VirusAttackPlugin.PlayerData[player].IsUploading = true;
            VirusAttackPlugin.PlayerData[player].UploadPercentage = 0;
            VirusAttackPlugin.PlayerData[player].StartPosition = player.Position;
            int sec = VirusAttackPlugin.Instance.Config.UploadDuration;
            player.Broadcast(10, GenerateProgressBar(0));

            Timing.RunCoroutine(UploadCoroutine(player));
            response = "Virus upload initiated.";
            return true;
        }

        /// <summary>
        /// Upload coroutine to handle the upload process.
        /// </summary>
        /// <param name="player">The player uploading the virus.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        private IEnumerator<float> UploadCoroutine(Player player)
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

            // Upload complete
            VirusAttackPlugin.PlayerData[player].IsUploading = false;
            player.ClearBroadcasts();
            player.Broadcast(5, "<color=green>Virus successfully uploaded!</color>");
            Cassie.Message(VirusAttackPlugin.Instance.Config.CassieVirusUpload, false, true, true);

            // Trigger lights off and on coroutine
            Timing.RunCoroutine(LightsOffOnCoroutine());
        }

        /// <summary>
        /// Coroutine to turn lights off and on.
        /// </summary>
        /// <returns>IEnumerator for coroutine.</returns>
        private IEnumerator<float> LightsOffOnCoroutine()
        {
                Map.TurnOffAllLights(0.2f);
                yield return Timing.WaitForSeconds(0.4f);
                Map.TurnOffAllLights(0.5f);
                yield return Timing.WaitForSeconds(0.6f);
                Map.TurnOffAllLights(0.5f);
                yield return Timing.WaitForSeconds(0.6f);
                Map.TurnOffAllLights(0.5f);
                Map.TurnOffAllLights(0.5f);
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