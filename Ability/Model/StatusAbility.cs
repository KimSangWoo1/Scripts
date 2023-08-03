using System;
using System.Linq;

[System.Serializable]
public class StatusAbility : LvAbilityBase
{
    public eAbType AbType;
    private eMainAbType mainAbType = eMainAbType.Status;

    public override eMainAbType GetMainAbType()
    {
        return mainAbType;
    }
}
