using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using Mirror;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance { get; private set; } // Tambahkan properti Instance

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private NetworkManagerCustom manager;

    public GameObject HostButton;
    public Text LobbyNameText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set instance pertama kali
            DontDestroyOnLoad(gameObject); // Pastikan tidak dihancurkan saat pindah scene
        }
        else
        {
            Destroy(gameObject); // Hancurkan jika duplikat ditemukan
        }
    }

    private void Start()
    {
        // Memeriksa apakah SteamManager telah diinisialisasi
        if (!SteamManager.Initialized)
        {
            Debug.LogError("SteamManager is not initialized.");
            return;
        }

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
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to create lobby.");
            return;
        }

        Debug.Log("Lobby created successfully.");
        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());

        string personaName = SteamFriends.GetPersonaName();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", personaName + "'S LOBBY");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        HostButton.SetActive(false);
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        LobbyNameText.gameObject.SetActive(true);
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        if (NetworkServer.active)
        {
            return;
        }

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();
    }
}
