using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Utils;

[CustomEditor(typeof(ScriptableObject))]
public class ScriptableObjectEditor : Editor
{
    private readonly Dictionary<ScriptableObject, Editor> m_configEditors = new();
    private readonly HashSet<ScriptableObject> m_staleEditorKeys = new();

    public override void OnInspectorGUI()
    {
        m_staleEditorKeys.UnionWith(m_configEditors.Keys);

        serializedObject.UpdateIfRequiredOrScript();

        SerializedProperty propertyIter = serializedObject.GetIterator();
        for (bool enterChildren = true; propertyIter.NextVisible(enterChildren); enterChildren = false)
        {
            DrawClassMember(propertyIter);
        }

        serializedObject.ApplyModifiedProperties();

        CleanupStaleEditors();
    }

    private void OnDisable()
    {
        foreach (Editor editor in m_configEditors.Values.Where(editor => editor != null))
        {
            DestroyImmediate(editor);
        }

        m_configEditors.Clear();
    }

    private void DrawClassMember(SerializedProperty serializedMemberProperty)
    {
        const string scriptPropertyName = "m_Script";
        using (new EditorGUI.DisabledScope(serializedMemberProperty.propertyPath == scriptPropertyName))
        {
            EditorGUILayout.PropertyField(serializedMemberProperty, true);
        }

        if (serializedMemberProperty.propertyType != SerializedPropertyType.ObjectReference)
        {
            return;
        }

        ScriptableObject configMember = serializedMemberProperty.objectReferenceValue as ScriptableObject;
        if (!configMember)
        {
            return;
        }

        SmartConfigAttribute smartConfigAttribute = configMember.GetType().GetCustomAttribute(typeof(SmartConfigAttribute)) as SmartConfigAttribute;
        using (new EditorGUI.DisabledScope(smartConfigAttribute is { BackupStrategy: SmartConfigAttribute.Strategy.DisableRuntimeEdits } && Application.isPlaying))
        {
            m_staleEditorKeys.Remove(configMember);
            if (!m_configEditors.TryGetValue(configMember, out Editor memberEditor) || !memberEditor)
            {
                memberEditor = CreateEditor(configMember);
                m_configEditors[configMember] = memberEditor;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (memberEditor.GetType().FullName == "UnityEditor.GenericInspector")
            {
                memberEditor.serializedObject.UpdateIfRequiredOrScript();
                DrawPropertiesExcluding(memberEditor.serializedObject, scriptPropertyName);
                memberEditor.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                // Supports custom editors for member types
                memberEditor.OnInspectorGUI();
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void CleanupStaleEditors()
    {
        foreach (ScriptableObject staleEditorKey in m_staleEditorKeys)
        {
            if (!m_configEditors.TryGetValue(staleEditorKey, out Editor staleEditor))
            {
                continue;
            }

            if (staleEditor)
            {
                DestroyImmediate(staleEditor);
            }

            m_configEditors.Remove(staleEditorKey);
        }

        m_staleEditorKeys.Clear();
    }

    [InitializeOnEnterPlayMode]
    private static void HandleEditorPlay(EnterPlayModeOptions options)
    {
        foreach (string guid in AssetDatabase.FindAssets("t:ScriptableObject"))
        {
            ScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid));
            if (scriptableObject.GetType().GetCustomAttribute(typeof(SmartConfigAttribute)) is SmartConfigAttribute)
            {
                SessionState.SetString($"sck_{scriptableObject.GetEntityId()}", EditorJsonUtility.ToJson(scriptableObject));
            }
        }

        EditorApplication.playModeStateChanged += HandleExitPlay;
    }

    private static void HandleExitPlay(PlayModeStateChange stateChange)
    {
        if (stateChange != PlayModeStateChange.ExitingPlayMode)
        {
            return;
        }

        EditorApplication.playModeStateChanged -= HandleExitPlay;

        Dictionary<EntityId, ScriptableObject> configsOnExit = Resources.FindObjectsOfTypeAll<ScriptableObject>()
            .Where(scriptableObject => scriptableObject.GetType().GetCustomAttribute(typeof(SmartConfigAttribute)) is SmartConfigAttribute)
            .ToDictionary(scriptableObject => scriptableObject.GetEntityId(), scriptableObject => scriptableObject);

        foreach ((EntityId configKey, ScriptableObject configOnExit) in configsOnExit)
        {
            string backupConfigJson = SessionState.GetString($"sck_{configKey}", string.Empty);
            string configOnExitJson = EditorJsonUtility.ToJson(configOnExit);
            if (!string.IsNullOrEmpty(backupConfigJson) && (configOnExitJson != backupConfigJson))
            {
                SmartConfigAttribute smartConfigAttribute = configOnExit.GetType().GetCustomAttribute(typeof(SmartConfigAttribute)) as SmartConfigAttribute;
                Debug.Assert(smartConfigAttribute != null);

                string originalPath = AssetDatabase.GetAssetPath(configOnExit);
                string originalName = Path.GetFileNameWithoutExtension(originalPath);
                string backupConfigName = originalName;
                string directoryPath = string.IsNullOrEmpty(smartConfigAttribute.BackupDirectory) ? Path.GetDirectoryName(originalPath) : smartConfigAttribute.BackupDirectory;

                EnsurePathExists(ref directoryPath);

                ScriptableObject backupConfig = CreateInstance(configOnExit.GetType()) as ScriptableObject;
                switch (smartConfigAttribute.BackupStrategy)
                {
                    case SmartConfigAttribute.Strategy.BackupEdits:
                        EditorJsonUtility.FromJsonOverwrite(configOnExitJson, backupConfig);
                        EditorJsonUtility.FromJsonOverwrite(backupConfigJson, configOnExit);
                        configOnExit.name = originalName;
                        backupConfigName += $"_Edited_{DateTime.Now:MMddyy.HHmmss}";
                        break;

                    case SmartConfigAttribute.Strategy.BackupOriginal:
                        EditorJsonUtility.FromJsonOverwrite(backupConfigJson, backupConfig);
                        backupConfigName += $"_Original_{DateTime.Now:MMddyy.HHmmss}";
                        break;
                }

                backupConfig.name = backupConfigName;
                AssetDatabase.CreateAsset(backupConfig, Path.Join(directoryPath, $"{backupConfigName}{Path.GetExtension(originalPath)}"));

                EditorUtility.SetDirty(configOnExit);
            }

            SessionState.EraseString($"sck_{configKey}");
        }

        AssetDatabase.SaveAssets();
    }

    private static void EnsurePathExists(ref string directoryPath)
    {
        Debug.Assert(!string.IsNullOrEmpty(directoryPath));

        const string requiredRoot = "Assets";
        if (!directoryPath.StartsWith(requiredRoot))
        {
            Debug.LogWarning($"Inserting required root '{requiredRoot}' for SmartConfig BackupDirectory path '{directoryPath}'");
            directoryPath = Path.Join(requiredRoot, directoryPath);
        }

        string root = string.Empty;
        directoryPath = directoryPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        foreach (string directory in directoryPath.Split(Path.DirectorySeparatorChar))
        {
            if (!AssetDatabase.IsValidFolder(Path.Combine(root, directory)))
            {
                AssetDatabase.CreateFolder(root, directory);
            }

            root = Path.Combine(root, directory);
        }
    }
}