using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StateDelegate();
public delegate State StateChangeDelegate();
public class State {
    private StateDelegate onEnter;
    private StateDelegate onUpdate;
    private StateChangeDelegate onGetNextState;
    private StateDelegate onExit;

    public State(StateDelegate onEnter, StateDelegate onUpdate, StateDelegate onExit) {
        this.onEnter = onEnter;
        this.onUpdate = onUpdate;
        this.onExit = onExit;
    }
    public void setOnGetNextState(StateChangeDelegate onGetNextState) {
        this.onGetNextState = onGetNextState;
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
}
