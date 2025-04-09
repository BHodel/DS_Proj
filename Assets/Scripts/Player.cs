using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour {
    void Awake() {
    }
    void Start() {
        
    }

    void Update() {
        if (!IsOwner) {
            return;
        }
            
        
    }
}
