﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationController : MonoBehaviour {

    [SerializeField] GameObject enemyPrefab;

    [SerializeField] Sprite[] enemySprites;
    [SerializeField] float width = 8.5f, height = 7f, padding = 2f, speed = 2f, spawnDelay = 0.75f;

    float distance, minX, maxX;
    bool moveRight = true;
    bool spawn = false;
    

    // Use this for initialization
    void Start () {
        distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));

        minX = leftWorldPos.x;
        maxX = rightWorldPos.x;

        SpawnUntilFull();
    }
	
	// Update is called once per frame
	void Update () {
        MoveShips();

        if (AllMembersDead())
        {
            SpawnUntilFull();
        }

        //NextFreePosition();
	}

    void SpawnEnemies()
    {
        foreach (Transform child in transform)
        {
            //Instantiate returns "Object" therefore use "as GameObject" to add the object to the gameobject "folder"       
            GameObject enemy = Instantiate(enemyPrefab, child.transform.position, Quaternion.identity);
            ChangeEnemySprite(enemy);
            //Child the new Enemy
            enemy.transform.parent = child;
        }
    }

    void SpawnUntilFull()
    {
        Transform freePosition = NextFreePosition();
        if(freePosition != null)
        {
            //Instantiate returns "Object" therefore use "as GameObject" to add the object to the gameobject "folder"       
            GameObject enemy = Instantiate(enemyPrefab, freePosition.position, Quaternion.identity);
            ChangeEnemySprite(enemy);
            //Child the new Enemy
            enemy.transform.parent = freePosition;
            Invoke("SpawnUntilFull", spawnDelay);
        }
    }

    void ChangeEnemySprite(GameObject enemy)
    {
        enemy.GetComponent<SpriteRenderer>().sprite = enemySprites[Random.Range(0, enemySprites.Length)];
    }

    //int RandomEnemyPrefab()
    //{
    //    return Random.Range(0, enemyPrefab.Length);
    //}

    Transform NextFreePosition()
    {
        foreach(Transform child in transform)
        {
            if(child.childCount == 0)
            {
                Debug.Log(child);
                return child;
            }
        }
        return null;
    }

    bool AllMembersDead()
    {
        foreach(Transform child in transform)
        {
            if(child.childCount > 0)
            {
                return false;
            }
        }
        return true;
    }

    void MoveShips()
    {
        float rightEdgeOfFormation = transform.position.x + (0.5f * width);
        float leftEdgeOfFormation = transform.position.x - (0.5f * width);
        if (moveRight)
        {
            transform.position += Vector3.right * (speed * Time.smoothDeltaTime);
        }
        else
        {
            transform.position += Vector3.left * (speed * Time.smoothDeltaTime);
        }

        if (rightEdgeOfFormation >= maxX)
        {
            moveRight = false;
        }
        else if(leftEdgeOfFormation <= minX)
        {
            moveRight = true;
        }

        //float newX = Mathf.Clamp(transform.position.x, minX, maxX);
        //transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height));
    }
}