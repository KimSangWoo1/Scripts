using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreModeControl : MonoBehaviour
{
    [Header("Solo Mode")]
    [SerializeField]
    private GameObject plane;
    [SerializeField]
    private GameObject leftBullet;
    [SerializeField]
    private GameObject rightBullet;

    public void SoloModeLeftShoot()
    {
        leftBullet.SetActive(true);
    }

    private void SoloModeRightShoot()
    {
        rightBullet.SetActive(true);
    }
}
