﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Spear : MonoBehaviour {
	public bool Attacking = false;
	public float BaseMouseY;
    public float EatDelay = 0.1f;
	public GameObject Character;
    Animator animator, HorseFace;
    AudioSource Sound,SpearSound,CaughtSound;
    float EatTime = 0;
    public char[] Achieve;
    public int Count; //0~5
    public AchievementsData achievementData;
    public GameObject AchieveMessage;

    List<AudioClip> SoundList = new List<AudioClip>();

    public List<GameObject> Caught = new List<GameObject>();

    public List<Vector2> PosList = new List<Vector2>();
    float LastAttackTime;
    public AudioSource[] Audiolist;
    public bool Lock = false;
    Component[] ChildRenderer;
    // Use this for initialization
    void Start ()
    {
        BaseMouseY = Screen.height / 2;
        animator = GetComponent<Animator>();
        HorseFace = GameObject.Find("HorseFace").GetComponent<Animator>();
        Audiolist = GetComponents<AudioSource>();
        SpearSound = Audiolist[0];
        CaughtSound = Audiolist[1];
        SoundList.AddRange(Resources.LoadAll<AudioClip>("Sounds"));
        for (int i = 0; i < 6; i++)
        {
            PosList.Add(GetComponentsInChildren<Transform>()[i].localPosition);
        } 
    }
	
	// Update is called once per frame
	void Update () {
        if(Count >= 5) Eat();

        if ((Time.time - LastAttackTime > 0.43f))
        {
            if (!Lock && GameManager.Instance.IsPlayed)
            {
                if (Input.GetMouseButtonDown(0))
                {
                        SpearSound.Play();
                        Attacking = true;
                        animator.SetTrigger("Push");
                        LastAttackTime = Time.time;
                }
                else Attacking = false;
            }
        }

        if(!Lock && GameManager.Instance.IsPlayed) UpdateAngle();

        if (EatTime != 0)
        {
            if ((Time.time - EatTime) > EatDelay)
            {
                for (int i = 5; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
                EatTime = 0;
                GetComponent<CircleCollider2D>().enabled = true;
            }
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (Attacking)
        {
            if (other.gameObject.layer == 9)
            {
                CaughtSound.Play();
                other.GetComponent<Mob>().ifCaught = true;
                other.GetComponent<Mob>().Spear = this;
                Caught.Add(other.gameObject);
            }
        }
    }

    int[] Score = { 50, 80, 80, 100, 100, -1, -1 };

    void Eat()
    {
        char temp;
        Achieve = new char[Caught.Count];
        GetComponent<CircleCollider2D>().enabled = false;
        EatTime = Time.time;
        HorseFace.SetTrigger("Eat");
        int tempScore = 0,i = 0;
        foreach(GameObject C in Caught)
        {
            Achieve[i++] = (char)(C.GetComponent<Mob>().Type + 49);
            tempScore += Score[(int) C.GetComponent<Mob>().Type];
        }
        Array.Sort(Achieve);
        string AchieveString = new string(Achieve);
        foreach (AchievementsData.Achievement a in achievementData.List)
        {
            if (a.Code == AchieveString) StartCoroutine(AchieveAchievement(a.Name));
        }
        GameManager.Instance.Score += tempScore;
        Count = 0;
        Caught.Clear();
    }

    public void ToggleLock() {
        Lock = !Lock;
        ChildRenderer = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer child in ChildRenderer)
            child.enabled = !child.enabled;
    }

	void UpdateAngle()
    {
        float angle = (Input.mousePosition.y - BaseMouseY) / 10;
        if (angle > 0) angle = (Input.mousePosition.y - BaseMouseY) / 5;
        if (angle < 0) angle += 360f;
        if (angle > 90 && angle < 180) angle = 90f;
        else if (angle > 180 && angle < 340) angle = 340f;
        transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void ChangeWeaponBread()
    {
        animator.SetBool("bread", true);
        animator.SetBool("fish", false);
        animator.SetBool("hotdog", false);
        animator.SetBool("spear", false);
    }
    public void ChangeWeaponFish()
    {
        animator.SetBool("bread", false);
        animator.SetBool("fish", true);
        animator.SetBool("hotdog", false);
        animator.SetBool("spear", false);
    }
    public void ChangeWeaponHotdog()
    {
        animator.SetBool("bread", false);
        animator.SetBool("fish", false);
        animator.SetBool("hotdog", true);
        animator.SetBool("spear", false);
    }
    public void ChangeWeaponSpear()
    {
        animator.SetBool("bread", false);
        animator.SetBool("fish", false);
        animator.SetBool("hotdog", false);
        animator.SetBool("spear", true);
    }
    public void ChangeWeaponDefault()
    {
        if (animator.GetBool("bread") == false && animator.GetBool("fish") == false && animator.GetBool("hotdog") == false)
        {
            animator.SetBool("bread", false);
            animator.SetBool("fish", false);
            animator.SetBool("hotdog", false);
            animator.SetBool("spear", true);
        }
        animator.enabled = true;
    }

    IEnumerator AchieveAchievement(string Message)
    {
        AchieveMessage.GetComponent<Text>().text = "你達成了「" + Message + "」!";
        for(float i = 0; i <= 3; i += Time.deltaTime)
        {
            AchieveMessage.SetActive(true);
            yield return 0;
        }
        AchieveMessage.SetActive(false);
    }
}
