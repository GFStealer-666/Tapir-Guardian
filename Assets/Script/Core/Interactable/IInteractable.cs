using UnityEngine;
public interface IInteractable
{
    // Can player currently do this?
    bool CanInteract();

    // Called when player presses E
    void Interact(PlayerBrain player);

    // For UI prompt like "Press E to talk"
    string GetPrompt();

    // Where is this thing located in the world? (usually transform.position)
    Vector3 GetWorldPosition();

    // How close the player must be to interact
    float MaxRange { get; }
}
