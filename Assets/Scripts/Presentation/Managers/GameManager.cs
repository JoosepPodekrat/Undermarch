using System.Linq;
using Undermarch.Presentation.UI;
using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Entities.Characters.DungeonMaster;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Levels;
using UnityEngine;

namespace Undermarch.Presentation.Managers
{
    public enum GameState
    {
        Placement,
        Combat
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Board Board { get; private set; }
        public TickSystem TickSystem { get; private set; }
        public GameState CurrentState { get; private set; }
        public EndGameUI endGameUI;

        private bool isHeroTurn = true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            CurrentState = GameState.Placement;
            Debug.Log("GameManager: Awake() - State: Placement");

            Board = new Board(20, 20);
            TickSystem = new TickSystem();
            
            Debug.Log("GameManager initialized. Board and TickSystem created.");
        }

        private void Start()
        {
            Debug.Log("GameManager: Start()");
            LevelLoader.LoadLevel1(Board);
            Debug.Log("GameManager: Level 1 loaded.");

            // Find the EndGameUI in the scene
            endGameUI = FindObjectOfType<EndGameUI>();
            if (endGameUI == null)
            {
                Debug.LogWarning("GameManager: Could not find an EndGameUI component in the scene.");
            }
        }

        public void StartCombat()
        {
            if (CurrentState == GameState.Placement)
            {
                CurrentState = GameState.Combat;
                Debug.Log("GameManager: StartCombat() - State: Combat");
                TickSystem.OnTick += HandleTick;
            }
        }

        public void InitializeTickDriver(Bootstrap.TickDriver driver)
        {
            if (driver != null)
            {
                driver.SetTickSystem(TickSystem);
                Debug.Log("GameManager: Successfully initialized TickDriver.");
            }
            else
            {
                Debug.LogError("GameManager: Attempted to initialize a null TickDriver.");
            }
        }

        private void OnDestroy()
        {
            if (TickSystem != null)
            {
                TickSystem.OnTick -= HandleTick;
            }
        }

        private void HandleTick(int tick)
        {
            if (CurrentState != GameState.Combat) return;

            var characters = Board.GetAllCharacters().ToList();
            Debug.Log($"HandleTick: Tick {tick}, Processing {characters.Count} characters. It is {(isHeroTurn ? "Hero" : "Monster")} turn.");

            if (isHeroTurn)
            {
                foreach (var character in characters.Where(c => c.faction == Faction.Hero && !c.IsDead))
                {
                    character.Act(Board);
                }
            }
            else
            {
                foreach (var character in characters.Where(c => c.faction == Faction.Defender && !c.IsDead))
                {
                    character.Act(Board);
                }
            }

            foreach (var character in characters)
            {
                if (character.IsDead)
                {
                    var pos = Board.GetPositionOf(character);
                    if (pos.IsValid())
                    {
                        Board.RemoveEntity(pos);
                        Debug.Log($"Removed dead character at {pos.x},{pos.y}");
                    }
                }
            }

            var remainingCharacters = Board.GetAllCharacters().ToList();
            bool dungeonMasterIsAlive = remainingCharacters.Any(c => c is DungeonMaster);
            bool heroesAreAlive = remainingCharacters.Any(c => c.faction == Faction.Hero);

            if (!dungeonMasterIsAlive)
            {
                Debug.Log("Game Over: The Dungeon Master has been defeated! YOU LOSE!");
                if (endGameUI != null)
                {
                    endGameUI.ShowEndGamePopup("You Lose!");
                }
                TickSystem.Stop();
                return;
            }
            
            if (!heroesAreAlive)
            {
                Debug.Log("Game Over: All heroes have been defeated! YOU WIN!");
                if (endGameUI != null)
                {
                    endGameUI.ShowEndGamePopup("You Win!");
                }
                TickSystem.Stop();
                return;
            }

            isHeroTurn = !isHeroTurn;
        }
    }
}
