using Exiled.API.Interfaces;
using System.ComponentModel;

namespace VirusAttackPlugin
{
    public class Config : IConfig
    {
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;
        [Description("Is the debug enabled?")]
        public bool Debug { get; set; } = false;
        [Description("Duration of the upload in seconds.")]
        public int UploadDuration { get; } = 35;
        [Description("Cassie successfully deleted the virus")]
        public string CassieVirusDelete { get; } = "VIRUS DELETED COMPLETE";
        [Description("Cassie successfully upload the virus")]
        public string CassieVirusUpload { get; } = "VIRUS UPLOAD COMPLETE";
    }
}
