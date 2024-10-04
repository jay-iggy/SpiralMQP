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
        public static StickerManager instance { get; private set; }
        public List<StickerState> stickerStates { get; private set; } = new List<StickerState>();

        public bool hitless = true;

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

        public void ShowSticker(int bossIndex)
        {
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
