using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraDirector : MonoBehaviour
{
    [SerializeField]
    CinemachineBlendListCameraControl[] blendCams;

    [SerializeField]
    ActionCam[] actionCams;

    private CinemachineBlendListCameraControl myBlendCam;

    public void SetPlayer(int playerNumber,GameObject player)
    {
        myBlendCam = blendCams[playerNumber];
        myBlendCam.gameObject.SetActive(true);
        myBlendCam.SetLookAtChildCam(player.transform);
    }

    public void ActionCamOn(int playerNum)
    {
        for (int i = 0; i < blendCams.Length; i++)
        {
            blendCams[i].gameObject.SetActive(false);
        }
        actionCams[playerNum - 1].gameObject.SetActive(true);
    }

    public void ActionCamOff(int playerNum)
    {
        actionCams[playerNum - 1].gameObject.SetActive(false);
    }

    public void ActionCamOff()
    {
        for (int i = 0; i < actionCams.Length; i++)
        {
            actionCams[i].gameObject.SetActive(false);
        }
        myBlendCam.gameObject.SetActive(true);
    }
}
