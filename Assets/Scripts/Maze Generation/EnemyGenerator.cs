using System;
using System.Collections;
using System.Collections.Generic;
using MazeGeneration;

/// <summary>
/// System which determines which types of enemies should be spawned in the rooms
/// of the maze.
/// </summary>
public class EnemyGenerator
{
    /// <summary>
    /// Types of enemies that can be generated in the maze. Includes both standard
    /// and boss enemies.
    /// </summary>
	public enum EnemyType
	{
		spider, skeleton, zombie,
		spiderBoss, skeletonBoss, zombieBoss,
	}

	/// <summary>
    /// Given a number of "points" worth of enemies to generate, returns a list of enemy types
    /// that should be put into your room. There is no guarantee that the types returned will
    /// be equivalent to the provided points - some amount of rounding may occur.
    /// 
    /// Does not generate boss enemies. Use generateBoss instead.
	/// </summary>
	/// <param name="points">Approximate "score" worth how many enemies should be generated.
    /// For an idea of value, Skeletons and Zombies are worth 5 points, Spiders are worth 1.</param>
	/// <returns>List of enemy types to be generated in the maze.</returns>
    public static List<EnemyType> generateEnemies(int points)
	{
		List<EnemyType> enemyList = new List<EnemyType>(); 
		int mod = points%5;

		//Check to see if we have an exact number of points.
		if(mod != 0)
		{
			//Round up if we get a value greater than our threshold.
            if (true)//(mod > Maze.rnd.Next(5))
			{
				points += (5 - mod);
			}
			//Round down if below threshold.
			else
			{
				points -= mod;
			}

			
		}
		//Fill in the remaining points with random enemies.
		for(int i = 0; i < points; i += 5)
		{
            
            switch (Maze.rnd.Next(3))
			{
				case 0: 				
					enemyList.Add(EnemyType.skeleton);
					break;
				
				case 1:				
					//Add 5 spiders. :)
					enemyList.Add (EnemyType.spider);
					enemyList.Add (EnemyType.spider);
					enemyList.Add (EnemyType.spider);
					enemyList.Add (EnemyType.spider);
					enemyList.Add (EnemyType.spider);
					break;
				
				case 2:				
					enemyList.Add(EnemyType.zombie);
					break;					
			}
		}
		return enemyList;
	}
	
    /// <summary>
    /// Pick a list of enemies to generate for the boss room. May return more than just
    /// one enemy - hence the list. Probably won't, though.
    /// 
    /// Definitely don't use this for any non-boss rooms. Use generateEnemies instead.
    /// </summary>
    /// <returns>List of enemies for the boss room.</returns>
	public static List<EnemyType> generateBoss()
	{
		List<EnemyType> enemyList = new List<EnemyType>();
		
        switch(Maze.rnd.Next(3))
			{
				case 0: 				
					enemyList.Add(EnemyType.skeletonBoss);
					break;
				
				case 1:				
					enemyList.Add (EnemyType.spiderBoss);
					break;
				
				case 2:				
					enemyList.Add(EnemyType.zombieBoss);
					break;					
			}
        return enemyList;
	}
}