using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class WeldingGunTracking : MonoBehaviour
{
    //Gameobject stuff
    XRGrabInteractable grabInteractable;
    ControllerData controllerData;
    [SerializeField] Transform raycastTransform;
    [SerializeField] TMP_Text[] labels;

    //Variable stuff
    bool isGrabbed = false;
    float gunVelocity;
    float gunDistance;
    string gunRotation;

    //Grab required components
    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        isGrabbed = true;
        grabInteractable.selectEntered.AddListener(GetControllerData);
    }

    private void OnDisable()
    {
        isGrabbed = false;
        grabInteractable.selectEntered.RemoveListener(GetControllerData);
    }

    //Get controller data from another script
    private void GetControllerData(SelectEnterEventArgs arg0)
    {
        Transform parent = arg0.interactorObject.transform.parent;

        if (parent != null)
        {
            controllerData = parent.GetComponent<ControllerData>();
        }
    }

    private void FixedUpdate()
    {
        if (isGrabbed)
        {
            //Update values on screen
            GetVelocityRotationDistance();

            labels[0].text = "Velocity: " + gunVelocity.ToString();
            labels[1].text = "Rotation: " + gunRotation;
            labels[2].text = "Distance: " + gunDistance.ToString();
        }
    }

    private void GetVelocityRotationDistance()
    {
        //Velocity based on controller velocity
        if (controllerData)
        {
            gunVelocity = controllerData.Velocity.magnitude;
        }

        //Model rotation
        gunRotation = gameObject.transform.rotation.eulerAngles.ToString();

        //Distance from tip to surface
        RaycastHit hit;
        Ray gunRay = new Ray(raycastTransform.position, raycastTransform.right);
        if (Physics.Raycast(gunRay, out hit))
        {
            gunDistance = hit.distance;
        }
        
    }
}
