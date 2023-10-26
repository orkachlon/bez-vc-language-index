using System;
using UnityEngine;

namespace Languages.LanguageItem.Containers {
    [ExecuteAlways]
    [Serializable]
    public class YearsContainer : TextContainer {

        public override void ToItem() {
            gameObject.SetActive(true);
        }

        public override void ToItemRelative() {
            gameObject.SetActive(false);
        }

        public override void ToNode() {
            gameObject.SetActive(false);
        }
    }
}