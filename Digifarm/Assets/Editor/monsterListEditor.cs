using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum listType{species,decoration,none};
public enum timeOfDay{None,Day,Night};


[CustomEditor(typeof(globalLists))]
public class monsterListEditor : Editor {
	public listType typeToAdd;

	#region Species Variables
	timeOfDay timeReq;
	public string speciesName,speciesDesc; 
	public int[] baseStats = new int[9];
	public int[] statReqs = new int[9];
	public Sprite overworldSprite,speciesPortrait;
	public int evoNum,hatchTime;
	public bool egg;
	public int[] evos;
	public decorationObject decReq;
	public baseSpecies seenMon;
	public float levelCoefficient;
	#endregion
	#region Decoration Variables
	public string decName,decDescription;
	public Sprite decSprite;
	public int[] statBoosts = new int[9];
	#endregion

	public override void OnInspectorGUI(){
		globalLists myGlobalLists = (globalLists)target;
		typeToAdd = (listType)EditorGUILayout.EnumPopup("Type to Add:",typeToAdd);
		switch(typeToAdd){
		case listType.species:
			#region Add Species
			speciesName = EditorGUILayout.TextField("Species Name:",speciesName);
			speciesDesc = EditorGUILayout.TextField("Species Description:",speciesName);
			baseStats[4] = EditorGUILayout.IntField("Base Max HP Stat:",baseStats[4]);
			baseStats[5] = EditorGUILayout.IntField("Base Attack Stat:",baseStats[5]);
			baseStats[6] = EditorGUILayout.IntField("Base Defense Stat:",baseStats[6]);
			baseStats[7] = EditorGUILayout.IntField("Base Speed Stat:",baseStats[7]);

			overworldSprite = EditorGUILayout.ObjectField("Overworld Sprite",overworldSprite,typeof(Sprite),allowSceneObjects:false) as Sprite;
			speciesPortrait = EditorGUILayout.ObjectField("Species Portrait Sprite",speciesPortrait,typeof(Sprite),allowSceneObjects:false) as Sprite;
			EditorGUI.BeginChangeCheck();
			evoNum = EditorGUILayout.DelayedIntField("Number of Evolutions:",evoNum);
			if(EditorGUI.EndChangeCheck()){
				evos = new int[evoNum];
			}
			if(evos != null && evos.Length > 0){
				for(int i = 0; i < evos.Length;i++){
					evos[i] = EditorGUILayout.IntField("Evolution "+i+":",evos[i]);
				}
			}

			statReqs[4] = EditorGUILayout.IntField("Minimum Max HP:",statReqs[4]);
			statReqs[5] = EditorGUILayout.IntField("Minimum Attack:",statReqs[5]);
			statReqs[6] = EditorGUILayout.IntField("Minimum Defense:",statReqs[6]);
			statReqs[7] = EditorGUILayout.IntField("Minimum Speed:",statReqs[7]);

			decReq = EditorGUILayout.ObjectField("Required Decoration",decReq,typeof(decorationObject),allowSceneObjects:false) as decorationObject;
			timeReq = (timeOfDay)EditorGUILayout.EnumPopup("Time Requirement:",timeReq);
			levelCoefficient = EditorGUILayout.Slider("Level Coefficient:",levelCoefficient,.5f,2f);
			seenMon = EditorGUILayout.ObjectField("Required Decoration",decReq,typeof(baseSpecies),allowSceneObjects:false) as baseSpecies;

			egg = EditorGUILayout.Toggle("Is Egg:",egg);
			if(egg){
				hatchTime = EditorGUILayout.IntField("Hatch Time in Sec:",hatchTime);
			}


			if(overworldSprite != null && speciesPortrait != null){
				if(GUILayout.Button("New Species")){
					baseSpecies newSpecies = ScriptableObject.CreateInstance("baseSpecies") as baseSpecies;
					newSpecies.speciesNumber = myGlobalLists.speciesList.Count;
					newSpecies.speciesName = speciesName == "" ? "Error" : speciesName;
					newSpecies.speciesDesc = speciesDesc == "" ? "Error" : speciesDesc;
					for(int i = 0;i<9;i++){
						newSpecies.baseStats[i] = baseStats[i] < 0 ? 0: baseStats[i];
					}
					newSpecies.overworld = overworldSprite;
					newSpecies.speciesPortrait = speciesPortrait;
					newSpecies.evolutions = new int[evoNum];
					for(int i = 0;i<evoNum;i++){
						newSpecies.evolutions[i] = evos[i] < 0 ? 0: evos[i];
					}
					for(int i = 0;i<9;i++){
						newSpecies.statReqs[i] = statReqs[i] < 0 ? 0: statReqs[i];
					}
					newSpecies.decReq = decReq;
					newSpecies.timeReq = (int)timeReq;
					newSpecies.levelCoefficient = levelCoefficient;
					newSpecies.seenMon = seenMon;
					newSpecies.egg = egg;
					if(egg){
						newSpecies.hatchtime = hatchTime < 0? 0:hatchTime;
					}else newSpecies.hatchtime = 0;
					myGlobalLists.speciesList.Add(newSpecies);
					if(speciesName != ""){
						AssetDatabase.CreateAsset(newSpecies,"Assets/Monsters/" + speciesName+".asset");
					}else{
						AssetDatabase.CreateAsset(newSpecies,"Assets/Monsters/Error.asset");
					}

				}
			}
			#endregion
			break;
		case listType.decoration:
			#region Add Decoration
			decName = EditorGUILayout.TextField("Decoration Name:",decName);
			decDescription = EditorGUILayout.TextField("Decoration Description:",decDescription);
			statBoosts[2] = EditorGUILayout.IntField("XP Boost:",statBoosts[2]);
			statBoosts[4] = EditorGUILayout.IntField("Max HP Boost:",statBoosts[4]);
			statBoosts[5] = EditorGUILayout.IntField("Attack Boost:",statBoosts[5]);
			statBoosts[6] = EditorGUILayout.IntField("Defense Boost:",statBoosts[6]);
			statBoosts[7] = EditorGUILayout.IntField("Speed Boost:",statBoosts[7]);
			statBoosts[8] = EditorGUILayout.IntField("Happiness Boost:",statBoosts[8]);

			decSprite = EditorGUILayout.ObjectField("Overworld Sprite",decSprite,typeof(Sprite),allowSceneObjects:false) as Sprite;

			if(decSprite != null){
				if(GUILayout.Button("New Decoration")){
					decorationObject newDecoration = ScriptableObject.CreateInstance("decorationObject") as decorationObject;
					newDecoration.decorationNumber = myGlobalLists.decorationList.Count;
					newDecoration.decName = decName == "" ? "Error" : decName;
					newDecoration.decDescription = decDescription == "" ? "Error" : decDescription;
					for(int i = 0;i<9;i++){
						newDecoration.statBoosts[i] = statBoosts[i] < 0 ? 0: statBoosts[i];
					}
					newDecoration.decSprite = decSprite;
					if(decName != ""){
						AssetDatabase.CreateAsset(newDecoration,"Assets/Decorations/" + decName+".asset");
					}else{
						AssetDatabase.CreateAsset(newDecoration,"Assets/Decorations/Error.asset");
					}
					myGlobalLists.decorationList.Add(newDecoration);
				}
			}
			#endregion
			break;
		case listType.none:
			break;
		}

		DrawDefaultInspector();
	}
}
