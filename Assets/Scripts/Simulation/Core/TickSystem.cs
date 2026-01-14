using System.Collections.Generic;
using System.Linq;

using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Interfaces;
using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Entities.Characters.DungeonMaster;
using Undermarch.Simulation.Entities.Characters.Heroes;

using Undermarch.Simulation.Core;



namespace Undermarch.Simulation.Core
{
    public class TickSystem : ITickSystem
    {
        public TickMode Mode { get; set; }
        public int TicksPerSecond { get; set; }
        public int CurrentTick { get; private set; }
        public WaveSpawner WaveSpawner => waveSpawner;

        private IBoard board;
        private IGameState gameState;
        private WaveSpawner waveSpawner;

        public event System.Action<int> OnTick;


        public TickSystem(IBoard board, IGameState gameState, int ticksPerSecond = 2, WaveSpawner waveSpawner = null)
        {
            this.board = board;
            this.gameState = gameState;
            this.waveSpawner = waveSpawner;
            TicksPerSecond = ticksPerSecond;
            Mode = TickMode.Paused;
            CurrentTick = 0;
        }

        public void SetWaveSpawner(WaveSpawner spawner)
        {
            waveSpawner = spawner;
        }

        public void Pause()
        {
            Mode = TickMode.Paused;
        }

        public void Resume()
        {
            Mode = TickMode.Auto;
        }

        public void Step()
        {
            Tick();
            Mode = TickMode.Paused;
        }

        public void Reset()
        {
            CurrentTick = 0;
            Mode = TickMode.Paused;
        }

        public void Tick()
        {
            if (gameState.Phase != GamePhase.Combat)
                return;

            CurrentTick++;
            SimulationLog.Log($"=== Tick {CurrentTick} ===");

            // Wave spawning
            if (waveSpawner != null && board is Board b)
                waveSpawner.CheckSpawn(CurrentTick, b);

            // Simulation phases
            EntitiesPhase();
            ProjectilesPhase();
            EffectsPhase();
            CleanupPhase();

            // Game-over logic (internal)
            CheckGameOver();

            // Fire tick event for GameManager/UI/etc.
            OnTick?.Invoke(CurrentTick);

            // Step mode
            if (Mode == TickMode.Step)
                Mode = TickMode.Paused;
        }


        private void EntitiesPhase()
        {
            // All characters act
            List<Character> characters = new List<Character>(board.GetAllCharacters());

            foreach (var character in characters)
            {
                // Skip dead characters
                if (character.IsDead || character.currentHP <= 0)
                {
                    continue;
                }

                // Recalculate stats before acting (in case buffs changed)
                character.CalculateStats();

                // Act
                if (board is Board b)
                {
                    character.Act(b);
                }
            }
        }

        private void ProjectilesPhase()
        {
            // Collect all projectiles
            List<Projectile> projectiles = new List<Projectile>();

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    TilePos pos = new TilePos(x, y);
                    object interactable = board.GetInteractableAt(pos);
                    if (interactable is Projectile projectile && projectile.IsActive)
                    {
                        projectiles.Add(projectile);
                    }
                }
            }

            // Tick all projectiles
            foreach (var projectile in projectiles)
            {
                if (projectile.IsActive)
                {
                    projectile.Tick(board);
                }
            }
        }

        private void EffectsPhase()
        {
            // Tick down tile effects
            List<TileEffect> expiredEffects = new List<TileEffect>();

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    TilePos pos = new TilePos(x, y);
                    object interactable = board.GetInteractableAt(pos);
                    if (interactable is TileEffect effect)
                    {
                        effect.Tick();
                        if (effect.IsExpired())
                        {
                            expiredEffects.Add(effect);
                        }
                    }
                }
            }

            // Remove expired effects
            foreach (var effect in expiredEffects)
            {
                board.RemoveInteractable(effect.Position);
            }

            // Tick character buffs/debuffs
            foreach (var character in board.GetAllCharacters())
            {
                character.TickBuffsAndDebuffs();
            }
        }

        private void CleanupPhase()
{
    List<Character> deadCharacters = new List<Character>();

    foreach (var character in board.GetAllCharacters())
    {
        if (character.IsDead || character.currentHP <= 0)
        {
            deadCharacters.Add(character);

            // Give gold to Dungeon Master
            if (character is Hero hero)
{
    foreach (var kvp in hero.ResourcesGiven)
    {
        gameState.AddResource(kvp.Key, kvp.Value);
    }
}

        }
    }

    foreach (var dead in deadCharacters)
    {
        TilePos pos = board.GetPositionOf(dead);
        if (pos.IsValid())
        {
            board.RemoveEntity(pos);
            SimulationLog.Log($"{dead.Name} has been removed from the board.");
        }
    }
}


        private void CheckGameOver()
        {
            bool anyHeroesAlive = board.GetAllCharacters().Any(c => c.faction == Faction.Hero && !c.IsDead);
            bool dungeonMasterAlive = board.GetAllCharacters().Any(c => c is DungeonMaster && !c.IsDead);

            // Defeat
            if (!dungeonMasterAlive)
            {
                gameState.Phase = GamePhase.GameOver;
                Mode = TickMode.Paused;
                SimulationLog.Log("Defeat! The Dungeon Master has fallen!");
                return;
            }

            // Victory: all waves spawned AND no heroes alive
            if (waveSpawner.AllWavesSpawned && !anyHeroesAlive)
            {
                gameState.Phase = GamePhase.GameOver;
                Mode = TickMode.Paused;
                SimulationLog.Log("Victory! All heroes defeated!");
                return;
            }
        }

    }
}