using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryCamGroup :MonoBehaviour
{
    public GameObject[] singleCams;
    public GameObject[] otsCams;
    public GameObject[] wideCams;
    public GameObject[] dollyCams;
    public GameObject[] insertCams;
    public GameObject[] titleCams;

    private GameObject cam;

    private void OnDisable()
    {
        Destroy(gameObject);
    }
    public void CameraShooting(Shot shot, int number)
    {
        if(cam !=null)
            cam.SetActive(false);
        
        switch (shot)
        {
            case Shot.SINGLE:
                cam = singleCams[number - 1];
                break;
            case Shot.OTS:
                cam = otsCams[number - 1];
                break;
            case Shot.WIDE:
                cam = wideCams[number - 1];
                break;
            case Shot.DOLLY:
                cam = dollyCams[number - 1];
                break;
            case Shot.INSERT:
                cam = insertCams[number - 1];
                break;
            case Shot.TITLE:
                cam = titleCams[number - 1];
                break;
        }
        cam.SetActive(true);
    }

    public void CameraOff()
    {
        cam.SetActive(false);
        Destroy(gameObject);
    }
}
