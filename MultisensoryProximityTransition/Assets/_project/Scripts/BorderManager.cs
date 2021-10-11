using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderManager : MonoBehaviour
{
    public float borderwidthDegree = 5f;
    public Camera camera;
    public GameObject arRenderTarget;
    public Vector2 arFOV = new Vector2(30f, 17.5f);
    public Transform borderParent;
    List<Transform> borderElements = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        if (borderElements != null)
            borderElements.Clear();
        else
            borderElements = new List<Transform>();

        borderElements = new List<Transform>();
        foreach (Transform t in borderParent)
        {
            borderElements.Add(t);
        }
        setVisible(false);

    }

    private void OnValidate()
    {

        return;
        if (camera == null)
            camera = Camera.main;
        if (arRenderTarget == null)
        {
            Debug.LogError("Please assign the arRenderTarget!");
            return;
        }
        if (borderParent == null)
        {
            Debug.LogError("Please assign the borderParent!");
            return;
        }

        this.transform.position = camera.transform.position + camera.transform.forward * 2f;
        float borderThickness = Mathf.Abs(2f * 2f * Mathf.Tan((Mathf.Deg2Rad * borderwidthDegree) / 2));//g = | 2r* tan(α/ 2)|
        float borderLength = Mathf.Abs(2f * 2f * Mathf.Tan((Mathf.Deg2Rad * arFOV.x) / 2f));//g = | 2r* tan(α/ 2)|
        float borderHeight = Mathf.Abs(2f * 2f * Mathf.Tan((Mathf.Deg2Rad * arFOV.y) / 2f));//g = | 2r* tan(α/ 2)|
        float upDistance = Mathf.Abs(2f * 2f * Mathf.Tan((Mathf.Deg2Rad * (arFOV.y / 2f - borderwidthDegree / 2f)) / 2f));//g = | 2r* tan(α/ 2)|
        float leftDistance = Mathf.Abs(2f * 2f * Mathf.Tan((Mathf.Deg2Rad * (arFOV.x / 2f - borderwidthDegree / 2f)) / 2f));//g = | 2r* tan(α/ 2)|
        if (borderElements != null)
            borderElements.Clear();
        
        borderElements = new List<Transform>();

        foreach (Transform t in borderParent)
        {
            borderElements.Add(t);
        }
        borderElements[0].localPosition = new Vector3(0f, upDistance, 0);
        borderElements[0].localScale = new Vector3(borderLength, borderThickness, 0.01f);

        borderElements[1].localScale = new Vector3(borderLength, borderThickness, 0.01f);
        borderElements[1].localPosition = new Vector3(0f, -upDistance, 0);

        borderElements[2].localScale = new Vector3(borderThickness, borderHeight, 0.01f);
        borderElements[2].localPosition = new Vector3(leftDistance, 0f, 0);

        borderElements[3].localScale = new Vector3(borderThickness, borderHeight, 0.01f);
        borderElements[3].localPosition = new Vector3(-leftDistance, 0f, 0);
    }

    public void setVisible(bool b)
    {
        foreach (Transform transform in borderElements)
        {
            transform.GetComponent<MeshRenderer>().enabled = b;
        }
    }
}
