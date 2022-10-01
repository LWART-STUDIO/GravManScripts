using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.LookAt(transform.position+Camera.main.transform.rotation*Vector3.back,Camera.main.transform.rotation*Vector3.up);
    }
}
