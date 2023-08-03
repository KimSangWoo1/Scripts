using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

[Serializable]
public class PlayerStatus :IData
{
    // �⺻ ���� Value
    [SerializeField]
    private int _hp;
    [SerializeField]
    private int _mp;

    public int Hp
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = Math.Clamp(value, 0, MaxHp);
            EventManager.Instance.Notify(EventType.HPChanged, new EventData.HpData(_hp,MaxHp));
        }
    }

    public int Mp
    {
        get => _mp;
        set
        { 
            _mp = Math.Clamp(value, 0, MaxMp);
            EventManager.Instance.Notify(EventType.MPChanged, new EventData.MpData(_mp, MaxMp));
        }
    }

    [field: SerializeField] public int MaxHp { get; set; }                        // �ִ� ü��
    [field: SerializeField] public int MaxMp { get; set; }                        // �ִ� ����
    [field: SerializeField] public int HpRegeneration { get; set; }               // ü�� ȸ����
    [field: SerializeField] public int MpRegeneration { get; set; }               // ���� ȸ����

    [field: SerializeField] public int Power { get; set; }                       // ���ݷ�
    [field: SerializeField] public int Damage { get; set; }                       // ���ط�

    [field: SerializeField] public int Defense { get; set; }                      // ����
    [field: SerializeField] public int MoveSpeed { get; set; }                    // �̵��ӵ�
    [field: SerializeField] public int DashSpeed { get; set; }                    // �뽬 �ӵ�
    [field: SerializeField] public int AttackSpeed { get; set; }                  // ���� �ӵ�
    [field: SerializeField] public int ActionSpeed { get; set; }                  // Ani �ӵ� (��ų ���� �ӵ�)

    // �⺻ ���� Percent
    [field: SerializeField] public float HpRate { get; set; }                     // ü�� ������
    [field: SerializeField] public float MpRate { get; set; }                     // ���� ������
    [field: SerializeField] public float MaxHpRate { get; set; }                  // �ִ� ü��%
    [field: SerializeField] public float MaxMpRate { get; set; }                  // �ִ� ����%
    [field: SerializeField] public float HpRegenerationRate { get; set; }         // ü�� ȸ����
    [field: SerializeField] public float MpRegenerationRate { get; set; }         // ���� ȸ���� 

    [field: SerializeField] public float PowerRate { get; set; }                  // �⺻ ���ݷ�%
    [field: SerializeField] public float DamageRate { get; set; }                 // ���ط�%

    [field: SerializeField] public float DefenseRate { get; set; }                // ���%
    [field: SerializeField] public float MoveSpeedRate { get; set; }              // �̵��ӵ�%
    [field: SerializeField] public float AttackSpeedRate { get; set; }            // ���� �ӵ�%
    [field: SerializeField] public float ActionSpeedRate { get; set; }            // ��ų ���� �ӵ�%

    [field: SerializeField] public float CriticalRate { get; set; }               // ġ��Ÿ Ȯ��
    [field: SerializeField] public float CriticalDamageRate { get; set; }         // ġ��Ÿ ������%
    [field: SerializeField] public float AvoidanceRate { get; set; }              // ȸ��%
    [field: SerializeField] public float LeechLifeRate { get; set; }             // ����%  

    private CancellationTokenSource _cts;
    public CancellationTokenSource Cts { get => _cts; set => _cts = value; }

    #region Update
    public void UpdateStatus(IEventData eventData)
    {
        var abilityData = (EventData.UpdateStatusData)eventData;
        eAbType abType = abilityData.AbType;
        int value = abilityData.Value;

        UpdateStatus(abType, value);
    }

    public void UpdateStatus(eAbType abType, int value)
    {
        switch (abType)
        {
            case eAbType.Hp:                        // ü��
                Hp += value;
                break;
            case eAbType.Mp:                        // ����
                Mp += value;
                break;
            case eAbType.HpRegeneration:            // ü�� ȸ����
                HpRegeneration += value;
                break;
            case eAbType.MpRegeneration:            // ���� ȸ����
                MpRegeneration += value;
                break;
            case eAbType.Power:                     // �⺻ ���ݷ� (��)
                Power += value;
                break;
            case eAbType.Damage:                    // ���ط�
                Damage += value;
                break;
            case eAbType.Defense:                   // ����
                Defense += value;
                break;
            case eAbType.MoveSpeed:                 // �̵��ӵ�
                MoveSpeed += value;
                break;
            case eAbType.DashSpeed:                 // ��� �ӵ�
                DashSpeed += value;
                break;
            case eAbType.AttackSpeed:               // ���ݼӵ�
                AttackSpeed += value;
                break;
            case eAbType.ActionSpeed:               // ��ų�ӵ�
                ActionSpeed += value;
                break;
            case eAbType.MaxHp:                     // �ִ� ü��
                MaxHp += value;
                break;
            case eAbType.MaxMp:                     // �ִ� ����
                MaxMp += value;
                break;
            case eAbType.HpRate:                    // ü�� ������
                HpRate += value;
                break;
            case eAbType.MpRate:                    // ���� ������
                MpRate += value;
                break;
            case eAbType.HpRegenerationRate:        // ü�� ȸ����
                HpRegenerationRate += value;
                break;
            case eAbType.MpRegenerationRate:        // ���� ȸ���� 
                MpRegenerationRate += value;
                break;
            case eAbType.DamageRate:                // ���� ������%
                DamageRate += value;
                break;
            case eAbType.DefenseRate:               // ���%
                DefenseRate += value;
                break;
            case eAbType.MoveSpeedRate:             // �̵��ӵ�%
                MoveSpeedRate += value;
                break;
            case eAbType.AttackSpeedRate:           // ���� �ӵ�%
                AttackSpeedRate += value;
                break;
            case eAbType.ActionSpeedRate:           // ��ų ���� �ӵ�%
                ActionSpeedRate += value;
                break;
            case eAbType.CriticalRate:              // ġ��Ÿ Ȯ��
                CriticalRate += value;
                break;
            case eAbType.CriticalDamageRate:        // ġ��Ÿ ������%
                CriticalDamageRate += value;
                break;
            case eAbType.AvoidanceRate:             // ȸ��%
                AvoidanceRate += value;
                break;
            case eAbType.LeechLifeRate:             // ����%  
                LeechLifeRate += value;
                break;
            case eAbType.MaxHpRate:                 // �ִ� ü��%
                MaxHpRate += value;
                break;
            case eAbType.MaxMpRate:                 // �ִ� ����%
                MaxMpRate += value;
                break;
        }
    }

    #endregion

    #region Second
    public void SecondUpdateStatus(IEventData eventData)
    {
        var abilityData = (EventData.SecondStatusData)eventData;
        eAbType abType = abilityData.AbType;
        int value = abilityData.Value;
        float second = abilityData.Second;

        AsyncShortTimeUpdateStatus(abType, value, second).Forget();
    }

    private async UniTaskVoid AsyncShortTimeUpdateStatus(eAbType abType, int value, float second)
    {
        UpdateStatus(abType, value);
        await UniTask.Delay(TimeSpan.FromSeconds(second), ignoreTimeScale: false);
        UpdateStatus(abType, -value);
    }
    #endregion

    #region Hp
    public void HpChanged()
    {
        if (Hp < MaxMp)
        {
            if (Cts != null) return;

            Cts = new CancellationTokenSource();

            RegenHP().Forget();
        }
    }

    private async UniTaskVoid RegenHP()
    {
        while (Hp < MaxHp)
        {
            Hp += HpRegeneration + (int)(HpRegeneration * HpRegenerationRate);
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: Cts.Token);
        }

        Cts.Dispose();
        Cts = null;
    }
    #endregion

    #region Mp
    public void MpChanged()
    {
        if (Mp < MaxMp)
        {
            if (Cts != null) return;

            Cts = new CancellationTokenSource();

            RegenMP().Forget();
        }
    }

    private async UniTaskVoid RegenMP()
    {
        while (Mp < MaxMp)
        {
            Mp += (int)(MpRegeneration * MpRegenerationRate);
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: Cts.Token);
        }

        Cts.Dispose();
        Cts = null;
    }
    #endregion
}
