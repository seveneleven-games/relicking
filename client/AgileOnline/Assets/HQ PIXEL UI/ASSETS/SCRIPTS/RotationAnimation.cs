using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAnimation : MonoBehaviour
{
    [Range(1, 300)]
    public int rotationSpeed;
    [Header("Clock-wise and Anti-clockwise 1 and -1")]
    [Range(-1, 1)]
    public int direction;

	float rotation = 0;

    private void Start()
    {
        if (direction == 0)
            direction = 1;
    }

    void Update ()
	{
      
        rotation = rotationSpeed * Time.deltaTime * direction;
		transform.Rotate (new Vector3 (0f,0f,rotation));
	}
}
