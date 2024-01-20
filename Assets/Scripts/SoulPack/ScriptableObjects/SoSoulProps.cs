using System;
using System.Collections.Generic;
using System.Linq;
using AbilityPack;
using Enum;
using JetBrains.Annotations;
using SoulPack.Enum;
using Unity.VisualScripting;
using UnityEngine;

namespace SoulPack.ScriptableObjects
{
    [CreateAssetMenu(fileName = "new Soul Props", menuName = "Custom/Soul")]
    public class SoSoulProps : ScriptableObject
    {
        [SerializeField] protected ESoulType soulType;
        [SerializeField] protected Sprite soulImage;
        [SerializeField, TextArea(4, 5)] protected string soulDescription;
    }
}