<p align="right"><b>한국어</b> · <a href="./README.en.md">English</a></p>

<p align="center">
  <img src="docs/images/splash.png" alt="Lumen" width="680">
</p>

<p align="center">
  달빛 아래 옥상에서 펼쳐지는 <b>2D 탄막 액션</b><br>
  대시와 비행으로 탄막을 가르고, 장신구로 전투를 바꿉니다.
</p>

<p align="center">
  <img src="docs/images/title.png" alt="타이틀 화면" width="860">
</p>

---

## 소개

**Lumen**은 밤의 도시 옥상을 배경으로 한 보스 러시 게임입니다.
로비에서 NPC와 대화하고 장비를 고른 뒤, 페이즈가 갈리는 보스전으로 들어갑니다.

이동·점프·대시·비행이 한 흐름으로 이어지고, 반지·팔찌·네일로 생존과 화력을 조절합니다.

<p align="center">
  <img src="docs/images/rooftop.png" alt="옥상 스테이지" width="860">
</p>

<p align="center"><sub>로비 / 전투 스테이지 — 야경이 깔린 옥상</sub></p>

---

## 조작

| 입력 | 동작 |
| --- | --- |
| `A` `D` / ← → | 이동 |
| `Space` | 점프 · 길게 누르면 체공 |
| `Left Shift` / 우클릭 | 대시 · 유지하면 비행 |
| `W` + 대시 | 비행 전환 |
| 좌클릭 | 공격 (팔찌에 따라 탄환 / 레이저) |
| `Q` | 장착한 네일 스킬 |
| `F` | NPC 대화 |
| `Esc` | 설정 · 대화 닫기 |

대시와 비행은 **에너지 게이지**를 소모합니다. 땅에 서 있으면 게이지가 회복됩니다.

---

## 전투

- **대시** — 지상에서 짧게 무적이 되며 탄막을 빠져나갑니다. 길게 누르면 거리가 늘어납니다.
- **비행** — 대시를 이어서 공중을 누빕니다. 비행 중에는 무적이지만 게이지가 빠르게 줄어듭니다.
- **스킬 게이지** — 보스에게 피해를 주면 찹니다. `Q`로 네일 스킬을 씁니다.
- **페이즈** — 보스는 체력이 바닥날 때마다 패턴이 바뀝니다. 궤도탄, 유도탄, 낙하탄이 섞입니다.

<p align="center">
  <img src="docs/images/skill-echo.png" width="56" alt="별의 메아리">
  &nbsp;&nbsp;
  <img src="docs/images/skill-spark.png" width="56" alt="레인보우 스파크">
</p>

<p align="center"><sub>네일 스킬 — 별의 메아리 · 레인보우 스파크</sub></p>

---

## 장비

로비 상점 NPC와 대화를 마치면 보석함 인벤토리가 열립니다.
**반지 2개 · 팔찌 1개 · 네일 1개**까지 동시에 장착할 수 있습니다.

<p align="center">
  <img src="docs/images/inventory.png" alt="인벤토리" width="280">
</p>

| | 이름 | 분류 | 효과 |
| :---: | --- | --- | --- |
| <img src="docs/images/ring-heart.png" width="56" alt="하트 로켓"> | 하트 로켓 | 반지 | 최대 체력 +1 |
| <img src="docs/images/ring-star.png" width="56" alt="별의 반지"> | 별의 반지 | 반지 | 최대 에너지 증가 |
| <img src="docs/images/bracelet-eris.png" width="72" alt="에리스의 팔찌"> | 에리스의 팔찌 | 팔찌 | 기본 탄환 공격 |
| <img src="docs/images/bracelet-luna.png" width="72" alt="루나의 팔찌"> | 루나의 팔찌 | 팔찌 | 레이저 공격 |
| <img src="docs/images/nail-echo.png" width="88" alt="별의 메아리"> | 별의 메아리 | 네일 | 주변 탄막 제거 |
| <img src="docs/images/nail-spark.png" width="56" alt="레인보우 스파크"> | 레인보우 스파크 | 네일 | 무적 + 공격속도 *(개발 중)* |

장착 내용은 JSON으로 저장되어, 보스 씬에 들어가면 다시 적용됩니다.

---

## 등장인물

<p align="center">
  <img src="docs/images/player.png" height="150" alt="플레이어">
  &nbsp;&nbsp;&nbsp;
  <img src="docs/images/boss.png" height="150" alt="보스">
  &nbsp;&nbsp;&nbsp;
  <img src="docs/images/boss-portrait.png" height="150" alt="보스 초상">
</p>

<p align="center"><sub>플레이어 · 보스 · 보스 초상</sub></p>

<p align="center">
  <img src="docs/images/npc-mushboy.png" height="150" alt="모자 NPC">
  &nbsp;&nbsp;&nbsp;
  <img src="docs/images/npc-halfseal.png" height="150" alt="은발 NPC">
  &nbsp;&nbsp;&nbsp;
  <img src="docs/images/npc-blue.png" height="150" alt="청발 NPC">
  &nbsp;&nbsp;&nbsp;
  <img src="docs/images/npc-white.png" height="150" alt="백발 NPC">
</p>

<p align="center"><sub>로비 NPC — 대화 · 상점 · 보스전 입구</sub></p>

`F`로 말을 걸면 타이핑 대화가 진행됩니다. NPC에 따라 상점, 1번째 보스, 2번째 보스로 나뉩니다.

---

## 씬

| 씬 | 역할 |
| --- | --- |
| `Main` | 타이틀 — 시작 / 종료 |
| `Lobby` | 허브 — NPC, 장비, 보스 선택 |
| `Game` | 보스전 |
| `Boss2` | 두 번째 보스전 |

실행 흐름은 **타이틀 → 로비 → 보스전** 입니다. 처치당하면 로비로 돌아갑니다.

---

## 실행 방법

1. **Unity 2021.3.45f2** (URP)로 이 저장소를 엽니다.
2. `Assets/Scenes/Main.unity`에서 Play 합니다.
3. 시작을 누르면 로비로 이동합니다.

키보드와 마우스를 함께 쓰는 조작입니다.

---

## 기술 스택

| | |
| --- | --- |
| 엔진 | Unity 2021.3 LTS · URP |
| 이동 / 연출 | Cinemachine · DOTween · Feel |
| 탄막 | BulletPro |
| 데이터 | Odin Inspector · Newtonsoft JSON · TextMesh Pro |

제작 **ChoCollect**
