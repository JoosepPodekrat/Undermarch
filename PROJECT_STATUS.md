# Undermarch - Project Status

## New Stuff

**Core Systems:**
- Interface layer for simulation/presentation separation
- Deterministic seeded RNG system
- Resource system (gold tracking, placement costs)
- Buff/debuff system with stat modifiers
- Projectile system (archer monsters)
- Tile effects (poison, slow, fire, fog)
- Chest system with hero looting AI
- Hero flee behavior when low HP + has gold
- Phase-based tick system (pause, resume, step)

**Unity UI Components:**
- ResourceDisplay.cs - auto-updating gold counter
- TickControlUI.cs - pause/resume/step controls

## Dungeon Layout

Single level with 4 areas, linear progression:
- Boss Room (top) - Dungeon Master, narrow chokepoint entrance
- Corridor
- Treasure Room - 2 chests (30 gold each)
- Main Corridor
- Entrance Room - 2 chests near entrance (30 gold each)
- Entrance (bottom) - hero spawn point

Total: 4 chests, 120 gold available

## Wave System

9 waves at 2 ticks/second, spawn every 30-60 seconds

Gameplay duration: 4-5 minutes

## Unity Integration

### GameManager Setup

```csharp
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Levels;

public class GameManager : MonoBehaviour
{
    private Board board;
    private GameState gameState;
    private TickSystem tickSystem;
    private WaveSpawner waveSpawner;

    public ResourceDisplay resourceDisplay;
    public TickControlUI tickControlUI;

    void Start()
    {
        board = new Board(20, 20);
        gameState = new GameState(startingGold: 200);

        List<TilePos> entrances;
        List<TilePos> chestPositions;
        LevelLoader.LoadDungeon(board, out entrances, out chestPositions);

        waveSpawner = LevelLoader.CreateWaveSchedule(entrances);
        tickSystem = new TickSystem(board, gameState, ticksPerSecond: 2, waveSpawner);

        if (resourceDisplay != null) resourceDisplay.Initialize(gameState);
        if (tickControlUI != null) tickControlUI.Initialize(tickSystem);
    }

    void Update()
    {
        if (tickSystem.Mode == TickMode.Auto)
        {
            tickSystem.Tick();
        }
    }

    public void OnStartCombatClicked()
    {
        gameState.Phase = GamePhase.Combat;
        tickSystem.Resume();
    }
}
```

### PlacementController Updates

Add cost checking:

```csharp
void PlaceEntity(string entityType, TilePos pos)
{
    int cost = gameState.PlacementCosts[entityType];

    if (!gameState.CanAfford(cost))
    {
        Debug.Log("Insufficient gold!");
        return;
    }

    // Place entity on board...

    gameState.SpendGold(cost);
}
```

### TilemapRenderer Updates

Add rendering for new entity types:

```csharp
void RenderTile(TilePos pos)
{
    // ... existing entity rendering ...

    object interactable = board.GetInteractableAt(pos);
    if (interactable is Projectile projectile && projectile.IsActive)
    {
        effectsTilemap.SetTile(ToVector3Int(pos), arrowTile);
    }
    else if (interactable is TileEffect effect)
    {
        effectsTilemap.SetTile(ToVector3Int(pos), GetEffectTile(effect.Type));
    }
    else if (interactable is Chest chest && !chest.Looted)
    {
        interactableTilemap.SetTile(ToVector3Int(pos), chestTile);
    }
}
```

## UI Setup Tasks

**Resource Display:**
1. Create TextMeshProUGUI in UI scene named "GoldText"
2. Attach ResourceDisplay.cs component
3. Assign goldText field in Inspector
4. Drag to GameManager.resourceDisplay field

**Tick Controls:**
1. Create 3 buttons: Pause, Resume, Step
2. Create 2 text displays: Tick Count, Mode
3. Create GameObject "TickControlUI", attach TickControlUI.cs
4. Assign all button/text references in Inspector
5. Drag to GameManager.tickControlUI field

## New Tiles Needed

Add to TilemapRenderer:
- arrowTile (projectile sprite)
- chestTile
- poisonCloudTile
- slowZoneTile

## Hero AI Behavior

Priority system (highest to lowest):
1. Combat - if enemy within range 5, fight
2. Flee - if (gold AND healthPercent < 20), path to nearest board edge
3. Loot - if chest available, path to and loot chest
4. Attack DM - default behavior

Final wave (wave 9) never flees.

## Balancing Parameters

Located in LevelLoader.cs and CharacterDatabase:
- Starting gold (currently 200)
- Wave spawn intervals (currently 30-60 sec)
- Hero types per wave
- Chest gold amounts (currently 30)
- Placement costs per entity type
- Character stats

## Gameplay Flow

1. Player places defenders with 200 gold budget
2. Click "Start Combat" button
3. Wave 1 spawns immediately at entrance
4. Heroes path to DM, may loot chests along the way
5. New wave spawns every 30-60 seconds
6. Heroes flee if low HP and carrying gold (except final wave)
7. Win: All waves defeated
8. Lose: DM killed

## Next Steps

**Required for playable game:**
- Add chest, arrow, and tile effect sprites
- Wire up UI buttons (Pause, Resume, Step, Start Combat)
- Add wave counter to UI
- Test full gameplay loop
- Balance difficulty

**Optional polish:**
- Victory/defeat screens
- Audio cues for wave spawns and combat
- Visual feedback when wave spawns
- Animations for projectiles and effects

## Testing

Run standalone tests:
```bash
dotnet run --project TestRunner.csproj
```

Should see 43 passing tests.
