# Scripts
제가 개발한 코드들을 모아 정리한 Repository입니다.
프로젝트 및 개발 최신 순서대로 정리하였습니다.
* * *
 <h2>로그라이크 Ability(개발중)</h2>
현재 팀 프로젝트로 개발중입니다.
능력치를 선택해서 Player에 전투력이 올라가는 시스템입니다.
80%완성된 코드이고 대부분의 구조와 기능이 완성된 코드입니다. 
* * *
<h3>1. Ability Data 구조</h3>

[데이터 코드](https://github.com/KimSangWoo1/Scripts/tree/main/Ability/Model)

![Ability 데이터 구조](https://github.com/KimSangWoo1/Scripts/assets/59047886/9a4ae361-2b50-4f83-8549-ebd069644c9b)
데이터 클래스

<h3>2. Ability 시스템</h3>
Player 능력을 강화 시켜주는 시스템입니다.
아직 전투 시스템이 개발되지 않았기 때문에 제대로 된 영상을 첨부하지는 못하지만 
보통적인 흐름은
1. Ability 카드 선택 -> Event -> 능력 강화
특정한 상황에 Player를 Buff 해주는 Ability일 경우
2. Ability 카드 선택 -> Event -> 버프Dictionry 저장 -> 해당 버프 조건 Event -> 버프 발동 및 해제

자세한 내용의 코드는 아래에서 확인하실 수 있습니다.

[Ability 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/Ability/System)

[Event 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/Ability/Event)

* * *
 <h2> Gate Guardian Project (개발중)</h2>
 3D기반 로그라이크 + 캐쥬얼 + 슈팅게임을 개발중에 있습니다.
 개발 초기 단계라 부수적인 시스템을 먼저 개발하고 있었습니다. 
 그중 소개할 것은 3가지
<h3>1. Enemy Spawner Point Tool </h3>

[NavWayPoint 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/Gate%20Guardian/Deploy%20Point)

[블로그 정리 글 (영상 포함) ](https://blog.naver.com/tkdqjadn/223149820991)

<h3>2. NavWayPoint Tool </h3>

[NavWayPoint 코드 보기](https://blog.naver.com/tkdqjadn/223149834412)

[블로그 정리 글 (영상 포함) ](https://blog.naver.com/tkdqjadn/223149834412)

<h3>3. Decal Shader </h3>

[블로그 정리 글 (영상 포함) ](https://blog.naver.com/tkdqjadn/223170324873)
[ToonShader Code 및 블로그 정리 글](https://blog.naver.com/tkdqjadn/222961976090)

* * *
<h2> xxxxxProject </h2>
