using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
[CreateAssetMenu (fileName = "DialogGroupSO", menuName = "ScriptableObjects/DialogGroupSO", order =1)]
public class DialogGroupSO : ScriptableObject
{
    public GameDataSO gameDataSO;
    public DialogSO defaultDialog;
    public StoryDialogSO[] storyDialogs;

    int index;

    //Story ��ȭ �켱���� üũ
    public bool IsStoryDialog()
    {
        for(int i=0; i<storyDialogs.Length; i++)
        {
            if (gameDataSO.saveData.chapter == storyDialogs[i].GetChapter() && gameDataSO.saveData.part == storyDialogs[i].GetPart())
            {
                index = i;
                return true;
            }
        }
        return false;
    }

    //��ȭ ���� ��������
    public string GetDialogText()
    {
        if (IsStoryDialog())
        {
            return storyDialogs[index].GetStoryDialogText();
        }
        else
        {
            return defaultDialog.GetDialogText();
        }
    }

    public DialogSO.Dialog[] GetDialogTalk()
    {
        if (IsStoryDialog())
        {
            return storyDialogs[index].GetStroryDialogTalk();
        }
        else
        {
            return defaultDialog.GetDialogTalk();
        }
    }

    public Act GetAct()
    {
        if (IsStoryDialog())
        {
            return storyDialogs[index].GetAct();
        }
        else
        {
            return defaultDialog.GetAct();
        }
    }

    public Emotion GetEmotion()
    {
        if (IsStoryDialog())
        {
            return storyDialogs[index].GetEmotion();
        }
        else
        {
            return defaultDialog.GetEmotion();
        }
    }
}
