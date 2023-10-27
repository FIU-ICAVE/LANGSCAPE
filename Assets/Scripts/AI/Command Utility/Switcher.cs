using UnityEngine;

public class ObjectSwitcher : MonoBehaviour
{
    public GameObject[] objectsToToggle; // Array of objects to toggle
    private int currentObjectIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Activate the initial object
        ToggleObject(currentObjectIndex);
    }

    public void SwitchObject()
    {
        // Deactivate the current object
        ToggleObject(currentObjectIndex);

        // Increment the index to switch to the next object
        currentObjectIndex = (currentObjectIndex + 1) % objectsToToggle.Length;

        // Activate the next object
        ToggleObject(currentObjectIndex);
    }

    private void ToggleObject(int index)
    {
        objectsToToggle[index].SetActive(!objectsToToggle[index].activeSelf);
    }
}