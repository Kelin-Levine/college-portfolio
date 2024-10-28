using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectScroller : MonoBehaviour
{
    // Parameters
    [Tooltip("Rate to scroll between pages.")]
    public float scrollRate = 5.0f;
    [Tooltip("Minimum page than can be scrolled to.")]
    public int minPage = -1;
    [Tooltip("Maximum page than can be scrolled to.")]
    public int maxPage = 5;

    // Working Variables
    private GridLayoutGroup layoutGroup;
    private Vector3 panTarget;

    // Reused Variables
    float spacing;
    int page;


    // Instance Functions
    private void OnEnable()
    {
        layoutGroup = GetComponent<GridLayoutGroup>();

        panTarget = -transform.localPosition;
        StartCoroutine(WaitResetScroll());
    }

    private IEnumerator WaitResetScroll()
    {
        yield return new WaitForEndOfFrame();
        panTarget = -transform.localPosition;
        Scroll(0);
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, -panTarget, MultiTargetCamera.CalculateContinuousLerpStep(scrollRate));
    }

    public void ScrollScreenRight()
    {
        Scroll(1);
    }

    public void ScrollScreenLeft()
    {
        Scroll(-1);
    }

    private float GetPageSpacing()
    {
        return layoutGroup.spacing.x + layoutGroup.cellSize.x;
    }

    private void Scroll(int pages)
    {
        spacing = GetPageSpacing();
        page = Mathf.RoundToInt(panTarget.x / spacing);
        page = Math.Clamp(page + pages, minPage, maxPage);
        panTarget = new(page * spacing, 0, 0);
        UIManager.S.ScrollRight.gameObject.SetActive(page != maxPage);
        UIManager.S.ScrollLeft.gameObject.SetActive(page != minPage);
    }
}
//:)