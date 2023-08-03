using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIType", menuName = "Scriptable Object/AIType", order = 2)]
public class AIType : ScriptableObject
{
    public enum AblityMode { Attack, Balance, Defence} //공격형 , 밸런스형, 수비형
    public AblityMode mode;
    public struct Ability
    {
        /**
         * 1. avoidWaitTime       기본 2f  : 작을수록 공격형 - 밸런스형 - 방어형 클수록  최소 : 1f ~ 최대 : 4f
         * 2. sensingSensitivity  기본 7f  : 작을수록 공격형 - 밸런스형 - 방어형 클수록  최소 : 5f ~ 최대 : 7f
         * 3. foundBusterAmount   기본 0.7f : 작을수록 공격형 - 밸런스형 - 방어형 클수록 최소 : 0.6f ~ 최대 0.8f
         * 4. attackBusterAmount  기본 0.8f : 작을수록 공격형 - 밸런스형 - 방어형 클수록 최소 : 0.4f ~ 최대 0.9f
         *
         * 5.nomarlBusterAmount   기본 0.6f : 작을수록 방어형 - 밸런스형 - 공격형 클수록 최소 : 0.4f ~ 최대 0.8f
         * 6. avoidBusterAmount   기본 0.3f : 작을수록 방어형 - 밸런스형 - 공격형 클수록 최소 : 0.1f ~ 최대 0.5f
         */
        public float avoidWaitTime; //도망가는 시간 설정 
        public float sensingSensitivity; //근접 거리 감지 감도  
        public float foundBusterAmount; //Found상태 부스터 사용 가능 양 
        public float attackBusterAmount; //Attack상태 부스터 사용 가능 양

        public float nomarlBusterAmount; //일반상태 부스터 사용 가능 양 
        public float avoidBusterAmount; //Avoid상태 부스터 사용 가능 양 

        //능력 생성자
        public Ability(float _avoidWaitTime, float _sensingSensitivity, float _foundBusterAmount, float _attackBusterAmount, float _nomarlBusterAmount, float _avoidBusterAmount)
        {
            avoidWaitTime = _avoidWaitTime;
            sensingSensitivity = _sensingSensitivity;
            nomarlBusterAmount = _nomarlBusterAmount;
            foundBusterAmount = _foundBusterAmount;
            attackBusterAmount = _attackBusterAmount;
            avoidBusterAmount = _avoidBusterAmount;
        }
    }
    //AI Mode 설정
    public Ability SetAIMode()
    {
        RandomAIType();
        Ability ability;
        switch (mode)
        {
            case AblityMode.Attack:
                ability = new Ability(2f, 5f, 0.6f, 0.4f, 0.8f, 0.5f);
                break;
            case AblityMode.Balance:
                ability = new Ability(3f, 6f, 0.7f, 0.8f, 0.6f, 0.3f);
                break;
            case AblityMode.Defence:
                ability = new Ability(4f, 7f, 0.8f, 0.9f, 0.4f, 0.1f );
                break;
            default:
                ability = new Ability(2f, 5f, 0.6f, 0.4f, 0.8f, 0.5f);
                break;
        }
        return ability;
    }

    //랜덤 AI Tyoe를 정하도록 한다.
    public AIType.AblityMode RandomAIType()
    {
        int type = Random.Range(0, 3);
        if (type == 0)
        {
            mode = AIType.AblityMode.Attack;
        }
        else if (type == 1)
        {
            mode = AIType.AblityMode.Balance;
        }
        else
        {
            mode = AIType.AblityMode.Defence;
        }
        return mode;
    }
}