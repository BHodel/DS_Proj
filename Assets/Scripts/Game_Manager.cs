using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

// goals of the game script
// 1. manage high level game state
//    eg, main menu, lobby, playing, paused, etc.
//
// 2. manage top level ui state
//    eg, opening the main menu, but not necessarily controling
//    submenus within the UI. UI logic must be seperated from game logic.
//    For example, if a button in the UI will cause the game state to
//    change, the code for handing the button press should be in
//    the game script.

public class Game_Manager : NetworkBehaviour {
    public static Game_Manager instance;

    // holds the game's primary state
    public Local_Game_State state;
    public bool             paused;
    public bool             is_singleplayer;

    [SerializeField] NetworkManager network_manager;

    // index for yourself
    int      local_player_index;
    Player[] players;

    async void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.LoadScene("Main_Menu");

            state = Local_Game_State.Main_Menu;

            await UnityServices.InitializeAsync();
            AuthenticationService.Instance.SignedIn += () => {
                Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        else {
            Destroy(gameObject);
        }
    }

    async Task<string> create_relay() {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(5);
            string join_code = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            network_manager = Instantiate(network_manager);
            var unity_transport = network_manager.GetComponent<UnityTransport>();
            unity_transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );
            network_manager.StartHost();

            return join_code;
        }
        catch (RelayServiceException e) {
            Debug.LogError(e);
        }
        return "";
    }

    async void join_relay(string join_code) {
        try {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(join_code);
            //var relayServerData = new RelayServerData(allocation, "dtls");

            network_manager = Instantiate(network_manager);
            var unity_transport = network_manager.GetComponent<UnityTransport>();
            unity_transport.SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );
            network_manager.StartClient();
        }
        catch (RelayServiceException e) {
            Debug.LogError(e);
        }
    }


    // returns the game code
    // called from main menu
    public async void host_game(bool singleplayer = false) {
        if (state != Local_Game_State.Main_Menu) {
            Debug.LogError("Cannot Host Game in Current Game State" + state.ToString());
            return;
        }

        string game_code = "";
        if (!is_singleplayer) {
            game_code = await create_relay();
        }
        Debug.Log("Hosting Game (code=')" + game_code + "'");

        is_singleplayer = singleplayer;

        await SceneManager.LoadSceneAsync("Lobby");
        state = Local_Game_State.Lobby;
    }

    public async void join_game(string join_code) {
        if (state != Local_Game_State.Main_Menu) {
            Debug.LogError("Cannot Join Game in Current Game State" + state.ToString());
            return;
        }
        Debug.Log("Joining Game (code=')" + join_code + "'");
        join_relay(join_code);

        await SceneManager.LoadSceneAsync("Lobby");
        state = Local_Game_State.Lobby;
    }

    void Update() {
    }
}

public enum Local_Game_State {
    // initialization game states
    Main_Menu,
    //Lobby_Creation,
    Lobby,
    // gameplay states
    Player_Turn,    // local player's turn
    Player_Waiting, // not local player's turn
    // gameplay resolution states
    Game_Over_Winner,
    Game_Over_Loser,
}
