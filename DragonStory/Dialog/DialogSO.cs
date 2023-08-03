using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
[CreateAssetMenu (fileName = "DialogSO", menuName = "ScriptableObjects/DialogSO", order =1)]
public class DialogSO : ScriptableObject
{
    public Dialog[] dialogs;

    private int index;

    [System.Serializable]
    public struct Dialog
    {
        public Character character;
        public LocalizedString loclText;
        public Emotion talkEmotion;
        public Act act;
    }

    public string GetDialogText()
    {
        if(index >= dialogs.Length-1)
        {
            index =0;
        }
        else
        {
            index++;
        }
        return dialogs[index].loclText.GetLocalizedString();
    }

    public Act GetAct()
    {
        return dialogs[index].act;
    }

    public Emotion GetEmotion()
    {
        return dialogs[index].talkEmotion;
    }

    public Dialog[] GetDialogTalk()
    {
        return dialogs;
    }
}
