using System;

namespace Utils
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SmartConfigAttribute : Attribute
    {
        public string BackupDirectory { get; private set; }
        public Strategy BackupStrategy { get; private set; }

        public enum Strategy
        {
            BackupOriginal,
            BackupEdits,
            DisableRuntimeEdits,
        };
    
        public SmartConfigAttribute(string backupDirectory = "", Strategy backupStrategy = Strategy.BackupOriginal)
        {
            BackupDirectory = backupDirectory;
            BackupStrategy = backupStrategy;
        }
    }
}