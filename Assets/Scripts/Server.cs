using Unity.Netcode;
using UnityEngine;

public class Server : NetworkBehaviour {
    public static Server instance;

    Server_Game_State state;

    void Awake() {
        if (instance == null && IsServer) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Update() {
    }
}

public enum Server_Game_State {
    Initialize,      
    Lobby,           // pregame lobby
    Setup,           // game setup
    Player_Turn,     // player turn in progress
    Player_End_Turn, // end current player's turn
    Ended,           // game ended
}
