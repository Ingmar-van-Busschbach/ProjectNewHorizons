using UnityEngine.UI;
using UnityEngine;

public class PlagueManager : MonoBehaviour
{
    public static PlagueManager instance;

    [SerializeField] private Button infectButton;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void checkPlagueVials()
    {
        if(ResourceManager.instance.plagueVials >= 1)
        {
            infectButton.gameObject.SetActive(true);
        }
        else
        { 
            infectButton.gameObject.SetActive(false);
        }
    }
}
