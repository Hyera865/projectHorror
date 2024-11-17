using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    public Text LobbyNameText;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalplayerController;

    public Button StartGameButton;

    public Text ReadyButtonText;
    private NetworkManagerCustom manager;
    private NetworkManagerCustom Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = NetworkManagerCustom.singleton as NetworkManagerCustom;
        }
    }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    public void ReadyPlayer(){
        LocalplayerController.ChangeReady();
    }

    public void UpdateButton() {
        if (LocalplayerController.Ready) {
            ReadyButtonText.text = "Unready";
        } else {
            ReadyButtonText.text = "Ready";
        }
    }



    public void CheckIfAllReady(){
        bool AllReady = true;

        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if(!player.Ready){
                AllReady = false;
                break;
            }
        }

        if(AllReady){
            if(LocalplayerController.PlayerIdNumber == 1){
                StartGameButton.interactable = true;
            } else {
                StartGameButton.interactable = false;
            }
        } else {
            StartGameButton.interactable = false;
        }
    }


    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated) { CreateHostPlayerItem(); }
        if (PlayerListItems.Count < Manager.GamePlayers.Count) { CreateClientPlayerItem(); }
        if (PlayerListItems.Count > Manager.GamePlayers.Count) { RemovePlayerItem(); }
        if (PlayerListItems.Count == Manager.GamePlayers.Count) { UpdatePlayerItem(); }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalplayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.Ready = player.Ready;
            NewPlayerItemScript.SetPlayerValues();
            
            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform, false);
            NewPlayerItem.transform.localScale = Vector3.one;

            PlayerListItems.Add(NewPlayerItemScript);
        }
        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (!PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.Ready = player.Ready;
                NewPlayerItemScript.SetPlayerValues();

                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                NewPlayerItem.transform.localScale = Vector3.one;

                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach (PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if (PlayerListItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.Ready = player.Ready;
                    PlayerListItemScript.SetPlayerValues();
                    if(player == LocalplayerController){
                        UpdateButton();
                    }
                }
            }
        }
        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem playerListItem in PlayerListItems)
        {
            if (!Manager.GamePlayers.Any(b => b.ConnectionID == playerListItem.ConnectionID))
            {
                playerListItemToRemove.Add(playerListItem);
            }
        }
        if (playerListItemToRemove.Count > 0)
        {
            foreach (PlayerListItem itemToRemove in playerListItemToRemove)
            {
                GameObject ObjectToRemove = itemToRemove.gameObject;
                PlayerListItems.Remove(itemToRemove);
                Destroy(ObjectToRemove);
            }
        }
    }
}