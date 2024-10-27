## What is this?

This is a ruleset for [osu!](https://github.com/ppy/osu). It generates Pump It Up charts that are playable with two feet, from osu! beatmaps.

The current the formula generates charts with:
- No spins
- No splits (nothing wider than notes being more than 1 panel horizontally apart)
- osu! slider rhythm simplification: 1/6 and 1/8 buzz sliders are simplified to 1/4's, and long sliders are simplified (e.g. 3/4 sliders are treated as 1/2)
- By default with no mods:
  - No 90 degree twists in singles (e.g. DL --> DR --> UR) for 1/4 rhythms and faster
  - No fast travels in doubles (e.g. P1C --> P1UR --> P2DL) for 1/4 rhythms and faster
- Adjustable through mods:
  - Regular twists (e.g. UL, C, UR)
  - Large twists (e.g. P2UL, P1C)
  - Diagonal twists (e.g. DL, C, UR)
  - Diagonal skips (e.g. UR, DL)

All examples above start with the left foot. See [this page](https://www.piucenter.com/skill) for more terminology.
(The terms I'm using here and in the code don't match exactly to the terms that page.)

## How to install

https://rulesets.info/install/rulesets

The ruleset file is provided here: https://github.com/hwabis/pump-trainer/releases

## Tips

- Keybinds:
```
Q E R Y
 S   G
Z C V N
```
- However, I believe playing with your feet with Autoplay mod is more fun.
- f3 and f4 in-game to change scroll speed. This is a keybind in osu! itself, not a ruleset keybind.

## Example converted beatmaps

Converted map: https://osu.ppy.sh/beatmapsets/298245#osu/679019

### Single - no twists (would be ~S15)

https://github.com/user-attachments/assets/91fb4707-e803-4fdc-be9d-0374acd1bb64

### Full-double - no twists (would be ~D16)

https://github.com/user-attachments/assets/8a4add67-482d-43ce-8a82-964e3aa20b54

### Full-double - with twists (would be ~D17)

https://github.com/user-attachments/assets/5226fec9-6ad1-4382-99e5-9381a1fab7b0
