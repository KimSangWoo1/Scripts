using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SingleGamePlay
{
    private static string[] names;
    public static Queue<string> randomName = new Queue<string>();
    public static List<Profile> playList = new List<Profile>();

    public static void Make_RandomName()
    { //98개의 랜덤 이름
        names = new string[99] {
                                "Sophi", "Jackson",
                                "Emma", "Aiden",
                                "Olivia", "Liam",
                                "Ava", "Lucas",
                                "Mia", "Noah",
                                "Isabella", "Mason",
                                "Zoe", "Ethan,",
                                "Lily", "Caden",
                                "Emily", "Logan",
                                "Madison", "Jacob",
                                "Amelia", "Jayden",
                                "Riley", "Oliver",
                                "Madelyn", "Elijah",
                                "Charlotte", "Alexander",
                                "Chloe", "Michael",
                                "Aubrey", "Carter",
                                "Aria", "James",
                                "Layla", "Caleb",
                                "Avery", "Benjamin",
                                "Abigail", "Jack",
                                "Harper", "Luke",
                                "Kaylee", "Grayson",
                                "Aaliyah", "William",
                                "Evelyn", "Ryan",
                                "Adalyn", "Connor",
                                "Ella", "Daniel",
                                "Arianna", "Gabriel",
                                "Hailey", "Owen",
                                "Ellie", "Henry",
                                "Nora", "Matthew",
                                "Hannah", "Isaac",
                                "Addison", "Wyatt",
                                "Mackenzie", "Jayce",
                                "Brooklyn", "Cameron",
                                "Scarlett", "Landon",
                                "Anna", "Nicholas",
                                "Mila", "Dylan",
                                "Audrey", "Nathan",
                                "Isabelle", "Muhammad",
                                "Elizabeth", "Sebastian",
                                "Leah", "Eli",
                                "Sarah", "David",
                                "Lillian", "Brayden",
                                "Grace", "Andrew",
                                "Natalie", "Joshua",
                                "Kylie", "Samuel",
                                "Lucy", "Hunter",
                                "Makayla, Anthony",
                                "Maya", "Julian",
                                "Kaitlyn", "Dominic"
                                };
        names = Utility.ShuffleArray(names, 20); //셔플
        randomName = new Queue<string>(names);
    }
    //AI 랜덤 이름 가져오기
    public static string Get_RandomName()
    {
        string name = randomName.Dequeue();
        randomName.Enqueue(name);
        return name;
    }
    //게임 참가자 설정 초기화
    public static void init()
    {
        randomName.Clear();
        playList.Clear();
    }
    //Player 게임 참가
    public static void ParticipatePlayer(Profile _profile)
    {
        playList.Add(_profile);
    }

}
