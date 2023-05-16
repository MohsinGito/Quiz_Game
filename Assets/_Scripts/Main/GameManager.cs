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

    private void Awake()
    {
        if (gameData.reset)
        {
            foreach (Level level in gameData.gameLevels)
                level.SetStars(0);
        }
    }

    private void Start()
    {
        AudioController.Instance.PlayAudio(AudioName.MENU);
        gameData.loadingScreenPopedUp = PlayerPrefs.GetInt("IsLaoded") == 1 ? true: false ;

        if (gameData.loadingScreenPopedUp)
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
            PlayerPrefs.SetInt("IsLaoded", 1);

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