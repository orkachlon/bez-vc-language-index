using System;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager: MonoBehaviour {

    public enum EViewMode {
        Node,
        Item
    }

    private static List<LanguageNodesInLevel> _langNodesByLevels;

     private static EViewMode _currentViewMode = EViewMode.Node;

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
         _currentViewMode = EViewMode.Item;
     }
     
     private static void ToNode() {
         _currentViewMode = EViewMode.Node;
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

     public static EViewMode GetCurrentViewMode() {
         return _currentViewMode;
     }
}