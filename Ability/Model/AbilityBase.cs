using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IData
{

}

public interface IAbility : IData
{
    public int GetId();
    public int GetValue();
    public eMainAbType GetMainAbType();
}

public class AbilityBase : IAbility
{
    public int Id;
    public string Name;
    public string Title;
    public string Details;
    public string Icon;

    public AbilityBase() { }

    public int GetId()
    {
        return Id;
    }

    public virtual eMainAbType GetMainAbType()
    {
        return default;
    }

    public virtual int GetValue() 
    {
        return default;
    }

    public virtual int GetNextValue()
    {
        return default;
    }
}
