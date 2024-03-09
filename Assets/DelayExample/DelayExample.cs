using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class DelayExample : MonoBehaviour
{
    [SerializeField] private float _delayStart = 1f;
    [SerializeField] private TMP_Text _statusText;

    [Space(10)]
    [Header("Trees")]
    [SerializeField] private Transform[] _treeObjects;
    [SerializeField] private float _treesAnimationScaleDuration = 0.5f;
    [SerializeField] private float _treesAnimationDelayNext = 0.1f;

    [Space(10)]
    [Header("Gas station")]
    [SerializeField] private Transform _gasStation;
    [SerializeField] private float _gasSationAnimationScaleDuration = 0.75f;

    [Space(10)]
    [Header("Car")]
    [SerializeField] private Transform _car;
    [SerializeField] private Transform _startPosition;
    [SerializeField] private float _carAnimationScaleDuration = 0.75f;
    [SerializeField] private float _carAnimationMoveDuration = 2f;

    private Vector3 _currentCarPosition;

    private async void Start()
    {
        CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();

        await AnimateTrees(_treeObjects, cancellationToken);
        SetStatusText("init trees");
        await AnimateGasStation(_gasStation, cancellationToken);
        SetStatusText("init gas station");
        await AnimateCar(_car, cancellationToken);
        SetStatusText("init car");
    }

    private async UniTask AnimateTrees(Transform[] trees, CancellationToken cancellationToken)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_delayStart), cancellationToken: cancellationToken);

        UniTask[] tasks = new UniTask[trees.Length];

        for (int i = 0; i < trees.Length; i++)
            tasks[i] = AnimateDelay(trees[i], cancellationToken, i * _treesAnimationDelayNext, i == trees.Length - 1);

        await UniTask.WhenAll(tasks);
    }

    private async UniTask AnimateGasStation(Transform gasStation, CancellationToken cancellationToken)
    {
        gasStation.gameObject.SetActive(true);
        await gasStation.DOScale(Vector3.zero, _gasSationAnimationScaleDuration).From().SetEase(Ease.InOutBack).WithCancellation(cancellationToken);
    }

    private async UniTask AnimateCar(Transform car, CancellationToken cancellationToken)
    {
        _currentCarPosition = car.position;
        car.position = _startPosition.position;

        car.gameObject.SetActive(true);
        await car.DOScale(Vector3.zero, _carAnimationScaleDuration).From().SetEase(Ease.InOutBack).WithCancellation(cancellationToken);
        await car.DOMove(_currentCarPosition, _carAnimationMoveDuration).SetEase(Ease.InOutBack).WithCancellation(cancellationToken);
    }

    private async UniTask AnimateDelay(Transform objectToAnimate, CancellationToken cancellationToken, float time, bool isLastObject)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: cancellationToken);

        objectToAnimate.gameObject.SetActive(true);

        if (isLastObject)
        {
            await objectToAnimate.DOScale(Vector3.zero, _treesAnimationScaleDuration).From().SetEase(Ease.OutBack).WithCancellation(cancellationToken);
        }
        else
        {
            objectToAnimate.DOScale(Vector3.zero, _treesAnimationScaleDuration).From().SetEase(Ease.OutBack).WithCancellation(cancellationToken);
        }
    }

    private void SetStatusText(string status)
    {
        _statusText.text = "Status: " + status;
    }
}