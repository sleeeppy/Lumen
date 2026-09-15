<p align="right"><a href="./README.md">한국어</a> · <b>English</b></p>

<p align="center">
  <img src="docs/images/splash.png" alt="Lumen" width="560">
</p>

<p align="center">
  A <b>2D bullet-hell action</b> game on a moonlit rooftop.<br>
  Cut through patterns with dash and flight, then rewrite the fight with jewelry.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Unity-2021.3_LTS-222222?style=flat-square&logo=unity">
  <img src="https://img.shields.io/badge/2D-Bullet_Hell-9b6bff?style=flat-square">
  <img src="https://img.shields.io/badge/URP-purple?style=flat-square">
</p>

<p align="center">
  <a href="https://github.com/sleeeppy/Lumen/releases/latest/download/Lumen-macOS.zip"><b>⬇ Download</b></a>
  &nbsp;·&nbsp;
  <a href="https://youtu.be/kgnrUYgdiXc"><b>▶ Trailer</b></a>
</p>

<p align="center">
  <a href="https://youtu.be/kgnrUYgdiXc">
    <img src="docs/images/demo-play.jpg" alt="Watch the trailer" width="860">
  </a>
</p>

---

## Screenshots

<table>
  <tr>
    <td width="50%"><img src="docs/images/screen-lobby.jpg" alt="Lobby"></td>
    <td width="50%"><img src="docs/images/screen-combat.jpg" alt="Boss fight"></td>
  </tr>
  <tr>
    <td align="center"><sub>Lobby — meet NPCs on a night rooftop</sub></td>
    <td align="center"><sub>Boss fight — patterns change every phase</sub></td>
  </tr>
  <tr>
    <td><img src="docs/images/screen-inventory.jpg" alt="Inventory"></td>
    <td><img src="docs/images/screen-laser.jpg" alt="Laser"></td>
  </tr>
  <tr>
    <td align="center"><sub>Loadout — rings, bracelets, and nails</sub></td>
    <td align="center"><sub>Attack — shots or a laser, depending on the bracelet</sub></td>
  </tr>
</table>

<p align="center">
  <img src="docs/images/screen-pattern.jpg" alt="Bullet pattern" width="860">
</p>
<p align="center"><sub>Cut through the barrage and push the boss back</sub></p>

---

## About

**Lumen** is a boss-rush set on a night-city rooftop.
Talk in the lobby, pick a loadout, then step into phased boss fights.

Move, jump, dash, and fly in one motion.

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

Dash and flight spend the **energy gauge**. Standing on the ground fills it back up.

---

## Combat

- **Dash** — a short invincible burst on the ground.
- **Flight** — chain off a dash to move freely in the air. Invincible, but the gauge drains fast.
- **Skills** — damaging the boss fills a gauge. Press `Q` to use a nail.
- **Phases** — each time HP hits zero, orbitals, homing shots, and falling bullets mix together.

---

## Loadout

Finish talking to the shop NPC to open the jewelry box.
You can wear **2 rings, 1 bracelet, and 1 nail** at once.

| | Name | Slot | Effect |
| :---: | --- | --- | --- |
| <img src="docs/images/ring-heart.png" width="52" alt="Heart Locket"> | Heart Locket | Ring | +1 max life |
| <img src="docs/images/ring-star.png" width="52" alt="Star Ring"> | Star Ring | Ring | Slightly more max energy |
| <img src="docs/images/bracelet-eris.png" width="68" alt="Eris Bracelet"> | Eris Bracelet | Bracelet | Basic projectile attack |
| <img src="docs/images/bracelet-luna.png" width="68" alt="Luna Bracelet"> | Luna Bracelet | Bracelet | Laser attack |
| <img src="docs/images/nail-echo.png" width="80" alt="Star Echo"> | Star Echo | Nail | Clears nearby bullets |
| <img src="docs/images/nail-spark.png" width="52" alt="Rainbow Spark"> | Rainbow Spark | Nail | Invincibility + fire rate *(in development)* |

---

## Characters

<p align="center">
  <img src="docs/images/screen-dialogue.jpg" alt="Dialogue" width="720">
</p>
<p align="center"><sub>Press `F` to start a typed conversation</sub></p>

<p align="center">
  <img src="docs/images/npc-mushboy.png" height="140" alt="Hat NPC">
  &nbsp;&nbsp;&nbsp;
  <img src="docs/images/npc-halfseal.png" height="140" alt="HalfSeal">
  &nbsp;&nbsp;&nbsp;
  <img src="docs/images/npc-blue.png" height="140" alt="Blue-haired NPC">
</p>

Depending on the NPC, you open the shop, the first boss, or the second boss.

---

## How to run

To play without the project, [download the game](https://github.com/sleeeppy/Lumen/releases/latest/download/Lumen-macOS.zip) and open `Lumen.app`. macOS only.

**Title → lobby → boss.** Dying sends you back to the lobby.

To open the project, use **Unity 2021.3.45f2** (URP) and press Play on `Assets/Scenes/Main.unity`.

| Scene | Role |
| --- | --- |
| `Main` | Title |
| `Lobby` | Hub · NPCs · loadout |
| `Game` | Boss fight |
| `Boss2` | Second boss fight |

---

<p align="center">
  <a href="https://github.com/sleeeppy/Lumen/releases/latest/download/Lumen-macOS.zip"><b>⬇ Download</b></a>
  &nbsp;·&nbsp;
  <a href="https://youtu.be/kgnrUYgdiXc"><b>▶ Trailer</b></a>
</p>

<p align="center">
  Made by <b>ChoCollect</b>
</p>
