using System;
using System.Collections.Generic;
using UnityEngine;
using MazeGeneration;

/// <summary>
/// System that handles allocation of Unity cubes in a resource pool, allowing
/// reuse and avoiding the expensive costs of instantiating objects at runtime.
/// </summary>
public class CubeAllocator : MonoBehaviour
{
    // We need to spawn some number of cubes by default. Let's go with this many.
	private const int DEFAULT_SIZE = 5000;
    // In the off chance that we ran out of cubes, we still need to allocate to the user.
    // Let's spawn this many more whenever we run out.
	private const int REALLOC_SIZE = 50;
    // This is where we hide the cubes we're not using right now.
	private Vector3 DEFAULT_POSITION = new Vector3(0, -10, 0);

	// Unity specified vars
	public MineableBlock cubeTransform;
	public Material[] cubeMaterials;

	// Personal state
	private int cubeCount;
	private Stack<MineableBlock> blocks;

    // Typically we would do this stuff in the Start call, but in this case
    // we need to make sure this happens before that.
	public void Awake()
	{
		blocks = new Stack<MineableBlock>(DEFAULT_SIZE);
		for (int i = 0; i < DEFAULT_SIZE; i++)
		{
			MineableBlock ct = (MineableBlock)Instantiate(cubeTransform, DEFAULT_POSITION, Quaternion.identity);
			blocks.Push(ct);
		}
		cubeCount = DEFAULT_SIZE;
	}

    /// <summary>
    /// Gives you an allocated cube for you to do with as you wish. Please give it
    /// back when you're done!
    /// 
    /// We take in the Cube data so we can put that into the MineableBlock for you,
    /// as well as properly set the texture so the cube looks as intended when you
    /// get it.
    /// </summary>
    /// <param name="cube">Cube data to stick into your MineableBlock.</param>
    /// <returns>Freshly allocated cube that's yours to do with as you wish.</returns>
	public MineableBlock GetCube(Cube cube)
	{
		if (blocks.Count == 0)
			AllocCubes();

		MineableBlock ct = blocks.Pop();
		ct._cube = cube;
		switch (cube.Type)
		{
		case ItemBase.tOreType.Bone:
			ct.renderer.material = cubeMaterials[0];
			ct._health = 4;
			break;
		case ItemBase.tOreType.Copper:
			ct.renderer.material = cubeMaterials[1];
			ct._health = 4;
			break;
		case ItemBase.tOreType.Dragon:
			ct.renderer.material = cubeMaterials[2];
			ct._health = 4;
			break;
		case ItemBase.tOreType.Ethereal:
			ct.renderer.material = cubeMaterials[3];
			ct._health = 4;
			break;
		case ItemBase.tOreType.Iron:
			ct.renderer.material = cubeMaterials[4];
			ct._health = 4;
			break;
		case ItemBase.tOreType.Mithril:
			ct.renderer.material = cubeMaterials[5];
			ct._health = 4;
			break;
		case ItemBase.tOreType.Steel:
			ct.renderer.material = cubeMaterials[6];
			ct._health = 4;
			break;
		case ItemBase.tOreType.Stone:
			ct.renderer.material = cubeMaterials[7];
			ct._health = 1;
			break;
		default:
			break;
		}

		return ct;
	}

    /// <summary>
    /// Returns a cube to the resource pool so others can make use of it later.
    /// Thanks for your patronage!
    /// </summary>
    /// <param name="ct">Instantiated cube you're returning to the pool.</param>
	public void ReturnCube(MineableBlock ct)
	{
		blocks.Push(ct);
		ct.transform.position = DEFAULT_POSITION;
	}

	/// <summary>
	/// If we ever run out of cubes, we need to get more or the game will fail.
	/// This function handles and documents that, so we can tweak DEFAULT_SIZE
	/// at a later date to accomodate this.
	/// </summary>
	private void AllocCubes()
	{
		cubeCount += REALLOC_SIZE;
		for (int i = 0; i < REALLOC_SIZE; i++)
		{
			MineableBlock ct = (MineableBlock)Instantiate(cubeTransform, DEFAULT_POSITION, Quaternion.identity);
			blocks.Push(ct);
		}
		Debug.Log("Cubes needed to be instantiated. Now at: " + cubeCount);
	}
}

