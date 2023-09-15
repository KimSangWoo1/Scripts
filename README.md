# Scripts
제가 개발한 코드들을 모아 정리한 Repository입니다.  <br/>
프로젝트 및 개발 최신 순서대로 정리하였습니다.  <br/>
자세한 설명보다는 개발의 전체코드 및 중요코드를 보는 목적인 Repository입니다.  <br/>

* * *
<h3>1. 로그라이크 2D 절차적 맵 생성</h3>
Unity TileMap + CelluarAutomata + Drunked walk + Flood Fill 알고리즘 사용
절차적 타일 구성 + 절차적 맵 구성(각 방의 특성 설정)

![로그라이크 2D 절차적 맵 구성](https://github.com/KimSangWoo1/Scripts/assets/59047886/51473c7f-a8f3-4abb-bdbb-9f454f8c6ac0)

[Map 코드 보기](https://github.com/KimSangWoo1/Scripts/Map)

[블로그 정리 글 (영상 포함) ](https://blog.naver.com/tkdqjadn/223212297657)

* * *
 <h2>로그라이크 Ability(개발중)</h2>
현재 팀 프로젝트로 개발중입니다.  <br/>
능력치를 선택해서 Player에 전투력이 올라가는 시스템입니다.  <br/>
80%완성된 코드이고 대부분의 구조와 기능이 완성된 코드입니다.   <br/>

* * *
<h3>1. Ability Data 구조</h3>

![Ability 데이터 구조](https://github.com/KimSangWoo1/Scripts/assets/59047886/9a4ae361-2b50-4f83-8549-ebd069644c9b)
<br/>
데이터 클래스 구조를 어떻게 짰는지 보여드리는 코드입니다.<br/>
[데이터 코드](https://github.com/KimSangWoo1/Scripts/tree/main/Ability/Model)

<h3>2. Ability 시스템</h3>
Player 능력을 강화 시켜주는 시스템입니다.  <br/>
아직 전투 시스템이 개발되지 않았기 때문에 제대로 된 영상을 첨부하지는 못하지만   <br/>
보통적인 흐름은  <br/>
1. Ability 카드 선택 -> Event -> 능력 강화  <br/>
특정한 상황에 Player를 Buff 해주는 Ability일 경우  <br/>
2. Ability 카드 선택 -> Event -> 버프Dictionry 저장 -> 해당 버프 조건 Event -> 버프 발동 및 해제  <br/>

자세한 내용의 코드는 아래에서 확인하실 수 있습니다.  <br/>

[Ability 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/Ability/System)

[Event 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/Ability/Event)

[![Video Label](http://img.youtube.com/vi/XwQMGnm2wgs/0.jpg)](https://youtu.be/XwQMGnm2wgs)

* * *
 <h2> Gate Guardian Project (개발중)</h2>
 3D기반 로그라이크 + 캐쥬얼 + 슈팅게임을 개발중에 있습니다.  <br/>
 개발 초기 단계라 부수적인 시스템을 먼저 개발하고 있었습니다.   <br/>
 그중 소개할 것은 3가지  <br/>
<h3>1. Deploy Point Tool (Enemy Spawner)</h3>

[Deploy Point 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/Gate%20Guardian/Deploy%20Point)

[블로그 정리 글 (영상 포함) ](https://blog.naver.com/tkdqjadn/223149820991)

[![Video Label](http://img.youtube.com/vi/jmYYkW8LOPk/0.jpg)](https://youtu.be/jmYYkW8LOPk)

<h3>2. NavWayPoint Tool </h3>

[NavWayPoint 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/Gate%20Guardian/NavWayPoint)

[블로그 정리 글 (영상 포함) ](https://blog.naver.com/tkdqjadn/223149834412)

[![Video Label](http://img.youtube.com/vi/uNlCN2wunyc/0.jpg)](https://www.youtube.com/uNlCN2wunyc)
<h3>3. Decal Shader </h3>

[블로그 정리 글 (영상 포함) ](https://blog.naver.com/tkdqjadn/223170324873)

[![Video Label](http://img.youtube.com/vi/pOORHSgzMJY/0.jpg)](https://www.youtube.com/pOORHSgzMJY)

그 외 [ToonShader Code 및 블로그 정리 글](https://blog.naver.com/tkdqjadn/222961976090)

* * *
<h2> 고라니 게임잼 JamJamJam </h2>
해당 프로젝트 코드는 깃허브에서 볼 수 있습니다.

[고라니 게임잼](https://github.com/KimSangWoo1/JamJamJam)


* * *
<h2> 슈퍼셀 하이퍼 케쥬얼 게임잼</h2>
제가 맡은 부분은 </br>
Player 조작, </br>
Item, </br>
Effect, </br>
Sound, </br>
Player와 관련된 UI </br>
배경 효과 및 설정 이었습니다. </br>

제가 개발한 코드들만 보실 수 있습니다. </br>
[Anger of Whale 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/AngerOfWhale) </br>
[게임 플레이하기 ](https://shinee0382.itch.io/butty-butty)


* * *
<h2> DragonStory </h2>
드래곤 스토리는 Unity Open Project1에서 ScriptableObject의 사용 방식을 보고</br>
따라 만들어 본 Project여서 ScriptableObject를 많이 사용했음을 미리 말씀드립니다.

[유니티 오픈프로젝트1 링크]([https://github.com/KimSangWoo1/JamJamJam](https://github.com/DapperDino/UOP1))

드래곤 스토리에 관한 영상들은 유튜부를 통해 모두 보실 수 있습니다.</br>
[![Video Label](http://img.youtube.com/vi/mm3fohTzxDE/0.jpg)](https://www.youtube.com/watch?v=mm3fohTzxDE&list=PL5YJPokUujK0LmcBRhScjPI-gGC4x7M7F&index=4)

<h3>1. 스토리 연출 , 컷씬 </h3>
DragonStory는 Story게임으로 메인 퀘스트 진행시 컷씬처럼 연출되는 느낌의 스토리 연출 기능을 만들어 보았다.</br>
기본 구성은 대본, 배우, 사운드, 카메라로 잡고 연출했다.</br>

![DragonStory StoryBoardManager Hieracrchy](https://github.com/KimSangWoo1/Scripts/assets/59047886/8f894ce1-f487-4a99-8bef-8802c7b6db36)

사용한 카메라 연출 기법 : Single Shot, Over the Shoulder Shot, Wide Shot, Dolly Shot, Insert Shot</br>

[DragonStory 연출 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/DragonStory/Story) </br>

0-1 악몽은 TimeLine을 사용하여 연출해보았고 나머지는 StoryBoardManager 등 기능으로 연출하였다.

<h3>2. Talk System </h3>

[DragonStory Talk System 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/DragonStory/Talk) </br>

[![Video Label](http://img.youtube.com/vi/5WbHuWxsRuo/0.jpg)](https://www.youtube.com/5WbHuWxsRuo)

<h3>3. Dialog System </h3>

[DragonStory Dialog System 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/DragonStory/Dialog) </br>

[![Video Label](http://img.youtube.com/vi/mnS3If7S_W0/0.jpg)](https://www.youtube.com/mnS3If7S_W0)

<h3>4. Quest System </h3>

[DragonStory Quest System 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/DragonStory/Quest) </br>

[![Video Label](http://img.youtube.com/vi/aHTmvMXNaJA/0.jpg)](https://www.youtube.com/aHTmvMXNaJA)

<h3>5. Inventory </h3>

[DragonStory Inventory 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/DragonStory/Inventory) </br>

<h3>6. TPS Cam </h3>
유니티에서 제공하는 시네머신 카메라를 사용하지 않고 구현한 TPS 카메라입니다.

[DragonStory TPS 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/DragonStory/TPS) </br>

[![Video Label](http://img.youtube.com/vi/QwdG4RfeQ0s/0.jpg)](https://www.youtube.com/QwdG4RfeQ0s)

* * *
<h2> SkyShooting </h2>
SkyShooting은 졸업 후 처음으로 Google Play Store에 출시한 게임입니다.
1인 개발로 브롤스타즈의 슛팅 , Snake.io 의 멀티 대전의 재미를 살려서 만드려고 했습니다.
Photon Pun2로 멀티 대전을 만드려 했지만 Pun2 동기화의 기술적 한계로 추후에 PVP로 변경하였음을 알려드립니다.
Goolge Play Store에는 현재 1인 모드만 가능한 빌드 파일입니다.

[Google Play Store Link](https://play.google.com/store/apps/details?id=com.ksw.SkyShooting)

<h3> PVP </h3>
닌텐도 스위치의 포켓몬 소드 실드에서 포켓몬이 대전을 참고하여 카메라 연출을 하였습니다.

[DragonStory TPS 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/SkyShootring/PVP) </br>

[![Video Label](http://img.youtube.com/vi/oa--9z9wPeU/0.jpg)](https://www.youtube.com/oa--9z9wPeU)

<h3> 매칭 </h3>
Photon Pun2를 이용한 매칭 시스템입니다.

[SkyShooting 매칭 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/SkyShootring/Match) </br>

<h3> 싱글 모드  </h3>
싱글 모드 게임 진행시 쓰는 모든 코드들입니다. 

[SkyShooting Single Mode 코드 보기](https://github.com/KimSangWoo1/Scripts/tree/main/SkyShootring/SingleMode) </br>

[![Video Label](http://img.youtube.com/vi/TU0buea7Gqo/0.jpg)](https://www.youtube.com/TU0buea7Gqo)

