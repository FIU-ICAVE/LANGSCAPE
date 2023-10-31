using UnityEngine;

public class LandSwitcher : MonoBehaviour
{
    public Gameland[] landsToToggle; // Array of lands to toggle
    private int currentlandIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Activate the initial land
        Toggleland(currentlandIndex);
    }

    public void SwitchLand()
    {
        // Deactivate the current land
        Toggleland(currentlandIndex);

        // Increment the index to switch to the next land
        currentlandIndex = (currentlandIndex + 1) % landsToToggle.Length;

        // Activate the next land
        Toggleland(currentlandIndex);
    }

    private void ToggleLand(int index)
    {
        landsToToggle[index].SetActive(!landsToToggle[index].activeSelf);
    }
}