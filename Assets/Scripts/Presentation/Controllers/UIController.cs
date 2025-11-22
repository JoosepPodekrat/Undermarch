using Undermarch.Presentation.Managers;
using UnityEngine;

namespace Undermarch.Presentation.Controllers
{
    public class UIController : MonoBehaviour
    {
        public PlacementController placementController;

        public void OnClick_SelectSlime()
        {
            if (placementController != null)
            {
                placementController.SelectSlime();

            }
        }

        public void OnClick_SelectSpikeTrap()
        {
            if (placementController != null)
            {
                placementController.SelectSpikeTrap();

            }
        }

        public void OnClick_StartCombat()
        {
            GameManager.Instance.StartCombat();
        }
    }
}
