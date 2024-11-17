using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Collections;

public class SteamLobbyInJoin : MonoBehaviour
{
    public static SteamLobbyInJoin Instance;
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private NetworkManagerCustom manager;

    private void Start()
    {
        // Memeriksa apakah SteamManager telah diinisialisasi
        if (!SteamManager.Initialized)
        {
            Debug.LogError("SteamManager is not initialized.");
            return;
        }

        if(Instance == null) { Instance = this;}

        // Mendapatkan komponen NetworkManagerCustom
        manager = GetComponent<NetworkManagerCustom>();
        if (manager == null)
        {
            Debug.LogError("NetworkManagerCustom component is not attached to this GameObject.");
            return;
        }

        // Menyiapkan callback untuk Steam Lobby
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        // Membuat lobby baru menggunakan Steam Matchmaking
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        // Memeriksa apakah lobby berhasil dibuat
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to create lobby.");
            return;
        }

        Debug.Log("Lobby created successfully.");

        // Memulai host untuk game
        manager.StartHost();

        // Menambahkan data pada lobby Steam
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());

        // Mendapatkan nama pengguna Steam dan menetapkan nama lobby
        string personaName = SteamFriends.GetPersonaName();
        if (string.IsNullOrEmpty(personaName))
        {
            Debug.LogError("Steam Persona Name is null or empty.");
            return;
        }

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", personaName + "'S LOBBY");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request To Join Lobby");
        // Bergabung dengan lobby yang diminta
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {      
        // Jika sudah menjadi server, tidak perlu melanjutkan
        if (NetworkServer.active)
        {
            return;
        }

        // Menyiapkan alamat jaringan untuk bergabung
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        // Memulai client untuk bergabung ke lobby
        manager.StartClient();
    }
}
