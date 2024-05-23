using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerData : MonoBehaviour
{
    [SerializeField] InputActionProperty velocityProperty;
    [SerializeField] InputActionProperty triggerProperty;


    [SerializeField] private Vector3 vel;
    [SerializeField] private float triggerVal;

    public Vector3 Velocity { private set { } get { return vel; } }    
    public float TriggerValue { private set { } get { return triggerVal; } }   

    // Update is called once per frame
    void Update()
    {
        vel = velocityProperty.action.ReadValue<Vector3>();
        triggerVal = triggerProperty.action.ReadValue<float>();
        //Debug.Log("Velocity: " + Velocity);
    }
}