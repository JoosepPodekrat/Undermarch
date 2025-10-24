namespace Undermarch
{
    public interface ICharacterEffect
    {
        string Name { get; set; }
        float Duration { get; set; }

        void Apply(Character target);
        void Remove(Character target);
    }
}
