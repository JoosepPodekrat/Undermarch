using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Effects.Buffs;
using Undermarch.Simulation.Effects.Debuffs;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Simulation.Entities
{
    public enum EffectType
    {
        Slow,
        Poison,
        Fog,
        Fire
    }

    public class TileEffect : IEntity
    {
        public TilePos Position { get; set; }
        public string Name { get; private set; }
        public EffectType Type { get; private set; }
        public int Duration { get; private set; }
        public int Intensity { get; private set; }

        public TileEffect(EffectType type, TilePos position, int duration, int intensity)
        {
            Type = type;
            Position = position;
            Duration = duration;
            Intensity = intensity;
            Name = GetEffectName(type);
        }

        private string GetEffectName(EffectType type)
        {
            return type switch
            {
                EffectType.Slow => "Slow Zone",
                EffectType.Poison => "Poison Cloud",
                EffectType.Fog => "Fog",
                EffectType.Fire => "Fire",
                _ => "Unknown Effect"
            };
        }

        public void Tick()
        {
            if (Duration > 0)
            {
                Duration--;
            }
        }

        public bool IsExpired()
        {
            return Duration <= 0;
        }

        public void ApplyTo(Character character)
        {
            switch (Type)
            {
                case EffectType.Slow:
                    // Apply slow debuff if not already slowed
                    bool hasSlowDebuff = false;
                    foreach (var debuff in character.debuffs)
                    {
                        if (debuff.name == "Slow")
                        {
                            hasSlowDebuff = true;
                            break;
                        }
                    }

                    if (!hasSlowDebuff)
                    {
                        SlowDebuff slow = new SlowDebuff(Intensity);
                        slow.Add(character);
                    }
                    break;

                case EffectType.Poison:
                    DamagePacket poisonDamage = new DamagePacket();
                    poisonDamage.Add(DamageType.Poison, Intensity);
                    character.TakeDamage(poisonDamage);
                    break;

                case EffectType.Fire:
                    DamagePacket fireDamage = new DamagePacket();
                    fireDamage.Add(DamageType.Fire, Intensity);
                    character.TakeDamage(fireDamage);
                    break;

                case EffectType.Fog:
                    // Placeholder for vision reduction
                    break;
            }
        }
    }
}
