using UnityEngine;
using System.Collections;

public class WeaponBowFiringScript : MonoBehaviour {

    private GameObject GraphicalArrow;
    private UnitPlayer Character;
    private Transform bulletOrigin;
    public GameObject arrowModel;

	// Use this for initialization
	void Start () 
    {
        bulletOrigin = transform.Find("BulletOrigin");
        arrowModel = (GameObject)Resources.Load("arrow");
        Character = GameObject.FindGameObjectWithTag("Player").GetComponent<UnitPlayer>();
	}

    void attachArrow()
    {
        gameObject.animation.Blend("BowstringDraw",1.0f,0.1f);
    }

    public void releaseShot()
    {   
        gameObject.animation.Play("BowstringRelease");
        gameObject.animation.Blend("Release",1.0f);
    }
    
    void fire()
    {
        ProjectileArrow p;

        GameObject arrow = (GameObject)GameObject.Instantiate(arrowModel, bulletOrigin.position,transform.parent.rotation);
        arrow.rigidbody.AddForce(arrow.transform.forward * WeaponBow.bulletSpeed);
        p = arrow.GetComponent<ProjectileArrow>();
        p.damage = Character.AttackDamage;
        
        GameObject arrow2 = (GameObject)GameObject.Instantiate(arrowModel, bulletOrigin.position,transform.parent.rotation);
        arrow2.transform.Rotate(arrow2.transform.forward,15);
        arrow2.rigidbody.AddForce(arrow2.transform.forward * WeaponBow.bulletSpeed);
        p = arrow2.GetComponent<ProjectileArrow>();
        p.damage = Character.AttackDamage;

        GameObject arrow3 = (GameObject)GameObject.Instantiate(arrowModel, bulletOrigin.position,transform.parent.rotation);
        arrow3.transform.Rotate(arrow3.transform.forward,-15);
        arrow3.rigidbody.AddForce(arrow3.transform.forward * WeaponBow.bulletSpeed);
        p = arrow3.GetComponent<ProjectileArrow>();
        p.damage = Character.AttackDamage;


    }

}
