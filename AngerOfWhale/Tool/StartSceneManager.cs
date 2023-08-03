using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    // Manager
    SoundManager soundManager;
    public GameObject soundOff;
    public GameObject soundOn;

    private void Awake()
    {
        soundManager = SoundManager.Instance;
    }

    private void OnEnable()
    {
        soundManager.OnBGM(0); //Main BGM ON       
        if (AudioListener.volume == 0)
        {
            soundOff.SetActive(true);
            soundOn.SetActive(false);

        }

    }
}
