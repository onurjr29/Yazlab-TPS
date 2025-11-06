using UnityEngine;

public class EnemyIdle : MonoBehaviour
{
    [Header("Duran düşman ayarları")]
    public float rotateSpeed = 30f; // Dilersen sağa sola baksın

    void Update()
    {
        // Düşman tamamen sabit kalsın istiyorsan bu satırı sil.
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}
