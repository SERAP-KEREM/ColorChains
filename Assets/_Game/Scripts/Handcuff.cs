using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using DG.Tweening;

public class Handcuff : MonoBehaviour
{
    [SerializeField] private GameObject RotationPoint;
    /// <summary>
    /// Kelepçeyi yumu?ak bir ?ekilde açmak için ça?r?lacak fonksiyon.
    /// </summary>
    /// <param name="rotatingPart">Dönme noktas? olan Transform.</param>
    public void OpenHandcuff()
    {
        // Y ekseninde -90 dereceye yumu?ak geçi?
        RotationPoint.transform.DOLocalRotate(new Vector3(0, -90, 0), 1f).SetEase(Ease.InOutSine);
    }
}

