using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerData : MonoBehaviour
{
    [SerializeField] InputActionProperty velocityProperty;


    [SerializeField] private Vector3 vel;

    public Vector3 Velocity { private set { } get { return vel; } }

    // Update is called once per frame
    void Update()
    {
        vel = velocityProperty.action.ReadValue<Vector3>();
        //Debug.Log("Velocity: " + Velocity);
    }
}