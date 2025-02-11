using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using DG.Tweening;

public class Handcuff : MonoBehaviour
{
    [SerializeField] private GameObject _rotationPoint;
    /// <summary>
    /// Kelepçeyi yumu?ak bir ?ekilde açmak için ça?r?lacak fonksiyon.
    /// </summary>
    /// <param name="rotatingPart">Dönme noktas? olan Transform.</param>
    public void OpenHandcuff()
    {
        // Y ekseninde -90 dereceye yumu?ak geçi?
        _rotationPoint.transform.DOLocalRotate(new Vector3(0, -90, 0), 1f).SetEase(Ease.InOutSine);
    }
}

