﻿using System.Collections;
using UnityEngine;

public class TurretControl : MonoBehaviour
{
    private GameObject target;
    private bool targetLocked;
    public GameObject TurretMovable;
    public GameObject bullet;
    public GameObject muzzle1;
    public GameObject muzzle2;
    private float fireTimer = 0.8f;  // Used to be 2.
    private bool readyToShoot;
    private bool readyToShoot2;
    private float delayTimer = 0.4f;  // Used to be 1.
    private PlayerControl playerScript;
    private SoundManager soundManager;

    // If player is within this distance of a turret, bullets will play a sound.
    private const float bulletSoundThreshold = 50f;

    void Start()
    {
        playerScript = FindObjectOfType<PlayerControl>();
        soundManager = FindObjectOfType<SoundManager>();
        readyToShoot = true;
        readyToShoot2 = false;
        StartCoroutine(delay());
    }

    void Update()
    {
        if (targetLocked)
        {
            Vector2 mag = playerScript.VelRelativeToLook();
            Vector3 playerVelocity = new Vector3(mag.x, mag.y, 0);
            TurretMovable.transform.LookAt(target.transform.position + playerVelocity * 1.0f);
            // Angle adjustments
            TurretMovable.transform.Rotate(8, 0, 0);

            if (readyToShoot)
            {
                Shoot();
            }
            if (readyToShoot2)
            {
                Shoot2();
            }
            // targetLocked = false;
            // target = null;
        }
    }

    private void PlayBulletSound(Vector3 muzzleLocation)
    {
        // Only play bullet sound if the player is near the turret.
        if (Vector3.Distance(playerScript.transform.position, muzzleLocation) < bulletSoundThreshold)
        {
            soundManager.PlayBulletSound(muzzleLocation);
        }
    }

    void Shoot()
    {
        Transform _bullet = Instantiate(bullet.transform, muzzle1.transform.position, Quaternion.identity);
        _bullet.transform.rotation = TurretMovable.transform.rotation;
        readyToShoot = false;
        StartCoroutine(FireRate());
        PlayBulletSound(muzzle1.transform.position);
    }

    void Shoot2()
    {
        Transform _bullet = Instantiate(bullet.transform, muzzle2.transform.position, Quaternion.identity);
        _bullet.transform.rotation = TurretMovable.transform.rotation;
        readyToShoot2 = false;
        StartCoroutine(FireRate2());
        PlayBulletSound(muzzle2.transform.position);
    }

    IEnumerator FireRate()
    {
        yield return new WaitForSeconds(fireTimer);
        readyToShoot = true;
    }

    IEnumerator FireRate2()
    {
        yield return new WaitForSeconds(fireTimer);
        readyToShoot2 = true;
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(delayTimer);
        readyToShoot2 = true;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.gameObject;
            targetLocked = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Stop shooting at the player when they're far away.
        if (other.tag == "Player")
        {
            targetLocked = false;
        }
    }

    public void ResetTurret()
    {
        targetLocked = false;
        target = null;
    }
}
