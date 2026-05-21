using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;

    public bool canAutoFire;

    public float fireRate;
    [HideInInspector]
    public float fireCounter;

    public int currentAmmo, maxAmmo, pickupAmount;

    public Transform firepoint;

    public float zoomAmount;

    public string gunName;

    void Awake()
    {
        EnsureMaxAmmo();
    }

    void EnsureMaxAmmo()
    {
        if (maxAmmo <= 0)
            maxAmmo = currentAmmo > 0 ? currentAmmo : 1;
    }

    public void UpdateAmmoDisplay()
    {
        EnsureMaxAmmo();
        UIController.instance.ammoText.text = $"{currentAmmo}/{maxAmmo}";
    }

    // Update is called once per frame
    void Update()
    {
        if(fireCounter > 0)
        {
            fireCounter -= Time.deltaTime;
        }
    }

    public void GetAmmo()
    {
        currentAmmo += pickupAmount;

        UpdateAmmoDisplay();
    }
}
