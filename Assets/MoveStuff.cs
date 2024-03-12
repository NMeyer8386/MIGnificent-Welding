using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MoveStuff : MonoBehaviour
{
    [SerializeField] Transform map;

    List<GameObject> chunks;
    XRGrabInteractable grabInteractable;
    bool isGrabbed = false;
    // Start is called before the first frame update
    void Start()
    {
        // Create a list of chunks to use later
        foreach (Transform child in map)
        {
            chunks.Add(child.gameObject);
        }
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
        isGrabbed = false;
    }
    /// <summary>
    /// Set the Chuck Meshes as convex
    /// </summary>
    void SetConvex(SelectEnterEventArgs arg0)
    {
        isGrabbed = true;
        for(int i = 0; i < chunks.Count; i++)
        {
            chunks[i].GetComponent<MeshCollider>().convex = true;
        }
        transform.GetComponent<Rigidbody>().isKinematic = false;
    }

    /// <summary>
    /// Set the Chunk Meshes as not convex
    /// </summary>
    void unSetConvex()
    {
        for (int i = 0; i < chunks.Count; i++)
        {
            chunks[i].GetComponent<MeshCollider>().convex = false;
        }
        transform.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "table" && isGrabbed == false)
        {
            StartCoroutine("RotateToNormal");
        }
    }
    IEnumerable RotateToNormal()
    {
        float timer = 0;
        Vector3 current = transform.rotation.eulerAngles;
        float y = transform.rotation.y;
        while(timer < 0.5f)
        {
            timer += Time.deltaTime;
            Vector3.Lerp(current, new Vector3(-90, y, 0), timer / 0.5f);
            yield return null;
        }
        unSetConvex();
    }
}
