using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private GameObject beginRope;
    [SerializeField] private GameObject endRope;

    public void SetRope(GameObject begin, GameObject end)
    {
        beginRope = begin;
        endRope = end;
    }

    private void OnDisable()
    {
        transform.localScale = new Vector3(transform.localScale.x , transform.localScale.y ,0) ;
    }

    // Update is called once per frame
    void Update()
    {
        if(beginRope && endRope)
        {
            Vector3 begin = beginRope.transform.position;
            Vector3 end = endRope.transform.position;
            Vector3 scale = transform.localScale;
            float distance = (end - begin).magnitude;

            transform.position = begin + (end - begin) / 2f;
            transform.rotation = Quaternion.LookRotation(end - begin);
            transform.localScale = new Vector3(scale.x, scale.y, distance);
        }
    }
}
