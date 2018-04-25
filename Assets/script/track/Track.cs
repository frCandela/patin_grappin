using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Track : MonoBehaviour
{
    public Vector3 trackDirection { get; private set; }

    //Editor parameters
    [Header("Track: ")]
    [SerializeField] private List<GameObject> trackSections = new List<GameObject>();
    [SerializeField] private int currentTrack;
    [SerializeField, Range( 1,10)] private int trackCount = 5;

    //Backup variables
    private int previousCurrentTrack;
    private int previoustrackCount = 1;


    bool regenerateTrackLater = false;

    //Private members
    private List<GameObject> trackSectionsInstances = new List<GameObject>();

    private void Awake()
    {
        ReGenerateTrack();
    }

    private void Update()
    {
        if (regenerateTrackLater)
        {
            ReGenerateTrack();
            regenerateTrackLater = false;
        }
    }


    private void OnValidate()
    {
        SetCurrentTrack(currentTrack);
    }

    public void SetCurrentTrack( int value )
    {
        //Regenerate the track when needed
        value = (int)Mathf.Clamp(value, 0f, trackSections.Count - 0.1f);
        if (value != previousCurrentTrack)
        {
            currentTrack = value;
            regenerateTrackLater = true;
            previousCurrentTrack = currentTrack;
        }
    }

    private void ReGenerateTrack()
    {
        //Destroy the previous track sections 
        #if UNITY_EDITOR
        foreach (GameObject section in trackSectionsInstances)
            DestroyImmediate(section);
        #else
            foreach (GameObject section in trackSectionsInstances)
                Destroy(section);
        #endif
        trackSectionsInstances.Clear();

        //Generates the new track sections
        for ( int i = currentTrack - trackCount; i < currentTrack + trackCount; ++i)
        {
            if (i >= 0 && i < trackSections.Count)
            {
                GameObject section = Instantiate(trackSections[i]);
                trackSectionsInstances.Add(section);
                section.transform.parent = transform;
            }
        }

        //Set the new track sections positions and rotations
        foreach (GameObject section in trackSectionsInstances)
        {
            section.transform.position = Vector3.zero;
        }

    }


}
