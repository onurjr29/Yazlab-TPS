using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Slider healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth; // başlangıçta tam dolu görünsün
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // negatif olmasın
        Debug.Log("Player Health: " + currentHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");

        // GameOverManager varsa, onun GameOver fonksiyonunu çağır
        GameOverManager gameOver = FindObjectOfType<GameOverManager>();
        if (gameOver != null)
        {
            gameOver.GameOver();
        }
        else
        {
            // yedek çözüm: sahneyi yeniden yükle (eğer GameOverManager sahnede yoksa)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
