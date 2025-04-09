using UnityEngine;
using UnityEngine.UIElements;

public class Main_Menu_UI : MonoBehaviour {
    void OnEnable() {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Main menu elements
        var mainMenu = root.Q<VisualElement>("main-menu");
        var singleplayerButton = root.Q<Button>("singleplayer-button");
        var multiplayerButton = root.Q<Button>("multiplayer-button");

        // Multiplayer submenu elements
        var multiplayerMenu = root.Q<VisualElement>("multiplayer-menu");
        var hostButton = root.Q<Button>("host-button");
        var joinButton = root.Q<Button>("join-button");
        var joinCodeField = root.Q<TextField>("join-code-field");
        var joinSubmitButton = root.Q<Button>("join-submit-button");
        var backButton = root.Q<Button>("back-button");

        // Bind main menu
        singleplayerButton.clicked += () => {
        };

        multiplayerButton.clicked += () => {
            mainMenu.style.display = DisplayStyle.None;
            multiplayerMenu.style.display = DisplayStyle.Flex;
        };

        // Bind multiplayer submenu
        hostButton.clicked += () => {
            // Trigger host game logic
            Game_Manager.instance.host_game();
        };

        joinButton.clicked += () => {
            joinCodeField.style.display = DisplayStyle.Flex;
            joinSubmitButton.style.display = DisplayStyle.Flex;
        };

        joinSubmitButton.clicked += () => {
            string code = joinCodeField.value;
            Game_Manager.instance.join_game(code);
        };

        backButton.clicked += () => {
            joinCodeField.style.display = DisplayStyle.None;
            joinSubmitButton.style.display = DisplayStyle.None;
            multiplayerMenu.style.display = DisplayStyle.None;
            mainMenu.style.display = DisplayStyle.Flex;
        };
    }
}
