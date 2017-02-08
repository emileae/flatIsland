using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishingSpot : MonoBehaviour {

	private bool setValues = false;
	private int buildCost = 0;
	private int cost = 0;
	private float buildTime = 0;
	private float makeTime = 0;
	private int yields = 0;
	private int hp = 0;


	// Use this for initialization
	void Start () {
		if (!setValues) {
			LevelUp (0);
			setValues = true;
		}
	}
	
	public void LevelUp(int level){
		switch (level) {
		case 0:
			buildCost = 5;
			cost = 3;
			buildTime = 10.0f;
			makeTime = 5.0f;
			yields = 5;
			hp = 5;
		case 1:
			buildCost = 10;
			cost = 5;
			buildTime = 20.0f;
			makeTime = 7.0f;
			yields = 10;
			hp = 8;
		}
	}
}
