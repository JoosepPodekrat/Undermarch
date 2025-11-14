namespace Undermarch.Simulation.Interfaces
{
    /// <summary>
    /// Interface for interactable entities like traps and chests.
    /// </summary>
    public interface IInteractable : IEntity
    {
        void Interact(ICharacter character);
        bool IsActive { get; }
    }
}
