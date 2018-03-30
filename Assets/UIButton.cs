using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour {

	[SerializeField] private GameObject targetObject;
	[SerializeField] private string targetMessage;
	public Color highlightColor = Color.green;
	public void OnMouseOver() {
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		if (sprite != null) {
			sprite.color = highlightColor;
		}
	}

	public void OnMouseExit() {
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		if (sprite != null) {
			sprite.color = Color.white;
		}
	}

	public void OnMouseDown() {
		transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
	}

	public void OnMouseUp() {
		transform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);
		if (targetObject != null) {
			targetObject.SendMessage(targetMessage);
		}
	}
}