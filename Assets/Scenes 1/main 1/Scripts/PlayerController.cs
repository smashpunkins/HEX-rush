using StarterAssets;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    private void Awake()
    {
        // Busca los componentes en el mismo GameObject.
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    public void EnableControl()
    {
        if (thirdPersonController != null)
            thirdPersonController.enabled = true;

        if (starterAssetsInputs != null)
            starterAssetsInputs.enabled = true;
    }

    public void DisableControl()
    {
        if (thirdPersonController != null)
            thirdPersonController.enabled = false;

        if (starterAssetsInputs != null)
            starterAssetsInputs.enabled = false;
    }
}
