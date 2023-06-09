﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/CharacterObject")]
public class CharacterObject : ScriptableObject
{
    [SerializeField] private string characterName;
    [SerializeField] private Sprite portraitSprite;
    [SerializeField] private AudioClip voice;

    public string CharacterName => characterName;
    public Sprite PortraitSprite => portraitSprite;
    public AudioClip Voice => voice;
}
