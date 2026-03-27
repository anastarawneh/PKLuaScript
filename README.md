# Pokémon Platinum Kaizo Lua Script

This is the official Lua script, to be used with the [Pokémon Platinum Kaizo Damage Calculator](https://pkcalc.anastarawneh.com).

**It's Windows exclusive, sorry Mac users!**

## Features:
- Automatically import your Pokémon into the calculator (Use the `Sync` button once both scripts are running!)
- Pre-status, pre-damage, and experience edging
- Export your Pokémon in a Showdown format
- Check the ability slots of your Pokémon, for those which have abilities added on evolution

# How to use:
1. Extract the bundled `lua-5.1.5_Win64_bin.zip` file in the same directory as your DeSmuMe executable.
2. Once your ROM is loaded, click `Tools` -> `Lua Scripting` -> `New Lua Script Window...`.
3. `Browse...` for the included `PK_Script.lua` file.
4. Run `PK_Script.exe`.

# For more help:
https://www.youtube.com/watch?v=ErHm6BNSNd8

# Available commands:
```
  sleep(slot)          -- Pre-sleeps the chosen party slot.
  poison(slot)         -- Pre-poisons the chosen party slot.
  burn(slot)           -- Pre-burns the chosen party slot.
  paralyze(slot)       -- Pre-paralyzes the chosen party slot.
  freeze(slot)         -- Pre-freezes the chosen party slot.
  bedtime()            -- Pre-sleeps the entire party.
  export()             -- Exports a Showdown paste of all Pokémon in your party and boxes to the clipboard.
  showabilities()      -- Displays the ability slots of all Pokémon in your party and boxes.
```