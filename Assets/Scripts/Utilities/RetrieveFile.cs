using System.IO;
using UnityEngine;

namespace Utilities
{
    public class RetrieveFile : MonoBehaviour
    {
        public static string[] GetFile(string pathFolder, string fileName)
        {
            var dataPath = Application.dataPath;

            var partialPath = Path.Combine(dataPath, pathFolder);
            var path = Path.Combine(partialPath, fileName);
            
            if (!File.Exists(path))
            {
                Debug.LogError("Invalid path");
                return null;
            }

            var lines = File.ReadAllLines(path);
            return lines;
        }
    }
}
