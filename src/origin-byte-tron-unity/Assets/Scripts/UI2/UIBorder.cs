using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBorder : MonoBehaviour
{
    public GUIStyle buttonBorderStyle;
    public string text = "buttonText";
    public Vector2 buttonPosition = Vector2.zero;
 
    Rect rect;
 
    void Start(){
 
        rect = new Rect();
        rect.position = buttonPosition;
        rect.size = buttonBorderStyle.CalcSize(new GUIContent(text));
 
    }
 
    void OnGUI(){
 
        GUI.Button(rect,text,buttonBorderStyle);
 
    }
}
