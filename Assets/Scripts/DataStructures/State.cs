using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StateDelegate();
public delegate State StateChangeDelegate();
public delegate bool StateTransitionableDelegate();
public class State {
    private string name;
    private StateTransitionableDelegate transitionIntoPrerequisite;
    private StateDelegate onEnter;
    private StateDelegate onUpdate;
    private StateChangeDelegate onGetNextState;
    private StateDelegate onExit;

    public State(string name, StateDelegate onEnter, StateDelegate onUpdate, StateDelegate onExit) {
        this.name = name;
        this.onEnter = onEnter;
        this.onUpdate = onUpdate;
        this.onExit = onExit;
        this.onGetNextState = (() => { return this; });
        this.transitionIntoPrerequisite = (() => { return true; });
    }

    public void setCanTransitionInto(StateTransitionableDelegate transitionIntoPrerequisite) {
        this.transitionIntoPrerequisite = transitionIntoPrerequisite;
    }

    public void setOnGetNextState(StateChangeDelegate onGetNextState) {
        this.onGetNextState = onGetNextState;
    }

    public bool canTransitionInto() {
        return this.transitionIntoPrerequisite();
    }

    public void enter() {
        this.onEnter();
    }

    public void update() {
        this.onUpdate();
    }

    public State getNextState() {
        return this.onGetNextState();
    }

    public void exit() {
        this.onExit();
    }

    public string getName() {
        return this.name;
    }
}
