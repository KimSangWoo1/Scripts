using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

[ExecuteInEditMode]
public class ScriptBoardController : Observer
{
    public TalkChannelSO talkChannelSO;

    [Header("UGUI")]
    public Image scriptBoard;
    public Image nextButton;
    public Text scriptName;
    public Text scriptContent;
    public LocalizeStringEvent localizedStringEvent;
    [Header("Audio")]
    public StoryAudioController storyAudioController;

    //
    private bool currentTyping;
    // 캐싱
    private  WaitForSeconds wait;
    // 상수
    private const float delay = 0.04f;

    private void OnEnable()
    {
        talkChannelSO.OnTalkRequested += OnTalk;
        talkChannelSO.TalkRequested += Talk;
        talkChannelSO.EndTalkRequested += EndTalk;
    }

    private void Start()
    {
        wait = new WaitForSeconds(delay);
        localizedStringEvent.SetTable("Character Name");
    }

    private void OnDisable()
    {
        talkChannelSO.OnTalkRequested -= OnTalk;
        talkChannelSO.TalkRequested -= Talk;
        talkChannelSO.EndTalkRequested -= EndTalk;
    }

    //스크립트 Board On/Off     
    private void ScriptBaordSwitch(bool onOff)
    {
        scriptBoard.gameObject.SetActive(onOff);
    }

    IEnumerator AutoComplete(string content, bool auto)
    {
        currentTyping = true;
        nextButton.gameObject.SetActive(false);
        scriptContent.text = "";
        
        for(int i=0; i<content.Length; i++)
        {
            yield return wait;
            scriptContent.text += content[i];
            if (i % 2 == 0)
                storyAudioController.Speaking();
        }
        currentTyping = false;
        if (!auto)
        {
            nextButton.gameObject.SetActive(true);
        }
    }
    #region Talk Channel
    public void OnTalk()
    {
        GameManager.ChangeMode(Mode.TALK);
        ScriptBaordSwitch(true);
    }

    public void Talk(DialogSO.Dialog talkDialog)
    {
        localizedStringEvent.SetEntry(talkDialog.character.actor.ToString());
        scriptName.text = localizedStringEvent.StringReference.GetLocalizedString();

        storyAudioController.VoiceSetting(talkDialog.character.actor);
        StartCoroutine(AutoComplete(talkDialog.loclText.GetLocalizedString(), false));
    }

    public void EndTalk()
    {
        // 끝날경우
        GameManager.ChangeMode(Mode.NONE);
        ScriptBaordSwitch(false); //Board Off
    }
    #endregion

    #region Observer Method
    public override void OnStartNotify()
    {
        GameManager.ChangeMode(Mode.STORY);
        ScriptBaordSwitch(true);
    }

    public override void OnNotify(Sequence sequence)
    {
        if (sequence.Script.Name.Length != 0)
        {
            if (!sequence.Script.Name.Contains("?"))
            {
                localizedStringEvent.SetEntry(sequence.Cast.actor.ToString());
                scriptName.text = localizedStringEvent.StringReference.GetLocalizedString();
            }
            else
            {
                scriptName.text = sequence.Script.Name;
            }

            ScriptBaordSwitch(true);
            StartCoroutine(AutoComplete(sequence.Script.Content, sequence.Script.Auto));
        }
        else
        {
            ScriptBaordSwitch(false);
        }
    }

    public override void OnStoryEnd()
    {
        GameManager.ChangeMode(Mode.NONE);
        ScriptBaordSwitch(false);
        StopAllCoroutines();
    }
    #endregion
}
