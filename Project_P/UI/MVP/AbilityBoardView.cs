using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBoardView : ViewBase, IEventRegister
{
    [SerializeField] Slider _rewardSlider;
    [SerializeField] List<AbilityRewardItem> _abilityRewardItems;
    [SerializeField] List<AbilityGroupItem> _abilityGroupItems;

    [SerializeField] GameObject _selectShowUI;
    [SerializeField] GameObject _banShowUI;

    //Event
    public Action ResetAbilities;
    public Action<int,int> ShowAbilities; // Group���� Abilities �����ֱ�
    public Action<int,int> EquipAbility; // Ability ����
    public Action<List<(int groupId,string iconName)>> ShowEquippedAbilities; // ������ Abilities �����ֱ�

    public void Initialization(int mococoTotal, SortedDictionary<int, List<IAbility>> abilityGroupDic)
    {
        ShowRewardAchievement(mococoTotal);
        SetAbilityData(abilityGroupDic);
    }

    // ������ �޼��� ǥ��
    private void ShowRewardAchievement(int value)
    {
        _rewardSlider.value = value;

        for (int i = 0; i < _abilityRewardItems.Count && i < _abilityGroupItems.Count; i++)
        {
            _abilityRewardItems[i].Initialization(value);
            _abilityGroupItems[i].Open(_abilityRewardItems[i].IsOpen);
        }
    }

    // Fixed Ability ǥ��
    private void SetAbilityData(SortedDictionary<int, List<IAbility>> abilityGroupDic)
    {
        var irator = abilityGroupDic.GetEnumerator();

        for (int i = 0; i < _abilityGroupItems.Count; i++)
        {
            if (!irator.MoveNext())
                break;

            _abilityGroupItems[i].Initialization(irator.Current.Value);
        }
    }

    private void HandleResetAbilities()
    {
        for(int i=0; i<_abilityGroupItems.Count; i++)
        {
            _abilityGroupItems[i].Reset();
        }
    }

    // Ability List Show
    private void HandleShowAbilities(int groupIndex, int equipAbilityId)
    {
        _abilityGroupItems[groupIndex].ShowAbilities();
        _abilityGroupItems[groupIndex].MarkingEquipAbilities(equipAbilityId);

        if (_abilityGroupItems[groupIndex].IsOpen)
        {
            _selectShowUI.SetActive(true);
            _banShowUI.SetActive(false);
        }
        else
        {
            _selectShowUI.SetActive(false);
            _banShowUI.SetActive(true);
        }
    }

    private void HandleEquipAbility(int groupIndex, int equipAbilityId)
    {
        _abilityGroupItems[groupIndex].MarkingEquipAbilities(equipAbilityId);
    }

    private void HandleShowEquippedAbilities(List<(int groupId, string iconName)> equipAbilities)
    {
        for(int i=0; i< equipAbilities.Count; i++)
        {
            _abilityGroupItems[equipAbilities[i].groupId].ShowEquippedAbility(equipAbilities[i].iconName);
        }
    }

    #region Implement Event
    public override void RegisterEvent()
    {
        base.RegisterEvent();

        ResetAbilities += HandleResetAbilities;
        ShowAbilities += HandleShowAbilities;
        EquipAbility += HandleEquipAbility;
        ShowEquippedAbilities += HandleShowEquippedAbilities;
    }

    public override void UnregisterEvent()
    {
        base.RegisterEvent();

        ResetAbilities -= HandleResetAbilities;
        ShowAbilities -= HandleShowAbilities;
        EquipAbility -= HandleEquipAbility;
        ShowEquippedAbilities -= HandleShowEquippedAbilities;
    }
    #endregion
}
