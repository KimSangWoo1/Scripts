using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TalkChannelSO", menuName = "ScriptableObjects/Channel/TalkChannelSO", order = 1)]
public class TalkChannelSO : ScriptableObject
{
    public UnityAction OnTalkRequested;
    public UnityAction<DialogSO.Dialog> TalkRequested;
    public UnityAction<int> NextTalkRequested;
    public UnityAction EndTalkRequested;
    public void OnTalkEvent()
    {
        if (OnTalkRequested != null)
        {
            OnTalkRequested.Invoke();
        }
        else
        {
            Debug.LogWarning("OnTalkRequested Null");
        }
    }

    public void TalkEvent(DialogSO.Dialog dialog)
    {
        if (TalkRequested != null)
        {
            TalkRequested.Invoke(dialog);
        }
        else
        {
            Debug.LogWarning("TalkRequested Null");
        }
    }

    public void NextTalkEvent(int next = 1)
    {
        if (NextTalkRequested != null)
        {
            NextTalkRequested.Invoke(next);
        }
        else
        {
            Debug.LogWarning("NextTalkRequested Null");
        }
    }

    public void EndTalkEvent()
    {
        if (EndTalkRequested != null)
        {
            EndTalkRequested.Invoke();
        }
        else
        {
            Debug.LogWarning("EndTalkRequested Null");
        }
    }

}



