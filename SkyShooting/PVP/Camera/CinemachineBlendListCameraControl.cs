using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineBlendListCameraControl : MonoBehaviour
{
    private CinemachineBlendListCamera blendCam;

    [SerializeField]
    private DollyCamControl[] dollyCams;

    void Start()
    {
        blendCam = GetComponent<CinemachineBlendListCamera>();
    }

    public void Chage(ICinemachineCamera cam1, ICinemachineCamera cam2)
    {
        //Cam2 -> Cam1
        if (cam1 != null)
        {
           // Debug.Log("1 " + cam1.Name);
        }
        if (cam2 != null)
        {
            cam2.VirtualCameraGameObject.SetActive(false);
           // Debug.Log("2 " + cam2.Name);
        }
    }

    public void SetLookAtChildCam(Transform player)
    {
        for (int i = 0; i < dollyCams.Length; i++)
        {
            dollyCams[i].SetLookTarget(player);
        }
    }
}
