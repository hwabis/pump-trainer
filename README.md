## What is this?

This is a ruleset for [osu!](https://github.com/ppy/osu). It generates Pump It Up charts that are playable with two feet, from osu! beatmaps.

The current the formula generates charts with:
- No spins
- No splits (consecutive notes have at most 1 panel horizontally between them)
- osu! slider rhythm simplification: 1/6 and 1/8 buzz sliders are simplified to 1/4's, and long sliders are simplified (e.g. 3/4 sliders are treated as 1/2)
- By default with no mods:
  - No 90 degree twists in singles (e.g. DL --> DR --> UR) for 1/4 rhythms and faster
  - No fast travels in doubles (e.g. P1C --> P1UR --> P2DL) for 1/4 rhythms and faster
- Adjustable through mods:
  - Regular twists (e.g. UL, C, UR)
  - Large twists (e.g. P2UL, P1C)
  - Diagonal twists (e.g. DL, C, UR)
  - Diagonal skips (e.g. UR, DL)

All examples above start with the left foot. See [this page](https://www.piucenter.com/skill) for more background.
(The terminology I'm using here and in the code don't match exactly to that page.)

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

Converted map: https://osu.ppy.sh/beatmapsets/2010589#osu/4183614

### Single

https://github.com/user-attachments/assets/443fd2df-9c3f-4c19-bf72-c9ba25ae3e57

### Half-double

https://github.com/user-attachments/assets/87712408-7cc8-42e1-b919-8e6acae75598

### Double with regular twists

https://github.com/user-attachments/assets/4037fae3-c224-4abb-92b9-d1bdeea1d639
