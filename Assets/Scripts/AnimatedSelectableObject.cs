using UnityEngine;

[RequireComponent (typeof (Animator))]
public class AnimatedSelectableObject : SelectableObject {
    private Animator anim;

	void Start() {
		anim = (Animator) GetComponent<Animator>();
	}

	public override void onClick () {
		startAnimation ();
	}

	private void startAnimation () {
		anim.SetTrigger("Active");
	}
}