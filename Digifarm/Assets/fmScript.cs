using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fmScript : MonoBehaviour {
	public enum playerState {farm,training,battle};
	public Vector2[] camPos,monPos;
	public playerState curState;
	public bool inMenu;
	public List<decorationObject> deployableDec,deployedDec;
	public List<baseMonster> ownedMon;
	public List<baseSpecies> seenMon;
	public baseMonster curMon;
	public decSpotHandling curDecSpot;
	public GameObject curMonPanel,decSpotPanel,evoPanel,battlePanel;
	public Color day,night;
	public bool isDay;
	string nightStart = "6:00:00 PM";
	string dayStart = "6:00:00 AM";
	public globalLists lists;
	public string lastLogin;
	public Button battleButton;
	public int stamina;
	public float stamRegen;
	[HideInInspector] public int stamRegenTimer = 1800;
	[HideInInspector] public int maxStamina = 3;

	public GameObject monPrefab;

	public GameObject projectile;
	public int serializedInts = 20;
	[HideInInspector] public Dropdown decDropdown,evoDropdown;
	[HideInInspector] public List<string> options;

	private float trainingTimer;

	void Awake(){
		DontDestroyOnLoad(this.gameObject);
		Screen.orientation = ScreenOrientation.Portrait;
		decDropdown = decSpotPanel.transform.Find("decList").GetComponent<Dropdown>();
		evoDropdown = evoPanel.transform.Find("evoDropdown").GetComponent<Dropdown>();
		ownedMon = new List<baseMonster>();
		if(deployableDec != null){
			RefreshDec(decDropdown,deployableDec);
		}
		saveLoadManager.LoadPlayer(this);
		MoveCamera();
	}

	// Use this for initialization
	void Start () {
		closeMenus();
	}

	void OnApplicationQuit(){
		saveLoadManager.SavePlayer(this);
	}
	
	// Update is called once per frame
	void Update () {
		#region Stamina Handling
		if(stamina < 3){
			stamRegen += Time.deltaTime;
		}else stamRegen = 0;
		if(stamRegen > stamRegenTimer){
			if(stamina < maxStamina){
				stamina ++;	
			}
			stamRegen = 0;
		}
		stamina = Mathf.Clamp(stamina,0,maxStamina);
		#endregion
		if(Input.GetKeyDown(KeyCode.Escape)){
			if(inMenu && !evoPanel.activeSelf){
				closeMenus();
			}else if(evoPanel.activeSelf){
				CloseEvo();
			}
			if(curState != playerState.farm){
				curMon.TrainStat();
				curMon.statMods[(int)baseMonster.statType.totalXp] += Random.Range(50,100);
				curState = playerState.farm;
				MoveCamera();
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha5)){
			ReleaseMonster();
		}

		if(Input.GetKeyDown(KeyCode.Alpha6)){
			AddMonster();
		}
		#region Display Current Monster
		if(curMonPanel.activeSelf){
			if(curMon != null){
				InputField nameInput = GameObject.Find("nameInput").GetComponent<InputField>();
				Text monName = GameObject.Find("monName").GetComponent<Text>();
				if(nameInput.isFocused){
					monName.text = nameInput.text+"|";
				}else{
					nameInput.text = curMon.monName;
					monName.text = curMon.monName;
				}
				Text specName = GameObject.Find("speciesName").GetComponent<Text>();
				specName.text = "the " + curMon.species.speciesName;
				Text hpDisplay = GameObject.Find("hpDisplay").GetComponent<Text>();
				hpDisplay.text = curMon.stats[(int)baseMonster.statType.hp]+ "/" + curMon.stats[(int)baseMonster.statType.maxhp];
				Slider hpSlider = GameObject.Find("hpSlider").GetComponent<Slider>();
				hpSlider.maxValue = curMon.stats[(int)baseMonster.statType.maxhp];
				hpSlider.value = curMon.stats[(int)baseMonster.statType.hp];
				Text xpDisplay = GameObject.Find("xpDisplay").GetComponent<Text>();
				xpDisplay.text = curMon.statMods[2] - (int)(Mathf.Pow(curMon.statMods[0]-1,3)*curMon.species.levelCoefficient)+ "/" + ((int)(Mathf.Pow(curMon.statMods[0],3)*curMon.species.levelCoefficient)-(int)(Mathf.Pow(curMon.statMods[0]-1,3)*curMon.species.levelCoefficient));
				Slider xpSlider = GameObject.Find("xpSlider").GetComponent<Slider>();
				xpSlider.maxValue = Mathf.Pow(curMon.statMods[0],3)*curMon.species.levelCoefficient-Mathf.Pow(curMon.statMods[0]-1,3)*curMon.species.levelCoefficient;;
				xpSlider.value = curMon.statMods[2] - (int)(Mathf.Pow(curMon.statMods[0]-1,3)*curMon.species.levelCoefficient);
				#region Stat Display
				Text stats = curMonPanel.transform.Find("Stats").GetComponent<Text>();
				stats.text = "";
				stats.text += "Level: " + curMon.stats[(int)baseMonster.statType.lv];
				stats.text += "\nHappiness: " + curMon.stats[(int)baseMonster.statType.hap];
				stats.text += "\nATK: " + curMon.stats[(int)baseMonster.statType.atk];
				stats.text += "\nDEF: " + curMon.stats[(int)baseMonster.statType.def];
				stats.text += "\nSPE: " + curMon.stats[(int)baseMonster.statType.spd];
				#endregion
				if(curMon.species.evolutions.Length == 0 || curMon.species.egg && curMon.hatchTime >= 0){
					curMonPanel.transform.GetChild(0).gameObject.SetActive(false);
				}else{
					curMonPanel.transform.GetChild(0).gameObject.SetActive(true);
				}
				if(curMon.species.egg){
					curMonPanel.transform.GetChild(1).gameObject.SetActive(false);
				}else{
					curMonPanel.transform.GetChild(1).gameObject.SetActive(true);
				}
				if(curMon.species.speciesPortrait != null){
					curMonPanel.transform.Find("curMonSprite").GetComponent<Image>().sprite = curMon.species.speciesPortrait;
				}
			}else{
				curMonPanel.SetActive(false);
			}

		}
		#endregion
		#region Display Decoration Spot Panels
		if(decSpotPanel.activeSelf){
			if(curDecSpot != null){
				Text description = decSpotPanel.transform.Find("Description").GetComponent<Text>();
				description.text = "";
				if(deployableDec.Count != 0){
					decSpotPanel.transform.Find("decImage").GetComponent<Image>().sprite = deployableDec[decDropdown.value].decSprite;
					description.text += deployableDec[decDropdown.value].decName;
					description.text += "\n"+deployableDec[decDropdown.value].decDescription;
				}
				if(curDecSpot.curDec != null){
					description.text += "\nCurrent Decoration: " +curDecSpot.curDec.decName;
				}else {
					description.text += "\nCurrent Decoration: None";
				}
			}else{
				decSpotPanel.SetActive(false);
			}

		}
		#endregion
		#region Display Evolution Panel
		if(evoPanel.activeSelf){
			if(curMon != null){
				evoPanel.transform.Find("curMon").GetComponent<Image>().sprite = curMon.species.speciesPortrait;
				Text curMonStats = evoPanel.transform.Find("curMonStats").GetComponent<Text>();
				curMonStats.text = "";
				curMonStats.text += "Species: "+curMon.species.speciesName;
				curMonStats.text += "\nBase MaxHP: "+curMon.species.baseStats[4];
				curMonStats.text += "\nBase ATK: "+curMon.species.baseStats[5];
				curMonStats.text += "\nBase DEF: "+curMon.species.baseStats[6];
				curMonStats.text += "\nBase SPD: "+curMon.species.baseStats[7];

				baseSpecies evoMon = lists.speciesList.Find(x => x.speciesNumber == curMon.species.evolutions[evoDropdown.value]) ?? lists.speciesList[0];
				evoPanel.transform.Find("evoMon").GetComponent<Image>().sprite = evoMon.speciesPortrait;
				if(seenMon.Contains(evoMon)){
					evoPanel.transform.Find("evoMon").GetComponent<Image>().color = Color.white;
					Text evoMonStats = evoPanel.transform.Find("evoMonStats").GetComponent<Text>();
					evoMonStats.text = "";
					evoMonStats.text += "Species: "+evoMon.speciesName;
					evoMonStats.text += "\nBase MaxHP: "+evoMon.baseStats[4];
					evoMonStats.text += "\nBase ATK: "+evoMon.baseStats[5];
					evoMonStats.text += "\nBase DEF: "+evoMon.baseStats[6];
					evoMonStats.text += "\nBase SPD: "+evoMon.baseStats[7];
				}else{
					evoPanel.transform.Find("evoMon").GetComponent<Image>().color = Color.black;
					Text evoMonStats = evoPanel.transform.Find("evoMonStats").GetComponent<Text>();
					evoMonStats.text = "";
					evoMonStats.text += "Species: "+"???";
					evoMonStats.text += "\nBase MaxHP: "+"???";
					evoMonStats.text += "\nBase ATK: "+"???";
					evoMonStats.text += "\nBase DEF: "+"???";
					evoMonStats.text += "\nBase SPD: "+"???";
				}


				Text evoReqs = evoPanel.transform.Find("evoReqs").GetComponent<Text>();
				evoReqs.text = "";
				evoReqs.text = "Requirements";
				if(evoMon.statReqs[0] != 0){evoReqs.text+="\nLevel: " + evoMon.statReqs[0];}
				if(evoMon.statReqs[4] != 0){evoReqs.text+="\nMaxHP: " + evoMon.statReqs[4];}
				if(evoMon.statReqs[5] != 0){evoReqs.text+="\nATK: " + evoMon.statReqs[5];}
				if(evoMon.statReqs[6] != 0){evoReqs.text+="\nDEF: " + evoMon.statReqs[6];}
				if(evoMon.statReqs[7] != 0){evoReqs.text+="\nSPD: " + evoMon.statReqs[7];}
				if(evoMon.decReq != null){evoReqs.text+="\nDecoration: " + evoMon.decReq.decName;}
				if(evoMon.timeReq == 1){evoReqs.text+="\nTime of Day: Day";}
				if(evoMon.timeReq == 2){evoReqs.text+="\nTime of Day: Night";}

				if(evoReqs.text == "Requirements"){evoReqs.text +="\n None";}

				if(evoMon.CanEvolve(curMon.stats,this)){
					evoPanel.transform.GetChild(0).GetComponent<Button>().interactable = true;
				}else{evoPanel.transform.GetChild(0).GetComponent<Button>().interactable = false;}



			}
			else{
				evoPanel.SetActive(false);
			}
		}

		#endregion
		#region Day/Night
		if(System.DateTime.Compare(System.DateTime.Now,System.Convert.ToDateTime(nightStart)) > 0 || System.DateTime.Compare(System.DateTime.Now,System.Convert.ToDateTime(dayStart)) < 0){
			Camera.main.backgroundColor = night;
			isDay = false;
		}else{
			Camera.main.backgroundColor = day;
			isDay = true;
		}
		#endregion

		if(curMon != null && curMon.statMods[3] > 0){
			battleButton.interactable = true;
		} else{
			battleButton.interactable = false;
		}

		if(curState == playerState.battle){
			battleButton.gameObject.SetActive(false);
			battlePanel.SetActive(true);
		}else{
			battleButton.gameObject.SetActive(true);
			battlePanel.SetActive(false);
		}

		#region Training Timer
		if( trainingTimer <= 0 && curState == playerState.training){
			curMon.statMods[(int)baseMonster.statType.totalXp] += Random.Range(50,100);
			curMon.TrainStat();
			curState = playerState.farm;
			MoveCamera();
		}else{
			trainingTimer -= Time.deltaTime;
		}
		#endregion

		if(curState == playerState.training){
			#region TargetPractice
			GameObject backSprite = GameObject.Find("curMonBackSprite");
			if(curMon != null && backSprite != curMon.species.speciesPortrait){
				backSprite.GetComponent<SpriteRenderer>().sprite = curMon.species.speciesPortrait;
			}
			if(Input.GetMouseButtonDown(0)){
				ShootProjectile(Camera.main.ScreenToWorldPoint(Input.mousePosition),backSprite.transform.position);
			}
			#endregion
		}


	}

	public void closeMenus(){
		inMenu = false;
		if(curMonPanel.activeSelf){
			curMonPanel.SetActive(false);
		}
		if(decSpotPanel.activeSelf){
			decSpotPanel.SetActive(false);
		}
		if(evoPanel.activeSelf){
			evoPanel.SetActive(false);
		}
	}


	public void CallEvolve(){
		curMon.Evolve(evoDropdown.value);
		evoDropdown.value = 0;
		if(curMon.species.evolutions.Length == 0){CloseEvo();}
		RefreshEvo(evoDropdown,curMon.species.evolutions);
	}

	public void StartTraining(){
		if(stamina > 0){
			stamina--;
			trainingTimer = 30;
			curState = playerState.training;
			closeMenus();
			MoveCamera();
		}
	}

	public void MoveCamera(){
		Camera.main.transform.position = (Vector3)camPos[(int)curState] + new Vector3(0,0,-10);
	}

	public void RefreshDec(Dropdown list, List<decorationObject> listToPull){
		list.ClearOptions();
		options.Clear();
		foreach(decorationObject decObj in listToPull){
			options.Add(decObj.decName);
		}
		if(options.Count > 0){
			list.AddOptions(options);
			list.interactable = true;
		}else {
			list.options.Add(new Dropdown.OptionData() {text = "None"});
			list.interactable = false;
		}
	}

	public void RefreshEvo(Dropdown list, int[] listToPull){
		list.ClearOptions();
		options.Clear();
		foreach(int evoNum in listToPull){
			baseSpecies newSpecies = lists.speciesList.Find(x => x.speciesNumber == evoNum) ?? lists.speciesList[0];
			if(seenMon.Contains(newSpecies)){
				options.Add(newSpecies.speciesName);
			}else{
				options.Add("???");
			}
		}
		if(options.Count > 0){
			list.AddOptions(options);
			list.interactable = true;
		}else {
			options.Add("None");
			list.interactable = false;
		}
	}

	public void RemoveDecoration(){
		decorationObject decoration = curDecSpot.curDec;
		if(decoration != null){
			deployedDec.Remove(decoration);
			deployableDec.Add(decoration);
			curDecSpot.GetComponent<SpriteRenderer>().sprite = decoration.baseSprite;
			decoration = null;
			RefreshDec(decDropdown,deployableDec);
		}
	}
	public void SetDecoration(){
		decorationObject decoration = deployableDec[decDropdown.value];
		RemoveDecoration();
		deployableDec.Remove(decoration);
		deployedDec.Add(decoration);
		curDecSpot.GetComponent<decSpotHandling>().curDec = decoration;
		curDecSpot.GetComponent<SpriteRenderer>().sprite = decoration.decSprite;
		RefreshDec(decDropdown,deployableDec);
	}

	public void ShootProjectile(Vector2 direction,Vector2 origin){
		GameObject newShot = Instantiate(projectile, origin,Quaternion.identity);
		newShot.transform.up = direction - origin;
	}

	public void newMonName(){
		InputField nameInput = GameObject.Find("nameInput").GetComponent<InputField>();
		if(nameInput.text == curMon.monName || nameInput.text == ""){
			nameInput.text = curMon.monName;
		} else{
			curMon.monName = nameInput.text;
		}
	}

	public void OpenEvo(){
		evoPanel.SetActive(true);
	}
	public void CloseEvo(){
		evoPanel.SetActive(false);
	}

	public void StartBattle(){
		curState = playerState.battle;
		GetComponent<battleHandling>().enabled = true;
		GetComponent<battleHandling>().playerMon = curMon;
		GetComponent<battleHandling>().StartBattle(curMon.species,curMon.statMods[0]-1);
		MoveCamera();
	}

	public void ReleaseMonster(){
		ownedMon.Remove(curMon);
		Destroy(curMon.gameObject);
	}

	//Add empty monster
	public void AddMonster(){
		GameObject newMon = Instantiate(monPrefab,Vector2.zero,Quaternion.identity);
		ownedMon.Add(newMon.GetComponent<baseMonster>());
		int numMon = ownedMon.Count-1;
		newMon.transform.position = monPos[numMon];

	}
	//Add specific monster
	public void AddMonster(baseSpecies species){
		GameObject newMon = Instantiate(monPrefab,Vector2.zero,Quaternion.identity);
		ownedMon.Add(newMon.GetComponent<baseMonster>());
		newMon.GetComponent<baseMonster>().species = species;
		int numMon = ownedMon.Count-1;
		newMon.transform.position = monPos[numMon];
	}
		
}
