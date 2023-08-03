using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DollyCamControl : MonoBehaviour
{
    enum Target { ON, OFF}

    [SerializeField]
    private Target target;

    private CinemachineVirtualCamera cvCam;
    private CinemachineTrackedDolly trackDolly;

    private Transform lookTarget;

    private float pathLength;
    [SerializeField]
    private float speed;


    void Awake()
    {
        cvCam = GetComponent<CinemachineVirtualCamera>();
        trackDolly = cvCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        pathLength = trackDolly.m_Path.MaxPos;
        
        if(lookTarget != null)
        {
            cvCam.LookAt = lookTarget;
        }
    }

    void Update()
    {
        if (trackDolly.m_PathPosition < pathLength)
        {
            trackDolly.m_PathPosition += speed;
        }
    }

    private void OnDisable()
    {
        ReSetPath();    
    }

    public void SetLookTarget(Transform _lookTarget)
    {
        if(target == Target.ON)
        {
            lookTarget = _lookTarget;
            if (cvCam != null)
            {
                cvCam.LookAt = lookTarget;
            }
        }
    }

    public void ReSetPath()
    {
        if (trackDolly != null)
        {
            trackDolly.m_PathPosition = 0;
        }
    }
}
