using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public void handlePause()
    {
        Time.timeScale = 0;
    }

    public void handleResume()
    {
        Time.timeScale = 1;
    }

}
