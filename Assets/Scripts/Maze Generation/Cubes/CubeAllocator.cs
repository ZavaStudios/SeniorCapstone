using System;
using System.Collections.Generic;
using UnityEngine;
using MazeGeneration;

public class CubeAllocator : MonoBehaviour
{
	private const int DEFAULT_SIZE = 5000;
	private const int REALLOC_SIZE = 50;
	private Vector3 DEFAULT_POSITION = new Vector3(0, -10, 0);	// This should be safely out of view

	// Unity specified vars
	public MineableBlock cubeTransform;
	public Material[] cubeMaterials;

	// Personal state
	private int cubeCount;

	public void Start()
	{
		for (int i = 0; i < DEFAULT_SIZE; i++)
		{
			MineableBlock ct = (MineableBlock)Instantiate(cubeTransform, DEFAULT_POSITION, Quaternion.identity);
			ct.transform.parent = gameObject.transform;
		}
		cubeCount = DEFAULT_SIZE;
	}

	public void Update()
	{
		// nah
	}

	public MineableBlock GetCube(Cube cube)
	{
		if (gameObject.transform.childCount == 0)
			AllocCubes();

		MineableBlock ct = gameObject.transform.GetChild(0).GetComponent<MineableBlock>();
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

		ct.transform.parent = null;
		return ct;
	}

	public void ReturnCube(MineableBlock ct)
	{
		ct.transform.parent = gameObject.transform;
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
			ct.transform.parent = gameObject.transform;
		}
		Debug.Log("Cubes needed to be instantiated. Now at: " + cubeCount);
	}
}

