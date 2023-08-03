using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("오디오 소스")]
    public AudioSource bgmAudio;
    public AudioSource playAudio;

    [Header("BGM")]
    public AudioClip[] bgms;

    [Header("UI 효과음")]
    public AudioClip buttonClick;
    public AudioClip scoreOn;

    [Header("고래 소리")]
    public AudioClip attack;
    public AudioClip damage;

    [Header("물 소리")]
    public AudioClip underWater;

    [Header("아이템 효과음")]
    public AudioClip heal;
    public AudioClip helmat;

    [Header("점수 효과음")]
    public AudioClip scoreHighlight;

    public float a;

    //BGM 초기화
    private void BgmInit()
    {
        bgmAudio.spatialBlend = 0f;
    }

    public void OnBGM(int num)
    {
        bgmAudio.clip = bgms[num]; //0. Main 1.Play 2.Over 3.Clear
        bgmAudio.Play();
    }

    public void UISound()
    {
        playAudio.volume = 1f;
        playAudio.PlayOneShot(buttonClick);
    }
    public void AttackSound()
    {
        BgmInit(); //물속 -> 공격시

        playAudio.Stop();
        playAudio.volume = 0.5f;
        playAudio.PlayOneShot(attack);
        
    }

    public void DamageSound()
    {
        BgmInit(); //물속 -> 피격시

        playAudio.Stop();
        if (!playAudio.isPlaying)
        {
            playAudio.volume = 0.7f;
            playAudio.PlayOneShot(damage);
        }
    }

    public void UnderWaterSound()
    {
        playAudio.Stop();
        playAudio.volume = 1f;
        playAudio.PlayOneShot(underWater);
        bgmAudio.spatialBlend = 0.8f; //사운드 공간감 주기

    }

    public void AidKitSound()
    {
        playAudio.volume = 0.5f;
         a = playAudio.volume;
        playAudio.PlayOneShot(heal);
    }

    public void HelmatSound()
    {
        playAudio.volume = 0.8f;
        playAudio.PlayOneShot(helmat);
    }

    public void ScoreHighlightSound()
    {
        playAudio.volume = 0.8f;
        playAudio.PlayOneShot(scoreHighlight);
    }

    public void SoundOff()
    {
        AudioListener.volume = 0;
    }
    public void SoundOn()
    {
       AudioListener.volume = 1;
    }
}
