using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow_2D_Topdown : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float zOffset = -10;

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + new Vector3(0, 0, zOffset);
    }
}
