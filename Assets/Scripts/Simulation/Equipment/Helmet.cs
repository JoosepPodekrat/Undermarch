namespace Undermarch
{
    public interface Helmet
    {
        string name { get; }
        string description { get; }
        void Equip(Character target);
        void Unequip(Character target);

        void Apply(Character target);
    }
}