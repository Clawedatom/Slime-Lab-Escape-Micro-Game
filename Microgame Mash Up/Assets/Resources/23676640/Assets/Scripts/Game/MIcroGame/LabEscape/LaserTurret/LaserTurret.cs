using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static GameMaster;

public class LaserTurret : MonoBehaviour
{
    TileManager tileManager;

    [SerializeField] private Transform laserTurretTip;

    [SerializeField] private List<LaserBullet> bulletPool = new List<LaserBullet>();
    [SerializeField] private List<LaserBullet> activeLasers = new List<LaserBullet>();

    [Header("Shoot Settings")]

    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float bulletTravelDist = 10f;


    [SerializeField] private float minBulletSpeed = 1f;
    [SerializeField] private float maxBulletSpeed = 5f;

    
    public void OnTurretInit(int difficulty)
    {
        tileManager = TileManager.Instance;

        bulletSpeed = Mathf.Lerp(maxBulletSpeed,minBulletSpeed,difficulty / 5);
        foreach (LaserBullet bullet in bulletPool)
        {
            bullet.OnBulletInit(laserTurretTip.position, bulletSpeed, bulletTravelDist);
        }
        float t = (difficulty - 1) / 4f;

      
    }

    public void OnTurretUpdate()
    {
        


        for (int i = activeLasers.Count - 1; i >= 0; i--)
        {
            LaserBullet bullet = activeLasers[i];

            if (bullet.IsActive)
            {
                bullet.OnBulletUpdate();


                if (!bullet.IsActive)
                {
                    activeLasers.RemoveAt(i);
                }
            }
        }
    }

    private void Shoot()
    {
        foreach(LaserBullet bullet in bulletPool)
        {
            if (!bullet.IsActive)
            {
                bullet.OnBulletInit(laserTurretTip.position, bulletSpeed, bulletTravelDist);

                activeLasers.Add(bullet);
                bullet.OnBulletEnable();
                break;
            }
        }


    }

    public void ManualShoot()
    {
        Shoot();
        SoundManager.PlayClip(GameManager.Instance.GetGameData.ShootSound);
    }
}
