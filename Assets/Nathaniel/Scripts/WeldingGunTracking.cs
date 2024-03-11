using System;
using Eldemarkki.VoxelTerrain.Meshing.MarchingCubes;
using Eldemarkki.VoxelTerrain.Utilities;
using Eldemarkki.VoxelTerrain.Utilities.Intersection;
using Eldemarkki.VoxelTerrain.World;
using System.Collections;
using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using TMPro;

public class WeldingGunTracking : MonoBehaviour
{
    //Gameobject stuff
    XRGrabInteractable grabInteractable;
    XRBaseController controller;
    ControllerData controllerData;
    [SerializeField] GameObject weldUI;
    [SerializeField] Transform raycastTransform;
    [SerializeField] TMP_Text[] labels;
    [SerializeField] Transform parentTransform;
    AudioSource audioSource;

    //Variable stuff
    public bool isGrabbed { get; private set; } = false;
    public float gunVelocity { get; private set; }
    public string gunRotation { get; private set; }
    public float gunDistance { get; private set; }
    public float gunTrigger { get; private set; }
    public float angleToPlaneX { get; private set; }

    /// <summary>
    /// The voxel data store that will be deformed
    /// </summary>
    [Header("Terrain Deforming Settings")]
    [SerializeField] private VoxelWorld voxelWorld;

    /// <summary>
    /// How fast the terrain is deformed
    /// </summary>
    [SerializeField] private float deformSpeed = 0.01f;

    /// <summary>
    /// How far the deformation can reach
    /// </summary>
    [SerializeField] private float deformRange = 3f;

    /// <summary>
    /// How far away points the player can deform
    /// </summary>
    [SerializeField] private float maxReachDistance = Mathf.Infinity;

    //Grab required components
    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        audioSource = GetComponent<AudioSource>();
        weldUI.SetActive(isGrabbed);
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
        weldUI.SetActive(isGrabbed);
    }

    //Get controller data from another script
    private void GetControllerData(SelectEnterEventArgs arg0)
    {
        isGrabbed = true;
        weldUI.SetActive(isGrabbed);
        Transform parent = arg0.interactorObject.transform.parent;

        if (parent != null)
        {
            controllerData = parent.GetComponent<ControllerData>();
            controller = parent.GetComponent<XRBaseController>();
        }
    }

    private void FixedUpdate()
    {
        if (isGrabbed)
        {
            //Get required values
            GetVelocityRotationDistance();

            /*  Update debug display
             *  labels[0].text = "Velocity: " + gunVelocity.ToString();
             *  labels[1].text = "Rotation: " + gunRotation;
             *  labels[2].text = "Distance: " + gunDistance.ToString();
             *  labels[3].text = "Trigger: " + gunTrigger.ToString();
             *  labels[3].text = "Angle: " + angleToPlaneX.ToString();
            */

            //Play sounds
            PlaySoundOnTrigger(controllerData.TriggerValue > 0.5f);

            //Run haptics
            RunHapticsOnTrigger(controllerData.TriggerValue > 0.5f);

            //If trigger is being pressed...
            if (controllerData.TriggerValue > 0.5f)
            {
                //...Make terrain
                RaycastToTerrain();
            }
        }
    }

    private void PlaySoundOnTrigger(bool triggerPressed)
    {
        if (!audioSource.isPlaying && triggerPressed)
        {
            audioSource.Play();
        } else if (audioSource.isPlaying && !triggerPressed) 
        {
            audioSource.Pause();
        }
        
    }

    private void RunHapticsOnTrigger(bool triggerPressed)
    {
        if (triggerPressed)
        {
            controller.SendHapticImpulse(0.5f, 0.1f);
        }

    }

    

    private void GetVelocityRotationDistance()
    {
        // Velocity based on controller velocity
        if (controllerData)
        {
            gunVelocity = controllerData.Velocity.magnitude;
        }

        // Distance from tip to surface
        RaycastHit hit;
        Ray gunRay = new Ray(raycastTransform.position, raycastTransform.forward);
        if (Physics.Raycast(gunRay, out hit))
        {
            gunDistance = hit.distance;

            // Calculate angles between gun direction and plane normal in x, y, and z axes
            Vector3 gunDirection = raycastTransform.forward;

            if (parentTransform != null)
            {
                Vector3 pointOnPlane = parentTransform.position;
                Vector3 gunToPlane = pointOnPlane - raycastTransform.position;
                angleToPlaneX = Vector3.Angle(gunDirection, new Vector3(gunToPlane.x, 0, 0));
            }
            else
            {
                Debug.LogError("Plane transform not assigned!");
            }
        }

        // Trigger Pull
        gunTrigger = controllerData.TriggerValue;
    }
    private void RaycastToTerrain()
    {
        Ray ray = new Ray(raycastTransform.position, raycastTransform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, maxReachDistance)) { return; }

        // Check if the hit has a collider
        if (hit.collider != null)
        {
            // Access the parent transform of the collider
            Transform parentTransform = hit.collider.transform.parent;

            //If we don't use a try catch then unity throws a hissy fit of nullreferenceexceptions
            try { parentTransform.parent.TryGetComponent<VoxelWorld>(out VoxelWorld voxelWorld); }

            catch (NullReferenceException)
            {
                Debug.Log("their ass is NOT welding");
                return;
            }

            Vector3 hitPoint = hit.point;

            Vector3 offset = parentTransform.transform.position;

            // Calculate the new position by subtracting offset to the current position
            Vector3 newPosition = hitPoint - offset;

            // Move the object to the new position
            hitPoint = newPosition / .01f;

            EditTerrain(hitPoint, deformSpeed, deformRange, voxelWorld);
        }
    }
    private void EditTerrain(Vector3 point, float deformSpeed, float range, VoxelWorld voxelWorld)
    {
        int buildModifier = 1;

        int hitX = Mathf.RoundToInt(point.x);
        int hitY = Mathf.RoundToInt(point.y);
        int hitZ = Mathf.RoundToInt(point.z);
        int3 hitPoint = new int3(hitX, hitY, hitZ);

        int intRange = Mathf.CeilToInt(range);
        int3 rangeInt3 = new int3(intRange, intRange, intRange);

        BoundsInt queryBounds = new BoundsInt((hitPoint - rangeInt3).ToVectorInt(), new int3(intRange * 2).ToVectorInt());

        voxelWorld.VoxelDataStore.SetVoxelDataCustom(queryBounds, (voxelDataWorldPosition, voxelData) =>
        {
            float distance = math.distance(voxelDataWorldPosition, point);
            if (distance <= range)
            {
                float modificationAmount = deformSpeed / distance * buildModifier;
                float oldVoxelData = voxelData / 255f;
                return (byte)math.clamp((oldVoxelData - modificationAmount) * 255, 0, 255);
            }

            return voxelData;
        });
    }
}
