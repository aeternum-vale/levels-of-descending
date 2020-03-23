using UnityEngine;

[RequireComponent (typeof (Animator))]
public class AnimatedSelectableObject : SelectableObject {
    private Animator anim;

	void Start() {
		anim = GetComponent<Animator>();
	}

	public override void onClick () {
		StartAnimation ();
	}

	private void StartAnimation () {
		anim.SetTrigger("Active");
	}
}