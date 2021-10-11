using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArDisplay : MonoBehaviour
{
    public Camera camera;
    public Vector2 fovDegree = new Vector2(30f, 17.5f);

    // Update is called once per frame
    void Update()
    {
        setDimensions();
    }

    void setDimensions()
    {
        if (camera == null)
        {
            Debug.LogError("AR-Display need camera assigned");
            return;
        }
        camera.fieldOfView = fovDegree.y;
        camera.aspect = fovDegree.x / fovDegree.y;
        float distanceToCamera = Vector3.Distance(gameObject.transform.position, camera.transform.position);
        transform.localScale = new Vector3(//g = |2r*tan(α/2) |
            Mathf.Abs(2f * distanceToCamera * Mathf.Tan((Mathf.Deg2Rad * fovDegree.x) / 2f)),
            Mathf.Abs(2f * distanceToCamera * Mathf.Tan((Mathf.Deg2Rad * fovDegree.y) / 2f)),
            0.00001f
            );
    }

    protected void OnValidate()
    {
        setDimensions();
    }
}
