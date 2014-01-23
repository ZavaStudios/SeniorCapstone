using System;
using System.Collections;

public class GenerateEnemies
{
	static ArrayList enemyList = new ArrayList();
	static Random rand = new Random();

	public enum enemy
	{
		spider, skeleton, zombie
	}

	//Generates enemies at random based on the number of points that are allocated to that room.
	// Returns an ArrayList of type enemy with the enemies that should be contained in any given room.
	public static ArrayList generateEnemies(int points)
	{
		//Fuck you Victor. Here's a comment for you! :) PS Angel Beats is the best! :DDDD 
		int mod = points%5;
		int dif;

		//Check to see if we have an exact number of points.
		if(mod != 0)
		{
			//Probability to round up or down is modified based on how close it is to 5.
			dif = 5 - mod;

			//50% chance to fill the remaining points with spiders.
			if(rand.Next(2) == 0)
			{
				//Make it even point values and add spiders in. 
				for(; points%5 != 0; points--)
					enemyList.Add(enemy.spider);
			}
			else
			{
				//Round up if we get a value greater than our threshold.
				if(dif < rand.Next(6))
				{
					points += dif;
				}
				//Round down if below threshold.
				else
				{
					points -= mod;
				}
			}
			
		}
		//Fill in the remaining points with random enemies.
		for(int i = 0; i < points; i += 5)
		{
			switch(rand.Next(3))
			{
				case 0: 				
					enemyList.Add(enemy.skeleton);
					break;
				
				case 1:				
					//Add 5 spiders. :)
					enemyList.Add (enemy.spider);
					enemyList.Add (enemy.spider);
					enemyList.Add (enemy.spider);
					enemyList.Add (enemy.spider);
					enemyList.Add (enemy.spider);
					break;
				
				case 2:				
					enemyList.Add(enemy.zombie);
					break;					
			}
		}
		return enemyList;
	}
}