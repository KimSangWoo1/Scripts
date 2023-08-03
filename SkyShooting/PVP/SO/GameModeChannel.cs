using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameModeChannel", menuName = "Assets/Channel/GameModeChannel", order =1)]
public class GameModeChannel : DescriptSO
{
    public UnityAction<TurnModeSceneManager.GameMode> modeRequested;
    public UnityAction startRequested;

    public void ModeEvent(TurnModeSceneManager.GameMode gameMode)
    {
        if(modeRequested != null)
        {
            modeRequested.Invoke(gameMode);
        }
    }

    public void GameStart()
    {
        if (startRequested != null)
        {
            startRequested.Invoke();
        }
    }
}
