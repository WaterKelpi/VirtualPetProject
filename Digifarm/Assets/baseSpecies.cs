using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Species")]
public class baseSpecies : ScriptableObject {
	public enum statType{lv,xp,totalXp,hp,maxhp,atk,def,spd,hap};
	public int speciesNumber; //Make all species numbers different. It's how they'll be called later
	public string speciesName,speciesDesc;
	public int[] baseStats = new int[9]; //Never set 0,1,2,3,or 9 It won't break, but it'll do some very bizzare things down the line.
	public Sprite overworld,speciesPortrait;
	public int[] evolutions;
	//Evolution Requirements
	public int[] statReqs = new int[9]; //NEVER SET 1,or 3 again it's not really going to break, just not going to be fun as getting specific lvl xp and current hp is dumb
	public decorationObject decReq;
	public int timeReq = 0; //0 means no requirement, 1 means daytime, 2 means nighttime
	public float levelCoefficient = 1;
	public baseSpecies seenMon;


	public bool egg;
	public float hatchtime;


	public bool StatCheck(statType stat,int reqAmt,int curStat){
		return curStat >= reqAmt ? true : false;
	}

	public bool CanEvolve(int[] oldStats, fmScript fm){
		for(int i = 0; i < oldStats.Length; i++){
			if(statReqs[i] > 0){
				if(!StatCheck((statType)i,statReqs[i],oldStats[i])){return false;}
			}
		}
		if(decReq != null && !fm.deployedDec.Contains(decReq)){return false;}
		if(seenMon != null && !fm.seenMon.Contains(seenMon)){return false;}
		timeReq = Mathf.Clamp(timeReq,0,2);
		if(timeReq != 0){
			if(timeReq == 1 && !fm.isDay){
				return false;
			}
			if(timeReq == 2 && fm.isDay){
				return false;
			}
		}
		return true;
	}
}
