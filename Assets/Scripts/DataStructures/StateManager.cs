using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager {
    private State currentState;

    public StateManager(State startingState) {
        this.currentState = startingState;
    }

    public void doUpdate() {
        State nextState = this.currentState.getNextState();
        if (nextState != this.currentState && nextState.canTransitionInto()) {
            this.currentState.exit();
            this.currentState = nextState;
            this.currentState.enter();
        }
        this.currentState.update();
    }

    public string getCurrentStateName() {
        return this.currentState.getName();
    }
}
