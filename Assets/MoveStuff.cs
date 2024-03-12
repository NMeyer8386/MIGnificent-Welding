using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MoveStuff : MonoBehaviour
{
    [SerializeField] Transform map;

    XRGrabInteractable grabInteractable;
    bool isGrabbed = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(SetConvex);
        grabInteractable.selectExited.AddListener(IsReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(SetConvex);
        grabInteractable.selectExited.RemoveListener(IsReleased);
    }
    void IsReleased(SelectExitEventArgs arg0)
    {
        Debug.Log("HERE");
        isGrabbed = false;
        transform.GetComponent<Rigidbody>().isKinematic = false;
        StartCoroutine(RotateToNormal());
    }
    /// <summary>
    /// Set the Chuck Meshes as convex
    /// </summary>
    void SetConvex(SelectEnterEventArgs arg0)
    {
        isGrabbed = true;
        foreach (Transform child in map)
        {
        child.GetComponent<MeshCollider>().convex = true;
        }
    }

    /// <summary>
    /// Set the Chunk Meshes as not convex
    /// </summary>
    void unSetConvex()
    {

        transform.GetComponent<Rigidbody>().isKinematic = true;
        foreach (Transform child in map)
        {
            child.GetComponent<MeshCollider>().convex = false;
        }
    }

    IEnumerator RotateToNormal()
    {
        yield return new WaitForSeconds(.5f);
        unSetConvex();
    }
}
