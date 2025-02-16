using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public class LevelEditorCustomButton
{
    private static readonly string assetPath = "Assets/Data/Level Editor Window.asset";

    static LevelEditorCustomButton()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }

    private static void OnToolbarGUI()
    {
        GUILayout.Space(100);
        if (GUILayout.Button("Level Editor" , GUILayout.Width(120)))
        {
            HighlightAssetByPath(assetPath);
            
            string currentScenePath = EditorSceneManager.GetActiveScene().path;

            if (currentScenePath != "Assets/Scenes/Level Editor Scene.unity")
            {
                if (EditorApplication.isPlaying)
                {
                    SceneManager.LoadSceneAsync(1);
                }
                else
                {
                    EditorSceneManager.OpenScene("Assets/Scenes/Level Editor Scene.unity", OpenSceneMode.Single);
                }
            }
            else
            {
                if (EditorApplication.isPlaying)
                {
                    return;
                }
            }
            
            EditorApplication.EnterPlaymode();
        }
    }

    private static void HighlightAssetByPath(string path)
    {
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);

        if (asset != null)
        {
            EditorGUIUtility.PingObject(asset);
            Selection.activeObject = asset;
            Debug.Log($"Highlighted Asset: {asset.name}");
        }
        else
        {
            Debug.LogWarning($"Asset not found at path: {path}");
        }
    }
}