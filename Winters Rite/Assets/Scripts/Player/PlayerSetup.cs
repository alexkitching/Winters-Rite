//////////////////////////////////////////////////////////////////////////
// File: <PlayerSetup.cs>
// Author: <Alex Kitching>
// Date Created: <8/03/17>
// Brief: <Script handling Player Setup.>
/////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof (Player))] // Requires Player Script
public class PlayerSetup : NetworkBehaviour
{
    #region Variables
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string sRemoteLayerName = "RemotePlayer";

    [SerializeField]
    string sDontDrawLayerName = "DontDraw";

    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;

    [HideInInspector]
    public GameObject playerUIInstance;
    #endregion

    void Start()
    {
        // Check if System is Controlling the Player
        if (!isLocalPlayer) // We are Remote Player
        { 
            DisableComponents(); // Disable Components
            AssignRemoteLayer(); // Assigns Remote Layer Name
        }
        else 
        {    
            // Disable Player Graphics for local player
            Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(sDontDrawLayerName));

            // Create PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            // Configure PlayerUI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();

            if (ui == null) // No UI Component
            {
                Debug.LogError("No PlayerUI component on PlayerUI prefab.");
            }

            ui.SetPlayer(GetComponent<Player>()); // Assigns player in UI

            // Runs Player Setup
            GetComponent<Player>().SetupPlayer();
        } 
    }

    public override void OnStartClient() // When Client is Started
    {
        base.OnStartClient();

        string sNetId = GetComponent<NetworkIdentity>().netId.ToString(); // Sets netID
        Player player = GetComponent<Player>();

        GameManager.RegisterPlayer(sNetId, player);
    }

    void AssignRemoteLayer() // Assigns object to remote layer
    {
        gameObject.layer = LayerMask.NameToLayer(sRemoteLayerName);
    }

    void DisableComponents() // Disables Components
    {
        for (int i = 0; i < componentsToDisable.Length; i++) // For Each component to disable
        {
            componentsToDisable[i].enabled = false;
        }
    }
    
    void OnDisable() //When player is destroyed
    {
        Destroy(playerUIInstance); // Destroys player UI

        if(isLocalPlayer) // We are the local player
        {
            GameManager.instance.SetSceneCameraActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name); // Unregisters player from dictionary
    }
}
