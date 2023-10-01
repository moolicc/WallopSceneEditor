using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;

namespace WallopEdit.SceneManagement
{
    public static class StoredSceneProtocol
    {
        private const string VERSIONS_ENTRY = "_versions";
        private const string NAME_ENTRY = "_name";
        private const string SCREENSHOT_ENTRY = "_thumb";

        public static StoredScene Load(ZipArchive zip)
        {
            StoredScene result = new StoredScene();

            Version[] versions = Parts.GetVersions(zip);
            string name = Parts.GetName(zip);
            string? thumb = Parts.GetThumbnailFile(zip);

            result.EditorVersion = versions[0];
            result.EngineVersion = versions[1];
            result.SceneName = name;
            result.ScreenShotFile = thumb;

            return result;
        }

        private static string ReadZipEntry(ZipArchive zip, string entryName)
        {
            ZipArchiveEntry entry = zip.GetEntry(entryName).Unwrap(() => new Exception());
            using (StreamReader reader = new StreamReader(entry.Open()))
            {
                return reader.ReadToEnd();
            }
        }

        private static IEnumerable<string> ReadZipEntryLines(ZipArchive zip, string entryName)
        {
            ZipArchiveEntry entry = zip.GetEntry(entryName).Unwrap(() => new Exception());
            using (StreamReader reader = new StreamReader(entry.Open()))
            {
                string? line = reader.ReadLine();
                while(line != null)
                {
                    yield return line;
                    line = reader.ReadLine();
                }
            }
        }

        private static string ExtractZipEntry(ZipArchive zip, string entryName)
        {
            ZipArchiveEntry entry = zip.GetEntry(entryName).Unwrap(() => new Exception());

            string tempPath = Path.GetTempPath();
            tempPath = Path.Combine(tempPath, entryName + DateTime.Now.Ticks.ToString());
            entry.ExtractToFile(tempPath, true);

            return tempPath;
        }

        public static class Parts
        {
            public static Version[] GetVersions(ZipArchive zip)
            {
                var versionLines = ReadZipEntryLines(zip, VERSIONS_ENTRY);
                string appVersion = versionLines.First();
                string engineVersion = versionLines.Last();
                Version editorVer = Version.Parse(appVersion);
                Version engineVer = Version.Parse(engineVersion);

                return new Version[2] { editorVer, engineVer };
            }

            public static string GetName(ZipArchive zip)
            {
                return ReadZipEntry(zip, NAME_ENTRY);
            }

            public static string? GetThumbnailFile(ZipArchive zip)
            {
                if (zip.GetEntry(SCREENSHOT_ENTRY) != null)
                {
                    return ExtractZipEntry(zip, SCREENSHOT_ENTRY);
                }
                return null;
            }
        }
    }
}
