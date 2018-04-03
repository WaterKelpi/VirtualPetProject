using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Global Lists")]
public class globalLists : ScriptableObject {
	public List<baseSpecies> speciesList;
	public List<decorationObject> decorationList;
}
