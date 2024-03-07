using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScoreSystem : MonoBehaviour
{
    /// <summary>
    /// Welder Tracking System
    /// </summary>
    [Header("Welding Gun Tracker")]
    [SerializeField] WeldingGunTracking GunTracking;

    [Header("Angle Settings")]
    [SerializeField] float targetAngle;
    [SerializeField] float acceptableAngle;

    [Header("Speed Settings")]
    [SerializeField] float targetSpeed;
    [SerializeField] float acceptableSpeed;

    [Header("Distance Settings")]
    [SerializeField] float targetDistance;
    [SerializeField] float acceptableDistance;

    public float angleOutput { get; private set; }
    public float distanceOutput { get; private set; }
    public float speedOutput { get; private set; }
    public float score { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GunTracking.isGrabbed)
        {
            angleOutput = RotationScoreing();
            speedOutput = SpeedScoreing();
            distanceOutput = DistanceScoreing();
            score += ScoreTotal();
        }
    }
    /// <summary>
    /// Calculate the portion of the score related to rotation
    /// </summary>
    /// <returns></returns>
    private float RotationScoreing() 
    {
        float minAngle = targetAngle - acceptableAngle;
        float maxAngle = targetAngle + acceptableAngle;

        if (GunTracking.angleToPlaneX >= minAngle && GunTracking.angleToPlaneX <= maxAngle)
        {
            // Angle is within the desired range
            return 0f;
        }
        else if (GunTracking.angleToPlaneX < 0 || GunTracking.angleToPlaneX > 90)
        {
            return -1f;
        }
        else if (GunTracking.angleToPlaneX < minAngle)
        {
            // Angle is less than the minimum desired angle
            return -Mathf.Pow((minAngle - GunTracking.angleToPlaneX) / (minAngle - 0), 2f);
        }
        else
        {
            // Angle is greater than the maximum desired angle
            return Mathf.Pow((GunTracking.angleToPlaneX - maxAngle) / (90 - maxAngle), 2f);
        }
    }

    /// <summary>
    /// Calculate the portion of the score related to speed
    /// </summary>
    /// <returns></returns>
    private float SpeedScoreing()
    {
        float minSpeed= targetSpeed - acceptableSpeed;
        float maxSpeed = targetSpeed + acceptableSpeed;

        if (GunTracking.gunVelocity >= minSpeed && GunTracking.gunVelocity <= maxSpeed)
        {
            // Speed is within the desired range
            return 0f;
        }
        else if (GunTracking.gunVelocity > 0.3f)
        {
            return 1;
        }
        else if (GunTracking.gunVelocity < minSpeed)
        {
            // Speed is less than the minimum desired angle
            return -Mathf.Pow((minSpeed - GunTracking.gunVelocity) / (minSpeed - 0), 2f);
        }
        else
        {
            // Speed is greater than the maximum desired angle
            return Mathf.Pow((GunTracking.gunVelocity - maxSpeed) / (0.3f - maxSpeed), 2f);
        }
    }

    /// <summary>
    /// Calculate the portion of the score related to Distance
    /// </summary>
    /// <returns></returns>
    private float DistanceScoreing()
    {
        float minDistance = targetDistance - acceptableDistance;
        float maxDistance = targetDistance + acceptableDistance;

        if (GunTracking.gunDistance >= minDistance && GunTracking.gunDistance <= maxDistance)
        {
            // distance is within the desired range
            return 0f;
        }
        else if (GunTracking.gunDistance > 0.3f)
        {
            return 1;
        }
        else if (GunTracking.gunDistance < minDistance)
        {
            // Speed is less than the minimum desired angle
            return -Mathf.Pow((minDistance - GunTracking.gunDistance) / (minDistance - 0), 2f);
        }
        else
        {
            // Speed is greater than the maximum desired angle
            return Mathf.Pow((GunTracking.gunDistance - maxDistance) / (0.3f - maxDistance), 2f);
        }
    }
    private float ScoreTotal()
    {
        //Get the inverse for a max score per catagory of welding
        float tempScore = (1 - Mathf.Abs(angleOutput)) + (1 - Mathf.Abs(speedOutput)) + (1 - Mathf.Abs(distanceOutput));
        // Max score per second is 30
        return (Time.deltaTime * 10 * tempScore);
    }
}
