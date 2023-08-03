using UnityEngine;

public class MessageSender : MonoBehaviour
{
    public GameObject mono;
    private Message.IMessageReceiver onInteractionMessageReceiver;
    Interaction.InteractMessage interactMsg;
    Interaction.DamageMessage damageMsg;
    private void Start()
    {
        OnValidate();
    }

    // IMessageReceiver를 상속 받은 Script가 있는 Object를 받도록 한다.!!
    void OnValidate()
    {
        onInteractionMessageReceiver = mono.GetComponent<Message.IMessageReceiver>();
        if (onInteractionMessageReceiver == null) mono = null;
    }
    #region HP Method
    //HP 증가
    public void Apply_AddHealth(ObjectPooling.Item_HealthState _healthState)
    {
        switch (_healthState)
        {
            case ObjectPooling.Item_HealthState.Red:
                Add_RedHealth();
                break;
            case ObjectPooling.Item_HealthState.Yellow:
                Add_YellowHealth();
                break;
            case ObjectPooling.Item_HealthState.Green:
                Add_GreenHealth();
                break;
        }
    }
    private void Add_RedHealth()
    {
        interactMsg = new Interaction.InteractMessage
        {
            amount = 30, //HP 0.3증가
            upgrade = false
        };
        onInteractionMessageReceiver.OnReceiver_InteractMessage(Message.MessageType.HEALTH, interactMsg);
    }

    private void Add_YellowHealth()
    {
        interactMsg = new Interaction.InteractMessage
        {
            amount = 60, //HP 0.6 증가
            upgrade = false
        };
        onInteractionMessageReceiver.OnReceiver_InteractMessage(Message.MessageType.HEALTH, interactMsg);
    }

    private void Add_GreenHealth()
    {
        interactMsg = new Interaction.InteractMessage
        {
            amount = 100, //HP 1증가
            upgrade = false
        };
        onInteractionMessageReceiver.OnReceiver_InteractMessage(Message.MessageType.HEALTH, interactMsg);
    }
    #endregion

    #region Money Method
    public void Apply_AddMoney(ObjectPooling.Item_DollarState _dollarState)
    {
        switch (_dollarState)
        {
            case ObjectPooling.Item_DollarState.Red:
                Add_RedMoney();
                break;
            case ObjectPooling.Item_DollarState.Yellow:
                Add_YellowMoney();
                break;
            case ObjectPooling.Item_DollarState.Green:
                Add_GreenMoney();
                break;
        }
    }
    private void Add_RedMoney()
    {
        interactMsg = new Interaction.InteractMessage
        {
            amount = 5, //돈 5 증가
            upgrade = false
        };
        onInteractionMessageReceiver.OnReceiver_InteractMessage(Message.MessageType.DOLLAR, interactMsg);
    }

    private void Add_YellowMoney()
    {
        interactMsg = new Interaction.InteractMessage
        {
            amount = 10, //돈 10 증가
            upgrade = false
        };
        onInteractionMessageReceiver.OnReceiver_InteractMessage(Message.MessageType.DOLLAR, interactMsg);
    }

    private void Add_GreenMoney()
    {
        interactMsg = new Interaction.InteractMessage
        {
            amount = 20, //돈 20 증가
            upgrade = false
        };
        onInteractionMessageReceiver.OnReceiver_InteractMessage(Message.MessageType.DOLLAR, interactMsg);
    }
    #endregion

    #region Item Upgrade Method
    //총알 증가
    public void Apply_AddBullet()
    {
        interactMsg = new Interaction.InteractMessage
        {
            amount = 0,
            upgrade = true
        };
        onInteractionMessageReceiver.OnReceiver_InteractMessage(Message.MessageType.BULLET, interactMsg);
    }
    //스피드 증가
    public void Apply_AddTurbin()
    {
        interactMsg = new Interaction.InteractMessage
        {
            amount = 5, //runSpeed 5 증가 , fireWaitTime  0.1증가
            upgrade = true
        };
        onInteractionMessageReceiver.OnReceiver_InteractMessage(Message.MessageType.TURBIN, interactMsg);
    }
    //총구 증가
    public void Apply_AddMuzzle()
    {
        interactMsg = new Interaction.InteractMessage
        {
            amount = 0,
            upgrade = true
        };
        onInteractionMessageReceiver.OnReceiver_InteractMessage(Message.MessageType.MUZZLE, interactMsg);
    }
    #endregion

    #region 데미지 메세지
    //데미지 적용
    public void ApplyDamage(string _name)
    {
        damageMsg = new Interaction.DamageMessage
        {
            name = _name,   
            damage = 10, // 대미지 0.1f
        };
        onInteractionMessageReceiver.OnReceiver_DamageMessage(Message.MessageType.DAMAGE, damageMsg);
    }
    //자폭
    public void Self_Destruction()
    {
        damageMsg = new Interaction.DamageMessage
        {
            name = "",
            damage = 100, // 대미지 1f
        };
        onInteractionMessageReceiver.OnReceiver_DamageMessage(Message.MessageType.CLASH, damageMsg);
    }
    #endregion

}
