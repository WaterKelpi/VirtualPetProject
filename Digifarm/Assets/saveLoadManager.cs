using System.Collections;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class saveLoadManager {
	public static void SavePlayer(fmScript fm){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream stream = new FileStream(Application.persistentDataPath + "/farmmanager.mon", FileMode.Create);

		PlayerData data = new PlayerData(fm);
		bf.Serialize(stream,data);
		stream.Close();
	}

	public static void LoadPlayer(fmScript fm){
		if(File.Exists(Application.persistentDataPath + "/farmmanager.mon")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream stream = new FileStream(Application.persistentDataPath + "/farmmanager.mon", FileMode.Open);
			PlayerData data = bf.Deserialize(stream) as PlayerData;

			if(fm.stamina < fm.maxStamina){
				fm.stamina += (int)(DateTime.Now - DateTime.Parse(data.lastLogin)).TotalSeconds/fm.stamRegenTimer+data.stamina;
				fm.stamRegen = (float)(DateTime.Now - DateTime.Parse(data.lastLogin)).TotalSeconds%fm.stamRegenTimer + data.stamRegen;
			}

			for(int i = 0;i<data.ownedMonNames.Length;i++){
					fm.AddMonster();
					fm.ownedMon[i].monName = data.ownedMonNames[i];
					fm.ownedMon[i].species = fm.lists.speciesList.Find(x => x.speciesNumber == data.ownedMonStats[i,0]) ?? fm.lists.speciesList[0];
					for(int i2 = 1;i2<10;i2++){
						fm.ownedMon[i].statMods[i2-1] = data.ownedMonStats[i,i2];
					}
					if(fm.ownedMon[i].species.egg){
						if((DateTime.Now - DateTime.Parse(data.lastLogin)).TotalSeconds > fm.ownedMon[i].hatchTime){
							fm.ownedMon[i].hatchTime = 0;
						} else{
							fm.ownedMon[i].hatchTime = data.monHatchTime[i] - (float)(DateTime.Now - DateTime.Parse(data.lastLogin)).TotalSeconds;
						}
						fm.ownedMon[i].warmth -= (float)(DateTime.Now - DateTime.Parse(data.lastLogin)).TotalSeconds/5;
					}
					if(fm.ownedMon[i].statMods[3] < fm.ownedMon[i].stats[4]){
						fm.ownedMon[i].statMods[3] += (int)(DateTime.Now - DateTime.Parse(data.lastLogin)).TotalSeconds/fm.ownedMon[i].healthRegenTimer;
						fm.ownedMon[i].healthRegen = (float)(DateTime.Now - DateTime.Parse(data.lastLogin)).TotalSeconds%fm.ownedMon[i].healthRegenTimer + data.monHealthRegen[i];
					}
			}
			fm.seenMon.Clear();
			for(int i = 0;i<data.seenMonIDs.Length;i++){
				fm.seenMon.Add(fm.lists.speciesList.Find(x => x.speciesNumber == data.seenMonIDs[i]) ?? fm.lists.speciesList[0]);
			}

			fm.lastLogin = data.lastLogin;

			stream.Close();
		}else{
			Debug.LogError("File does not exist.");
		}
	}

	public static void ClearPlayer(){
		if(File.Exists(Application.persistentDataPath + "/farmmanager.mon")){
			File.Delete(Application.persistentDataPath + "/farmmanager.mon");
		}
	}

	[Serializable]
	public class PlayerData{
		public string[] ownedMonNames;
		public int[,] ownedMonStats;
		public float[] monHatchTime,monHealthRegen;
		public string lastLogin;
		public int[] seenMonIDs;
		public int stamina;
		public float stamRegen;

		public PlayerData(fmScript fm){
			ownedMonNames = new string[fm.ownedMon.Count];
			monHatchTime = new float[fm.ownedMon.Count];
			monHealthRegen = new float[fm.ownedMon.Count];
			ownedMonStats = new int[fm.ownedMon.Count,10];
			seenMonIDs = new int[fm.seenMon.Count];
			stamina = fm.stamina;
			stamRegen = fm.stamRegen;
			for(int i = 0;i<fm.ownedMon.Count;i++){
				if(fm.ownedMon[i] != null){
					ownedMonNames[i] = fm.ownedMon[i].monName;
					monHatchTime[i] = fm.ownedMon[i].hatchTime;
					monHealthRegen[i] = fm.ownedMon[i].healthRegen;
					ownedMonStats[i,0] = fm.ownedMon[i].species.speciesNumber;
					for(int i2 = 1;i2<10;i2++){
						ownedMonStats[i,i2]	= fm.ownedMon[i].statMods[i2-1];
					}
				}
			}
			for(int i = 0;i<seenMonIDs.Length;i++){
				seenMonIDs[i] = fm.seenMon[i].speciesNumber;
			}
			lastLogin = DateTime.Now.ToString();
		}
	}
}
