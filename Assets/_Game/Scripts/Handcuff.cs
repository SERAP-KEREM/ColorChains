using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using DG.Tweening;

public class Handcuff : MonoBehaviour
{
    [SerializeField] private GameObject RotationPoint;
    /// <summary>
    /// Kelep�eyi yumu?ak bir ?ekilde a�mak i�in �a?r?lacak fonksiyon.
    /// </summary>
    /// <param name="rotatingPart">D�nme noktas? olan Transform.</param>
    public void OpenHandcuff()
    {
        // Y ekseninde -90 dereceye yumu?ak ge�i?
        RotationPoint.transform.DOLocalRotate(new Vector3(0, -90, 0), 1f).SetEase(Ease.InOutSine);
    }
}

