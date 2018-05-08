using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Track : MonoBehaviour
{
    //Editor parameters

    [Header("Track parameters: ")]
    [SerializeField, Tooltip("If false use camera position instead")] private bool usePlayerPosition = true;
    [SerializeField] private int maxIntersections = 3;
    [SerializeField] private int currentTrackIndex;

    [Header("Track Section: ")]
    [SerializeField] private List<TrackSection> trackSections = new List<TrackSection>();


    //Private members
    Transform targetTransform = null;

    //Debug
    float splineUpdateMs = 0f;

    public TrackSection trackSection
    {
        get { return trackSections[currentTrackIndex]; }
        set { }
    }

    private void Awake()
    {
        Util.EditorAssert(trackSections.Count != 0, "Track.Awake: no track set"); 
    }

    private void Start()
    {
        if (trackSections.Count > 0)
            trackSections[currentTrackIndex].UpdateTrack(Camera.main.transform.position);

        SetupTarget();
    }

    private void OnValidate()
    {
        SetupTarget();
    }

    private void Update()
    {
        //Use the closest track section
        float t1 = Time.realtimeSinceStartup;
        currentTrackIndex = GetClosestTrackIndex();
        splineUpdateMs = 1000 * (Time.realtimeSinceStartup - t1);


        //if end track reached go to the next track
        if (trackSections[currentTrackIndex].endTrackReached)
            currentTrackIndex = (currentTrackIndex + 1) % trackSections.Count;

        //Update track
        if (trackSections.Count > 0)
             trackSections[currentTrackIndex].UpdateTrack(targetTransform.position);
    }

    private void SetupTarget()
    {
        if (usePlayerPosition)
            targetTransform = FindObjectOfType<PlayerController>().transform;
        else
            targetTransform = Camera.main.transform;
    }

    public int GetClosestTrackIndex()
    {
        int bestTrackIndex = currentTrackIndex;
        float bestDistance = float.MaxValue;

        //Iterates throught all next tracks to find the closest one
        for ( int i = currentTrackIndex - maxIntersections; i < currentTrackIndex + maxIntersections; ++i)
        {
            //Get looped index and update the corresponding track section
            int loopedIndex = (i + trackSections.Count) % trackSections.Count;
            trackSections[loopedIndex].UpdateTrack(targetTransform.position);

            //Test if we have a closer track position
            float dist = Vector3.SqrMagnitude(targetTransform.position - trackSections[loopedIndex].trackPosition);
            if (dist < bestDistance)
            {
                bestDistance = dist;
                bestTrackIndex = loopedIndex;
            }
        }

        return bestTrackIndex;

        //print(currentTrackIndex + " " + bestTrackIndex);
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(trackSections[currentTrackIndex].trackPosition, 4f);
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;

        GUI.Label(new Rect(0, 0, 100, 10), "Spline update ms: " + splineUpdateMs, style);

    }
}
