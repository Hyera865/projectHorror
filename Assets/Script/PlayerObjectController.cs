using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour {
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;

    private NetworkManagerCustom manager;
    
    private NetworkManagerCustom Manager{
        get {
            if(manager!=null){
                return manager;
            }
            return manager = NetworkManagerCustom.singleton as NetworkManagerCustom;
        }
    }

    private void PlayerReadyUpdate(bool oldValue, bool newValue) {
        if (isServer) {
            Ready = newValue; // Server sets the status
        }
        if (isClient) {
            LobbyController.Instance.UpdatePlayerList();
        }
    }


    [Command]
    private void CmdSetPlayerReady() {
        bool newReadyState = !Ready;
        PlayerReadyUpdate(Ready, newReadyState);
    }


   public void ChangeReady()
    {
        // Pastikan hanya pemain lokal yang memanggil
        if (!isLocalPlayer) 
        {
            Debug.Log("This is not the local player.");
            return;
        }

        // Properti 'hasAuthority' seharusnya bisa diakses langsung
        if (isLocalPlayer && isOwned)
        {
            CmdSetPlayerReady();
        }
    }


    public override void OnStartAuthority(){
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();

    }

    public override void OnStartClient(){
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient(){
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName){
        PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    public void PlayerNameUpdate(string OldValue, string NewValue){
        if(isServer){
            this.PlayerName = NewValue;
        }
        if(isClient){
            LobbyController.Instance.UpdatePlayerList();
        }
    }
}
