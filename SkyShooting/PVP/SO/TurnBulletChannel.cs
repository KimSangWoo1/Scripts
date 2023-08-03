using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (fileName = "TurnBulletChannel", menuName = "Assets/Channel/TurnBulletChannel", order =1)]
public class TurnBulletChannel : VoidChannel
{
    public UnityAction playerHitRequested;
    public UnityAction otherPlayerHitRequested;

    public void PlayerHitEvent()
    {
        if (playerHitRequested != null)
        {
            playerHitRequested.Invoke();
        }
    }

    public void OtherPlayerHitEvent()
    {
        if(otherPlayerHitRequested != null)
        {
            otherPlayerHitRequested.Invoke();
        }
    }
}
