using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(CapsuleCollider2D))]
public class baseMonster : MonoBehaviour {
	public enum statType{lv,xp,totalXp,hp,maxhp,atk,def,spd,hap};
	public string monName;
	public int[] statMods = new int[9];
	public bool curMon;
	public float hatchTime;
	public float warmth;
	public float healthRegen; //in seconds
	public fmScript fm;
	public baseSpecies species;

	[HideInInspector] public int healthRegenTimer = 30;

	//Stat Returns
	public int[] stats{
		get{
			int[] tempStats = new int[9];
			for(int i = 0;i<9;i++){
				tempStats[i] = species.baseStats[i]+statMods[i];
			}
			return tempStats;
		}
	}

	void Awake(){
		fm = GameObject.Find("FarmManager").GetComponent<fmScript>();
		//fm.ownedMon.Add(this);
		if(hatchTime == 0){
			hatchTime = species.hatchtime;
		}
	}


	// Use this for initialization
	void Start () {
		fm = GameObject.Find("FarmManager").GetComponent<fmScript>();
	}
	
	// Update is called once per frame
	void Update () {
		#region Egg Handling
		if(species.egg){
			warmth -= Time.deltaTime/300;
			warmth = Mathf.Clamp(warmth,1,5);
			hatchTime -= Time.deltaTime*warmth;
			if(hatchTime <= 0 && curMon && fm.inMenu){
				Evolve(Random.Range(0,species.evolutions.Length));
			}
		}
		#endregion
		#region Getting Selected
		if(Input.GetMouseButtonDown(0) && !fm.inMenu){
			RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,Camera.main.ScreenToWorldPoint(Input.mousePosition).y),Vector2.zero,0);
			SelectMon(hit);
		}
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !fm.inMenu){
			RaycastHit2D hit = Physics2D.Raycast(Input.GetTouch(0).position,Vector2.zero,0);
			SelectMon(hit);
		}
		#endregion
		#region Updating Apearances
		if(GetComponent<SpriteRenderer>().sprite != species.overworld){
			GetComponent<SpriteRenderer>().sprite = species.overworld;
		}
		#endregion
		#region Health Handling
		if(statMods[3] < stats[4]){
			healthRegen += Time.deltaTime;
		}else healthRegen = 0;
		if(healthRegen > healthRegenTimer){
			if(statMods[3] < stats[4]){
				statMods[3] ++;	
			}
			healthRegen = 0;
		}
		#endregion
		#region StatCapping
		statMods[0] = Mathf.Clamp(statMods[0],0,99);
		statMods[2] = (int)Mathf.Clamp(statMods[2],0,Mathf.Pow(99,3)*(float)species.levelCoefficient);
		statMods[3] = Mathf.Clamp(statMods[3],0,stats[4]);
		statMods[4] = Mathf.Clamp(statMods[4],0,9999);
		statMods[5] = Mathf.Clamp(statMods[5],0,999);
		statMods[5] = Mathf.Clamp(statMods[5],0,999);
		statMods[6] = Mathf.Clamp(statMods[6],0,999);
		statMods[7] = Mathf.Clamp(statMods[7],0,999);
		statMods[8] = Mathf.Clamp(statMods[8],0,100);
		#endregion

		#region Exp Handling
		if(statMods[2] >= Mathf.Pow(statMods[0],3)*(float)species.levelCoefficient){
			LevelUp();
		}
		statMods[1] = statMods[2]-(int)(Mathf.Pow(statMods[0]-1,3)*species.levelCoefficient);
		#endregion

		#region Update Seen Mon
		if(!fm.seenMon.Contains(species)){
			fm.seenMon.Add(species);
		}
		#endregion
	}

	public void Evolve(int evoNum){
		if(species.evolutions.Length != 0){
			baseSpecies newSpecies = fm.lists.speciesList.Find(x => x.speciesNumber == species.evolutions[evoNum]) ?? fm.lists.speciesList[0];
		 	if(newSpecies.CanEvolve(stats,fm)){
				for(int i = 4; i < 8;i++){
					statMods[i] -= Mathf.Clamp(species.baseStats[i]/25,1,999);
				}
				species = newSpecies;
				for(int i = 4; i < 8;i++){
					statMods[i] += Mathf.Clamp(species.baseStats[i]/25,1,999)*stats[0];
				}
				statMods[(int)statType.hp] = stats[(int)statType.maxhp];
			}
		}
	}

	public void LevelUp(){
		statMods[0]++;
		for(int i = 4; i < 8;i++){
			statMods[i] += Mathf.Clamp(species.baseStats[i]/25,1,999);
		}
		foreach(decorationObject dec in fm.deployedDec){
			for(int i = 2; i < 9;i++){
				if(i == 3){	i++;}
				statMods[i] += dec.statBoosts[i];
			}
		}
	}
	/*
	public void SetStats(GameObject parent){
		friendship = parent.GetComponent<baseMonster>().friendship;
		statMods = new int[parent.GetComponent<baseMonster>().statMods.Length];
		System.Array.Copy(parent.GetComponent<baseMonster>().statMods, statMods,parent.GetComponent<baseMonster>().statMods.Length);
	}*/

	public void TrainStat(){
		statMods[Random.Range(5,8)] += 10;
	}

	public void TrainStat(int stat){
		statMods[stat] += 10;
	}

	public void TrainStat(int stat,int amt){
		statMods[stat] += amt;
	}

	void SelectMon(RaycastHit2D hit){
		if(hit){
			if(hit.transform.gameObject == this.gameObject){
				fm.inMenu = true;
				fm.curMon = this;
				if(species.evolutions.Length > 0){
					fm.RefreshEvo(fm.evoDropdown,species.evolutions);
				}
				fm.curMonPanel.SetActive(true);
				curMon = true;
			}
		}else{
			curMon = false;
		}
	}
}
