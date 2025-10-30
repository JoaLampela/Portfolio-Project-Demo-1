using System.IO;
using UnityEngine;

public static class JoaFilePaths
{
    private static string BasePath => Application.persistentDataPath;

    public static class SaveFile
    {
        private const string DEFAULT_PATH = "DefaultSavePath";
        public static string Default => Path.Combine(BasePath, DEFAULT_PATH);
    }
    
    public static class Settings
    {
        private const string DEFAULT_PATH = "DefaultSettingsPath";
        public static string Default => Path.Combine(BasePath, DEFAULT_PATH);
    }
    
    public static class Bindings
    {
        private const string DEFAULT_PATH = "DefaultBindingsPath";
        public static string Default => Path.Combine(BasePath, DEFAULT_PATH);
    }
}
