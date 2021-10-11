using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Credits to Eldoir from https://forum.unity.com/threads/linerenderer-to-create-an-ellipse.74028/#post-2752976
 */

[RequireComponent(typeof(LineRenderer))]

public class DrawCircle : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The number of lines that will be used to draw the circle. The more lines, the more the circle will be \"flexible\".")]
    [Range(0, 1000)]
    public int _segments = 60;

    [SerializeField]
    [Tooltip("The radius of the horizontal axis.")]
    public float _horizRadius = 10;

    [SerializeField]
    [Tooltip("The radius of the vertical axis.")]
    public float _vertRadius = 10;

    [SerializeField]
    [Tooltip("The offset will be applied in the direction of the axis.")]
    public float _offset = 0;

    [SerializeField]
    public float _degreeToDraw = 90f;


    [SerializeField]
    [Tooltip("If checked, the circle will be rendered again each time one of the parameters change.")]
    public bool _checkValuesChanged = true;

    private int _previousSegmentsValue;
    private float _previousHorizRadiusValue;
    private float _previousVertRadiusValue;
    private float _previousOffsetValue;
    private float _previousDegreeToDraw;


    private LineRenderer _line;

    void Start()
    {
        _line = gameObject.GetComponent<LineRenderer>();

        _line.positionCount = _segments + 1;
        _line.useWorldSpace = false;

        UpdateValuesChanged();

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (_checkValuesChanged)
        {
            if (_previousSegmentsValue != _segments ||
                _previousHorizRadiusValue != _horizRadius ||
                _previousVertRadiusValue != _vertRadius ||
                _previousOffsetValue != _offset ||
                _previousDegreeToDraw != _degreeToDraw
                )
            {
                CreatePoints();
            }

            UpdateValuesChanged();
        }
    }

    void UpdateValuesChanged()
    {
        _previousSegmentsValue = _segments;
        _previousHorizRadiusValue = _horizRadius;
        _previousVertRadiusValue = _vertRadius;
        _previousOffsetValue = _offset;
        _previousDegreeToDraw = _degreeToDraw;
    }

//    [Sirenix.OdinInspector.Button]
    public void CreatePoints()
    {
        
        _line.positionCount = _segments + 1;

        float x;
        float y;
        float z;

        float angle = 0f;

        for (int i = 0; i < (_segments + 1); i++)
        {
            if (angle > _degreeToDraw)
            {
                _line.positionCount = i;
                break;
            }

            z = Mathf.Sin(Mathf.Deg2Rad * angle) * _horizRadius;
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * _vertRadius;
            y = 0;
            _line.SetPosition(i,  new Vector3(x,y,z));
            angle += (360f / _segments);
        }
    }
}
