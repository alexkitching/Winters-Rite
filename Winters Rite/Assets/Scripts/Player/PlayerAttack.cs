//////////////////////////////////////////////////////////////////////////
// File: <PlayerAttack.cs>
// Author: <Alex Kitching>
// Date Created: <8/03/17>
// Brief: <Script handling Player Attack Functionality.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof(WeaponManager))] // Requires Weapon Manager
public class PlayerAttack : NetworkBehaviour
{
    #region Variables
    private const string PLAYER_TAG = "Player";
	private const string PRIMARYANIM = "PrimaryAttack";
	private const string SECONDARYANIM = "PrimaryAttack";

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	private PlayerWeapon currentWeapon;
	private WeaponManager weaponManager;

    #endregion

    void Start()
	{
		if (cam == null) // No Camera 
		{
			Debug.LogError("PlayerAttack: No Camera Referenced!");
			this.enabled = false;
		}

		weaponManager = GetComponent<WeaponManager>();
	}

	void Update()
	{
		if (PauseMenu.bIsOn) // Pause Menu is on, exit
			return;

		if (Input.GetButtonDown("Fire1")) // Attack on Left Click
		{
            currentWeapon = weaponManager.GetCurrentWeapon(); // Sets Current Weapon

            Attack(); // Attacks
		}
	}

	// Is Called on Server when a player attacks
	[Command]
	void CmdOnAttack()
	{
		RpcPlayPrimaryAnimation();
	}

	// Is Called on all clients when we need to perform an attack animation
	[ClientRpc]
	void RpcPlayPrimaryAnimation()
	{
		//Insert Play Animation Here
	}

	[Client]
	void Attack()
	{
		if(!isLocalPlayer) // No Local Player, exit
		{
			return;
		}

		// Call OnAttacking Method on Server
		CmdOnAttack();

		RaycastHit hit;
		if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask)) // Raycast Hit
		{
			if(hit.collider.tag == PLAYER_TAG) // Hit Player
			{
				CmdPlayerHit(transform.name,hit.collider.name, currentWeapon.damage); // Call Server Palyer Hit
			}
		}
	}

	[Command]
	void CmdPlayerHit(string a_sAttackingPlayerID, string a_sTargetPlayerID, int a_iDamage)
    {
		Player targetPlayer = GameManager.GetPlayer(a_sTargetPlayerID); // Gets Target Player

        targetPlayer.RpcTakeDamage(a_iDamage); // Target Player Takes Damage

        Debug.Log(a_sTargetPlayerID + " has been shot by " + a_sAttackingPlayerID +
                   " taking " + a_iDamage + " damage and has " +
                   (int)targetPlayer.GetCurrentHealth() + " health remaining.");
    }
}
