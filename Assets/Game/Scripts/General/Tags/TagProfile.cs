using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts {
    [CreateAssetMenu(fileName = "New Tag Profile")]
    public class TagProfile : ScriptableObject {
        public List<string> tags;

        /// <summary>
        /// Returns true if the tag is in the list of tags.
        /// </summary>
        /// <param name="tag">Tag to evaluate.</param>
        /// <returns></returns>
        public bool Evaluate(string tag) {
            return tags.Contains(tag);
        }
    }
}