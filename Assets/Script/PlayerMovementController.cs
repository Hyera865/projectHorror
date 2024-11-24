using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerMovementController : NetworkBehaviour
{
    public GameObject PlayerModel;

    private ThirdPersonCharacter thirdPersonCharacter; // Reference to the ThirdPersonCharacter component

    private void Start() {
        // Disable the player model initially
        PlayerModel.SetActive(false);

        // Ensure we have the ThirdPersonCharacter component
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();

        // Set position to (0, 0, 0)
        SetPosition();
    }

    private void Update() {
        // Check if the current scene is "Game"
        if (SceneManager.GetActiveScene().name == "Game") {
            // Activate the player model if not already active
            if (!PlayerModel.activeSelf) {
                PlayerModel.SetActive(true);
            }

            // Check if the player has ownership
            if (isOwned) {
                // You can add additional logic here if needed
            }
        }
    }

    public void SetPosition() {
        // Set the player's position to (0, 0, 0)
        transform.position = Vector3.zero;
    }
}
