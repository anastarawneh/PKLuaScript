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
  sethp(slot,hp)       -- Sets the HP of the chosen party slot.
  export()             -- Exports a Showdown paste of all Pokémon in your party and boxes to the clipboard.
  showabilities()      -- Displays the ability slots of all Pokémon in your party and boxes.
  edge(slot)           -- Edges the chosen party slot by 1 HP.
  edge(slot, amount)   -- Edges the chosen party slot by the specified HP.
  edgeparty()          -- Edges the entire party by 1 HP.
  heal(slot)           -- Restores the chosen party slot's HP, PP and status.
  nursejoy()           -- Restores the entire party's HP, PP and status.
```