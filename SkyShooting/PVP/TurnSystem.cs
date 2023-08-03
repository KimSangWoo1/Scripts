using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystem : MonoBehaviour
{
    [Header("SO")]
    [SerializeField]
    CommandChannel commandChannel;
    [SerializeField]
    GameModeChannel gameModeChannel;

    [Header("Component")]
    [SerializeField]
    private TurnModeSceneManager turnModeSceneManager;

    [Header("UI")]
    [SerializeField]
    Text round;
    [SerializeField]
    Text myTime;
    [SerializeField]
    private GameObject CommandBoard;
    [SerializeField]
    private GameObject roundBoard;
    [SerializeField]
    private GameObject networkInfo;

    private int command = -1;
    private int timeAmount;
    private int roundCount;

    private readonly WaitForSeconds wait = new WaitForSeconds(1f);
    private const int timeMax = 15;

    void Start()
    {
        roundCount = 0;
        timeAmount = timeMax;
    }

    private void OnEnable()
    {
        gameModeChannel.modeRequested += ChangeMode;
    }

    private void OnDisable()
    {
        gameModeChannel.modeRequested -= ChangeMode;
    }

    private void ChangeMode(TurnModeSceneManager.GameMode gameMode)
    {
        switch (gameMode)
        {
            case TurnModeSceneManager.GameMode.READY:
                break;
            case TurnModeSceneManager.GameMode.COMMAND:
                CommandReady();
                CommandStart();
                break;
            case TurnModeSceneManager.GameMode.WAIT:
                CommandEnd();
                break;
            case TurnModeSceneManager.GameMode.ACTION:
                CommuicationWaitEnd();
                break;
            case TurnModeSceneManager.GameMode.RESULT:
                break;
            case TurnModeSceneManager.GameMode.END:
                break;
        }
    }
    
    private void CommuicationWaitEnd()
    {
        networkInfo.SetActive(false);
    }

    private void CommandReady()
    {
        switch (command)
        {
            case 0:
                commandChannel.AttackEvent();
                break;
            case 1:
                commandChannel.ReloadEvent();
                break;
            case 2:
                commandChannel.AvoidEvent();
                break;
            default:
                break;
        }
    }
    private void CommandStart()
    {
        command = -1;
        roundCount++;
        timeAmount = timeMax;

        round.text = roundCount.ToString();
        myTime.text = timeMax.ToString();

        roundBoard.SetActive(true);
        CommandBoard.SetActive(true);
        networkInfo.SetActive(false);

        StartCoroutine(AutoTimeCheck());
    }

    private void CommandEnd()
    {
        StopAllCoroutines();
        
        CommandBoard.SetActive(false);
        roundBoard.SetActive(false);
        networkInfo.SetActive(true);
    }

    #region 코루틴
    IEnumerator AutoTimeCheck()
    {
        while (timeAmount > 0)
        {
            yield return wait;
            timeAmount -= 1;
            myTime.text = timeAmount.ToString();
        }
        //명령 시간 초과시

        command = 1; //장전
        turnModeSceneManager.InputCommand(command);
        turnModeSceneManager.WaitOtherPlyerCommand();

        yield return null;
    }
    #endregion

    #region Button Event
    public void Attack()
    {
        command = 0;
        CommandEnd();
        turnModeSceneManager.InputCommand(command);
        turnModeSceneManager.WaitOtherPlyerCommand();
    }

    public void Reload() 
    {
        command = 1;
        CommandEnd();
        turnModeSceneManager.InputCommand(command);
        turnModeSceneManager.WaitOtherPlyerCommand();
    }

    public void Avoid()
    {
        command = 2;
        CommandEnd();
        turnModeSceneManager.InputCommand(command);
        turnModeSceneManager.WaitOtherPlyerCommand();
    }
    #endregion
}