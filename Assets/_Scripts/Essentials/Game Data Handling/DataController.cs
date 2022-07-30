using UnityEngine;

namespace Utilities
{
    namespace Data
    {
        public class DataController : Singleton<DataController>
        {

            #region Private Readonly Attributes

            private static readonly string Music_State = "music";
            private static readonly string Sfx_State = "sfx";
            private static readonly string Coins_Count = "coins";
            private static readonly int DEFAULT_VAL = 0;

            #endregion

            #region Properties

            public int Music
            {
                get
                {
                    return GetInt(Music_State);
                }
                set
                {
                    SaveInt(Music_State, value);
                }
            }

            public int Sfx
            {
                get
                {
                    return GetInt(Sfx_State);
                }
                set
                {
                    SaveInt(Sfx_State, value);
                }
            }

            public int Coins
            {
                get
                {
                    return GetInt(Coins_Count);
                }
                set
                {
                    SaveInt(Coins_Count, value);
                }
            }

            #endregion

            #region Private Functions

            private void SaveFloat(string data, float value)
            {
                PlayerPrefs.SetFloat(data, value);
            }

            private float GetFloat(string data)
            {
                return PlayerPrefs.GetFloat(data, DEFAULT_VAL);
            }

            private void SaveInt(string data,int value)
            {
                PlayerPrefs.SetInt(data, value);
            }

            private int GetInt(string data)
            {
                return PlayerPrefs.GetInt(data, DEFAULT_VAL);
            }
            private void SaveString(string data, string value)
            {
                PlayerPrefs.SetString(data, value);
            }

            private string GetString(string data)
            {
                return PlayerPrefs.GetString(data, null); 
            }

            #endregion
        }

    }
}