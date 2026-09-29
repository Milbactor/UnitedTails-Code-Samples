using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterID : MonoBehaviour
{
    [SerializeField] private string characterId;

    public string CharacterId { get => characterId; }
}
