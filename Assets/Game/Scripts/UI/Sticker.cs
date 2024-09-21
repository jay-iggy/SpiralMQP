using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class Sticker : MonoBehaviour
    {

        private Image img;
        private Color hitlessColor = new Color(.85f, .65f, .2f);

        private void Awake()
        {
            img = GetComponent<Image>();
        }

        public void beatBossHitless()
        {
            img.color = hitlessColor;
        }
    }

}
