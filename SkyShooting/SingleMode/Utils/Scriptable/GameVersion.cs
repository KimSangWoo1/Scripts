using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameVersion", menuName = "Scriptable Object/GameVersion", order = 1)]
public class GameVersion : ScriptableObject
{
    [SerializeField]
    private string version;

    private const string startGameText ="- Touch to Start -";
   public string get_Version()
    {
        return version;
    }
    public string get_StartGameText()
    {
        return startGameText;
    }
}
