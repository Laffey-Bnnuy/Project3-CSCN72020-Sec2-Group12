using System;
using System.IO;

namespace VehicleControlAndMonitorApp.Utilities
{
    public static class PathResolver
    {
        public static string GetDynamicProjectDataPath(string projectFolderName, params string[] subPaths)
        {
            string currentPath = AppContext.BaseDirectory;

            while (currentPath != null)
            {
                string targetPath = Path.Combine(currentPath, projectFolderName);

                if (Directory.Exists(targetPath))
                {
                    return Path.Combine(targetPath, Path.Combine(subPaths));
                }

                currentPath = Directory.GetParent(currentPath)?.FullName;
            }

            throw new DirectoryNotFoundException(
                $"Could not locate the folder '{projectFolderName}' in parent directories.");
        }
    }
}
