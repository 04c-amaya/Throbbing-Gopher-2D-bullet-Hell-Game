using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PlayerCombat : CharacterCombat
{
    PlayerManager playerManager;
    [Header("Text")]
    public Text currentBullet;
    public Text previousBullet;
    public Text nextBullet;
    public Text currentWeapon;
    public Text previousWeapon;
    public Text nextWeapon;
    public GameObject bulletUI;

    [Header("Crosshair")]
    public Sprite HipFireCrosshair;
    public Sprite AimingCrosshair;
    public SpriteRenderer playerCrosshair;
    [HideInInspector]
    public Vector3 mousePosition;

    private int bulletNumber = 1;
    private int weaponNumber = 1;
    protected override void Awake()
    {
        base.Awake();
        playerManager = GetComponent<PlayerManager>();

    }

    protected override void FixedUpdate()
    {
     
        if (characterManager.isNotGrounded || playerManager.isDead || playerManager.cantMove)
            return;
        if (transform.localScale.x == 1)
        {
        //checks the position of the player's mouse and records the position
            Vector3 rotation = playerCrosshair.transform.position - playerManager.playerCombat.gun.transform.parent.transform.position;
            float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            playerManager.playerCombat.gun.transform.parent.rotation = Quaternion.Euler(0, 0, rot);
        }
        else
        {
            Vector3 rotation = playerCrosshair.transform.position + playerManager.playerCombat.gun.transform.parent.transform.position;
            float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            playerManager.playerCombat.gun.transform.parent.rotation = Quaternion.Euler(0, 0, rot);
        }
        //Checks to see if the player can fire their weapon and what audio to play
        if (Input.GetMouseButton(0) && canFire == true)
        {
            if (GameObject.Find("AudioManager").GetComponent<AudioManager>().gunFire != null)
            {
                GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySfx(GameObject.Find("AudioManager").GetComponent<AudioManager>().gunFire);
            }
            playerManager.playerAnimation.anim.Play("Player Shooting");
            base.FixedUpdate();
           


            playerManager.playerCombat.gun.transform.parent.gameObject.GetComponent<Animator>().Play("Fire");
        }
        weaponCycle();
        if (playerManager.projectileTpye == ProjectileType.bullet)
        {
            bulletUI.SetActive(true);
            bulletCycle();
        }
        else
        {
            bulletUI.SetActive(false);
        }
        playerManager.playerStats.finalBulletDamage = playerManager.playerStats.damageTaken + playerManager.playerStats.baseBulletDamage;
        mousePosition = playerManager.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        playerCrosshair.transform.position = mousePosition;
       //checks to see if the player is aiming down
        if (Input.GetMouseButton(1))
        {
            playerManager.isAiming = true;
        }
        else
        {
            playerManager.isAiming = false;
        }
       Aiming();
 
    }
    protected override void FireProjectile()
    {
        base.FireProjectile();
            if (playerManager.bulletType == BulletType.tracking)
            {
                RegularBullet();
            }
    }

    private void Aiming()
    {
        if (playerManager.isAiming)
        {
            playerCrosshair.sprite = AimingCrosshair;
        }
        else
        {
            playerCrosshair.sprite = HipFireCrosshair;
        }
    }
//checks to see what bullet type the player is currently using and if "C" or "V" keys are pressed and see what bullets to change to.
    private void bulletCycle()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            bulletNumber += 1;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            bulletNumber -= 1;
        }
        if(bulletNumber > 3)
        {
            bulletNumber = 1;
        }
        if (bulletNumber < 1)
        {
            bulletNumber = 3;
        }

        if(bulletNumber == 1)
        {
            currentBullet.text = "Regular";
            previousBullet.text = "Tracking";
            nextBullet.text = "Buckshot";
            playerManager.bulletType = BulletType.regular;
        }
        if (bulletNumber == 2)
        {
            currentBullet.text = "Buckshot";
            previousBullet.text = "Regular";
            nextBullet.text = "Tracking";
            playerManager.bulletType = BulletType.buckshot;
        }
        if (bulletNumber == 3)
        {
            currentBullet.text = "Tracking";
            previousBullet.text = "Buckshot";
            nextBullet.text = "Regular";
            playerManager.bulletType = BulletType.tracking;
        }
    }
    //Checks to see what the current weapon the player is currently using and seeing if the "G" or "F" were pressed and what weapon to change too.
    private void weaponCycle()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            weaponNumber += 1;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            weaponNumber -= 1;
        }
        if (weaponNumber > 3)
        {
            weaponNumber = 1;
        }
        if (weaponNumber < 1)
        {
            weaponNumber = 3;
        }

        if (weaponNumber == 1)
        {
            currentWeapon.text = "Bullet";
            previousWeapon.text = "Missile";
            nextWeapon.text = "Bomb";
            playerManager.projectileTpye = ProjectileType.bullet;
        }
        if (weaponNumber == 2)
        {
            currentWeapon.text = "Bomb";
            previousWeapon.text = "Bullet";
            nextWeapon.text = "Missile";
            playerManager.projectileTpye = ProjectileType.bomb;
        }
        else if (weaponNumber == 3)
        {
            currentWeapon.text = "Missile";
            previousWeapon.text = "Bomb";
            nextWeapon.text = "Bullet";
            playerManager.projectileTpye = ProjectileType.missile;
        }
    }
}
