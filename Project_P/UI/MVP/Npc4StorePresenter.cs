using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Npc4StorePresenter : MonoBehaviour
{
    [SerializeField]
    NpcAbilityAddtiveProductSO _npcAbilityProductSO;
    Npc4StoreView _npcStoreView;

    private void Awake()
    {
        _npcStoreView = GetComponent<Npc4StoreView>();

        EventInit();
    }

    private void OnEnable()
    {
        _npcStoreView.Msg?.Invoke(new Npc4StoreView.Npc4StoreMsg(-1, _npcAbilityProductSO._npcAbilityAddtiveProduct));
    }

    private void EventInit()
    {
        _npcStoreView.Buy += OnBuy;
        _npcStoreView.Exit += OnExit;
    }

    private void OnBuy(InputAction.CallbackContext callbackContext)
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

            _npcStoreView.Msg.Invoke(new Npc4StoreView.Npc4StoreMsg(0));
        }
        else
        {
            //구매 불가
            _npcStoreView.Msg.Invoke(new Npc4StoreView.Npc4StoreMsg(10));
        }
    }

    private void OnExit(InputAction.CallbackContext callbackContext)
    {
        _npcStoreView.Msg?.Invoke(new Npc4StoreView.Npc4StoreMsg(1));
    }
}