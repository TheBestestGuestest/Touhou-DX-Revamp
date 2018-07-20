﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputControl : MonoBehaviour {
    private Transform trans;
    private PlayerStats ps;
    private PlayerStatsCounter psc;
    private SpriteRenderer[] sr;

    private float shootTimer = 0.05f;

    private Sprite normalHitbox;
    private Sprite slowHitBox;
    private Sprite invulnPlayer;
    private Sprite normalPlayer;

    void Awake() {
        trans = gameObject.transform;
        ps = GetComponent<PlayerStats>();
        psc = GetComponent<PlayerStatsCounter>();
        sr = GetComponentsInChildren<SpriteRenderer>();
    }

    void Start() {
        normalHitbox = Resources.Load("Sprites/CharacterSprites/hitbox", typeof(Sprite)) as Sprite;
        slowHitBox = Resources.Load("Sprites/CharacterSprites/hitboxSlow", typeof(Sprite)) as Sprite;

        normalPlayer = Resources.Load("Sprites/CharacterSprites/cirno", typeof(Sprite)) as Sprite;
        invulnPlayer = Resources.Load("Sprites/CharacterSprites/cirnoHit", typeof(Sprite)) as Sprite;
    }

    void Update() {
        //sprites
        //  player
        if (ps.isInvuln) {
            if (!sr[0].sprite.Equals(invulnPlayer)) sr[0].sprite = invulnPlayer;
        }
        else if (!sr[0].sprite.Equals(normalPlayer)) {
            sr[0].sprite = normalPlayer;
        }
        //  hitbox
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (!sr[1].sprite.Equals(slowHitBox)) sr[1].sprite = slowHitBox;
        }
        else if (!sr[1].sprite.Equals(normalHitbox)) {
            sr[1].sprite = normalHitbox;
        }

        //movement
        float dist = ps.speed * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 2.5f / ps.speed : 1f);
        Vector3 pos = trans.position;
        if (Input.GetKey(KeyCode.LeftArrow)) pos.x -= dist;
        if (Input.GetKey(KeyCode.RightArrow)) pos.x += dist;
        if (Input.GetKey(KeyCode.UpArrow)) pos.y += dist;
        if (Input.GetKey(KeyCode.DownArrow)) pos.y -= dist;
        pos.x = Mathf.Max(Mathf.Min(pos.x, InGameDimentions.rightEdge), InGameDimentions.leftEdge);
        pos.y = Mathf.Max(Mathf.Min(pos.y, InGameDimentions.topEdge), InGameDimentions.bottomEdge);
        trans.position = pos;

        //shooting
        if (Input.GetKey(KeyCode.Z) && shootTimer >= ps.shootingRate) {
            shootTimer = 0f;
            useShot();
        }

        //bombs
        if (Input.GetKeyDown(KeyCode.X) && ps.currBombs > 0) useBomb();

        shootTimer += Time.deltaTime;
    }
    public void useShot() {
        Vector3 pos = trans.position;
        switch (ps.powerLevel) {
            case 0:
                Projectile.Create("Prefabs/PlayerBullet", new Vector3(pos.x, pos.y + 0.5f, pos.z), straightShot(), ps.bulletDamage, false);
                break;
            case 1:
            case 2:
                Projectile.Create("Prefabs/PlayerBullet", new Vector3(pos.x - 0.25f, pos.y + 0.5f, pos.z), straightShot(), ps.bulletDamage, false);
                Projectile.Create("Prefabs/PlayerBullet", new Vector3(pos.x + 0.25f, pos.y + 0.5f, pos.z), straightShot(), ps.bulletDamage, false);
                break;
        }
    }
    public MovePath straightShot() {
        return delegate (float t, Vector3 pos) {
            return new Vector3(pos.x, pos.y + t * 25, pos.z);
        };
    }

    public void useBomb() {
        ps.currBombs--;
        psc.bombs.text = ps.currBombs.ToString();

        string prefab;
        int amount;
        int speed;
        switch (ps.powerLevel) {
            case 0:
                prefab = "Prefabs/PlayerBomb";
                amount = 8;
                speed = 5;
                break;
            case 1:
                prefab = "Prefabs/PlayerBomb";
                amount = 16;
                speed = 5;
                break;
            case 2:
                prefab = "Prefabs/PlayerBomb";
                amount = 16;
                speed = 2;
                break;
            default:
                prefab = null;
                amount = 0;
                speed = 0;
                break;
        }
        for (int i = 0; i < amount; i++) Projectile.Create(prefab, new Vector3(trans.position.x, trans.position.y, trans.position.z), bombLine(i, amount, speed), ps.bombDamage, false);

    }
    public MovePath bombLine(int i, int amt, int spd) {
        return delegate (float t, Vector3 pos) {
            float rads = i * 2 * Mathf.PI / amt;
            return new Vector3(pos.x + t * spd * Mathf.Cos(rads), pos.y + t * spd * Mathf.Sin(rads), pos.z);
        };
    }
}
