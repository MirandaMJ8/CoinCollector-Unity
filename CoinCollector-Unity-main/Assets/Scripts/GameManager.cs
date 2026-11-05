using UnityEngine;
using UnityEngine.SceneManagement;

// Gestor de la partida (Singleton). Guarda la puntuació i reinicia el nivell.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private int score = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Coin collected! Score: " + score);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
