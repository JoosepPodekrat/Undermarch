using System;
using Undermarch.Simulation.Entities;

namespace Undermarch.Simulation.Events
{
    public static class CharacterEvents
    {
        public static event Action<Character> OnCharacterSpawned;
        public static event Action<Character> OnCharacterDied;
        public static event Action<Character> OnCharacterAttacked;
        public static event Action<Character> OnCharacterHurt;

        public static void RaiseSpawn(Character c) => OnCharacterSpawned?.Invoke(c);
        public static void RaiseDeath(Character c) => OnCharacterDied?.Invoke(c);
        public static void RaiseAttack(Character c) => OnCharacterAttacked?.Invoke(c);
        public static void RaiseHurt(Character c) => OnCharacterHurt?.Invoke(c);
    }
}
