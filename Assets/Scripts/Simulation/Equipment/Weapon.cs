namespace Undermarch
{
    public interface Weapon
    {
        string name { get; }
        string description { get; }
        void Equip (Character target);
        void Unequip (Character target);

        void Apply(Character target);
    }
}