using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LanguageData {
    private const float LineRendererWidthMultiplier = 0.1f;
    public string name;
    public string years;
    public Dictionary<string, EChildType> Children;
    public List<string> influences;
    public string pathToMap;
    public string pathToAlphabet;
    
    public override string ToString() {
        return $"\tName: {name},\n" +
               $"\tYears: {years},\n" +
               $"\tChildren: [{string.Join(", ", Children)}],\n" +
               $"\tInfluences: [{string.Join(", ", influences)}]\n";
    }
}