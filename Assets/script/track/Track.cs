using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Track : MonoBehaviour
{




    //Editor parameters
    [Header("Track: ")]
    [SerializeField] private List<TrackSection> trackSections = new List<TrackSection>();
    [SerializeField] private int currentTrackIndex;
    [SerializeField, Range( 1,10)] private int trackLenght = 3;
    [SerializeField] private float cameraAnticipation = 3;

    private void Awake()
    {
        Util.EditorAssert(trackSections.Count != 0, "Track.Awake: no track set");
    }



    private void Start()
    {
        if (trackSections.Count > 0)
            trackSections[currentTrackIndex].UpdateTrack(Camera.main.transform.position + cameraAnticipation * Camera.main.transform.forward);
    }

    private void Update()
    {
        if (trackSections.Count > 0)
             trackSections[currentTrackIndex].UpdateTrack(Camera.main.transform.position + cameraAnticipation * Camera.main.transform.forward );
    }

    private void OnValidate()
    {
        //SetCurrentTrack(currentTrackIndex);
    }

    public TrackSection GetCurrentTrackSection()
    {
        return trackSections[currentTrackIndex];
    }

    public void SetCurrentTrack( int value )
    {
        //Regenerate the track when needed
        value = (int)Mathf.Clamp(value, 0f, trackSections.Count - 0.1f);
        currentTrackIndex = value;
        for( int i = 0; i < trackSections.Count; ++i)
        {
            if (i == currentTrackIndex)
                trackSections[i].gameObject.SetActive(true);
            else
                trackSections[i].gameObject.SetActive(false);
        }
    }
}
