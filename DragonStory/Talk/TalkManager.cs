using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : Singleton<TalkManager>
{
    public TalkChannelSO talkChannelSO;
    private DialogSO.Dialog[] dialogs;
    private Dictionary<string, ChangePerformance> chracterDic;
    private int talkIndex;

    private void OnEnable()
    {
        talkChannelSO.NextTalkRequested += NextTalk;
        talkChannelSO.EndTalkRequested += EndTalk;
    }

    private void OnDisable()
    {
        talkChannelSO.NextTalkRequested -= NextTalk;
        talkChannelSO.EndTalkRequested -= EndTalk;
    }

    public void OnTalk(DialogSO.Dialog[] _dialogs)
    {
        if(_dialogs !=null)
        {
            if (_dialogs.Length != 0)
            {
                talkIndex = 0;
                dialogs = _dialogs;
                chracterDic = new Dictionary<string, ChangePerformance>();
                GameManager.ChangeMode(Mode.TALK);
                talkChannelSO.OnTalkEvent();
                talkChannelSO.NextTalkEvent(0);
            }
        }
    }

    public void NextTalk(int next)
    {
        talkIndex += next;
        if (talkIndex<dialogs.Length)
        {
            string name = dialogs[talkIndex].character.actor.ToString();
          

            // TalkChannel - NPC 
            if (chracterDic.Count != 0)
            {
                if (chracterDic.ContainsKey(name))
                {
                    TalkerPerformance(name, dialogs[talkIndex].talkEmotion, dialogs[talkIndex].act); //실행
                }
                else
                {
                     FindTalker(dialogs[talkIndex].character.actor.ToString());  //찾고 넣기
                    TalkerPerformance(name, dialogs[talkIndex].talkEmotion, dialogs[talkIndex].act); //실행
                }
            }
            else
            {
                FindTalker(dialogs[talkIndex].character.actor.ToString());  //찾고 넣기
                TalkerPerformance(name, dialogs[talkIndex].talkEmotion, dialogs[talkIndex].act); //실행
            }

            // TalkChannel - ScriptBoard    
            talkChannelSO.TalkEvent(dialogs[talkIndex]);
        }
        else
        {
            talkChannelSO.EndTalkEvent();
        }
    }

    public void EndTalk()
    {
        GameManager.ChangeMode(Mode.NONE);
        foreach (string key in chracterDic.Keys)
        {
            chracterDic[key].init();
        }
        chracterDic.Clear();
    }

    private void FindTalker(string nameTag)
    {
        chracterDic.Add(nameTag, GameObject.FindWithTag(nameTag).GetComponent<ChangePerformance>());
    }

    private void TalkerPerformance(string key, Emotion emotion, Act act)
    {
        // 실행
        chracterDic[key].ChageFace(emotion);
        chracterDic[key].Action(act);
    }
}
