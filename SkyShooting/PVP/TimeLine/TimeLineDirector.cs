using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLineDirector : MonoBehaviour
{
    [Header("SO")]
    [SerializeField]
    private GameModeChannel gameModeChannel;
    [SerializeField]
    private TurnBulletChannel turnBulletChannel;

    [Header("Component")]
    [SerializeField]
    TurnSystem turnSystem;

    [Header("UI")]
    [SerializeField]
    private Canvas aniCanvas;
    [SerializeField]
    private Canvas gameCanvas;

    [Header("Cam")]
    [SerializeField]
    private GameObject mainCam;
    [SerializeField]
    private GameObject aniLeftCam;
    [SerializeField]
    private GameObject aniRightCam;

    private readonly WaitForSeconds wait = new WaitForSeconds(3.5f);

    void Start()
    {
        StartCoroutine(StartAni());    
    }

    IEnumerator StartAni()
    {
        yield return wait;

        aniCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);
        mainCam.gameObject.SetActive(true);
        aniLeftCam.gameObject.SetActive(false);
        aniRightCam.gameObject.SetActive(false);

        gameModeChannel.GameStart();
        turnBulletChannel.Event();
    }
}
