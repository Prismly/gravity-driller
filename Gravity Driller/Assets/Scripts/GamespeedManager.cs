using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GamespeedManager : object
{
    private const float normalGamespeed = 1.0f;
    private const float slowGamespeed = 0.05f;

    public static void GamespeedToSlow()
    {
        Time.fixedDeltaTime = Time.fixedDeltaTime * (slowGamespeed / Time.timeScale);
        Time.timeScale = slowGamespeed;
    }

    public static void GamespeedToNormal()
    {
        Time.fixedDeltaTime = Time.fixedDeltaTime * (normalGamespeed / Time.timeScale);
        Time.timeScale = normalGamespeed;
    }
}
