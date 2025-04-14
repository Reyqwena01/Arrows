using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;

# if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneInBuildAttribute))]
public class ScenePropertyDrawer : PropertyDrawer
{
    private const string SCENE_LIST_ITEM = "{0} ({1})"; // name (index)

    // Warning message related consts :
    private const string TYPE_WARNING_MESSAGE = "{0} must be an int or a string";
    private const string BUILD_SETTINGS_WARNING_MESSAGE = "No scenes in the build settings";

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {

        EditorGUI.BeginProperty(rect, label, property);

        string[] scenes = GetScenes();
        bool anySceneInBuildSettings = GetScenes().Length > 0;

        if (!anySceneInBuildSettings) {
            DrawDefaultPropertyAndHelpBox(property, BUILD_SETTINGS_WARNING_MESSAGE, MessageType.Warning);
            return;
        }

        string[] sceneOptions = GetSceneOptions(scenes);
        switch (property.propertyType) {
            case SerializedPropertyType.String:
                DrawPropertyForString(rect, property, label, scenes, sceneOptions);
                break;
            case SerializedPropertyType.Integer:
                DrawPropertyForInt(rect, property, label, sceneOptions);
                break;
            default:
                string message = string.Format(TYPE_WARNING_MESSAGE, property.name);
                DrawDefaultPropertyAndHelpBox(property, message, MessageType.Warning);
                break;
        }

        EditorGUI.EndProperty();
    }

    #region ScenesInBuild
    private string[] GetScenes() {

        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .Select(path => Path.GetFileNameWithoutExtension(path))
            .ToArray();
    }

    private string[] GetSceneOptions(string[] scenes) {
        return scenes.Select((s, i) => string.Format(SCENE_LIST_ITEM, s, i)).ToArray();
    }
    #endregion ScenesInBuild

    private static void DrawPropertyForString(Rect rect, SerializedProperty property, GUIContent label, string[] scenes, string[] sceneOptions) {
        int index = IndexOf(scenes, property.stringValue);
        int newIndex = EditorGUI.Popup(rect, label.text, index, sceneOptions);
        string newScene = scenes[newIndex];

        if (!property.stringValue.Equals(newScene, StringComparison.Ordinal)) {
            property.stringValue = scenes[newIndex];
        }
    }

    private static void DrawPropertyForInt(Rect rect, SerializedProperty property, GUIContent label, string[] sceneOptions) {
        int index = property.intValue;
        int newIndex = EditorGUI.Popup(rect, label.text, index, sceneOptions);

        if (property.intValue != newIndex) {
            property.intValue = newIndex;
        }
    }

    #region EditorDraw
    public void DrawDefaultPropertyAndHelpBox(SerializedProperty property, string message, MessageType messageType) {
        HelpBox(message, MessageType.Warning);

        EditorGUILayout.PropertyField(property, true);
    }

    public static void HelpBox(string message, MessageType type) {
        EditorGUILayout.HelpBox(message, type);
    }
    #endregion EditorDraw

    private static int IndexOf(string[] scenes, string scene) {
        int index = Array.IndexOf(scenes, scene);
        return Mathf.Clamp(index, 0, scenes.Length - 1);
    }
}
#endif