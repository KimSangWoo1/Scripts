using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCam : MonoBehaviour
{
    [SerializeField]
    private GameObject target;


    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(target.transform.position-transform.position, Vector3.up);
    }

}
