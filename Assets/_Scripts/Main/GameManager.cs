using UnityEngine;
using Utilities.Audio;
using Utilities.Data;

public class GameManager : MonoBehaviour
{

    //https://www.eslkidsgames.com/guess-the-word dummy game link

    #region Public Attributes

    public GameData gameData;
    public LaodingScreen laodingScreen;
    public GameplayUIManager gameplayUiManager;
    public GameplayPopupsManager gameplayPopupsManager;

    #endregion

    #region Main Methods

    private void Start()
    {
        gameData.coinsEarned = gameData.testBuild ? 5000 : DataController.Instance.Coins;

        if(gameData.loadingScreenPopedUp)
        {
            laodingScreen.gameObject.SetActive(false);
            gameplayUiManager.Init(gameData, gameplayPopupsManager);
            gameplayPopupsManager.Init(gameData, gameplayUiManager);
            gameplayPopupsManager.CatagorySelectionPopUp(true);
        }
        else
        {
            DataController.Instance.Sfx = 1;
            DataController.Instance.Music = 1;

            laodingScreen.Init(() =>
            {
                gameplayUiManager.Init(gameData, gameplayPopupsManager);
                gameplayPopupsManager.Init(gameData, gameplayUiManager);
                gameplayPopupsManager.CatagorySelectionPopUp(true);

            }, gameData);
        }

        gameData.sfxOn = DataController.Instance.Sfx == 1 ? true : false;
        gameData.musicOn = DataController.Instance.Music == 1 ? true : false;
    }

    #endregion
}
