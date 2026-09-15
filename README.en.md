<p align="right"><a href="./README.md">한국어</a> · <b>English</b></p>

<p align="center">
  <img src="docs/images/splash.png" alt="Lumen" width="680">
</p>

<p align="center">
  A <b>2D bullet-hell action</b> game on a moonlit rooftop.<br>
  Cut through patterns with dash and flight, then rewrite the fight with jewelry.
</p>

<p align="center">
  <img src="docs/images/title.png" alt="Title screen" width="860">
</p>

---

## About

**Lumen** is a boss-rush set on a night-city rooftop.
Talk to NPCs in the lobby, pick your loadout, then step into phased boss fights.

Movement, jump, dash, and flight chain into one motion.
Rings, bracelets, and nails decide how you survive and how you fire.

<p align="center">
  <img src="docs/images/rooftop.png" alt="Rooftop stage" width="860">
</p>

<p align="center"><sub>Lobby / combat stage — a rooftop over the city lights</sub></p>

---

## Controls

| Input | Action |
| --- | --- |
| `A` `D` / ← → | Move |
| `Space` | Jump · hold to stay airborne |
| `Left Shift` / Right-click | Dash · hold to fly |
| `W` + dash | Enter flight |
| Left-click | Attack (shots or laser, depending on bracelet) |
| `Q` | Equipped nail skill |
| `F` | Talk to an NPC |
| `Esc` | Settings · close dialogue |

Dash and flight spend the **energy gauge**. Standing on the ground restores it.

---

## Combat

- **Dash** — a short invincible burst on the ground. Hold to travel farther.
- **Flight** — chain off a dash to move freely in the air. You are invincible, but the gauge drains fast.
- **Skill gauge** — fills as you damage the boss. Press `Q` to use a nail skill.
- **Phases** — each time the boss's HP hits zero, the pattern changes. Orbitals, homing shots, and falling bullets mix together.

<p align="center">
  <img src="docs/images/skill-echo.png" width="56" alt="Star Echo">
  &nbsp;&nbsp;
  <img src="docs/images/skill-spark.png" width="56" alt="Rainbow Spark">
</p>

<p align="center"><sub>Nail skills — Star Echo · Rainbow Spark</sub></p>

---

## Loadout

Finish talking to the shop NPC in the lobby to open the jewelry box.
You can equip **2 rings, 1 bracelet, and 1 nail** at once.

<p align="center">
  <img src="docs/images/inventory.png" alt="Inventory" width="280">
</p>

| | Name | Slot | Effect |
| :---: | --- | --- | --- |
| <img src="docs/images/ring-heart.png" width="56" alt="Heart Locket"> | Heart Locket | Ring | +1 max life |
| <img src="docs/images/ring-star.png" width="56" alt="Star Ring"> | Star Ring | Ring | Slightly more max energy |
| <img src="docs/images/bracelet-eris.png" width="72" alt="Eris Bracelet"> | Eris Bracelet | Bracelet | Basic projectile attack |
| <img src="docs/images/bracelet-luna.png" width="72" alt="Luna Bracelet"> | Luna Bracelet | Bracelet | Laser attack |
| <img src="docs/images/nail-echo.png" width="88" alt="Star Echo"> | Star Echo | Nail | Clears nearby bullets |
| <img src="docs/images/nail-spark.png" width="56" alt="Rainbow Spark"> | Rainbow Spark | Nail | Invincibility + fire rate *(in development)* |

Equipped items are saved as JSON and reapplied when a boss scene loads.

---

## Characters

<p align="center">
  <img src="docs/images/player.png" height="150" alt="Player">
  &nbsp;&nbsp;&nbsp;
  <img src="docs/images/boss.png" height="150" alt="Boss">
  &nbsp;&nbsp;&nbsp;
  <img src="docs/images/boss-portrait.png" height="150" alt="Boss portrait">
</p>

<p align="center"><sub>Player · Boss · Boss portrait</sub></p>

<p align="center">
  <img src="docs/images/npc-mushboy.png" height="150" alt="Hat NPC">
  &nbsp;&nbsp;&nbsp;
  <img src="docs/images/npc-halfseal.png" height="150" alt="Silver-haired NPC">
  &nbsp;&nbsp;&nbsp;
  <img src="docs/images/npc-blue.png" height="150" alt="Blue-haired NPC">
  &nbsp;&nbsp;&nbsp;
  <img src="docs/images/npc-white.png" height="150" alt="White-haired NPC">
</p>

<p align="center"><sub>Lobby NPCs — dialogue, shop, and boss entrances</sub></p>

Press `F` to start a typed conversation. Depending on the NPC, you open the shop, the first boss, or the second boss.

---

## Scenes

| Scene | Role |
| --- | --- |
| `Main` | Title — start / quit |
| `Lobby` | Hub — NPCs, loadout, boss select |
| `Game` | Boss fight |
| `Boss2` | Second boss fight |

The loop is **title → lobby → boss**. Dying sends you back to the lobby.

---

## How to run

1. Open this repo in **Unity 2021.3.45f2** (URP).
2. Press Play on `Assets/Scenes/Main.unity`.
3. Start the game to enter the lobby.

The game is built around keyboard + mouse.

---

## Stack

| | |
| --- | --- |
| Engine | Unity 2021.3 LTS · URP |
| Motion / juice | Cinemachine · DOTween · Feel |
| Bullets | BulletPro |
| Data | Odin Inspector · Newtonsoft JSON · TextMesh Pro |

Made by **ChoCollect**
