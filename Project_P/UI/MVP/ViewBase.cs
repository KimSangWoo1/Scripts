using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ViewBase : MonoBehaviour, IEventRegister, IMessageReceiver
{
    [Header("Audio SO")]
    [SerializeField] protected UIAudioSO _uiAudioSO;

    //Event
    public Action<IMessage> Msg;

    #region Common View Handle
    protected virtual void HandleOpen(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Open, index));
    }
    protected virtual void HandleClose(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Close, index));
    }
    protected virtual void HandleMove(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Move, index));
    }
    protected virtual void HandleClick(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Click, index));
    }
    protected virtual void HandleSuccess(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Success, index));
    }
    protected virtual void HandleFailed(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Failed, index));
    }
    protected virtual void HandleResult(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Result, index));
    }
    protected virtual void HandleBack(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Back, index));
    }
    protected virtual void HandleEquip(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Equip, index));
    }
    protected virtual void HandleBuy(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Buy, index));
    }
    protected virtual void HandleMove2(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Move2, index));
    }
    protected virtual void HandleMove3(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Move3, index));
    }
    protected virtual void HandleClick2(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Click2, index));
    }
    protected virtual void HandleClick3(int index = 0)
    {
        AudioManager.Instance.PlayUISFX(_uiAudioSO?.GetAudioClip(eUIEventType.Click3, index));
    }
    #endregion

    #region Implement EventRegister
    public virtual void RegisterEvent()
    {
        Msg += ReceiveMsg;
    }

    public virtual void UnregisterEvent()
    {
        Msg -= ReceiveMsg;
    }
    #endregion

    #region Implement MessageReceiver
    public virtual void ReceiveMsg(IMessage msg)
    {
        var newMsg = (UIMsg)msg;
        switch (newMsg.uiEventType)
        {
            case eUIEventType.Open:
                HandleOpen();
                break;
            case eUIEventType.Close:
                HandleClose();
                break;
            case eUIEventType.Move:
                HandleMove();
                break;
            case eUIEventType.Click:
                HandleClick();
                break;
            case eUIEventType.Success:
                HandleSuccess();
                break;
            case eUIEventType.Failed:
                HandleFailed();
                break;
            case eUIEventType.Result:
                HandleResult();
                break;
            case eUIEventType.Back:
                HandleBack();
                break;
            case eUIEventType.Equip:
                HandleEquip();
                break;
            case eUIEventType.Buy:
                HandleBuy();
                break;
            case eUIEventType.Move2:
                HandleMove2();
                break;
            case eUIEventType.Move3:
                HandleMove3();
                break;
            case eUIEventType.Click2:
                HandleClick2();
                break;
            case eUIEventType.Click3:
                HandleClick3();
                break;
        }
    }
    #endregion
}
