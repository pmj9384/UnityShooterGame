using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum State
    {
        Ready,
        Empty,
        Reloading,
    }

    public State GunState { get; private set; }
    public GunData gundata;
    
    public Transform firePosition;

    public float fireDistance = 50f;

    private LineRenderer lineRenderer;
    private AudioSource audioSource;
    public ParticleSystem muzzleEffect;
    public ParticleSystem shellEffect;

    private float lastFireTime;
    private int currentAmmo;
    private int ammoRemain;
    private int magAmmo;

    private UiManager uiManager;

    public void AddAmmo(int addAmmo)
    {
        ammoRemain = Mathf.Min(ammoRemain + addAmmo, gundata.startAmmoRemain);
    }
    private void Awake()
    {
        
        lineRenderer = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();
        uiManager = FindObjectOfType<UiManager>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
        
        // uiManager.UpdateAmmoText(currentAmmo, ammoRemain);
    }
    private void OnEnable()
    {
        GunState = State.Ready;
        lastFireTime = 0f;
        currentAmmo = gundata.startAmmoRemain; 
    }

public void Fire()
{
    if (GunState == State.Ready && Time.time > lastFireTime + gundata.fireRate)
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            lastFireTime = Time.time;
            var endPos = Vector3.zero;
            Ray ray = new Ray(firePosition.position, firePosition.forward);

     
            RaycastHit[] hits = Physics.RaycastAll(ray, fireDistance);

            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    endPos = hit.point;

                  
                    var iDamageable = hit.collider.GetComponent<IDamageable>();
                    if (iDamageable != null)
                    {
                        iDamageable.OnDamage(gundata.damage, hit.point, hit.normal);
                    }
                }
            }
            else
            {
                endPos = firePosition.position + firePosition.forward * fireDistance;
            }

            StartCoroutine(ShotEffect(endPos));

            if (currentAmmo == 0)
            {
                GunState = State.Empty; 
            }
            // uiManager.UpdateAmmoText(currentAmmo, ammoRemain);
        }
    }
}

    //    public void Reload()
    // {
    //     if (GunState == State.Reloading || currentAmmo == gundata.magCapacity) return;
    //     if (gundata.startAmmoRemain <= 0)
    //     {
    //     Debug.Log("No ammo left!");
    //     return;
    //     }

    // StartCoroutine(ReloadRoutine());
    // }

    // private IEnumerator ReloadRoutine()
    // {
    // GunState = State.Reloading;

    // // 리로드 애니메이션 대기 시간
    // // yield return new WaitForSeconds(gundata.reloadTime);

    // // int ammoToReload = Mathf.Min(gundata.magCapacity - currentAmmo, gundata.startAmmoRemain);

    // // // 탄창 재장전
    // // currentAmmo = gundata.magCapacity;
    // GunState = State.Ready;
    // }
    private IEnumerator ShotEffect(Vector3 hitPoint)

    {
        audioSource.PlayOneShot(gundata.shotClip);  

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, firePosition.position);
        lineRenderer.SetPosition(1, hitPoint);

        muzzleEffect.Play();
        shellEffect.Play();

        yield return new WaitForSeconds(0.03f);
        lineRenderer.enabled = false;
    }

    private void OnDrawGizmos() // 테스트용 선 그리기 
    {

        Gizmos.DrawLine(firePosition.position,firePosition.position + firePosition.forward * fireDistance);

    }
}
