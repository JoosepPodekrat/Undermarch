using Undermarch.Presentation.Managers;
using UnityEngine;

namespace Undermarch.Presentation.Controllers
{
    public class UIController : MonoBehaviour
    {
        public PlacementController placementController;

        public void OnClick_SelectSlime()
        {
            SelectByType(Data.PlacementType.Slime);
        }

        public void OnClick_SelectSpikeTrap()
        {
            SelectByType(Data.PlacementType.SpikeTrap);
        }

        private void SelectByType(Data.PlacementType type)
        {
            if (placementController == null) return;

            var levelData = GameManager.Instance.CurrentLevelData;
            if (levelData != null && levelData.availableBuildables != null)
            {
                foreach (var buildable in levelData.availableBuildables)
                {
                    if (buildable.placementType == type)
                    {
                        placementController.SelectBuildable(buildable);
                        return;
                    }
                }
            }
            Debug.LogWarning($"UIController: Could not find buildable definition for {type} in current level.");
        }

        public void OnClick_StartCombat()
        {
            GameManager.Instance.StartCombat();
        }
    }
}
