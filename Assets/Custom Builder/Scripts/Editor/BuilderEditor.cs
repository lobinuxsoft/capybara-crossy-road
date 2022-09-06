#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Utilities.Builder
{
    [CustomEditor(typeof(Builder))]
    public class BuilderEditor : Editor
    {

        private Builder builder = default;

        private void OnEnable() => builder = (Builder)target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (builder.Building) return;

            GUILayout.Space(20);

            builder.useGlobalDefineSymbols = EditorGUILayout.ToggleLeft("Use global define symbols?", builder.useGlobalDefineSymbols);

            if (builder.useGlobalDefineSymbols)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("globalDefineSymbols"));
                serializedObject.ApplyModifiedProperties();

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Enable Define Symbols"))
                {
                    builder.EnableSymbols();
                }

                if (GUILayout.Button("Disable Define Symbols"))
                {
                    builder.DisableSymbols();
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(20);
            EditorGUILayout.HelpBox($"Current Build Version: {PlayerSettings.bundleVersion}", MessageType.Info);
            builder.ManualVersionOverride = EditorGUILayout.ToggleLeft("Manual version override?", builder.ManualVersionOverride);

            if (builder.ManualVersionOverride) 
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("releaseVersion"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("majorPatchVersion"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minorPatchVersion"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("bundleVersion"));
                serializedObject.ApplyModifiedProperties();
            }
            else 
            {
                GUILayout.Label("Current Version: " + builder.ReleaseVersion + "." + builder.MajorPatchVersion + "." + builder.MinorPatchVersion);
                GUILayout.Label("Current Bundle Version: " + builder.BundleVersion);
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Build Release Version"))
            {

                var path = EditorUtility.SaveFilePanel("Builds Locations", "", builder.GetReleaseFolderName(), "");

                if (path.Length == 0) return;

                builder.BuildAllReleaseBuilds(GetAllCurrentScenes(), path);

            }

            GUILayout.Space(10);

            if (GUILayout.Button("Build Major Version"))
            {

                var path = EditorUtility.SaveFilePanel("Builds Locations", "", builder.GetMajorFolderName(), "");

                if (path.Length == 0) return;

                builder.BuildAllMajorBuilds(GetAllCurrentScenes(), path);

            }

            GUILayout.Space(10);

            if (GUILayout.Button("Build Minor Version"))
            {

                var path = EditorUtility.SaveFilePanel("Builds Locations", "", builder.GetMinorFolderName(), "");

                if (path.Length == 0) return;

                builder.BuildAllMinorBuilds(GetAllCurrentScenes(), path);
            }
        }

        private string[] GetAllCurrentScenes() 
        {
            List<string> scenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                    scenes.Add(scene.path);
            }
            return scenes.ToArray();
        }
    }
}
#endif