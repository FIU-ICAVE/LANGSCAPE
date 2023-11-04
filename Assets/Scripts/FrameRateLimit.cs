using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateLimit : MonoBehaviour
{

    public enum limits
    {
        noLimit = 0,
        limit30 = 30,
        limit60 = 60,
        limit120 = 120,
        limit240 = 240,
        limit480 = 480,
    }

    public limits limit;
    void Awake()
    {
        Application.targetFrameRate = (int)limit;
    }

    void Update()
    {
        Application.targetFrameRate = (int)limit;
    }
}