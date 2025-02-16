using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomEditor(typeof(ScriptableObject), true)]
[CanEditMultipleObjects]
public class ButtonAttributeScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        foreach (var t in targets)
        {
            DrawButtonsForTarget(t);
        }
    }

    private void DrawButtonsForTarget(Object targetObject)
    {
        MethodInfo[] methods = targetObject.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (MethodInfo method in methods)
        {
            var buttonAttribute = (ButtonAttribute)System.Attribute.GetCustomAttribute(method, typeof(ButtonAttribute));
            if (buttonAttribute != null)
            {
                string buttonLabel = string.IsNullOrEmpty(buttonAttribute.Label) ? method.Name : buttonAttribute.Label;

                if (GUILayout.Button(buttonLabel))
                {
                    method.Invoke(targetObject, null);
                }
            }
        }
    }
}