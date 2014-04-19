using UnityEngine;
using System.Collections;
using MazeGeneration;

/// <summary>
/// Script that allows a block to be mined.
/// </summary>
public class MineableBlock : MonoBehaviour
{
    // "Health" of a cube refers to how many hits it takes to destroy
    // the block. _maxHealth is the total health of the cube, _health is
    // the current health.
    public float _maxHealth = 4;
    public float _health = 4;
    // Holds all the data necessary to track what type the cube is, who
    // owns the cube (so we can alert them we destroyed it), etc.
	public Cube _cube;
    protected GameObject floatingOreText;
    protected FloatingXPText floatingOreTextScript;

    // Sequence of textures to apply when a cube is damaged, but not destroyed.
    public static Texture2D[] crackTextures;

    /// <summary>
    /// Apply some damage to the cube.
    /// </summary>
    /// <param name="damage">Amount of damage to deal to the block.</param>
	public void doDamage(float damage)
	{
		_health -= damage;

        if (_health <= 0)
        {
            killUnit();
        }

        else
        {
            int index = (int)(crackTextures.Length * Mathf.Max(((_maxHealth - _health) / _maxHealth), 0.0f));
            Debug.Log("((_maxHealth - _health) / _maxHealth): " + ((_maxHealth - _health) / _maxHealth));
            renderer.material.SetTexture("_DecalTex", crackTextures[index]);
        }
	}

    /// <summary>
    /// Destroys the cube, and alerts all necessary parties that it was.
    /// </summary>
	void killUnit()
	{
        if (_cube.Type != ItemBase.tOreType.Stone && _cube.Type != ItemBase.tOreType.NOT_ORE)
        {
            floatingOreText = (GameObject)GameObject.Instantiate(Resources.Load("FloatingXPText"), transform.position, Quaternion.identity);
            floatingOreTextScript = floatingOreText.GetComponent<FloatingXPText>();
            floatingOreTextScript.parent2 = transform;
            floatingOreTextScript.enabled = true;
            floatingOreTextScript.textColor = new Color(1.0f, 0.6f, 0.0f, 1.0f);
            ItemOre ore = new ItemOre(_cube.Type);
            floatingOreTextScript.displayText("+1 " + ore.ToString());
            Inventory.getInstance().inventoryAddItem(ore);
        }
		//_cube.Parent.DestroyCube(_cube);
        // Surface level parent should be a rogue room, so assume:
        renderer.material.SetTexture("_DecalTex", crackTextures[0]);
		((RogueRoom)_cube.Parent).DestroyMineableBlock(this);
        
	    //Destroy (gameObject);
    }
}
