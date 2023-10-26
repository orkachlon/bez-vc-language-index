using UnityEngine;

namespace Languages.LanguageItem {
    public interface IItemContainer {
        void ToNode();
        void ToItem();
        void ToItemRelative();

        void MoveToLayer(string layerName);

        void SetPosition(Vector3 newPos);

        void SetLocalPosition(Vector3 newPos);

        Vector3 GetTopRight();
        Vector3 GetBotLeft();
    }
}