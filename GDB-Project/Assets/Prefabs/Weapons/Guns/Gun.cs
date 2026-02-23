using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] public Sprite GunIcon;
    [SerializeField] public Sprite CrosshairIcon;
    [SerializeField] public string GunName;
    public int AmmoCur;
    [SerializeField] public int AmmoMax;
    [SerializeField] public float ReloadTime = 1f;
    [SerializeField] public float FireRate = 1f;
    [SerializeField] bool bRaycastBullet;
    [SerializeField] public int HitScanDamage = 10;
    [SerializeField] public GameObject Bullet;
    [SerializeField] public Transform BulletOrigin;
    [SerializeField] LayerMask ignorelayer;

    bool bInReload;
    public bool bFireCooldown;

    public PlayerState OwningPlayer;

    [Header("Recoil Lerp")]
    [SerializeField] Vector3 recoilLocalPosOffset = new Vector3(0f, 0f, -0.06f);
    [SerializeField] Vector3 recoilLocalRotOffset = new Vector3(-6f, 2f, 0f); // euler degrees
    [SerializeField] float recoilKickTime = 0.05f;     // seconds to reach recoil
    [SerializeField] float recoilReturnTime = 0.1f;    // seconds to return
    [SerializeField] float recoilHoldTime = 0.03f;

    Vector3 recoilStartLocalPos;
    Quaternion recoilStartLocalRot;
    Coroutine recoilRoutine;

    [Header("VFX")]
    [SerializeField] GameObject muzzleFlashPrefab;

    [Header("SFX")]
    [SerializeField] AudioClip shootSound;
    [SerializeField] float shootVolume = 1f;
    [SerializeField] Vector2 shootPitchRange = new Vector2(0.95f, 1.05f);

    [Header("Camera Shake")]
    [SerializeField] float cameraShakeAmplitude = 0.15f;
    [SerializeField] float cameraShakeDuration = 0.1f;

    [Header("Reload Spin")]
    [SerializeField] bool spinOnReload = true;
    Vector3 reloadSpinAxis = new Vector3(-1, 0, 0); // local axis
    [SerializeField] bool reloadSpinUseEase = true;

    Coroutine reloadSpinRoutine;

    AudioSource audioSource;
    public void OnEquip(PlayerState owner)
    {
        OwningPlayer = owner;
        GameManager.instance.updateGunUI(GunIcon, CrosshairIcon, AmmoMax, AmmoCur, GunName);
    }

    public void OnUnequip()
    {
        OwningPlayer = null;
        GameManager.instance.ClearGunUI();
    }

    public virtual void Reload()
    {
        if (bInReload) return;
        if (spinOnReload)
        {
            StartReloadSpin();
        }
        StartCoroutine(DoReload());
    }
    void StartReloadSpin()
    {
        if (reloadSpinRoutine != null)
        {
            StopCoroutine(reloadSpinRoutine);
        }

        // Ensure baseline is current equipped pose
        CacheRecoilLocals();

        reloadSpinRoutine = StartCoroutine(CoReloadSpin());
    }

    void StopReloadSpin()
    {
        if (reloadSpinRoutine != null)
        {
            StopCoroutine(reloadSpinRoutine);
            reloadSpinRoutine = null;
        }

        transform.localRotation = recoilStartLocalRot;
    }

    IEnumerator CoReloadSpin()
    {
        Vector3 axis = reloadSpinAxis;
        if (axis.sqrMagnitude <= 0.0001f)
        {
            axis = Vector3.forward;
        }
        axis.Normalize();

        float duration = Mathf.Max(0.01f, ReloadTime);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (reloadSpinUseEase)
            {
                // smoothstep ease in-out
                t = t * t * (3f - 2f * t);
            }

            float angle = 360f * t;

            transform.localRotation = recoilStartLocalRot * Quaternion.AngleAxis(angle, axis);

            yield return null;
        }

        transform.localRotation = recoilStartLocalRot;
        reloadSpinRoutine = null;
    }
    IEnumerator DoReload()
    {
        bInReload = true;
        yield return new WaitForSeconds(ReloadTime);
        bInReload = false;
        AmmoCur = AmmoMax;
        GameManager.instance.updateAmmoUI(AmmoMax, AmmoCur);
    }

    public IEnumerator FireCooldown()
    {
        bFireCooldown = true;
        yield return new WaitForSeconds(FireRate);
        bFireCooldown = false;
    }

    public virtual void SecondaryFire() { }

    public virtual void Shoot(PlayerState Instagator)
    {
        if (!GetCanFire()) return;

        StartCoroutine(FireCooldown());
        AmmoCur--;
        GameManager.instance.updateAmmoUI(AmmoMax, AmmoCur);

        iOwner owner = Instagator.EntityRef.GetComponent<iOwner>();
        if (owner != null)
        {
            Transform cam = owner.GetCameraTransform();

            PlayRecoil();
            SpawnMuzzleFlash();
            PlayShootSound();
            PlayCameraShake();

            Vector3 spawnPosition = cam.position + cam.forward * 0.7f;
            Quaternion spawnRotation = cam.rotation;

            if (!bRaycastBullet)
            {
                GameObject abullet = Instantiate(Bullet, spawnPosition, spawnRotation);
                abullet.GetComponent<Projectile>().MyOwner = Instagator;
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(cam.position, cam.forward, out hit))
                {
                    iDamage dmg = hit.collider.GetComponent<iDamage>();
                    if (dmg != null)
                    {
                        PlayerState otherplayer = null;
                        iOwner otherplayerowner = hit.transform.root.GetComponent<iOwner>();
                        if (otherplayerowner != null)
                        {
                            otherplayer = otherplayerowner.OwningPlayer();
                        }

                        dmg.takeDamage(HitScanDamage, OwningPlayer);
                    }
                }
            }
        }

        if (AmmoCur == 0)
        {
            Reload();
        }
    }

    public bool GetCanFire()
    {
        return AmmoCur > 0 && !bInReload && !bFireCooldown;
    }

    protected virtual void Start()
    {
        AmmoCur = AmmoMax;
        OwningPlayer = transform.root.GetComponent<iOwner>().OwningPlayer();

        CacheRecoilLocals();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound
        }
    }

    protected virtual void Update()
    {
        if (BulletOrigin != null)
        {
            Debug.DrawRay(BulletOrigin.position, transform.forward, Color.red, Time.deltaTime);
        }
    }

    void CacheRecoilLocals()
    {
        recoilStartLocalPos = transform.localPosition;
        recoilStartLocalRot = transform.localRotation;
    }

    public void PlayRecoil()
    {
        if (recoilRoutine != null)
        {
            StopCoroutine(recoilRoutine);
        }
        recoilRoutine = StartCoroutine(CoRecoil());
    }

    IEnumerator CoRecoil()
    {
        Vector3 recoilPos = recoilStartLocalPos + recoilLocalPosOffset;
        Quaternion recoilRot = recoilStartLocalRot * Quaternion.Euler(recoilLocalRotOffset);

        float elapsed = 0f;

        // Kick phase
        while (elapsed < recoilKickTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / recoilKickTime);

            transform.localPosition = Vector3.Lerp(recoilStartLocalPos, recoilPos, t);
            transform.localRotation = Quaternion.Slerp(recoilStartLocalRot, recoilRot, t);

            yield return null;
        }

        // Hold
        if (recoilHoldTime > 0f)
            yield return new WaitForSeconds(recoilHoldTime);

        // Return phase
        elapsed = 0f;

        while (elapsed < recoilReturnTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / recoilReturnTime);

            transform.localPosition = Vector3.Lerp(recoilPos, recoilStartLocalPos, t);
            transform.localRotation = Quaternion.Slerp(recoilRot, recoilStartLocalRot, t);

            yield return null;
        }

        transform.localPosition = recoilStartLocalPos;
        transform.localRotation = recoilStartLocalRot;

        recoilRoutine = null;
    }
    void SpawnMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || BulletOrigin == null)
            return;

        GameObject flash = Instantiate(
            muzzleFlashPrefab,
            BulletOrigin.position,
            BulletOrigin.rotation
        );

        // Optional: parent it to barrel so it follows recoil
        flash.transform.SetParent(BulletOrigin);

        Destroy(flash, 1f); // adjust if needed
    }
    void PlayShootSound()
    {
        if (shootSound == null || audioSource == null)
            return;

        audioSource.pitch = Random.Range(shootPitchRange.x, shootPitchRange.y);
        audioSource.PlayOneShot(shootSound, shootVolume);
    }

    public void PlayCameraShake()
    {
        if (cameraShakeAmplitude <= 0f || cameraShakeDuration <= 0f)
            return;

        if (OwningPlayer == null)
            return;

        iOwner owner = OwningPlayer.EntityRef.GetComponent<iOwner>();
        if (owner == null)
            return;

        Transform camTransform = owner.GetCameraTransform();
        if (camTransform == null)
            return;

        CameraController camController = camTransform.GetComponent<CameraController>();
        if (camController != null)
        {
            camController.ShakeCamera(cameraShakeAmplitude, cameraShakeDuration);
        }
    }
}