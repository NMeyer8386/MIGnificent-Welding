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
using UnityEditor.ShaderGraph.Internal;

public class WeldingGunTracking : MonoBehaviour
{
    //Gameobject stuff
    XRGrabInteractable grabInteractable;
    ControllerData controllerData;
    [SerializeField] Transform raycastTransform;
    [SerializeField] TMP_Text[] labels;
    public Transform parentTransform;

    //Variable stuff
    bool isGrabbed = false;
    float gunVelocity;
    float gunDistance;
    string gunRotation;
    float triggerPull;

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

    /// <summary>
    /// The game object that the deformation raycast will be cast from
    /// </summary>
    [Header("Player Settings")]
    [SerializeField] private Transform playerCamera;

    /// <summary>
    /// The game object that the deformation raycast will be cast from
    /// </summary>
    [Header("Voxel Position")]
    [SerializeField] private Transform voxelPosition;

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
            //labels[3].text = "Trigger: " + controllerData.TriggerValue.ToString();

            if (controllerData.TriggerValue > 0.5f)
            {
                RaycastToTerrain();
            }
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
    private void RaycastToTerrain()
    {
        Ray ray = new Ray(raycastTransform.position, raycastTransform.right);

        if (!Physics.Raycast(ray, out RaycastHit hit, maxReachDistance)) { return; }

        // Check if the hit has a collider
        if (hit.collider != null)
        {
            // Access the parent transform of the collider
            Transform parentTransform = hit.collider.transform.parent;

            if (parentTransform != null)
            {
                VoxelWorld voxelWorld = parentTransform.parent.GetComponent<VoxelWorld>();

                Vector3 hitPoint = hit.point;

                Vector3 offset = parentTransform.transform.position;

                // Calculate the new position by subtracting offset to the current position
                Vector3 newPosition = hitPoint - offset;

                // Move the object to the new position
                hitPoint = newPosition / .01f;

                EditTerrain(hitPoint, deformSpeed, deformRange, voxelWorld);
            }
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
