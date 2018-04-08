using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script controlling a 3rd person camera following a GameObject
/// </summary>
[RequireComponent(typeof(Camera))]
public class PlayerCameraController : MonoBehaviour
{
    [Header("Linked Instances:")]
    [SerializeField] private GameObject playerGameObject = null;

    [Header("Mouse properties")]
    [SerializeField] private float sensitivityX = 15F;
    [SerializeField] private float sensitivityY = 15F;

    [Header("AnglesLock")]
    [SerializeField] private float minimumY = -80F;
    [SerializeField] private float maximumY = 30;

    [Header("Scrolling")]
    [SerializeField] private float highScrollSpeed = 0.2f;
    [SerializeField] private float lowScrollSpeed = 0.05f;  
    [SerializeField] private float minScrollDistance = 0.9f;

    //Private scrolling paremeters
    private float maxScrollDistance = 2f;
    private float scrollSpeed;
    private float currentScroll;
    private float scrollTarget;
    private float userScrollTarget;
    private const float scrollDelta = 0.2f;

    //Camera 
    private Vector3 previousTranslation;

    private void Awake()
    {
        Util.EditorAssert(playerGameObject != null, "PlayerCameraController.Awake(): playerGameObject not set");

        //Set variables
        previousTranslation = transform.position - playerGameObject.transform.position;

        //Scroll
        currentScroll = previousTranslation.magnitude;
        scrollTarget = currentScroll;
        userScrollTarget = currentScroll;
        scrollSpeed = lowScrollSpeed;

        maxScrollDistance = previousTranslation.magnitude;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        //Lock / unlock the cursor when pressing escape
        if( Input.GetButtonDown("Cancel"))
        {
            if(Cursor.visible)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if( ! Cursor.visible )
            SetCamera();
    }

    /// <summary>
    /// Rotates the camera with the mouse input
    /// </summary>
    private void SetCamera()
    {
        //Input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        userScrollTarget = Mathf.Clamp(userScrollTarget - scrollDelta * Input.mouseScrollDelta.y, minScrollDistance, maxScrollDistance);

        //usefull Data
        Vector3 dir = 
            transform.position - 
            playerGameObject.transform.position;
        Vector3 hDir = new Vector3(dir.x, 0, dir.z);
        Vector3 rightDir = Vector3.Cross(Vector3.up, transform.position - playerGameObject.transform.position).normalized;

        //Expends or compress the player/camera scroll distance
        if (currentScroll != scrollTarget)
        {
            float delta = Mathf.MoveTowards(0, scrollTarget - currentScroll, scrollSpeed);
            currentScroll += delta;
            previousTranslation = previousTranslation.normalized * (previousTranslation.magnitude + delta);
        }
        
        //Replace the camera at the right place
        transform.position = playerGameObject.transform.position + previousTranslation;

        //do Y rotation
        float rotationY = Vector3.SignedAngle(dir, hDir, Vector3.Cross(hDir, Vector3.up));

        //Camera angles overflow
        if (rotationY < minimumY)
        {
            transform.RotateAround(playerGameObject.transform.position, rightDir, minimumY - rotationY);
            rotationY = Vector3.SignedAngle(dir, hDir, Vector3.Cross(hDir, Vector3.up));
        } 
        else if (rotationY > maximumY)
        {
            transform.RotateAround(playerGameObject.transform.position, rightDir, maximumY - rotationY);
            rotationY = Vector3.SignedAngle(dir, hDir, Vector3.Cross(hDir, Vector3.up));
        }

        ////do Y rotation
        if ( (rotationY + mouseY * sensitivityY > minimumY) && (rotationY + mouseY * sensitivityY < maximumY))
           transform.RotateAround(playerGameObject.transform.position, rightDir, mouseY * sensitivityX);

        //do X rotation
        transform.RotateAround(playerGameObject.transform.position, Vector3.up, mouseX * sensitivityX);

        //Remove Z rotation
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);

        //Save the new camera translation
        previousTranslation = transform.position - playerGameObject.transform.position ;

        //Camera distance compression using raycast
        Ray ray = new Ray(playerGameObject.transform.position, previousTranslation);
        RaycastHit hit;
        if (Physics.SphereCast(ray, 0.2f, out hit, maxScrollDistance))
        {
            scrollTarget = Mathf.Min(hit.distance, userScrollTarget);
            scrollSpeed = highScrollSpeed;
        }
        else
        {
            scrollTarget = userScrollTarget;
            scrollSpeed = lowScrollSpeed;
        }
    }



}
