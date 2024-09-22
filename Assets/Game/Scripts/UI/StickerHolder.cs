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
                    if(states[i] == StickerState.Enabled)
                    {
                        transform.GetChild(i).gameObject.SetActive(true);
                    }
                    else if(states[i] == StickerState.Hitless)
                    {
                        GameObject sticker = transform.GetChild(i).gameObject;
                        sticker.SetActive(true);
                        sticker.GetComponent<Sticker>().beatBossHitless();
                    }
                }
            }
        }


    }
}
