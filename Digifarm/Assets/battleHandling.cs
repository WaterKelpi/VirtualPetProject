using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class battleHandling : MonoBehaviour {
	public enum battleState {idle,player,enemy,inactive};
	public battleState curState;
	public baseSpecies enemySpecies;
	public int[] eStatMods = new int[9];
	public SpriteRenderer eMonBattler,pMonBattler;
	public Slider eHealth,pHealth,powerGauge;
	public bool increasing;

	public float enemyTimer, playerTimer,multiplier;

	public baseMonster playerMon;

	//Stat Returns
	public int[] eStats{
		get{
			int[] tempStats = new int[9];
			for(int i = 0;i<9;i++){
				tempStats[i] = enemySpecies.baseStats[i]+eStatMods[i];
			}
			return tempStats;
		}
	}


	// Use this for initialization
	void Start () {
		for(int i = 4; i < 8;i++){
			eStatMods[i] += Mathf.Clamp(enemySpecies.baseStats[i]/25,1,999)*eStatMods[0];
		}
		eStatMods[3] = eStats[4];
	}
	
	// Update is called once per frame
	void Update () {
		switch(curState){
		case battleState.idle:
			eMonBattler.sprite = enemySpecies.speciesPortrait;
			pMonBattler.sprite = playerMon.species.speciesPortrait;
			enemyTimer += Time.deltaTime * 20;
			playerTimer += Time.deltaTime * ((float)playerMon.stats[7]/(float)eStats[7]) * 20;

			eHealth.maxValue = eStats[4];
			eHealth.value = eStatMods[3];
			pHealth.maxValue = playerMon.stats[4];
			pHealth.value = playerMon.statMods[3];


			if(enemyTimer >= 100){
				curState = battleState.enemy;
			}
			if(playerTimer >= 100){
				powerGauge.gameObject.SetActive(true);
				powerGauge.value = 0;
				curState = battleState.player;
			}
			if(eStatMods[3] <= 0){
				playerMon.statMods[2] += 100; //REPLACE WITH BETTER FORMULA
				curState = battleState.inactive;
				GameObject.Find("FarmManager").GetComponent<fmScript>().curState = fmScript.playerState.farm;
				GameObject.Find("FarmManager").GetComponent<fmScript>().MoveCamera();
				this.enabled = false;
			}
			if(playerMon.statMods[3] <= 0){
				playerMon.statMods[2] += 50; //REPLACE WITH BETTER FORMULA
				GameObject.Find("FarmManager").GetComponent<fmScript>().curState = fmScript.playerState.farm;
				GameObject.Find("FarmManager").GetComponent<fmScript>().MoveCamera();
				curState = battleState.inactive;
				this.enabled = false;
			}
			break;
		case battleState.enemy:
			enemyTimer = 0;
			playerMon.statMods[3] -=(int) Mathf.Clamp((((float)(eStats[0]*2)/5)+2)*((float)eStats[5]/(float)playerMon.stats[6]),1,9999);
			curState = battleState.idle;
			break;
		case battleState.player:
			playerTimer = 0;
			if(increasing){
				powerGauge.value += Time.deltaTime*200;
				if(powerGauge.value >= 100){
					increasing = false;
				}
			}else{
				powerGauge.value -= Time.deltaTime*200;
				if(powerGauge.value <= 0){
					increasing = true;
				}
			}
			if(Input.GetKeyDown(KeyCode.Space)){
				if(powerGauge.value < 16.5 || powerGauge.value > 83.5){
					multiplier = .75f;
				}else if(powerGauge.value < 33 || powerGauge.value > 67){
					multiplier = 1;
				}else if(powerGauge.value < 40 || powerGauge.value > 60){
					multiplier = 1.25f;
				}else if(powerGauge.value < 45 || powerGauge.value > 55){
					multiplier = 1.5f;
				}else{
					multiplier = 1.75f;
				}
				eStatMods[3] -=(int)(Mathf.Clamp((((float)(playerMon.stats[0]*2)/5)+2)*((float)playerMon.stats[5]/(float)eStats[6]),1,9999)*multiplier);
				powerGauge.gameObject.SetActive(false);
				curState = battleState.idle;
			}
			break;
		}
	}

	public void StartBattle(baseSpecies eSpecies){
		enemySpecies = eSpecies;
		for(int i = 4; i < 8;i++){
			eStatMods[i] += Mathf.Clamp(enemySpecies.baseStats[i]/25,1,999)*eStatMods[0];
		}
		eStatMods[3] = eStats[4];
		curState = battleState.idle;
	}
	public void StartBattle(baseSpecies eSpecies, int lv){
		enemySpecies = eSpecies;
		eStatMods[0] = lv;
		for(int i = 4; i < 8;i++){
			eStatMods[i] += Mathf.Clamp(enemySpecies.baseStats[i]/25,1,999)*eStatMods[0];
		}
		eStatMods[3] = eStats[4];
		curState = battleState.idle;
	}

	public void StartBattle(){
		for(int i = 4; i < 8;i++){
			eStatMods[i] += Mathf.Clamp(enemySpecies.baseStats[i]/25,1,999)*eStatMods[0];
		}
		eStatMods[3] = eStats[4];
		curState = battleState.idle;
	}
}
