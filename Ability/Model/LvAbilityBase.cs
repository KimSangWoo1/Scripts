using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class LvAbilityBase : AbilityBase
{
    public int[] LvValues;

    [SerializeField]
    private int _currentLevel = -1;
    public int CurrentLevel
    {
        get => _currentLevel;
        set => _currentLevel = value;
    }

    public override int GetValue()
    {
        return LvValues[_currentLevel];
    }

    public override int GetNextValue()
    {
        int nextLevel = Mathf.Clamp(_currentLevel + 1, 0, LvValues.Length);
        return LvValues[nextLevel];
    }
}
