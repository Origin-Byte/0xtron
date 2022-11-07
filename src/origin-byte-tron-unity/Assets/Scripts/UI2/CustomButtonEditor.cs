using UnityEngine;
using UnityEditor;
 
[CustomEditor(typeof(CustomButton))]
public class CustomButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // CustomButton targetButton = (CustomButton)target;
        //
        // targetButton.audioSource =
        //     EditorGUILayout.ObjectField("AudioSource", targetButton.audioSource, typeof(AudioSource), true) as AudioSource;
        //
        // targetButton.clickSound =
        //     EditorGUILayout.ObjectField("Click sound", targetButton.clickSound, typeof(AudioClip), true) as AudioClip;
        //
        // targetButton.hoverSound =
        //     EditorGUILayout.ObjectField("Hover sound", targetButton.hoverSound, typeof(AudioClip), true) as AudioClip;
        //
        // targetButton.defaultTextColor =
        //     EditorGUILayout.ColorField("Default Text Color", targetButton.defaultTextColor);
        //
        // targetButton.hoveredTextColor =
        //     EditorGUILayout.ColorField("Hovered Text Color", targetButton.hoveredTextColor);

        
        // Show default inspector property editor
        DrawDefaultInspector();
    }
}
