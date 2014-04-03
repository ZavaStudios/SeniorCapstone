﻿using UnityEngine;
using System.Collections;

public class WeaponBowFiringScript : MonoBehaviour {

    private GameObject GraphicalArrow;
    private UnitPlayer Character;
    private Transform bulletOrigin;
    public WeaponBow weaponBow;

	// Use this for initialization
	void Start () 
    {
        bulletOrigin = transform.Find("BulletOrigin");

        GraphicalArrow = (GameObject)GameObject.Instantiate(Resources.Load("CrappyArrow"), bulletOrigin.position,bulletOrigin.rotation);
        GraphicalArrow.transform.parent = bulletOrigin;
        GraphicalArrow.rigidbody.useGravity = false;

        GraphicalArrow.SetActive(false);

        Character = GameObject.FindGameObjectWithTag("Player").GetComponent<UnitPlayer>();
	}

    void attachArrow()
    {
        GraphicalArrow.SetActive(true);
    }

    public void releaseShot()
    {   
        gameObject.animation.PlayQueued("CrappyBowRelease",QueueMode.CompleteOthers);
    }
    
    void fire()
    {
        GraphicalArrow.SetActive(false);
        
        ProjectileArrow p;

        GameObject arrow = (GameObject)GameObject.Instantiate(Resources.Load("CrappyArrow"), bulletOrigin.position,transform.parent.rotation);
        arrow.rigidbody.AddForce(arrow.transform.forward * weaponBow.bulletSpeed);
        p = arrow.GetComponent<ProjectileArrow>();
        p.damage = Character.AttackDamage;
        
        GameObject arrow2 = (GameObject)GameObject.Instantiate(Resources.Load("CrappyArrow"), bulletOrigin.position,transform.parent.rotation);
        arrow2.transform.Rotate(arrow2.transform.forward,15);
        arrow2.rigidbody.AddForce(arrow2.transform.forward * weaponBow.bulletSpeed);
        p = arrow2.GetComponent<ProjectileArrow>();
        p.damage = Character.AttackDamage;

        GameObject arrow3 = (GameObject)GameObject.Instantiate(Resources.Load("CrappyArrow"), bulletOrigin.position,transform.parent.rotation);
        arrow3.transform.Rotate(arrow3.transform.forward,-15);
        arrow3.rigidbody.AddForce(arrow3.transform.forward * weaponBow.bulletSpeed);
        p = arrow3.GetComponent<ProjectileArrow>();
        p.damage = Character.AttackDamage;


    }

}
