using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FighterStats : MonoBehaviour, IComparable<FighterStats>
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject healthFill;
    [SerializeField] private GameObject magicFill;

    [Header("Stats")]
    public float health;
    public float magic;
    public float attack;
    public float defense;
    public float range;
    public float speed;
    public float experience;

    private float startHealth;
    private float startMagic;

    [HideInInspector] public int nextActTurn;
    public Vector3 initialPosition;

    private bool dead = false;

    private Transform healthTransform;
    private Transform magicTransform;
    private Vector2 healthScale;
    private Vector2 magicScale;
    private float xNewHealthScale;
    private float xNewMagicScale;

    private GameObject GameControllerObj;

    private void Awake()
    {
        healthTransform = healthFill.GetComponent<RectTransform>();
        healthScale = healthFill.transform.localScale;

        magicTransform = magicFill.GetComponent<RectTransform>();
        magicScale = magicFill.transform.localScale;

        startHealth = health;
        startMagic = magic;

        GameControllerObj = GameObject.Find("GameControllerObject");

        initialPosition = transform.position;
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;
        animator.Play("Damage");

        if (health <= 0)
        {
            dead = true;
            gameObject.tag = "Dead";
            Destroy(healthFill);
            Destroy(gameObject);
        }
        else
        {
            xNewHealthScale = healthScale.x * (health / startHealth);
            healthFill.transform.localScale = new Vector2(xNewHealthScale, healthScale.y);
        }

        if (damage > 0)
        {
            GameControllerObj.GetComponent<GameController>().battleText.gameObject.SetActive(true);
            GameControllerObj.GetComponent<GameController>().battleText.text = damage.ToString();
        }

        Invoke("ContinueGame", 2);
    }

    public void UpdateMagicFill(float cost)
    {
        if (cost > 0)
        {
            magic -= cost;
            xNewMagicScale = magicScale.x * (magic / startMagic);
            magicFill.transform.localScale = new Vector2(xNewMagicScale, magicScale.y);
        }
    }

    public bool IsDead() => dead;

    public void CalculateNextTurn(int currentTurn)
    {
        nextActTurn = currentTurn + Mathf.CeilToInt(100f / speed);
    }

    private void ContinueGame()
    {
        GameControllerObj.GetComponent<GameController>().NextTurn();
    }

    public int CompareTo(FighterStats other)
    {
        return nextActTurn.CompareTo(other.nextActTurn);
    }

    public Animator GetAnimator() => animator;
    public Vector3 GetInitialPosition() => initialPosition;
}
