using UnityEngine;

public class pirateStationMenu : MonoBehaviour
{
    [SerializeField] GameObject UI;
    [SerializeField] PlayerController playerController;
    bool shouldTrigger = true;
    bool ogMouseMode;
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "PlayerShip" && shouldTrigger)
        {
            UI.SetActive(true);
            ogMouseMode = playerController.useMouseAim;
            playerController.enabled = false;
            playerController.useMouseAim = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            shouldTrigger = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.name == "PlayerShip")
        {
            UI.SetActive(false);
            playerController.enabled = true;
            shouldTrigger = true;
        }
    }

    public void leaveMenu()
    {
        UI.SetActive(false);
        playerController.enabled = true;
        playerController.useMouseAim = ogMouseMode;
        if(ogMouseMode == true)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        shouldTrigger = false;
    }
}
