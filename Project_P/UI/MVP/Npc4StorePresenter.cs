using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Npc4StorePresenter : NpcStorePresenterBase
{
    [SerializeField]
    NpcAbilityAddtiveProductSO _npcAbilityProductSO;
    Npc4StoreView _npcStoreView;

    #region Mono
    private void Awake()
    {
        _npcStoreView = GetComponent<Npc4StoreView>();
    }

    private void OnEnable()
    {
        _npcStoreView.OnBuy += HandleBuy;
        _npcStoreView.OnClose += HandleClose;

        _npcStoreView.RegisterEvent();
        RegisterInput();
    }

    private void OnDisable()
    {
        _npcStoreView.OnBuy -= HandleBuy;
        _npcStoreView.OnClose -= HandleClose;

        _npcStoreView.UnregisterEvent();
        UnRegisterInput();
    }
    #endregion

    #region UIBase Override Method
    public override void Open()
    {
        base.Open();
        _npcStoreView.Msg?.Invoke(new Npc4StoreView.Npc4StoreMsg(eUIEventType.Open, _npcAbilityProductSO._npcAbilityAddtiveProduct));
    }

    public override void Close()
    {
        base.Close();
        _npcStoreView.Msg?.Invoke(new Npc4StoreView.Npc4StoreMsg(eUIEventType.Close));
    }

    public override void Show()
    {
        base.Show();
        RegisterInput();
    }

    public override void Hide()
    {
        base.Hide();
        UnRegisterInput();
    }

    public override void RegisterInput()
    {
        base.RegisterInput();
        EventManager.Instance.Notify(EventType.AddKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Confirm.ToString(), HandleBuy));
        EventManager.Instance.Notify(EventType.AddKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Exit.ToString(), HandleClose));
    }

    public override void UnRegisterInput()
    {
        base.UnRegisterInput();
        EventManager.Instance?.Notify(EventType.RemoveKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Confirm.ToString(), HandleBuy));
        EventManager.Instance?.Notify(EventType.RemoveKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Exit.ToString(), HandleClose));
    }
    #endregion

    #region  NpcStorePresenterBase Override Method
    protected override void HandleBuy(InputAction.CallbackContext callbackContext)
    {
        float maxHpDecreasePercentage = 0.01f * _npcAbilityProductSO._npcAbilityAddtiveProduct.AbValue;
        //최대 체력 비교
        if (ModelManager.PlayerModel.Stats.Hp > ModelManager.PlayerModel.Stats.MaxHp * maxHpDecreasePercentage)
        {
            DataManager.Instance.GameUtilsData.Npc4StoreTicket--;
            ModelManager.PlayerModel.Stats.MaxHp -= (int)(ModelManager.PlayerModel.Stats.MaxHp * maxHpDecreasePercentage); // MaxHp를 감소
            float hp = ModelManager.PlayerModel.Stats.Hp;
            hp *= (1.0f - maxHpDecreasePercentage); // 현재 체력을 비율을 곱하여 변경
            ModelManager.PlayerModel.Stats.Hp = (int)hp;

            _npcStoreView.Msg.Invoke(new Npc4StoreView.Npc4StoreMsg(eUIEventType.Click));
            UnRegisterInput();
        }
        else
        {
            //구매 불가
            _npcStoreView.Msg.Invoke(new Npc4StoreView.Npc4StoreMsg(eUIEventType.Failed));
        }
    }

    protected override void HandleClose(InputAction.CallbackContext callbackContext)
    {
        _npcStoreView.Msg?.Invoke(new Npc4StoreView.Npc4StoreMsg(eUIEventType.Close));
        Close();
    }
    #endregion

    #region Animation Event
    public void AddNpcAbility()
    {
        // 어빌리티 ID로 추가
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
        Close();
    }
    #endregion
}
