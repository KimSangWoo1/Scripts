using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public enum BGM {Start, Lobby, Store, DeathMatch}
    BGM bgm;

    DataManager DM;

    public AudioSource[] audioSources;

    [Header("BGM Clips")]
    public AudioClip[] bgmClips;
    [Header("UI Clips")]
    public AudioClip[] sfxClips;

    
    private bool sfxSound;
    private bool bgmSound;

    private void Awake()
    {
        DM = DataManager.Instance;

    }
    private void OnEnable()
    {
        //초기 사운드 설정 (뮤트인지 아닌지)
        SetSFXControl();
        SetBGMCotnrol();

    }
    #region SFX Sound Method
    //SFX뮤트 시키기
    public void SetSFXControl()
    {
        sfxSound = DM.GetSFSXSound();//데이터 받아오기

        if (sfxSound)
        {
            audioSources[1].mute = false;
        }
        else
        {
            audioSources[1].mute = true;
        }
    }

    // UI 앞으로 사운드
    public void GoSound()
    {
        audioSources[1].PlayOneShot(sfxClips[0]);
    }
    // UI 뒤로 사운드
    public void BackSound()
    {
        audioSources[1].PlayOneShot(sfxClips[1]);
    }
    // UI 아이템 구매 성공 사운드
    public void ItemBuySuccess()
    {
        audioSources[1].clip = sfxClips[2];
        audioSources[1].Play();
    }
    #endregion

    #region BGM Sound Method

    //BGM뮤트 시키기
    public void SetBGMCotnrol()
    {
        bgmSound = DM.GetBGMXSound();//데이터 받아오기

        if (bgmSound)
        {
            audioSources[0].mute = false;
        }
        else
        {
            audioSources[0].mute = true;
        }
    }


    //StartScene BGM
    public void BGMPlay(BGM _bgm)
    {
        //Clip 설정
        switch (_bgm)
        {
            case BGM.Start:
                audioSources[0].clip = bgmClips[0];
                break;
            case BGM.Lobby:
                audioSources[0].clip = bgmClips[1];
                break;
            case BGM.Store:
                audioSources[0].clip = bgmClips[0];
                break;
            case BGM.DeathMatch:
                audioSources[0].clip = bgmClips[2];
                break;
        }

        audioSources[0].Play(); //실행
    }

    //LobbyScene BGM

    //StoreScene BGM 

    //DeathMatchScene BGM

    #endregion
}
