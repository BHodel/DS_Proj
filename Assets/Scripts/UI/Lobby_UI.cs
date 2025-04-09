using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class LobbyUI : MonoBehaviour {
    UIDocument uiDoc;
    Label joinCodeLabel;
    ScrollView playerList;
    Button startButton;

    void OnEnable() {
        uiDoc = GetComponent<UIDocument>();

        joinCodeLabel = uiDoc.rootVisualElement.Q<Label>("joinCodeLabel");
        playerList     = uiDoc.rootVisualElement.Q<ScrollView>("playerList");
        startButton    = uiDoc.rootVisualElement.Q<Button>("startButton");

        startButton.visible = NetworkManager.Singleton.IsHost;
        startButton.clicked += OnStartGame;

        UpdateJoinCode("N/A");
        RefreshPlayerList();
    }

    public void UpdateJoinCode(string code) {
        joinCodeLabel.text = $"Join Code: {code}";
    }

    public void RefreshPlayerList() {
        playerList.Clear();
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList) {
            var playerName = new Label($"Player {client.ClientId}");
            playerList.Add(playerName);
        }
    }

    void OnStartGame() {
        if (NetworkManager.Singleton.IsHost) {
            // trigger game start logic
            Debug.Log("Starting game...");
        }
    }
}
