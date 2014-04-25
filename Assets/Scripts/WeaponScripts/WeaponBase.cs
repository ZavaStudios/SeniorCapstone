using UnityEngine;
using System.Collections;

public class WeaponBase : MonoBehaviour
{
	//The name of the weapon base
	public virtual string strWeaponName {get{return "Default";}}
	public virtual string strWeaponType {get{return "Default";}}

	public Unit Character;
	
	protected RaycastHit rayHit;
		
	public float attackRange = 0f;
    public float specialRange = 0.0f;
    public float specialAttackSpeedRelative = 1.0f;
    public float specialAttackDamageRelative = 1.0f;

	private float nextAttack = 0.0f;
    protected float nextSpecialAttack = 0.0f;

    //Action sounds
    protected AudioSource attackSound;

	// Use this for initialization
	virtual protected void Start ()
	{
		Character = GetComponent<Unit>();
	}
	
	// Update is called once per frame
	virtual protected void Update ()
	{
		
	}

    //attack routine checks attack timing information and allows the attack to occur at specified rate
    virtual public void attack()
    {
        if (Time.time >= nextAttack)
	    {
            if (attackSound != null && !attackSound.isPlaying)
                attackSound.Play();

	        nextAttack = Time.time + Character.AttackDelay;
       		attackRoutine(Character.getEyePosition(),Character.getLookDirection());
                
			if(Character is UnitPlayer)
				Character.playAttackAnimation();
	    }
    }

	virtual protected void attackRoutine(Vector3 startPos, Vector3 faceDir)
	{

	}
	
	virtual protected void specialAttackRoutine ()
	{
	}

    virtual public void attackSpecial ()
    {
        if (Time.time >= nextSpecialAttack)
	    {
	        nextSpecialAttack = Time.time + Character.AttackDelay * specialAttackSpeedRelative;
       		specialAttackRoutine();
	    }

    }
}
