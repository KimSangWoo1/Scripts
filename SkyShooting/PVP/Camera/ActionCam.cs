using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCam : MonoBehaviour
{
    [SerializeField]
    CameraShake cameraShake;

    private Quaternion StartRot;

    public static GameObject target;
    public static bool shake;

    [SerializeField]
    private float lookSpeed;

    void Start()
    {
        StartRot = transform.rotation;        
    }

    void Update()
    {
        if(target != null)
        {
            if (target.gameObject.activeSelf)
            {
                Vector3 direct = target.transform.position - transform.position;
                direct = direct.normalized;
                Quaternion look = Quaternion.LookRotation(direct, transform.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, look, Time.deltaTime * lookSpeed);
            }
            else
            {
                transform.rotation = StartRot;
            }
        }
        else
        {
            target = GameObject.FindGameObjectWithTag("Bullet");
        }

        if (shake)
        {
            cameraShake.enabled = true;
            shake = false;
        }
    }

    private void OnDisable()
    {
        target = null;
        shake = false;
        cameraShake.enabled = false;
        transform.rotation = StartRot;
    }
}
