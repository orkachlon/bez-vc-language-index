using System;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager: MonoBehaviour {

    public enum ViewMode {
        Node,
        Item
    }
    
     private static List<LanguageDataInLevel> _langDataByLevels;
     private static List<LanguageNodesInLevel> _langNodesByLevels;

     private static ViewMode _currentViewMode = ViewMode.Node;

     private void Start() {
         LanguageNode.OnLangNodeClicked += ToItem;
         AncestryConnection.OnConnectionClicked += ToItem;
         BackClickReceiver.OnBackArrowClicked += ToNode;
     }

     private void OnDestroy() {
         LanguageNode.OnLangNodeClicked -= ToItem;
         AncestryConnection.OnConnectionClicked -= ToItem;
         BackClickReceiver.OnBackArrowClicked -= ToNode;
     }

     private static void ToItem(LanguageNode langNode) {
         _currentViewMode = ViewMode.Item;
     }
     
     private static void ToNode() {
         _currentViewMode = ViewMode.Node;
     }

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

     public static ViewMode GetCurrentViewMode() {
         return _currentViewMode;
     }
}