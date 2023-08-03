using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CommandChannel", menuName = "Assets/Channel/CommandChannel")]
public class CommandChannel : DescriptSO
{
    public  UnityAction<string> player1ActionReuqested;
    public  UnityAction<string> player2ActionReuqested;

    public void Player1ActionEvent(string action)
    {
        if(player1ActionReuqested != null)
        {
            player1ActionReuqested.Invoke(action);
        }
    }

    public void Player2ActionEvent(string action)
    {
        if (player2ActionReuqested != null)
        {
            player2ActionReuqested.Invoke(action);
        }
    }
    
    public UnityAction attackRequested;
    public UnityAction ReloadRequested;
    public UnityAction AvoidRequested;

    public void AttackEvent()
    {
        if (attackRequested != null)
        {
            attackRequested.Invoke();
        }
    }

    public void ReloadEvent()
    {
        if (ReloadRequested != null)
        {
            ReloadRequested.Invoke();
        }
    }

    public void AvoidEvent()
    {
        if (AvoidRequested != null)
        {
            AvoidRequested.Invoke();
        }
    }
}
