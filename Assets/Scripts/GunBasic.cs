using UnityEngine;

public class GunBasic : MonoBehaviour
{
    [Header("Crosshair Ayarları")]
    public RectTransform crosshair;      // Crosshair UI objesi
    public Vector3 zoomedCrosshairScale = new Vector3(0.6f, 0.6f, 0.6f); // Zoom halindeki boyut
    public Vector3 normalCrosshairScale = new Vector3(1f, 1f, 1f);       // Normal boyut
    public float crosshairLerpSpeed = 8f; // Zoom geçiş hızı

    [Header("Zoom Ayarları")]
    public float zoomFOV = 30f;        // Yakınlaşınca kamera FOV
    public float normalFOV = 60f;      // Normal görüş açısı
    public float zoomSpeed = 10f;      // Zoom geçiş hızı
    private bool isZoomed = false;

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
    public AudioSource audioSource;     // Ana ses kaynağı
    public int maxAudioSources = 5;     // Aynı anda kaç ses kanalı kullanılacak

    private AudioSource[] audioPool;    // Ses havuzu
    private int audioIndex = 0;         // Sıradaki ses kaynağı
    private float nextTime;             // Ateş gecikme kontrolü

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

        // Ses havuzunu oluştur
        audioPool = new AudioSource[maxAudioSources];
        for (int i = 0; i < maxAudioSources; i++)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.clip = shootSound;
            src.playOnAwake = false;
            src.spatialBlend = 1f; // 3D ses
            src.volume = 0.8f;
            audioPool[i] = src;
        }
    }

    void Update()
    {
        // 🔫 Ateş etme (sol tık)
        if (Input.GetMouseButton(0) && Time.time >= nextTime)
        {
            nextTime = Time.time + (1f / fireRate);
            Fire();
        }

        // 🔍 Zoom (sağ tık)
        if (Input.GetMouseButtonDown(1))
        {
            isZoomed = !isZoomed; // Toggle zoom
        }

        // 🎥 Kamera zoom geçişi
        if (cam != null)
        {
            float targetFOV = isZoomed ? zoomFOV : normalFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        }

        // 🎯 Crosshair küçülme/büyüme geçişi
        if (crosshair != null)
        {
            Vector3 targetScale = isZoomed ? zoomedCrosshairScale : normalCrosshairScale;
            crosshair.localScale = Vector3.Lerp(crosshair.localScale, targetScale, Time.deltaTime * crosshairLerpSpeed);
        }
    }



    void Fire()
    {
        // Ses çal
        var source = audioPool[audioIndex];
        audioIndex = (audioIndex + 1) % maxAudioSources;
        source.PlayOneShot(shootSound);

        // Muzzle Flash
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(true);
            Invoke(nameof(HideMuzzleFlash), 0.05f);
        }

        // Mermi oluştur
        if (bulletPrefab != null && firePoint != null)
        {
            var bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // 🔥 ARTIK DOĞRU OWNER’I ATA (her durumda)
            // FirePoint’in parent’ı “onursuz” objesi
            GameObject realOwner = null;

            // Eğer bu FirePoint bir düşmanın altındaysa:
            var enemyAI = GetComponentInParent<EnemyAI>();
            if (enemyAI != null)
                realOwner = enemyAI.gameObject;
            else
            {
                // Eğer oyuncuysa:
                var player = GetComponentInParent<PlayerHealth>();
                if (player != null)
                    realOwner = player.gameObject;
            }

            // Eğer hâlâ boşsa son çare parent’ı ata
            if (realOwner == null)
                realOwner = transform.parent != null ? transform.parent.gameObject : this.gameObject;

            // Mermiye owner’ı bildir
            var bulletScript = bulletObj.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.owner = realOwner;
                Debug.Log($"[GunBasic] Owner atandı: {realOwner.name}");
            }
            else
            {
                Debug.LogWarning("[GunBasic] Bullet script bulunamadı!");
            }

            // Kendi collider’larına çarpmasın
            if (bulletObj.TryGetComponent<Collider>(out var bcol))
            {
                var colliders = GetComponentsInChildren<Collider>();
                foreach (var col in colliders)
                    Physics.IgnoreCollision(bcol, col);
            }

            // Mermiyi ileri fırlat
            if (bulletObj.TryGetComponent<Rigidbody>(out var rb))
                rb.velocity = firePoint.forward * bulletSpeed;
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
