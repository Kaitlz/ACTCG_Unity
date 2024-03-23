using UnityEngine;

public class FpsCap : MonoBehaviour
{
    void Update()
    {
        //Caps framerate
        Application.targetFrameRate = 60;
    }
}
