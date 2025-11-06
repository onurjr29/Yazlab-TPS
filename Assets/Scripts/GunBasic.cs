using UnityEngine;

public class GunBasic : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform firePoint;         // Merminin çıkacağı nokta
    public GameObject muzzleFlash;      // Kısa süreli ışık efekti
    public Camera cam;                  // Player içindeki kamera
    public GameObject bulletPrefab;     // Bullet prefab'ı

    [Header("Ayarlar")]
    public float bulletSpeed = 60f;     // Mermi hızı
    public float fireRate = 10f;        // Saniyede 10 mermi

    [Header("Ses Ayarları")]
    public AudioClip shootSound;        // Ateş sesi
    public AudioSource audioSource;     // Ses kaynağı

    private float nextTime;

    void Start()
    {
        // Eğer Inspector'da atanmadıysa, bu GameObject üzerindeki AudioSource'u bul
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Oyun başlarken MuzzleFlash'ı kapalı başlat
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextTime)
        {
            nextTime = Time.time + (1f / fireRate);
            Fire();
        }
    }

    void Fire()
    {
        //  Ses çal
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        //  Muzzle Flash göster
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(true);
            Invoke(nameof(HideMuzzleFlash), 0.05f);
        }

        //  Mermi oluştur
        if (bulletPrefab != null && firePoint != null)
        {
            var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            //  Oyuncuya çarpmayı engelle
            if (bullet.TryGetComponent<Collider>(out var bcol))
            {
                // Player üzerindeki tüm collider’ları bul
                var playerColliders = GetComponentsInChildren<Collider>();
                foreach (var col in playerColliders)
                {
                    Physics.IgnoreCollision(bcol, col);
                }

                // Ekstra koruma: Merminin doğduğu noktadaki yakın objeleri yok say
                Collider[] nearbyColliders = Physics.OverlapSphere(firePoint.position, 0.3f);
                foreach (var ncol in nearbyColliders)
                {
                    Physics.IgnoreCollision(bcol, ncol);
                }
            }

            //  Mermiyi ileri fırlat
            if (bullet.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.velocity = firePoint.forward * bulletSpeed;
            }
        }
    }


    void HideMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }
    }
}
