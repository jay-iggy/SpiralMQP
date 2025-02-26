using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts
{
    public enum StickerState
    {
        Disabled,
        Enabled,
        Hitless
    }

    public class StickerManager : MonoBehaviour
    {
        public const int NUM_BOSSES = 13;

        public static StickerManager instance { get; private set; }
        public List<StickerState> stickerStates { get; private set; } = new List<StickerState>();

        [HideInInspector] public bool hitless = true;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                transform.parent = null;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            for(int i = 0; i<NUM_BOSSES; i++)
            {
                string bossKey = "boss" + i + "defeated";
                int wasDefeated = PlayerPrefs.GetInt(bossKey, 0);
                switch (wasDefeated)
                {
                    case 0:
                        stickerStates.Insert(i, StickerState.Disabled);
                        break;
                    case 1:
                        stickerStates.Insert(i, StickerState.Enabled);
                        break;
                    case 2:
                        stickerStates.Insert(i, StickerState.Hitless);
                        break;
                }
            }
            
        }

        public void ShowSticker(int bossIndex)
        {
            if (bossIndex < 0) {
                Debug.LogError("StickerManager::ShowSticker(): Boss index is less than 0");
            }
            for(int i = stickerStates.Count; i<bossIndex; i++) //add to list until count = bossIndex
            {
                stickerStates.Add(StickerState.Disabled);
            }

            if (hitless) {
                stickerStates.Insert(bossIndex, StickerState.Hitless);
            }
            else
            {
                stickerStates.Insert(bossIndex, StickerState.Enabled);
            }
        }
    }

}
