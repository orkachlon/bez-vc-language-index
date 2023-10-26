using Languages.Graph;
using System;
using System.Collections.Generic;
using Utils;

namespace Languages {
    [Serializable]
    public class LanguageData {
        public string name;
        public string phonetic;
        public string years;
        public ChildToTypeDictionary childToType;
        public List<string> influences;
        public string alphabet;
        public string description;
        public string picTooltip;

        public LanguageData() {
            childToType = new ChildToTypeDictionary();
        }

        public override string ToString() {
            return $"\tName: {name},\n" +
                   $"\tYears: {years},\n" +
                   $"\tChildren: [{string.Join(", ", childToType)}],\n" +
                   $"\tInfluences: [{string.Join(", ", influences)}]\n";
        }

        [Serializable]
        public class ChildToTypeDictionary : SerializableDictionary<string, EChildType> {

        }
    }
}