using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    /// <summary>
    /// Welder Tracking System
    /// </summary>
    public WeldingGunTracking GunTracking;

    public Image[] Images; 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Calculate the portion of the score related to rotation
    /// </summary>
    /// <returns></returns>
    private float RotationScoreing() 
    {
        return 0f;
    }

    /// <summary>
    /// Calculate the portion of the score related to speed
    /// </summary>
    /// <returns></returns>
    private float SpeedScoreing()
    {
        return 0f;
    }

    /// <summary>
    /// Calculate the portion of the score related to Distance
    /// </summary>
    /// <returns></returns>
    private float DistanceScoreing()
    {
        return 0f;
    }
}
