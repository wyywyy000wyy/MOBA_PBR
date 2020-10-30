using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConditionBuildDefineSymbols {
        List<string> defineSymbols_ = new List<string>();
        string symbols_ = string.Empty;
        BuildTargetGroup targetGroup_ = EditorUserBuildSettings.selectedBuildTargetGroup;

        public void Prepare(BuildTargetGroup target, string desc) {
            foreach( string define in desc.Split(';') ) {
                defineSymbols_.Add(define);
            }

            targetGroup_ = target;
            symbols_ = desc;
        }

        public void Apply() {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup_, symbols_);
        }

        public void Clear() {
            defineSymbols_.Clear();
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup_, "");
        }

        public bool Include(string define) {
            return defineSymbols_.Contains(define);
        }

		static ConditionBuildDefineSymbols instance_;
		public static ConditionBuildDefineSymbols Instance() {
			if( instance_ == null )
				instance_ = new ConditionBuildDefineSymbols();

			return instance_;
		}
}
