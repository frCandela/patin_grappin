#if UNITY_EDITOR

/*using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Track))]
public class TrackEditor : Editor
{
    private Track track;


    int selectedTrack = 0;

    private GUIStyle nodeButtonStyle, directionButtonStyle;

    private void OnEnable()
    {
        track = (Track)target;
    }

   

    void OnSceneGUI()
    {
       
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        // hint
        EditorGUILayout.HelpBox("Hold Alt and drag a node to create a new one.", MessageType.Info);

        EditorGUILayout.Slider(selectedTrack, 0, track.GetTrackSectionsCount());

    }
}*/

#endif