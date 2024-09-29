## what is this 🤔

this is a ruleset for [osu](https://github.com/ppy/osu). it generates random but playable pump it up charts, so no spins or impossible twists. only supports single notes at a time (no holds or simultaneous notes).
this is for me to test the theory that [playing random mod](https://iidx.org/intermediate/tips#justification-for-always-using-random) helps you improve at rhythm games faster. also i get to practice to my favorite songs in osu! :)

## helpful tips if u want to try this out

- currently you can't actually play with a keyboard. i just shadow play with my feet because i don't really believe in hand PIU. (and i'm lazy to implement it)
- f3 and f4 in-game to change scroll speed. this is a keybind in osu! itself, not a ruleset keybind
- taiko maps create the best charts because chart conversions only care about rhythm. but currently only osu! (standard) maps are able to be converted from song select. but you can build osu! from scratch but with [this to true](https://github.com/ppy/osu/blob/9fd6449fd8b7c8b7a9019d1d3a25cb46a5b5562c/osu.Game/Screens/Select/Carousel/CarouselBeatmap.cs#L34), and you'll see converted taiko maps. (thanks peppy for pointing this out)
- you might have to tweak the mods to get good singles charts (or help figure out a better formula 😢)

## example outputs

converted map: [https://osu.ppy.sh/beatmapsets/2087136#taiko/4373682](https://osu.ppy.sh/beatmapsets/2087136#taiko/4373682)

### single no twists 

https://github.com/user-attachments/assets/c9c61dd3-7718-482a-bb88-f08d606cdd41

### single with a good amount of twists

https://github.com/user-attachments/assets/33d785c5-49f4-4038-a564-96976f352459

### half-double

https://github.com/user-attachments/assets/cc90dbfa-ffef-413a-9ba6-c1bcce81fe00
