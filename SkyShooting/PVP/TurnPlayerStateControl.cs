using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnPlayerStateControl : MonoBehaviour
{
    [Header("SO")]
    [SerializeField]
    CommandChannel commandChannel;
    [SerializeField]
    TurnBulletChannel turnBulletChannel;

    [Header("UI")]
    [SerializeField]
    private Image[] playerHP;
    [SerializeField]
    private Image[] otherPlayerHP;
    [SerializeField]
    private Image[] magazine;
    [SerializeField]
    private Image[] gas;

    [SerializeField]
    private Button attackBtn;
    [SerializeField]
    private Button avoidBtn;

    private int magazineAmount = 3;
    private int gasAmount = 5;

    private void OnEnable()
    {
        commandChannel.attackRequested += Attack;
        commandChannel.ReloadRequested += Reload;
        commandChannel.AvoidRequested += Avoid;

        turnBulletChannel.playerHitRequested += PlayerHit;
        turnBulletChannel.otherPlayerHitRequested += OtherPlayerHit;
    }

    private void OnDisable()
    {
        commandChannel.attackRequested -= Attack;
        commandChannel.ReloadRequested -= Reload;
        commandChannel.AvoidRequested -= Avoid;

        turnBulletChannel.playerHitRequested -= PlayerHit;
        turnBulletChannel.otherPlayerHitRequested -= OtherPlayerHit;
    }

    #region Command Delegate
    private void Attack()
    {
        for(int i=0; i < magazine.Length; i++)
        {
            if (magazine[i].gameObject.activeSelf)
            {
                magazine[i].gameObject.SetActive(false);
                break;
            }
        }
        magazineAmount--;

        if (magazineAmount <= 0) {
            attackBtn.interactable = false;
        }
    }

    private void Reload()
    {
        for (int i = 0; i < magazine.Length; i++)
        {
            magazine[i].gameObject.SetActive(true);
        }

        magazineAmount = 3;
        attackBtn.interactable = true;
    }

    private void Avoid()
    {
        for (int i = 0; i < gas.Length; i++)
        {
            if (gas[i].gameObject.activeSelf)
            {
                gas[i].gameObject.SetActive(false);
                break;
            }
        }
        gasAmount--;

        if (gasAmount <= 0)
        {
            avoidBtn.interactable = false;
        }
    }

    #endregion

    #region HP Delegate
    private void PlayerHit()
    {
        for(int i=0; i < playerHP.Length; i++)
        {
            if (playerHP[i].gameObject.activeSelf)
            {
                playerHP[i].gameObject.SetActive(false);
                break;
            }
        }
    }

    private void OtherPlayerHit()
    {
        for (int i = 0; i < otherPlayerHP.Length; i++)
        {
            if (otherPlayerHP[i].gameObject.activeSelf)
            {
                otherPlayerHP[i].gameObject.SetActive(false);
                break;
            }
        }
    }
    #endregion
}
