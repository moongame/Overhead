﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour {

	public GameObject buildEffect;

	private Material originalMaterial;
	private GameObject tower;
	private BuildManager buildManager;
	private SoulsCounter soulsCounter;
	private ScoreCounter scoreCounter;
	private GameObject currentBuildingTower;
	private SphereShop sphereShop;
	private GameObject towerToBuild;

	void Start(){
		buildManager = GameObject.Find ("GameMaster").GetComponent<BuildManager> ();
		sphereShop = GameObject.Find ("Icosphere").GetComponent<SphereShop> ();
		soulsCounter = SoulsCounter.instance;
		scoreCounter = ScoreCounter.instance;
		towerToBuild = null;
	}
		
	void OnMouseEnter (){
		if (EventSystem.current.IsPointerOverGameObject ()) {
			return;
		}
		//if the towerToBuild variable is null dont do anything 
		if (buildManager.GetTowerToBuild () == null) {
			return;
		}
		GameObject selecTowerInst = (GameObject) Instantiate (buildManager.GetSelectionTowerToBuild(), transform.position, transform.rotation);
		selecTowerInst.transform.rotation = Quaternion.Euler (0,0,0);
		buildManager.SetSelectionTowerToBuildInstance (selecTowerInst);
	}

	void OnMouseExit (){
		buildManager.DestroySelectionTowerToBuildInstance ();
	}

	void OnMouseDown(){
		//Avoid pointing to something with a UI element in front of it
		if (EventSystem.current.IsPointerOverGameObject ()) {
			return;
		}
		//if the towerToBuild variable is null dont do anything 
		if (buildManager.GetTowerToBuild () == null) {
			return;
		}

		if (tower != null) {
			Debug.Log ("can't build there! - TODO: Display on screen.");
			return;
		}
		buildManager.DestroySelectionTowerToBuildInstance ();
		currentBuildingTower = buildManager.GetTowerToBuild();
		soulsCounter.BuildTower (buildManager.GetTowerToBuildIndex());
		scoreCounter.BuildTower (buildManager.GetTowerToBuildIndex());
		StartCoroutine (EventInstantiator ());
	}

	IEnumerator EventInstantiator () {
		GameObject tempBuildEffect = Instantiate (buildEffect, transform.position, Quaternion.identity);
		yield return new WaitUntil (() => tempBuildEffect.GetComponent<Animator> ().GetBool ("finished"));
		BuildTower ();
	}

	void BuildTower () {
		StopCoroutine (EventInstantiator ());
		towerToBuild = buildManager.GetTowerToBuild ();
		tower = (GameObject)Instantiate (currentBuildingTower, transform.position, transform.rotation);
		tower.transform.rotation = Quaternion.Euler (0,0,0);
	}

	/// <summary>
	/// Tells the sphere shop tower to build.
	/// </summary>
	private void TellSphereShopTowerToBuild(){
		sphereShop.SetTowerToBuild (towerToBuild);
	}
}
