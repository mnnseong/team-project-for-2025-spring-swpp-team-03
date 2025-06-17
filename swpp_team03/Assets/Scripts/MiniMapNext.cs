using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapNext : MonoBehaviour
{
	public Transform target;
	[SerializeField] private RectTransform miniMapIcon;
	[SerializeField] private Transform player;

	private Vector2 canvasTopLeft = new Vector2(-952f, -217f);
	private Vector2 canvasBottomRight = new Vector2(-627f, -531f);

	private Vector2 worldSize = new Vector2(964f, 1036f);

	private void LateUpdate()
	{
		if (player == null || target == null || miniMapIcon == null)
			return;

		Vector2 iconPos = GetTargetUIPosition(player.position, target.position);
		miniMapIcon.localPosition = ClampToMiniMap(iconPos);
	}

	private Vector2 GetTargetUIPosition(Vector3 playerPos, Vector3 targetPos)
	{
		Vector3 offset = targetPos - playerPos;

		float scaleX = (canvasBottomRight.x - canvasTopLeft.x) / worldSize.x;
		float scaleY = (canvasTopLeft.y - canvasBottomRight.y) / worldSize.y;

		float offsetX = offset.x * scaleX;
		float offsetY = offset.z * scaleY;

		float centerX = (canvasTopLeft.x + canvasBottomRight.x) / 2f;
		float centerY = (canvasTopLeft.y + canvasBottomRight.y) / 2f;

		return new Vector2(centerX + offsetX, centerY + offsetY);
	}

	private Vector2 ClampToMiniMap(Vector2 iconPos)
	{
		float minX = canvasTopLeft.x + 10f;
		float maxX = canvasBottomRight.x - 10f;
		float maxY = canvasTopLeft.y - 10f;
		float minY = canvasBottomRight.y + 10f;

		iconPos.x = Mathf.Clamp(iconPos.x, minX, maxX);
		iconPos.y = Mathf.Clamp(iconPos.y, minY, maxY);

		return iconPos;
	}

}
