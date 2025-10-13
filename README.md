# Undermarch

Tile-based tick-driven reverse-dungeon game. Headless simulation with Unity presentation.

## Team:
- Joosep Podekrat (master)
- Osvald Nigola (labourer)
- Leo-Martin Pala (labourer)
- Anna Liisa Nurm (labourer)
- Katarina Podekrat (artist)

## Requirements
- Unity 6.2 (6000.2.7f2)
- Git + Git LFS

## Eventual quick start (how should be when ready)
0) Clone repo
1) Install Git LFS (once per machine):
   ```sh
   git lfs install
   ```
2) Open the project with Unity 6000.2.7f2.
3) Open `Scenes/Bootstrap/Bootstrap.unity` and press Play.

## Smart Merge (UnityYAMLMerge)
Unity's YAML merge tool resolves many scene/prefab conflicts so it's good to use, but it's not in $PATH by default.

**Adjust paths as necessary.**

- macOS:
  ```sh
  git config --global merge.unityyamlmerge.name "Unity Smart Merge"
  git config --global merge.unityyamlmerge.driver '"/Applications/Unity/Hub/Editor/6000.2.7f2/Unity.app/Contents/Tools/UnityYAMLMerge" merge -p %O %A %B %L'
  ```
- Windows (something like this in powershell, adjust path if installed somewhere else):
  ```powershell
  git config --global merge.unityyamlmerge.name "Unity Smart Merge"
  git config --global merge.unityyamlmerge.driver '"C:\Program Files\Unity\Hub\Editor\6000.2.7f2\Editor\Data\Tools\UnityYAMLMerge.exe" merge -p %O %A %B %L'
  ```
- Linux:
  Idk depends on how and where you installed it.

`.gitattributes` marks Unity YAML assets to use smart merge (`.unity`, `.prefab`, etc).

## Project Structure (subject to change)
```
Assets/
  Scenes/
    Bootstrap/    (loads other scenes additively)
    Simulation/   (headless managers only)
    Rendering/    (Tilemaps, visuals, pools)
    UI/           (HUD, debug overlays)
  Scripts/
    Simulation/   (just C# scripts, deterministic, no UnityEngine)
    Presentation/ (MonoBehaviours, reads simulation and renders)
    Editor/       (editor utilities)
    Tests/
      EditMode/
      PlayMode/
  ScriptableObjects/
    Balancing/
    Catalogs/
    Effects/
    DamageTypes/
  Prefabs/
  Art/
  Audio/
```

## Technical
- Bootstrap scene loads Simulation, Rendering, UI
- Simulation: pure C# in `Undermarch.Simulation.*`, deterministic, tick-based, no UnityEngine
- Presentation: `Undermarch.Presentation.*` bridges sim -> visuals (tilemaps, sprites, UI)
- Time: all gameplay uses ticks (rational speeds)
- Data: ScriptableObjects hold tunables (damage, resistances, durations). Catalogs list buildable/spawnable content.

## Grid layers
- Ground (1 per tile)
- Wall (0/1 per tile; blocks movement/placement)
- Interactable (0/1 per tile; traps/chests/ladders with faction filters)
- Entity (0/1 per tile)
- Effects (0..n per tile; slow zones, fog, etc.)
- ...midagi veel?

## Tick and Movement
Tick events order TBD

All speeds are rational (harilik murd, i.e. 2/3 not 0.666...). Maybe use the class Rational(2, 3) for it [https://introcs.cs.luc.edu/classes/rational.html](https://introcs.cs.luc.edu/classes/rational.html)

Speeds use integer budgets each tick, e.g. `CARDINAL_COST = 1000`, `DIAGONAL_COST = 1414`

Movement budget each tick is added `speed * CARDINAL_COST` (i.e. 1k * speed, like 3/4 speed = 750 per turn), up to **(TODO kas tundub mõistlik ceiling)** `ceil(speed) * CARDINAL_COST`, so 3/4 speed would have max budget of 1000, 3/2 (1.5) speed would have max budget of 2000.

## Damage and Interactions
- DamagePacket with types (like physical, fire, ...)
- Factions something like hero, defender/minion, neutral, projectile, ...?
- `InteractionMatrix` defines trap triggers, projectile collisions (entities/projectiles/walls...)

