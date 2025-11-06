using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverCanvas;

    private void Start()
    {
        // Baþlangýçta GameOver ekraný kapalý
        gameOverCanvas.SetActive(false);
    }

    public void GameOver()
    {
        // Oyunu durdur ve ekraný aç
        Time.timeScale = 0f;
        gameOverCanvas.SetActive(true);
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartGame()
    {
        // Sahneyi yeniden yükle
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        // Oyundan çýk
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
