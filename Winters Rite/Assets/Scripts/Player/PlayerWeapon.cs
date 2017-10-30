//////////////////////////////////////////////////////////////////////////
// File: <PlayerWeapon.cs>
// Author: <Alex Kitching>
// Date Created: <8/03/17>
// Brief: <Class Holding Data for Player Weapons.>
/////////////////////////////////////////////////////////////////////////

[System.Serializable]
public class PlayerWeapon
{
	public int ID;

	public bool Obtained = false;
	public bool Equipped = false;

	public string name;
	public bool Ranged;

	public int damage;
	public float range;
	public int level;

	public float fireRate;
}
