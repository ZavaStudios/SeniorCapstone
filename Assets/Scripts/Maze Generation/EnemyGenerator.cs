using System;
using System.Collections;
using System.Collections.Generic;
using MazeGeneration;

public class EnemyGenerator
{
	public enum EnemyType
	{
		spider, skeleton, zombie,
		spiderBoss, skeletonBoss, zombieBoss,
	}

	//Generates enemies at random based on the number of points that are allocated to that room.
	// Returns an ArrayList of type enemy with the enemies that should be contained in any given room.
	public static List<EnemyType> generateEnemies(int points)
	{
		List<EnemyType> enemyList = new List<EnemyType>();		//Fuck you Victor. Here's a comment for you! :) PS Angel Beats is the best! :DDDD 
		int mod = points%5;

		//Check to see if we have an exact number of points.
		if(mod != 0)
		{
			//Round up if we get a value greater than our threshold.
            if (mod > Maze.rnd.Next(5))
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
	
	public static List<EnemyType> generateBoss()
	{
		List<EnemyType> enemyList = new List<EnemyType>();
		switch(rand.Next(3))
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