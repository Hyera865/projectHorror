using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using System.Collections.Generic;
public class NetworkManagerCustom : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;

    public List<PlayerObjectController> GamePlayers {get;} = new List<PlayerObjectController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn){
        if (SceneManager.GetActiveScene().name == "Lobby"){
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(
                new CSteamID(SteamLobby.Instance.CurrentLobbyID), 
                GamePlayers.Count
            );

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }
}
