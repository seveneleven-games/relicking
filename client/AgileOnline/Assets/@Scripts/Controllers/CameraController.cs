using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : InitBase
{
    private BaseController _target;

    public float mapMinX = -6f;
    public float mapMaxX = 6f;
    public float mapMinY = -4f;
    public float mapMaxY = 3.3f;
    
    public BaseController Target
    {
        get { return _target; }
        set { _target = value; }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Camera.main.orthographicSize = 10.0f;

        return true;
    }

    private void LateUpdate()
    {
        if (Target == null)
            return;

        Vector3 targetPosition = new Vector3(Target.CenterPosition.x, Target.CenterPosition.y, -10f);
        float clampedX = Mathf.Clamp(targetPosition.x, mapMinX, mapMaxX);
        float clampedY = Mathf.Clamp(targetPosition.y, mapMinY, mapMaxY);

        transform.position = new Vector3(clampedX, clampedY, targetPosition.z);
    }
}
