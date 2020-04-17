﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scalpel : InventoryObject
{

    Animator anim;
    protected override void Awake()
    {
        base.Awake();
        IsEnabled = false;
        anim = GetComponent<Animator>();
    }

    public void Emerge()
    {
        gameObject.SetActive(true);
        anim.SetTrigger("Active");
    }

    void OnEmergeAnimationEnd()
    {
        IsEnabled = true;
    }

}
