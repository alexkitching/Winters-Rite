//////////////////////////////////////////////////////////////////////////
// File: <Player.cs>
// Author: <Alex Kitching>
// Date Created: <8/03/17>
// Brief: <Script handling Room List Item Functionality.>
/////////////////////////////////////////////////////////////////////////

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool bisDead = false;
    public bool isDead
    {
        get { return bisDead; }
        protected set { bisDead = value; }
    }

    #region Variables

    [SerializeField]
    private float fMaxHealth = 100f;
    [SerializeField]
    private float fMaxHunger = 100f;
    [SerializeField]
    private float fMaxThirst = 100f;

    [SerializeField]
    private float fThirstRate = 0.7f;
    [SerializeField]
    private float fHungerRate = 0.4f;

    [SerializeField]
    private int iStarvationRate = 1;
    [SerializeField]
    private int iDehydrationRate = 3;

    [SerializeField]
    private float fThirstIncreaseRateMultiplier = 3f;

    [SerializeField]
    private float fThirstSprintMultiplier = 2f;


    [SerializeField]

    [SyncVar]
    private float fCurrentHealth;
    [SyncVar]
    private float fCurrentHunger;
    [SyncVar]
    private float fCurrentThirst;

    public float GetCurrentHealthBarAmount()
    {
        return fCurrentHealth / fMaxHealth;
    }

    public float GetCurrentHealth()
    {
        return fCurrentHealth;
    }

    public float GetCurrentHungerBarAmount()
    {
        return fCurrentHunger / fMaxHunger;
    }

    public float GetCurrentHunger()
    {
        return fCurrentHunger;
    }

    public float GetCurrentThirstBarAmount()
    {
        return fCurrentThirst / fMaxThirst;
    }

    public float GetCurrentThirst()
    {
        return fCurrentThirst;
    }

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] bWasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    private bool bFirstSetup = true;

    #endregion

    public void SetupPlayer()
    {
        if(isLocalPlayer) // We are local player
        {
            //Switch Cameras from scene to player
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadCastNewPlayerSetup(); // Calls New Player Setup on network
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients(); // Sets up Player on all clients
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (bFirstSetup) // First Setup
        {
            bWasEnabled = new bool[disableOnDeath.Length];

            for (int i = 0; i < bWasEnabled.Length; i++) // For Each Item not enabled
            {
                bWasEnabled[i] = disableOnDeath[i].enabled;  // Enable
            }

            //No Longer First Setup
            bFirstSetup = false;
        }

        // Set Player Defaults
        SetDefaults();
        // Starting in 1 Second, deplete Player Stats every second
        InvokeRepeating("RpcDecreaseHungerByRate", 1, 1);
        InvokeRepeating("RpcDecreaseThirstByRate", 1, 1);
    }

    [ClientRpc]
    public void RpcTakeDamage(int amount) //Deals Damage to player
    {
        if (isDead) // Exit if already dead
            return;

        fCurrentHealth -= amount; // Take Damage

        Debug.Log(transform.name + " now has " + fCurrentHealth + " health.");

        if (fCurrentHealth <= 0) // If Less than 0 Health
        {
            Die(); //Player Dies
        }
    }

    [ClientRpc]
    private void RpcDecreaseHungerByRate()
    {
        

        if (fCurrentHunger <= 0) // Are we starving?
        { // Yes
            fCurrentHunger = 0;
            RpcTakeDamage(iStarvationRate);
        }
        else
        { // No
            // Decrease Hunger of Player
            fCurrentHunger -= fHungerRate;
        }
    }

    [ClientRpc]
    private void RpcDecreaseThirstByRate()
    {
        
        if (fCurrentThirst <= 0) // Are we starving?
        { // Yes
            fCurrentThirst = 0;
            RpcTakeDamage(iDehydrationRate);
        }
        else
        { // No
            // Decrease Thirst of Player
            fCurrentThirst -= fThirstRate;
        }
    }

    [ClientRpc]
    private void RpcIncreaseThirstByRate()
    {
        // Increase Thirst of player
        fCurrentThirst += fThirstRate * fThirstIncreaseRateMultiplier;
    }

    [Command]
    public void CmdPlayerHungerIncrease(GameObject a_foodObject, string a_sPlayerID, string a_sFoodName, int a_iHungerIncreaseAmount)
    {
        Player player = GameManager.GetPlayer(a_sPlayerID);
        if(player.GetCurrentHunger() + a_iHungerIncreaseAmount <= 100)
        {
            player.RpcIncreaseHunger(a_iHungerIncreaseAmount);
            Debug.Log(a_sPlayerID + " ate " + a_sFoodName + " and gained " + a_iHungerIncreaseAmount + " hunger.");
            a_foodObject.GetComponent<FoodController>().DestroyFood();
        }
    }

    [Command]
    public void CmdPlayerStartThirstIncrease(string a_sPlayerID)
    {
        Player player = GameManager.GetPlayer(a_sPlayerID);
        if(player.GetCurrentThirst() <= 100)
        {
            // Starting in 1 Second, deplete Player Stats every second
            CancelInvoke("RpcDecreaseThirstByRate");
            InvokeRepeating("RpcIncreaseThirstByRate", 1, 1);
        }
    }

    [Command]
    public void CmdPlayerStopThirstIncrease(string a_sPlayerID)
    {
        Player player = GameManager.GetPlayer(a_sPlayerID);

        // We are increasing thirst
        if(IsInvoking("RpcIncreaseThirstByRate"))
        {
            // Stop Increasing Thirst
            CancelInvoke("RpcIncreaseThirstByRate");
            InvokeRepeating("RpcDecreaseThirstByRate", 1, 1);
        }
    }

    [ClientRpc]
    public void RpcIncreaseHunger(float a_fAmount)
    {
        // Increase Hunger by Amount
        fCurrentHunger += a_fAmount;
    }

    [ClientRpc]
    public void RpcIncreaseThirst(float a_fAmount)
    {
        // Increase Thirst by Amount
        fCurrentThirst += a_fAmount;
    }

    public void MultiplyThirstRate()
    {
        // Increase the rate at which thirst decreases
        fThirstRate = fThirstRate * fThirstSprintMultiplier;
    }

    public void ResetThirstRate()
    {
        // Reset the rate at which thirst decrease
        fThirstRate = fThirstRate / fThirstSprintMultiplier;
    }

    private void Die() // Player is dead
    {
        isDead = true;

        // Disable Components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false; 
        }

        // Disable GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        // Disable the Collider
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        //Switch Cameras to Scene Camera
        if(isLocalPlayer) // We are the local player
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        // Cancel Decrease Stat Invoke
        CancelInvoke("RpcDecreaseHungerByRate");
        
        // If We died whilst decreasing thirst
        if(IsInvoking("RpcDecreaseThirstByRate"))
        {
            CancelInvoke("RpcDecreaseThirstByRate");
        }

        // If We died whilst increasing thirst
        if(IsInvoking("RpcIncreaseThirstByRate")) 
        {
            CancelInvoke("RpcIncreaseThirstByRate");
        }

        // Prints to Console
        Debug.Log(transform.name + " is dead!"); 

        // Call Respawn Method
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.roundSettings.respawnDelay); // Wait for Respawn Delay

        // Moves Player to Respawn Point
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f); // Wait for player to move to spawn position

        // Resets Player Defaults
        SetupPlayer();

        // Prints to Console
        Debug.Log(transform.name + " Respawned"); 
    }

    public void SetDefaults()
    {
        // Default Not Dead
        isDead = false;

        // Reset Stats
        fCurrentHealth = fMaxHealth;
        fCurrentHunger = fMaxHunger;
        fCurrentThirst = fMaxThirst;

        // Re-enable Components
        for (int i = 0; i < disableOnDeath.Length; i++) 
        {
            disableOnDeath[i].enabled = bWasEnabled[i]; 
        }

        // Re-enable GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        // Enable the collider
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        // Starting in 1 Second, deplete Player Stats every second
		InvokeRepeating("RpcDecreaseHungerByRate", 1, 1);
		InvokeRepeating("RpcDecreaseThirstByRate", 1, 1);
    }
}
