using System;
using System.IO;
using UnityEngine;

namespace Languages.LanguageItem.Containers {
    [Serializable]
    public class MapContainer : ImageContainer {

        public override void LoadImage(string languageName, bool alphabetExists, string picToolTip = "") {
            if (!File.Exists(Directory.GetCurrentDirectory() + $"/Assets/Resources/Maps/{languageName}.png")) {
                return;
            }
            image.sprite = Resources.Load<Sprite>($"Maps/{languageName}");
        }
    }
}