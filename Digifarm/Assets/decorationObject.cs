using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (menuName = "Decoration")]
public class decorationObject : ScriptableObject {
	public int decorationNumber;
	public string decName,decDescription;
	public Sprite decSprite,baseSprite;
	public Animator animations;
	public int[] statBoosts = new int[9]; //Never set 0,1, or 3
}
