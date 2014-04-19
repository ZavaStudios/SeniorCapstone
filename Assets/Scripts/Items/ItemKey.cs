using System;

/// <summary>
/// A key which the player can use to open boss doors.
/// </summary>
public class ItemKey : ItemWeapon
{
	public ItemKey (string name) : base(name, tWeaponType.WeaponKey)
	{
	}
}

