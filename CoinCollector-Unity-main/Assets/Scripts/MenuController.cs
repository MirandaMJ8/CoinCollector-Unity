using UnityEngine;
using UnityEngine.SceneManagement;

// Menú principal: el botó Play crida OnPlayPressed().
public class MenuController : MonoBehaviour
{
    public void OnPlayPressed()
    {
        SceneManager.LoadScene("Level1");
    }
}
