using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [SerializeField] float money;

    public event Action OnMoneyChanged;

    public float Money => money;

    public static Wallet Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddMoney(float amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke();
    }

    public void TakeMoney(float amount)
    {
        money -= amount;
        OnMoneyChanged?.Invoke();
    }
}
