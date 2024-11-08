using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    #region Inspector

    public SpriteRenderer spriteRenderer;
    public float interval;
    public float speed = 1f;

    #endregion

    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private int firstIndex = 1;

    private void Awake()
    {
        // 원본 이미지를 하나 더 복제한다.
        var newSpriteRenderer = Instantiate<SpriteRenderer>(spriteRenderer);
        newSpriteRenderer.transform.SetParent(this.transform);
        spriteRenderers.Add(spriteRenderer);
        spriteRenderers.Add(newSpriteRenderer);
        SortImage();
    }

    /// <summary>
    /// 이미지 정렬
    /// </summary>
    private void SortImage()
    {
        for (int i = spriteRenderers.Count - 1; i >= 0 ; i--)
        {
            var spriteRenderer = spriteRenderers[i];
            spriteRenderer.transform.localPosition = Vector3.left * interval * i;
        }
    }

    private void Update()
    {
        UpdateMoveImages();
    }

    /// <summary>
    /// 이미지 이동 업데이트
    /// </summary>
    private void UpdateMoveImages()
    {
        float move = Time.deltaTime * speed;
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            var spriteRenderer = spriteRenderers[i];
            spriteRenderer.transform.localPosition += Vector3.right * move;

            if (spriteRenderer.transform.localPosition.x >= interval)
            {
                spriteRenderer.transform.localPosition = new Vector3(spriteRenderers[firstIndex].transform.localPosition.x - interval, 0f, 0f);
                firstIndex = spriteRenderers.IndexOf(spriteRenderer);
            }
        }
    }
}