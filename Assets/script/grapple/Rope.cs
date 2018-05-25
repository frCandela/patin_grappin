using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{

    private Transform beginRope;
    private Transform endRope;
    private Animator m_animator;


    Vector3 m_baseScale;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_baseScale = transform.localScale;
    }

    public void SetRope(Transform begin, Transform end)
    {
        beginRope = begin;
        endRope = end;
        UpdateRopeTransform();
        gameObject.SetActive(true);
        m_animator.Play("grappin_rope_anim");
    }

    public void ResetRope()
    {
        m_animator.Play("grappin_rope_base");
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (beginRope && endRope)
            UpdateRopeTransform();
    }

    private void UpdateRopeTransform()
    {
        Vector3 begin = beginRope.position;
        Vector3 end = endRope.position;

        float distance = (end - begin).magnitude;

        transform.position = begin + (end - begin) / 2f;

        Quaternion correction = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
        transform.rotation = Quaternion.LookRotation(begin - end) * correction;

        transform.localScale = new Vector3(5f * m_baseScale.x, 50 * distance, 5f * m_baseScale.z);
    }
}
