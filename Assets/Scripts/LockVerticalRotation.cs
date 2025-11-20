using UnityEngine;

public class LockVerticalRotation : MonoBehaviour
{
    void Start()
    {
        // Piespiež landscape tikai šajā scenā
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        Screen.orientation = ScreenOrientation.LandscapeRight;
    }

    void OnDestroy()
    {
        // Atjauno normālu auto-rotation pārējām scēnām
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}

