using UnityEngine;

namespace VirusAttackPlugin
{
    /// <summary>
    /// Class to store player-specific information.
    /// </summary>
    public class PlayerInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether the player is currently uploading.
        /// </summary>
        public bool IsUploading { get; set; } = false;

        /// <summary>
        /// Gets or sets the upload percentage.
        /// </summary>
        public float UploadPercentage { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the start position of the player when uploading.
        /// </summary>
        public Vector3 StartPosition { get; set; }
    }
}
