using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;

public class Npc4StoreView : MonoBehaviour, IEventView
{
    [Header("UI-Ability")]
    [SerializeField] Image _abilityIcon;
    [SerializeField] LocalizeStringEvent _localAbilityName;
    [SerializeField] LocalizeStringEvent _localAbilityDescript;

    [Header("Ani")]
    [SerializeField] GameObject _buyAnimation;
    [SerializeField] RefuseBuy refuseBuy;

    [Header("Audio SO")]
    [SerializeField] UIAudioSO _uiAudioSO;

    //Event
    public Action<IMessage> Msg;

    public Action<InputAction.CallbackContext> OnBuy;
    public Action<InputAction.CallbackContext> OnClose;

    #region Store
    private void OpenView()
    {
        AudioManager.Instance.PlaySFX(_uiAudioSO.GetAudioName(eUIAudioType.Open, 0));
    }

    private void CloseView()
    {
        AudioManager.Instance.PlaySFX(_uiAudioSO.GetAudioName(eUIAudioType.Close, 0));
    }

    private void BuyFailed()
    {
        AudioManager.Instance.PlaySFX(_uiAudioSO.GetAudioName(eUIAudioType.Event, 0));
        refuseBuy.OnRefuse();
    }

    private void BuyAbility()
    {
        AudioManager.Instance.PlaySFX(_uiAudioSO.GetAudioName(eUIAudioType.Click, 0));
        BuyAnimationOnOff(true);
    }

    private void ShowAbility(NpcAbilityProduct npcAbilityProduct)
    {
        _abilityIcon.sprite = npcAbilityProduct.AbilityIcon;
        _localAbilityName.StringReference = npcAbilityProduct.LocalAbilityName;
        _localAbilityDescript.StringReference = npcAbilityProduct.LocalAbilityDescript;

        _localAbilityName.RefreshString();
        _localAbilityDescript.RefreshString();
    }
    #endregion

    #region Inherit Event
    public void RegisterEvent()
    {
        Msg += ReceiveMsg;
    }

    public void UnregisterEvent()
    {
        Msg -= ReceiveMsg;
    }

    public void ReceiveMsg(IMessage msg)
    {
        var newMsg = (Npc4StoreMsg)msg;
        switch (newMsg.uiEventType)
        {
            case eUIEventType.Open:
                OpenView();
                ShowAbility(newMsg.npcAbilityAddtiveProduct);
                break;
            case eUIEventType.Click:
                BuyAbility();
                break;
            case eUIEventType.Close:
                CloseView();
                break;
            //돈 부족
            case eUIEventType.Failed:
                BuyFailed();
                break;
        }
    }
    #endregion

    #region Animation Event
    public void BuyAnimationOnOff(bool on)
    {
        _buyAnimation.SetActive(on);
    }
    #endregion

    #region Message
    public struct Npc4StoreMsg : IMessage
    {
        public eUIEventType uiEventType;
        public NpcAbilityAddtiveProduct npcAbilityAddtiveProduct;
        public Npc4StoreMsg(eUIEventType uiEventType, NpcAbilityAddtiveProduct npcAbilityAddtiveProduct = null) : this()
        {
            this.uiEventType = uiEventType;
            this.npcAbilityAddtiveProduct = npcAbilityAddtiveProduct;
        }
    }
    #endregion
}
