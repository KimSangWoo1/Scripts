using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;

public class Npc4StoreView : UIBase, IView
{
    [Header("UI")]
    [SerializeField]
    private Image _abilityIcon;
    [SerializeField]
    private LocalizeStringEvent _localAbilityName;
    [SerializeField]
    private LocalizeStringEvent _localAbilityDescript;

    [Header("Ani")]
    [SerializeField]
    private GameObject _buyAnimation;
    [SerializeField]
    RefuseBuy refuseBuy;

    [Header("Audio")]
    [SerializeField]
    private UIAudioSO _uiAudioSO;

    //Event
    public event Action<InputAction.CallbackContext> Buy;  // 0
    public event Action<InputAction.CallbackContext> Exit;  // 1

    public Action<IMessage> Msg;

    private void Awake()
    {
        Msg += ReceiveMsg;
    }

    public override void InputActive(bool isActive)
    {
        if (isActive)
        {
            EventManager.Instance.Notify(EventType.AddKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Confirm.ToString(), Buy));
            EventManager.Instance.Notify(EventType.AddKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Exit.ToString(), Exit));
        }
        else
        {
            EventManager.Instance?.Notify(EventType.RemoveKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Confirm.ToString(), Buy));
            EventManager.Instance?.Notify(EventType.RemoveKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Exit.ToString(), Exit));
        }
    }

    public override void Close()
    {
        base.Close();
        InputActive(false);
    }

    private void ShowAbility(NpcAbilityProduct npcAbilityProduct)
    {
        _abilityIcon.sprite = npcAbilityProduct.AbilityIcon;
        _localAbilityName.StringReference = npcAbilityProduct.LocalAbilityName;
        _localAbilityDescript.StringReference = npcAbilityProduct.LocalAbilityDescript;

        _localAbilityName.RefreshString();
        _localAbilityDescript.RefreshString();
    }

    private void BuyAnimationOnOff(bool on)
    {
        _buyAnimation.SetActive(on);
    }

    public void ReceiveMsg(IMessage msg)
    {
        var newMsg = (Npc4StoreMsg)msg;
        switch (newMsg.ActionNumber)
        {
            // Init Msg
            case -1:
                InputActive(true);
                AudioManager.Instance.PlaySFX(_uiAudioSO.GetAudioName(eUIAudioType.Open, 0));
                ShowAbility(newMsg.NpcAbilityAddtiveProduct);
                break;
            // Event Msg
            case 0:
                AudioManager.Instance.PlaySFX(_uiAudioSO.GetAudioName(eUIAudioType.Click, 0));
                EventManager.Instance?.Notify(EventType.RemoveKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Confirm.ToString(), Buy));
                EventManager.Instance?.Notify(EventType.RemoveKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Exit.ToString(), Exit));

                BuyAnimationOnOff(true);
                break;
            case 1:
                AudioManager.Instance.PlaySFX(_uiAudioSO.GetAudioName(eUIAudioType.Close, 0));
                Close();
                break;
            //µ· ºÎÁ·
            case 10:
                AudioManager.Instance.PlaySFX(_uiAudioSO.GetAudioName(eUIAudioType.Event, 0));
                refuseBuy.OnRefuse();
                break;
        }
    }

    #region Animation Event
    public void AddNpcAbility()
    {
        // ¾îºô¸®Æ¼ ID·Î Ãß°¡
        var enumerator = DataManager.Instance.NpcAbilityDataDic.Values.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var npcAbility = enumerator.Current as NpcAbility;
            if (npcAbility.NPCType == eNPCType.NPC4)
            {
                npcAbility.LevelUp();
                EventManager.Instance.Notify(EventType.AddAbility, new EventData.AddAbilityData(npcAbility.GetId(), true));
                break;
            }
        }
        BuyAnimationOnOff(false);
        Close();
    }
    #endregion

    #region Message
    public struct Npc4StoreMsg : IMessage
    {
        public int ActionNumber;
        public NpcAbilityAddtiveProduct NpcAbilityAddtiveProduct;
        public Npc4StoreMsg(int actionNumber, NpcAbilityAddtiveProduct npcAbilityAddtiveProduct = null) : this()
        {
            ActionNumber = actionNumber;
            NpcAbilityAddtiveProduct = npcAbilityAddtiveProduct;
        }
    }
    #endregion
}