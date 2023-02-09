using System.Collections.Generic;
using UnityEngine;

public class LanguageManager: MonoBehaviour {
    
     private static List<LanguageDataInLevel> _langDataByLevels;
     private static List<LanguageNodesInLevel> _langNodesByLevels;

     public static void PopulateData(List<LanguageDataInLevel> newLangDataByLevels) {
         _langDataByLevels = newLangDataByLevels;
     }
     
     public static void PopulateNodes(List<LanguageNodesInLevel> newLangNodesByLevels) {
         _langNodesByLevels = newLangNodesByLevels;
     }
     
     public static List<LanguageNode> GetParents(LanguageNode langNode) {
         var parentList = new List<LanguageNode>();
         foreach (var langNodesInLevel in _langNodesByLevels) {
             foreach (var (_, node) in langNodesInLevel) {
                 if (node.GetChildren().ContainsKey(langNode.GetName())) {
                     parentList.Add(node);
                 }
             }
         }
         return parentList;
     }
}