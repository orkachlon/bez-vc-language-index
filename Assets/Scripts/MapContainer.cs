using System;
using System.IO;
using UnityEngine;

[Serializable]
public class MapContainer : ImageContainer {
    
    public override void LoadImage(string languageName) {
        if (!File.Exists(Directory.GetCurrentDirectory() + $"/Assets/Resources/Maps/{languageName}.png")) {
            return;
        }
        image.sprite = Resources.Load<Sprite>($"Maps/{languageName}");
    }
}