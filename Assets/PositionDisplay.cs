using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PositionDisplay : MonoBehaviour
{

    public TextMeshProUGUI displayText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;
        int z = (int)transform.position.z;

        string position = string.Format("{0}, {1}, {2}", x, y, z);

        displayText.text = position;
    }
}
