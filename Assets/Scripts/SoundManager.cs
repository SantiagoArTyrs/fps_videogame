using UnityEngine;
using static Weapon;
using System.Collections;
using System.Collections.Generic;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource shootingSoundCrossbow;
    public AudioSource reloadingSoundCrossbow;

    public AudioSource shootingSoundShotgun;
    public AudioSource reloadingSoundShotgun;
    public AudioSource emptyMagazineSoundCrossbow;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Crossbow:
                shootingSoundCrossbow.Play();
                break;
            case WeaponModel.Shotgun:
                shootingSoundShotgun.Play();
                break;
        }
    }

    public void PlayReloadingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Crossbow:
                reloadingSoundCrossbow.Play();
                break;
            case WeaponModel.Shotgun:
                reloadingSoundShotgun.Play();
                break;
        }
    }



}
