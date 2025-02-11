using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityBoardPresenter : UIBase
{
    private AbilityBoardView _abilityBoardView;
    [SerializeField] Selector _abilitySelector;
    [SerializeField] Selector _abilityGroupSelector;
    [SerializeField] RefuseBuy refuseBuy;

    #region Mono
    private void Awake()
    {
        _abilityBoardView = GetComponent<AbilityBoardView>();
    }

    private void OnEnable()
    {
        _abilityGroupSelector.Move += HandleMoveAbilityGroup;
        _abilitySelector.Move += HandleMoveAbility;
        _abilitySelector.Confirm += HandleConfirmAbility;

        _abilityBoardView.RegisterEvent();
        RegisterInput();
    }

    private void OnDisable()
    {
        _abilityGroupSelector.Move -= HandleMoveAbilityGroup;
        _abilitySelector.Move -= HandleMoveAbility;
        _abilitySelector.Confirm -= HandleConfirmAbility;

        _abilityBoardView.UnregisterEvent();
        UnRegisterInput();
    }
    #endregion

    #region UIBase Override Method
    public override void Initialize()
    {
        base.Initialize();

        int mococoTotal = 0;
#if UNITY_EDITOR
        if (CheatKey.IsDevOn)
        {
            mococoTotal = CheatKey.mococo;
        }
        else
        {
            mococoTotal = DataManager.Instance.GameData.GetMococoCount();
        }
#else
        mococoTotal = DataManager.Instance.GameData.GetMococoCount();
#endif
        _abilityBoardView.Initialization(mococoTotal, DataManager.Instance.AbilityGroupDic);

        ShowEquipAbilities();
    }

    public override void Open()
    {
        base.Open();
        _abilityBoardView.Msg?.Invoke(new UIMsg(eUIEventType.Open));
        HandleMoveAbilityGroup();
    }

    public override void Close()
    {
        UIManager.Instance?.Close<UIBase>(_uiType, false);
        _abilityBoardView.Msg?.Invoke(new UIMsg(eUIEventType.Close));
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
        EventManager.Instance.Notify(EventType.AddKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Exit.ToString(), HandleClose));
        EventManager.Instance.Notify(EventType.AddKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.R.ToString(), HandleResetAbilities));
    }

    public override void UnRegisterInput()
    {
        base.UnRegisterInput();
        EventManager.Instance?.Notify(EventType.RemoveKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.Exit.ToString(), HandleClose));
        EventManager.Instance?.Notify(EventType.RemoveKeyDown, new EventData.InputKeyData(eActionMapType.UI, eUIKeyType.R.ToString(), HandleResetAbilities));
    }
#endregion

    #region Handle
    protected override void HandleClose(InputAction.CallbackContext callbackContext)
    {
        Close();
    }

    private void HandleResetAbilities(InputAction.CallbackContext callbackContext)
    {
        //장착된 어빌리티 초기화
        DataManager.Instance.GameData.PlayerAbility.Clear();
        _abilityBoardView.ResetAbilities?.Invoke();
    }

    private void HandleMoveAbilityGroup()
    {
        int groupId = _abilityGroupSelector.CurrentSelect;
        int abilityId = DataManager.Instance.GameData.PlayerAbility.TryGetValue(groupId, out int result) ? result : 0;

        _abilityBoardView.ShowAbilities?.Invoke(groupId, abilityId);
        _abilityBoardView.Msg?.Invoke(new UIMsg(eUIEventType.Move2));
        refuseBuy.Refresh();
    }

    private void HandleMoveAbility()
    {
        _abilityBoardView.Msg?.Invoke(new UIMsg(eUIEventType.Move));
    }

    private void HandleConfirmAbility()
    {
        var groupItem = (AbilityGroupItem)_abilityGroupSelector.CurrentItem;

        if (groupItem.IsOpen)
        {
            int groupId = _abilityGroupSelector.CurrentSelect;
            int abilityId = ((AbilityItem)_abilitySelector.CurrentItem).Id;

            if (DataManager.Instance.GameData.PlayerAbility.ContainsKey(groupId))
            {
                //교체
                if (DataManager.Instance.GameData.PlayerAbility[groupId] != 0)
                {
                    int equipedAbilityId = DataManager.Instance.GameData.PlayerAbility[groupId];
                    EventManager.Instance.Notify(EventType.ChageAbility, new EventData.ChagneAbilityData(equipedAbilityId, abilityId));
                }
                else
                {
                    Debug.LogWarning($"Error Ability (어빌리티 추가중 오류가 있습니다.");
                }
            }
            else
            {
                //장착
                EventManager.Instance.Notify(EventType.AddAbility, new EventData.AddAbilityData(abilityId));
            }

            DataManager.Instance.GameData.PlayerAbility[groupId] = abilityId;
            _abilityBoardView.EquipAbility?.Invoke(_abilityGroupSelector.CurrentSelect, abilityId);

            ShowEquipAbilities();
            _abilityBoardView.Msg?.Invoke(new UIMsg(eUIEventType.Success));
        }
        else
        {
            refuseBuy.OnRefuse();
            _abilityBoardView.Msg?.Invoke(new UIMsg(eUIEventType.Failed));
        }
    }
    #endregion

    private void ShowEquipAbilities()
    {
        //장착된 어빌리티 Data 전달
        List<(int groupId, string iconName)> EquipAbilities = new();
        foreach (var data in DataManager.Instance.GameData.PlayerAbility)
        {
            var ab = DataManager.Instance.AbilityDataDic[data.Value] as AbilityBase;
            EquipAbilities.Add((data.Key, ab.Icon));
        }

        _abilityBoardView.ShowEquippedAbilities?.Invoke(EquipAbilities);
    }
}
