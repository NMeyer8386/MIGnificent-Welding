using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using TMPro;
using UnityEditor.ShaderGraph.Internal;

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
    float triggerPull;

    //Grab required components
    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(GetControllerData);
        grabInteractable.selectExited.AddListener(IsReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(GetControllerData);
        grabInteractable.selectExited.RemoveListener(IsReleased);
    }

    //Sets isGrabbed to false when the gun is not being grabbed
    private void IsReleased(SelectExitEventArgs arg0)
    {
        isGrabbed = false;
    }

    //Get controller data from another script
    private void GetControllerData(SelectEnterEventArgs arg0)
    {
        isGrabbed = true;
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
            labels[3].text = "Trigger: " + controllerData.TriggerValue.ToString();
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
