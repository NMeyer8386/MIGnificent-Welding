using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using VInspector;

public class ScoreSystem : MonoBehaviour
{
    [Tab("Score")]
    /// <summary>
    /// Welder Tracking System
    /// </summary>
    [Header("Welding Gun Tracker")]
    [SerializeField] WeldingGunTracking GunTracking;

    [Header("Angle Settings")]
    [SerializeField] float targetAngle = 45f;
    [SerializeField] float acceptableAngle = 5f;

    [Header("Speed Settings")]
    [SerializeField] float targetSpeed = .04f;
    [SerializeField] float acceptableSpeed = .01f;

    [Header("Distance Settings")]
    [SerializeField] float targetDistance = .06f;
    [SerializeField] float acceptableDistance = .015f;

    public float angleOutput { get; private set; }
    public float distanceOutput { get; private set; }
    public float speedOutput { get; private set; }
    public float score { get; private set; }

    [Tab("UI")]
    [Header("UI Components")]
    [SerializeField] UnityEngine.UI.Image angleVisual;
    [SerializeField] UnityEngine.UI.Image speedVisual;
    [SerializeField] UnityEngine.UI.Image distanceVisual;

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

            DisplayScoreOnUI(angleOutput, speedOutput, distanceOutput);

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

    /// <summary>
    /// Uses the params passed to display optimal angle/speed/distance on UI components
    /// </summary>
    /// <param name="angle">angleOutput Variable</param>
    /// <param name="speed">speedOutput Variable</param>
    /// <param name="distance">distanceOutput Variable</param>
    private void DisplayScoreOnUI(float angle, float speed, float distance)
    {
        //Angle
        //Use lerp to move image between min/max angle, pass absolute of angle to get lerp value
        angleVisual.transform.localPosition = new Vector3(Mathf.Lerp(-11, 11, Mathf.Abs(angle)), 0, 0);

        //Speed
        speedVisual.transform.localPosition = new Vector3(Mathf.Lerp(-11, 11, Mathf.Abs(speed)), 0, 0);

        //Distance
        distanceVisual.transform.localPosition = new Vector3(Mathf.Lerp(62, -1, Mathf.Abs(distance)), 0, 0);
    }

    private float ScoreTotal()
    {
        //Get the inverse for a max score per catagory of welding
        float tempScore = (1 - Mathf.Abs(angleOutput)) + (1 - Mathf.Abs(speedOutput)) + (1 - Mathf.Abs(distanceOutput));
        // Max score per second is 30
        return (Time.deltaTime * 10 * tempScore);
    }
}
