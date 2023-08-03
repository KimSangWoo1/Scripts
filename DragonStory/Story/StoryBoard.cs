using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryBoard
{
    public string Title; //����
    public string Date; //�ۼ���¥
    public string Writer; //�ۼ���
    public int Chapter; // ���� ���丮 ����
    public int Part; // ���丮 ���� ����
    [Header("�뺻 �� ����")]
    public List<Sequence> Sequence; //���丮 �帧 (Scene ����)

    //���� ���
    public void PrintInformation()
    {
        Debug.Log("Title : " + Title + "\n Date : " + Date + "\n Writer : " + Writer);
    }

    public Sequence GetSequence(int num)
    {
        return this.Sequence[num];
    }
}

[System.Serializable]
public struct Sequence
{
    public Script Script;
    public Cast Cast;
    public Cinematography Cinematography;
    public Audio Audio;
}

[System.Serializable]
public struct Script
{
    public string Name;
    [TextArea(1, 3)]
    public string Content;
    public bool Auto;
}

[System.Serializable]
public struct Cast
{
    [HideInInspector]
    public string Actor;
    public Actor actor;
    [HideInInspector]
    public string Emotion;
    public Emotion emotion;
    [HideInInspector]
    public string Act;
    public Act act;
}

[System.Serializable]
public struct Cinematography
{
    [HideInInspector]
    public string Shot;
    public Shot shot;
    public int Number;
    public string Explanation;
}

[System.Serializable]
public struct Audio
{
    [HideInInspector]
    public string Speaker;
    public Actor speaker;
    public bool BackgroundPlaying;
    public int BackgroundSoundNumber;
    public string EffectSound;
}