using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts {
    public class StickerHolder : MonoBehaviour
    {
        void Start()
        {
            List<StickerState> states = StickerManager.instance.stickerStates;

            for(int i = 0; i<states.Count; i++)
            {
                if (i < transform.childCount)
                {
                    GameObject sticker = transform.GetChild(i).gameObject;
                    Sticker stickerScript = sticker.GetComponent<Sticker>();

                    switch (states[i])
                    {
                        case StickerState.Disabled:
                            stickerScript.Hide();
                            break;
                        case StickerState.Enabled:
                            stickerScript.Show(); 
                            break;
                        case StickerState.Hitless:
                            stickerScript.BeatBossHitless();
                            break;
                    }
                }
            }
        }


    }
}
