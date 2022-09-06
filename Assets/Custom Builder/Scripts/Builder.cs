#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Builder
{

    [CreateAssetMenu(fileName = "Builder", menuName = "Utilities/Builder", order = 1)]

    public class Builder : ScriptableObject
    {

        public string ProjectName = default;
        public BaseBuildConfiguration[] ReleaseBuilds = default;
        public BaseBuildConfiguration[] MajorBuilds = default;
        public BaseBuildConfiguration[] MinorBuilds = default;

        [HideInInspector] public bool ManualVersionOverride = false;
        [HideInInspector][SerializeField] private int releaseVersion = 0;
        [HideInInspector][SerializeField] private int majorPatchVersion = 1;
        [HideInInspector][SerializeField] private int minorPatchVersion = 0;
        [HideInInspector][SerializeField] private int bundleVersion = 1;

        [HideInInspector] public bool useGlobalDefineSymbols = default;
        [HideInInspector][SerializeField] private string[] globalDefineSymbols = default;

        static List<string> buildLogs = new List<string>();

        public int ReleaseVersion => releaseVersion;
        public int MajorPatchVersion => majorPatchVersion;
        public int MinorPatchVersion => minorPatchVersion;
        public int BundleVersion => bundleVersion;

        public bool Building { get; private set; }

        public string GetReleaseFolderName() => ProjectName + " " + (releaseVersion + 1).ToString() + ".0.0";
        public string GetMajorFolderName() => ProjectName + " " + releaseVersion + "." + (majorPatchVersion + 1).ToString() + ".0";
        public string GetMinorFolderName() => ProjectName + " " + releaseVersion + "." + majorPatchVersion + "." + (minorPatchVersion + 1).ToString();

        public void BuildAllReleaseBuilds(string[] levels, string path) 
        {
            BuildTarget currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;

            Building = true;

            releaseVersion++;
            minorPatchVersion = 0;
            majorPatchVersion = 0;
            PlayerSettings.bundleVersion = releaseVersion + "." + majorPatchVersion + "." + minorPatchVersion;

            foreach (var config in ReleaseBuilds)
            {
                if(config) BuildConfigurationTask(config, levels, path, ProjectName);
            }

            bundleVersion = PlayerSettings.Android.bundleVersionCode;
            Building = false;

            if( EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(currentBuildTarget), currentBuildTarget))
            {
                Debug.ClearDeveloperConsole();

                foreach (var log in buildLogs)
                {
                    Debug.Log(log);
                }

                buildLogs.Clear();
            }
        }

        public void BuildAllMajorBuilds(string[] levels, string path)
        {
            BuildTarget currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;

            Building = true;

            majorPatchVersion++;
            minorPatchVersion = 0;
            PlayerSettings.bundleVersion = releaseVersion + "." + majorPatchVersion + "." + minorPatchVersion;

            foreach (var config in MajorBuilds)
            {
                if (config) BuildConfigurationTask(config, levels, path, ProjectName);
            }

            bundleVersion = PlayerSettings.Android.bundleVersionCode;
            Building = false;

            if (EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(currentBuildTarget), currentBuildTarget))
            {
                Debug.ClearDeveloperConsole();

                foreach (var log in buildLogs)
                {
                    Debug.Log(log);
                }

                buildLogs.Clear();
            }
        }

        public void BuildAllMinorBuilds(string[] levels, string path)
        {
            BuildTarget currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;

            Building = true;

            minorPatchVersion++;
            PlayerSettings.bundleVersion = releaseVersion + "." + majorPatchVersion + "." + minorPatchVersion;

            foreach (var config in MinorBuilds)
            {
                if (config) BuildConfigurationTask(config, levels, path, ProjectName);
            }

            bundleVersion = PlayerSettings.Android.bundleVersionCode;
            Building = false;

            if (EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(currentBuildTarget), currentBuildTarget))
            {
                Debug.ClearDeveloperConsole();

                foreach (var log in buildLogs)
                {
                    Debug.Log(log);
                }

                buildLogs.Clear();
            }
        }

        private void BuildConfigurationTask(BaseBuildConfiguration config, string[] levels, string path, string gameName) 
        {
            if (useGlobalDefineSymbols)
                EnableSymbols();
            else
                DisableSymbols();

            bool preparedToBuild = EditorUserBuildSettings.activeBuildTarget == config.GetTarget();

            if (!preparedToBuild) 
            {
                preparedToBuild = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(config.GetTarget()), config.GetTarget());
            }

            if (preparedToBuild) 
            {
                config.SetBuildConfigurations();

                string folderPath = path + "/" + config.GetFolderName();

                BuildGame(levels, folderPath, gameName + config.GetExtension(), config.GetTarget(), config.buildOptions);
            }
        }

        private static void BuildGame(string[] levels, string path, string gameName, BuildTarget target, BuildOptions options)
        {
            UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(levels, path + "/" + gameName, target, options);

            switch (report.summary.result)
            {
                case UnityEditor.Build.Reporting.BuildResult.Unknown:
                    buildLogs.Add($"Platform: {report.summary.platform}, Unknown... <color=red>Errors: {report.summary.totalErrors}</color>, <color=yellow>Warnings: {report.summary.totalWarnings}</color>");
                    break;
                case UnityEditor.Build.Reporting.BuildResult.Succeeded:
                    buildLogs.Add($"<color=green>Platform: {report.summary.platform}, Build size: {report.summary.totalSize} bytes</color>");
                    break;
                case UnityEditor.Build.Reporting.BuildResult.Failed:
                    buildLogs.Add($"<color=red>Platform: {report.summary.platform}, Errors: {report.summary.totalErrors}</color>");
                    break;
                case UnityEditor.Build.Reporting.BuildResult.Cancelled:
                    buildLogs.Add($"Platform: {report.summary.platform}, Canceled!");
                    break;
            }
        }

        private List<string> GetCurrentDefines()
        {
            List<string> currentDefines;

            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            currentDefines = defines.Split(';').ToList();

            return currentDefines;
        }

        private void SetCurrentDefines(List<string> currentDefines)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = string.Join(";", currentDefines);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
        }

        public void EnableSymbols()
        {
            List<string> currentDefines = GetCurrentDefines();

            foreach (string define in globalDefineSymbols)
            {
                if (!currentDefines.Contains(define))
                    currentDefines.Add(define);
            }

            SetCurrentDefines(currentDefines);
        }

        public void DisableSymbols()
        {
            List<string> currentDefines = GetCurrentDefines();

            foreach (string define in globalDefineSymbols)
            {
                if (currentDefines.Contains(define))
                    currentDefines.Remove(define);
            }

            SetCurrentDefines(currentDefines);
        }
    }
}

#endif