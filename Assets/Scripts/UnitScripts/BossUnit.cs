using UnityEngine;
using System.Collections;
using MazeGeneration;

/// <summary>
/// Boss unit.
/// Base class for all bosses. Spawns the enemies and defines all common behaviors of all bosses.
/// </summary>
public class BossUnit : UnitEnemy 
{
	protected bool healthAt25 = false;
	protected bool healthAt50 = false;
	protected bool healthAt75 = false;
	protected Transform spawnedEnemy;
	public static GeneralRoom bossRoom;
	public static CharacterController playercc;
    public static Transform endGamePortal;
	
	//Every 5 seconds generate a new enemy.
	protected float spawnTimer = 10.0f;
	protected float delay = 2.0f;

	//Caps the number of enemies that can be spawned.
	protected float enemyCap = 0.0f;
	private int numEnemies = 0;

    // Save position of the boss upon death so we can spawn the portal there
    private Vector3 diePosition;


	//ToDo: Once the enemies have been killed should we regenerate new ones? 

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();


		//This is a boss, so lets make it a little harder ;) 
		Health = 100;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		//Behaves the same as a normal enemy. 
		base.Update();

		//First attack: Spawn an enemy every so often.
		if (Time.time >= spawnTimer && numEnemies < enemyCap)
		{
			//Spawn an enemy every delay seconds.
			spawnedEnemy = spawnEnemy();
			spawnedEnemy.GetComponent<UnitEnemy>().boss = this; 
			spawnTimer = Time.time + delay;
			numEnemies++;
		}
	}

	//Spawns an enemy in the room. This function needs to be overriden by super classes. 
	virtual protected Transform spawnEnemy()
	{
		return null;
	}

	public void decreaseEnemyCount()
	{
		numEnemies--;
		Debug.Log(numEnemies);
        if (numEnemies == 0 && !gameObject.activeSelf)
            win();
		spawnTimer = Time.time + delay;
	}
	
	protected override void killUnit()
	{
		gameObject.SetActive(false);
		playercc.enabled = true;
        diePosition = gameObject.transform.position;
        diePosition.y = 0;  // We want the portal to be on the ground, so ignore the "up-ness"

		Destroy(healthBar.gameObject);
        if (numEnemies == 0)
            win();
		//base.killUnit();
	}

    protected virtual void win()
    {
        Debug.Log("You Win!");
        GameObject.Instantiate(endGamePortal, diePosition, Quaternion.identity);
    }
}
