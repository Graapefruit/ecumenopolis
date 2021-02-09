using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{   
    private static readonly Color dayLightColour = new Color(0.851f, 0.698f, 0.433f, 1.0f);
    private static readonly Color nightLightColour = new Color(0.694f, 0.914f, 1.0f, 1.0f);
    private static readonly float dayLightIntensity = 1.0f;
    private static readonly float nightLightIntensity = 0.2f;
    private static readonly float dayDurationSeconds = 15.0f;
    private static readonly float nightDurationSeconds = 60.0f;
    private bool isDay = false;
    private float timeLeftInCycle = 0.0f;
    public Light overheadLight;
    private static GameTimeManager gtm;
    
    void Awake() {
        if (gtm != null) {
            Debug.LogWarning("Static gtm object already found! Deleting and making another");
            GameObject.Destroy(gtm);
        }
        gtm = this;
        switchToNightLight();
        timeLeftInCycle = nightDurationSeconds;
    }

    void Update()
    {
        timeLeftInCycle -= Time.deltaTime;
        if (timeLeftInCycle <= 0.0f) {
            isDay = !isDay;
            if (isDay) {
                timeLeftInCycle = dayDurationSeconds;
                switchToDayLight();
                EnemyManager.removeAllEnemies();
            } else {
                timeLeftInCycle = nightDurationSeconds;
                switchToNightLight();
            }
        }
    }

    public static bool isCurrentlyDay() {
        return gtm.isDay;
    }

    private void switchToDayLight() {
        overheadLight.color = dayLightColour;
        overheadLight.intensity = dayLightIntensity;
    }

    private void switchToNightLight() {
        overheadLight.color = nightLightColour;
        overheadLight.intensity = nightLightIntensity;
    }
}
