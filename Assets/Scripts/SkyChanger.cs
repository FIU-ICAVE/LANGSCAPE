using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this changes the skybox and lighting options with presets for day, night, dusk, and a moonless night. 
public class SkyChanger : MonoBehaviour
{
    // the skybox material, if you want to check where this goes, go to the "Windows" bar, then "Rendering", then "Lighting", then click on "Environment"
    public Material dayMat;
    public Material nightMat;
    public Material duskMat;
    public Material nightMoonlessMat;
    public Material rainyMat;

    // Defines rotations for each sky type, so the right time and lighting is set
    private Vector3 dayRotation = new Vector3(53,370,0);
    private Vector3 nightRotation = new Vector3(28,539,10); 
    private Vector3 duskRotation = new Vector3(17,447,5);
    private Vector3 nightMoonlessRotation = new Vector3(30,157,0); // vector 4?
    private Vector3 rainyRotation = new Vector3( 56, 312, -38);

    public GameObject directionalLight;

    public TMPro.TMP_Dropdown SkyDropdown; // for the dropdown menu
    // Start is called before the first frame update
    void Start()
    {
        SkyDropdown.onValueChanged.AddListener(delegate { ChangeSky(SkyDropdown.value); });
    }

    public void ChangeSky(int skyType)
    {
        // Find the Directional Light GameObject
        directionalLight = GameObject.Find("Directional Light");

        // Get the Light component
        Light light = directionalLight.GetComponent<Light>();

        // Use switch-case to handle each skyType. NOTE: they are indexed starting from 1-onwards, not 0-onwards
        switch(skyType)
        {
            case 1:  // Day
                RenderSettings.skybox = dayMat;                                 // Establishes the Material
                light.transform.rotation = Quaternion.Euler(dayRotation);       // Sets the rotational parameters for "DirectionalLight" gameObject
                light.intensity = 1;                                            // Sets the Intesity of the light
                light.shadowStrength = 1;                                       // Sets the strength of the shadow cast
                light.color = new Color( 1f, 0.9513f, 0.5613f);                 // Sets the color of the DirectionalLight itself, it is slightly yellow in day, and silver at night
                RenderSettings.fogColor = new Color( 0.698f, 0.469f, 0.3194f);  // Sets the Fog color, it is orange during day, and silver at night and rainy
                DynamicGI.UpdateEnvironment();                                  // Updates the enviroment lighting
                break;
            case 2:  // Night
                RenderSettings.skybox = nightMat;
                light.transform.rotation = Quaternion.Euler(nightRotation);
                light.intensity = 0.5f;
                light.shadowStrength = 0.3f;
                light.color = new Color( 0.7547f, 0.7547f, 0.7547f);
                RenderSettings.fogColor = new Color( 0.75f, 0.75f, 0.75f);
                DynamicGI.UpdateEnvironment();
                break;
            case 3:  // Dusk
                RenderSettings.skybox = duskMat;
                light.transform.rotation = Quaternion.Euler(duskRotation);
                light.intensity = 1;
                light.shadowStrength = 1;
                light.color = new Color( 1f, 0.9513f, 0.5613f);
                RenderSettings.fogColor = new Color( 0.698f, 0.469f, 0.3194f);
                DynamicGI.UpdateEnvironment();
                break;
            case 4:  // Night Moonless
                RenderSettings.skybox = nightMoonlessMat;
                light.transform.rotation = Quaternion.Euler(nightMoonlessRotation);
                light.intensity = 0.3f;
                light.shadowStrength = 0.1f;
                light.color = new Color( 0.4245f, 0.4245f, 0.4245f);
                RenderSettings.fogColor = new Color( 0.75f, 0.75f, 0.75f);
                DynamicGI.UpdateEnvironment();
                break;
            case 5:  // Rainy
                RenderSettings.skybox = rainyMat;
                light.transform.rotation = Quaternion.Euler(rainyRotation);
                light.intensity = 0.2f;
                light.shadowStrength = 0.5f;
                light.color = new Color( 0.7547f, 0.7547f, 0.7547f);
                RenderSettings.fogColor = new Color( 0.75f, 0.75f, 0.75f);
                DynamicGI.UpdateEnvironment();
                break;
            default:
                Debug.Log("Invalid sky type");
                break;
        }

        // Updates the environment lighting again, just in case. could probably be deleted.
        DynamicGI.UpdateEnvironment();
    }
}
