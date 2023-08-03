using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BoardManager : Singleton<BoardManager>
{
    public bool update;

    public Text[] playerNames;
    public Text[] scores;

    private int maxShowNum =5;
    private void Awake()
    {

    }
    void Start()
    {
        ShowPlayerList();
    }

    void Update()
    {
        if (update)
        {
            ShowPlayerList();
            update = false;
        }
    }
    
    public void ShowPlayerList()
    {
        //Linq
        var aliveList = from player in SingleGamePlay.playList
                   orderby player.score descending
                   select player;

        int i = 0;
        foreach(Profile profile in aliveList)
        {
            if (i < maxShowNum)
            {
                playerNames[i].text = profile.name;
                scores[i].text = profile.score.ToString();
                i++;
            }
        }
    }

    public void Add_Score(string _name, int _score)
    {
        foreach (Profile profile in SingleGamePlay.playList)
        {
            if (profile.name.Equals(_name))
            {
                profile.score += _score;
                break;
            }
        }
        update = true;
    }

    public int GetScore(string _name)
    {
        int score = 0;
        foreach (Profile profile in SingleGamePlay.playList)
        {
            if (profile.name.Equals(_name))
            {
                score = profile.score;
                break;
            }
        }
        return score;
    }

    //죽은 플레이어와 살아있는 플레이어를 나눈다.
    public void ResetScore(string _name)
    {
        foreach (Profile profile in SingleGamePlay.playList)
        {
            if(profile.name.Equals(_name))
            {
                profile.score = 0;
                break;
            }
        }
        update = true;
    }
}
