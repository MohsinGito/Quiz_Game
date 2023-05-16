using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "SO/GameData")]
public class GameData : ScriptableObject
{
    public bool reset;
    public int totalTries;
    public int coinsEarned;
    public int rewardCoins;
    public int coinsForHint;
    public int coinsForTimeUp;
    public int coinsForTimeIncrement;
    public Catagory selectedCatagory;
    public List<Level> gameLevels;
    public bool sfxOn;
    public bool musicOn;
    public bool loadingScreenPopedUp;

    public void GiveReward() => coinsEarned += rewardCoins;
    public void TakeCoinsForHint() => coinsEarned -= coinsForHint;
    public void TakeCoinsForTimeUp() => coinsEarned -= coinsForTimeUp;
    public void TakeCoinsForTimeIncrement() => coinsEarned -= coinsForTimeIncrement;
    public float GetLevelTimeDuration() => gameLevels.Find(n => n.levelCatagory == selectedCatagory).timeDuration;
    public List<QuizWord> GetSelectedCatagoryWords() => gameLevels.Find(n => n.levelCatagory == selectedCatagory).levelQuizWords;

}

[System.Serializable]
public class Level
{
    public Catagory levelCatagory;
    public float timeDuration;
    public List<QuizWord> levelQuizWords;
    public int starsAwarded;

    public void SetStars(int _stars)
    {
        starsAwarded = _stars;
    }
}

[System.Serializable]
public class QuizWord
{
    public string word;
    [TextArea(1, 3)]
    public string hint;
}

public enum Catagory
{
    NONE,
    ANIMAL,
    FOOD,
    BODY,
    SCHOOL,
    CLOTHES,
    HOME,
    PLACES,
    HEALTH,
    SPORT,
    FAMILY
}